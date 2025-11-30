#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APIs.Xbox
{
    public static class Suscripcion
    {
        public static Suscripciones2.Suscripcion Generar()
        {
            Suscripciones2.Suscripcion gamepass = new Suscripciones2.Suscripcion
            {
                Id = Suscripciones2.SuscripcionTipo.PCGamePass,
                Nombre = "PC Game Pass",
                ImagenLogo = "/imagenes/suscripciones/pcgamepass.webp",
				ImagenIcono = "/imagenes/suscripciones/pcgamepass_icono.webp",
                Enlace = "https://www.xbox.com/xbox-game-pass/pc-game-pass",
                DRMDefecto = JuegoDRM.Microsoft,
                AdminInteractuar = true,
                UsuarioEnlacesEspecificos = true,
                ParaSiempre = false,
                Precio = 14.99,
				AdminPendientes = true,
				TablaPendientes = "tiendamicrosoftstore"
            };

            return gamepass;
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

			BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString().ToLower(), DateTime.Now, 0, conexion);

            int cantidad = 0;

			List<string> enlaces = [
				"https://catalog.gamepass.com/sigls/v2?id=fdd9e2a7-0fee-49f6-ad69-4354098401ff&language=en-us&market=US",
                "https://catalog.gamepass.com/sigls/v2?id=1d33fbb9-b895-4732-a8ca-a55c8b99fa2c&language=en-us&market=US"];

			foreach (var enlace2 in enlaces)
			{
                string html = await Decompiladores.GZipFormato3(enlace2);

                if (string.IsNullOrEmpty(html) == false)
                {
                    int int1 = html.IndexOf("{" + Strings.ChrW(34) + "id" + Strings.ChrW(34));
                    string temp1 = html.Remove(0, int1);
                    html = "[" + temp1;

                    List<XboxGamePassJuego> juegos = JsonSerializer.Deserialize<List<XboxGamePassJuego>>(html);

                    if (juegos != null)
                    {
                        foreach (var juego in juegos)
                        {
                            string enlace = "https://www.microsoft.com/store/productid/" + juego.Id;

                            bool encontrado = false;

							conexion = CogerConexion(conexion);

							string sqlBuscar = "SELECT idJuegos FROM tiendamicrosoftstore WHERE enlace=@enlace";

							var filas = (await conexion.QueryAsync<string>(sqlBuscar, new { Enlace = enlace })).ToList();

							if (filas.Count > 0)
							{
								cantidad += 1;
								BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString().ToLower(), DateTime.Now, cantidad);

								encontrado = true;

								string idJuegosTexto = filas.FirstOrDefault();

								if (string.IsNullOrWhiteSpace(idJuegosTexto) == false && idJuegosTexto != "0")
                                {
									List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

									if (idJuegos.Count > 0)
									{
										foreach (var id in idJuegos)
										{
											Juegos.Juego juegobd = BaseDatos.Juegos.Buscar.UnJuego(int.Parse(id));

											if (juegobd != null)
											{
												bool añadirSuscripcion = true;

												if (juegobd.Suscripciones?.Count > 0)
												{
													bool actualizar = false;

													foreach (var suscripcion in juegobd.Suscripciones)
													{
														if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.PCGamePass)
														{
															añadirSuscripcion = false;
															actualizar = true;

															DateTime nuevaFecha = suscripcion.FechaTermina;
															nuevaFecha = DateTime.Now;
															nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);
															suscripcion.FechaTermina = nuevaFecha;
														}
													}

													if (actualizar == true)
													{
														BaseDatos.Juegos.Actualizar.Suscripciones(juegobd, conexion);

														if (string.IsNullOrEmpty(juegobd.IdXbox) == true)
														{
															BaseDatos.Juegos.Actualizar.IdXbox(juegobd.Id, juego.Id);
														}

														JuegoSuscripcion suscripcion2 = BaseDatos.Suscripciones.Buscar.UnJuego(enlace);

														if (suscripcion2 != null)
														{
															DateTime nuevaFecha = suscripcion2.FechaTermina;
															nuevaFecha = DateTime.Now;
															nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);
															suscripcion2.FechaTermina = nuevaFecha;
															BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion2, conexion);
														}
													}
												}

												if (añadirSuscripcion == true)
												{
													DateTime nuevaFecha = DateTime.Now;
													nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

													JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
													{
														DRM = JuegoDRM.Microsoft,
														Nombre = juegobd.Nombre,
														FechaEmpieza = DateTime.Now,
														FechaTermina = nuevaFecha,
														Imagen = juegobd.Imagenes.Header_460x215,
														ImagenNoticia = juegobd.Imagenes.Header_460x215,
														JuegoId = juegobd.Id,
														Enlace = enlace,
														Tipo = Suscripciones2.SuscripcionTipo.PCGamePass
													};

													if (juegobd.Suscripciones == null)
													{
														juegobd.Suscripciones = new List<JuegoSuscripcion>();
													}

													juegobd.Suscripciones.Add(nuevaSuscripcion);

													BaseDatos.Suscripciones.Insertar.Ejecutar(juegobd.Id, juegobd.Suscripciones, nuevaSuscripcion, conexion);
												}
											}
										}
									}
								}
							}

							if (encontrado == false)
							{
								BaseDatos.Suscripciones.Insertar.Temporal(conexion, Generar().Id.ToString().ToLower(), enlace);
							}
                        }
                    }
                }
            }
        }
    }

    public class XboxGamePassJuego
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
