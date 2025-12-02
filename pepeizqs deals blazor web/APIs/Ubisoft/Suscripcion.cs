//https://public-ubiservices.ubi.com/v1/applications/global/webservices/ubisoftplus/vault/products?storefront=ie

#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APIs.Ubisoft
{
    public static class Suscripcion
    {
        public static Suscripciones2.Suscripcion Generar()
        {
            Suscripciones2.Suscripcion ubisoft = new Suscripciones2.Suscripcion
            {
                Id = Suscripciones2.SuscripcionTipo.UbisoftPlusClassics,
                Nombre = "Ubisoft+ Classics",
                ImagenLogo = "/imagenes/suscripciones/ubisoftplusclassics.webp",
                ImagenIcono = "/imagenes/tiendas/ubisoft_icono.webp",
                Enlace = "https://store.ubisoft.com/ubisoftplus",
                DRMDefecto = JuegoDRM.Ubisoft,
                AdminInteractuar = true,
                UsuarioEnlacesEspecificos = false,
                ParaSiempre = false,
                Precio = 7.99,
                AdminPendientes = true,
                TablaPendientes = "tiendaubisoft"
            };

            return ubisoft;
        }

        public static Suscripciones2.Suscripcion GenerarPremium()
        {
            Suscripciones2.Suscripcion ubisoft = new Suscripciones2.Suscripcion
            {
                Id = Suscripciones2.SuscripcionTipo.UbisoftPlusPremium,
                Nombre = "Ubisoft+ Premium",
                ImagenLogo = "/imagenes/suscripciones/ubisoftpluspremium.webp",
                ImagenIcono = "/imagenes/tiendas/ubisoft_icono.webp",
                Enlace = "https://store.ubisoft.com/ubisoftplus",
                DRMDefecto = JuegoDRM.Ubisoft,
                AdminInteractuar = true,
                UsuarioEnlacesEspecificos = false,
                ParaSiempre = false,
                Precio = 17.99,
                IncluyeSuscripcion = Suscripciones2.SuscripcionTipo.UbisoftPlusClassics,
                AdminPendientes = true,
                TablaPendientes = "tiendaubisoft"
            };

            return ubisoft;
        }

		public static string Referido(string enlace)
		{
			enlace = enlace.Replace(":", "%3A");
			enlace = enlace.Replace("/", "%2F");
			enlace = enlace.Replace("/", "%2F");
			enlace = enlace.Replace("?", "%3F");
			enlace = enlace.Replace("=", "%3D");

			return "https://ubisoft.pxf.io/c/1382810/1186371/12050?u=" + enlace;
		}

		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static async Task Buscar(SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString().ToLower(), DateTime.Now, 0);

            int cantidad = 0;

            HttpClient cliente = new HttpClient();
            cliente.BaseAddress = new Uri("https://store.ubisoft.com/");
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://xely3u4lod-dsn.algolia.net/1/indexes/production__us_ubisoft__products__en_US__release_date/query?x-algolia-agent=Algolia for JavaScript (4.13.1); Browser (lite)&x-algolia-api-key=5638539fd9edb8f2c6b024b49ec375bd&x-algolia-application-id=XELY3U4LOD");
            peticion.Content = new StringContent("{\"query\":\"\",\"attributesToRetrieve\":[\"title\",\"image_link\",\"short_title\",\"id\",\"MasterID\",\"Genre\",\"release_date\",\"partOfUbisoftPlus\",\"anywherePlatforms\",\"subscriptionExpirationDate\",\"Edition\",\"adult\",\"partofSubscriptionOffer\"],\"hitsPerPage\":9999,\"facetFilters\":[[\"partOfUbisoftPlus:true\"],[],[],[\"partofSubscriptionOfferID:5ebe5e920d253c3638a9521b\"]],\"clickAnalytics\":true}",
                                                Encoding.UTF8, "application/json");

            HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

            string html = string.Empty;
            
            try
            {
                html = await respuesta.Content.ReadAsStringAsync();
            }
            catch { }
            
            if (string.IsNullOrEmpty(html) == false)
            {
                UbisoftSuscripcion datos = JsonSerializer.Deserialize<UbisoftSuscripcion>(html);

                if (datos != null)
                {
                    foreach (var juego in datos.Juegos)
                    {
                        string enlace = "https://store.ubisoft.com/" + juego.Id + ".html";

                        bool encontrado = false;

						conexion = CogerConexion(conexion);

						string idJuegosTexto = await conexion.QueryFirstOrDefaultAsync<string>("SELECT idJuegos FROM tiendaubisoft WHERE enlace=@enlace", new { enlace });

                        if (string.IsNullOrEmpty(idJuegosTexto) == false)
                        {
							encontrado = true;

							if (idJuegosTexto != "0")
							{
								List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

								if (idJuegos.Count > 0)
								{
									foreach (var id in idJuegos)
									{
										cantidad += 1;
										BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.UbisoftPlusClassics)
												{
													insertar = false;

													suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(2);

													BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
												}
											}
										}

										if (insertar == true)
										{
											DateTime nuevaFecha = DateTime.Now;
											nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

											JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
											{
												DRM = JuegoDRM.Ubisoft,
												Nombre = juego.Nombre,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.Id,
												Tipo = Suscripciones2.SuscripcionTipo.UbisoftPlusClassics
											};

											BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
							BaseDatos.Suscripciones.Insertar.Temporal(conexion, Generar().Id.ToString().ToLower(), enlace, juego.Nombre, juego.Imagen);
						}
					}
				}
            }
        }

		public static async Task BuscarPremium(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			BaseDatos.Admin.Actualizar.Tiendas(GenerarPremium().Id.ToString().ToLower(), DateTime.Now, 0);

			int cantidad = 0;

			HttpClient cliente = new HttpClient();
			cliente.BaseAddress = new Uri("https://store.ubisoft.com/");
			cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://xely3u4lod-dsn.algolia.net/1/indexes/production__us_ubisoft__products__en_US__release_date/query?x-algolia-agent=Algolia for JavaScript (4.13.1); Browser (lite)&x-algolia-api-key=5638539fd9edb8f2c6b024b49ec375bd&x-algolia-application-id=XELY3U4LOD");
			peticion.Content = new StringContent("{\"query\":\"\",\"attributesToRetrieve\":[\"title\",\"image_link\",\"short_title\",\"id\",\"MasterID\",\"Genre\",\"release_date\",\"partOfUbisoftPlus\",\"anywherePlatforms\",\"subscriptionExpirationDate\",\"Edition\",\"adult\",\"partofSubscriptionOffer\"],\"hitsPerPage\":9999,\"facetFilters\":[[\"partOfUbisoftPlus:true\"],[],[],[\"partofSubscriptionOfferID:5f44de7b5cdf9a0c2027ca78\"]],\"clickAnalytics\":true}",
												Encoding.UTF8, "application/json");

			HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

			string html = string.Empty;

			try
			{
				html = await respuesta.Content.ReadAsStringAsync();
			}
			catch { }

			if (string.IsNullOrEmpty(html) == false)
			{
				UbisoftSuscripcion datos = JsonSerializer.Deserialize<UbisoftSuscripcion>(html);

				if (datos != null)
				{
					foreach (var juego in datos.Juegos)
					{
						string enlace = "https://store.ubisoft.com/" + juego.Id + ".html";

						bool encontrado = false;

						conexion = CogerConexion(conexion);

						string idJuegosTexto = await conexion.QueryFirstOrDefaultAsync<string>("SELECT idJuegos FROM tiendaubisoft WHERE enlace=@enlace", new { enlace });

						if (string.IsNullOrEmpty(idJuegosTexto) == false)
						{
							encontrado = true;

							if (idJuegosTexto != "0")
							{
								List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

								if (idJuegos.Count > 0)
								{
									foreach (var id in idJuegos)
									{
										cantidad += 1;
										BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.UbisoftPlusPremium)
												{
													insertar = false;

													suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(2);

													BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
												}
											}
										}

										if (insertar == true)
										{
											DateTime nuevaFecha = DateTime.Now;
											nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

											JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
											{
												DRM = JuegoDRM.Ubisoft,
												Nombre = juego.Nombre,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.Id,
												Tipo = Suscripciones2.SuscripcionTipo.UbisoftPlusPremium
											};

											BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
                            BaseDatos.Suscripciones.Insertar.Temporal(conexion, GenerarPremium().Id.ToString(), enlace, juego.Nombre, juego.Imagen);
                        }
					}
				}
			}
		}
	}

    public class UbisoftSuscripcion
    {
        [JsonPropertyName("hits")]
        public List<UbisoftSuscripcionJuego> Juegos { get; set; }
    }

    public class UbisoftSuscripcionJuego
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Nombre { get; set; }

        [JsonPropertyName("image_link")]
        public string Imagen { get; set; }
    }
}
