#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace APIs.GreenManGaming
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "greenmangaming",
				Nombre = "Green Man Gaming",
				ImagenLogo = "/imagenes/tiendas/greenmangaming_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/greenmangaming_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/greenmangaming_icono.webp",
				Color = "#97ff9a",
				AdminEnseñar = true,
				AdminInteractuar = true
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			enlace = enlace.Replace(":", "%3A");
			enlace = enlace.Replace("/", "%2F");
			enlace = enlace.Replace("/", "%2F");
			enlace = enlace.Replace("?", "%3F");
			enlace = enlace.Replace("=", "%3D");

			return "https://greenmangaming.sjv.io/c/1382810/1219987/15105?u=" + enlace;
		}

		public static Tiendas2.Tienda GenerarGold()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "greenmangaminggold",
				Nombre = "Green Man Gaming Gold",
				ImagenLogo = "/imagenes/tiendas/greenmangaminggold_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/greenmangaminggold_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/greenmangaming_icono.webp",
				Color = "#97ff9a",
				AdminEnseñar = true,
				AdminInteractuar = true
			};

			return tienda;
		}

		public static async Task BuscarOfertas()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(Generar().Id, DateTime.Now, 0);

			string html = await Decompiladores.Estandar("https://api.greenmangaming.com/api/productfeed/prices/current?cc=es&cur=eur&lang=en");

			if (string.IsNullOrEmpty(html) == false)
			{
                GreenManGamingJuegos listaJuegos = new GreenManGamingJuegos();

                XmlSerializer xml = new XmlSerializer(typeof(GreenManGamingJuegos));
                listaJuegos = (GreenManGamingJuegos)xml.Deserialize(new StringReader(html));

                if (listaJuegos != null)
				{
					if (listaJuegos.Juegos != null)
					{
						if (listaJuegos.Juegos.Count > 0)
						{
							int juegos2 = 0;

							foreach (GreenManGamingJuego juego in listaJuegos.Juegos)
							{
								if (int.Parse(juego.Stock) > 0)
								{
									decimal precioBase = decimal.Parse(juego.PrecioBase);
									decimal precioRebajado = decimal.Parse(juego.PrecioRebajado);

									int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

									if (descuento > 0)
									{
										string nombre = WebUtility.HtmlDecode(juego.Nombre);

										string enlace = juego.Enlace;

										string imagen = juego.Imagen;

										JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = nombre,
											Enlace = enlace,
											Imagen = imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = drm,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										try
										{
											await BaseDatos.Tiendas.Comprobar.Resto(oferta);
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
										}

										juegos2 += 1;

										try
										{
											await BaseDatos.Admin.Actualizar.Tiendas(Generar().Id, DateTime.Now, juegos2);
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static async Task BuscarOfertasGold()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(GenerarGold().Id, DateTime.Now, 0);

			int juegos2 = 0;

			for (decimal precioMin = 0; precioMin < 80; precioMin += 1)
			{
				HttpClient cliente = new HttpClient();
				cliente.BaseAddress = new Uri("https://www.greenmangaming.com/");
				cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				decimal precioMax = precioMin + 1;

				string peticionEnBruto = $@"{{
					""requests"":[{{
						""indexName"":""prod_ProductSearch_ES_Mrp_ASC"",
						""params"":""query=xp-view-all&ruleContexts=%5B%22EUR%22%2C%22EUR_ES%22%2C%22ES%22%5D&filters=IsSellable%3Atrue%20AND%20AvailableRegions%3AES%20AND%20NOT%20ExcludeCountryCodes%3AES&hitsPerPage=1000&distinct=true&analytics=false&clickAnalytics=true&maxValuesPerFacet=10&facets=%5B%22Franchise%22%2C%22IsEarlyAccess%22%2C%22Genre%22%2C%22PlatformName%22%2C%22PublisherName%22%2C%22SupportedVrs%22%2C%22Regions.ES.ReleaseDateStatus%22%2C%22Regions.ES.Mrp%22%2C%22Regions.ES.IsOnSaleVip%22%2C%22Regions.ES.IsXpOffer%22%2C%22DrmName%22%5D&tagFilters=&facetFilters=%5B%5B%22DrmName%3ASteam%22%5D%5D&numericFilters=%5B%22Regions.ES.Mrp%3E%3D{precioMin}%22,%22Regions.ES.Mrp%3C{precioMax}%22%5D"" 
					}}]
				}}";

				HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://sczizsp09z-dsn.algolia.net/1/indexes/*/queries?x-algolia-agent=Algolia%20for%20JavaScript%20(4.5.1)%3B%20Browser%20(lite)%3B%20instantsearch.js%20(4.8.3)%3B%20JS%20Helper%20(3.2.2)&x-algolia-api-key=3bc4cebab2aa8cddab9e9a3cfad5aef3&x-algolia-application-id=SCZIZSP09Z");
				peticion.Content = new StringContent(peticionEnBruto, Encoding.UTF8, "application/json");

				HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

				string html = string.Empty;

				try
				{
					html = await respuesta.Content.ReadAsStringAsync();
				}
				catch { }

				if (string.IsNullOrEmpty(html) == false)
				{
					GreenManGamingGold datos = JsonSerializer.Deserialize<GreenManGamingGold>(html);

					if (datos != null)
					{
						if (datos.Resultados[0].Juegos.Count == 0)
						{
							break;
						}
						else
						{
							foreach (var juego in datos.Resultados[0].Juegos)
							{
								if (juego.SinStocks.ES == false)
								{
									decimal precioBase = juego.Regiones.ES.PrecioBase;
									decimal precioRebajado = juego.Regiones.ES.PrecioRebajado;

									int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

									if (descuento > 0)
									{
										JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = WebUtility.HtmlDecode(juego.Nombre),
											Enlace = "https://www.greenmangaming.com" + juego.Enlace,
											Imagen = "https://images.greenmangaming.com" + juego.Imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = GenerarGold().Id,
											DRM = drm,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										try
										{
											await BaseDatos.Tiendas.Comprobar.Resto(oferta);
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje(GenerarGold().Id, ex);
										}

										juegos2 += 1;

										try
										{
											await BaseDatos.Admin.Actualizar.Tiendas(GenerarGold().Id, DateTime.Now, juegos2);
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje(GenerarGold().Id, ex);
										}
									}
								}					
							}
						}
					}
				}
				else
				{
					break;
				}
			}
		}
	}

	#region Normal

	[XmlRoot("products")]
	public class GreenManGamingJuegos
	{
		[XmlElement("product")]
		public List<GreenManGamingJuego> Juegos { get; set; }
	}

	public class GreenManGamingJuego
	{
		[XmlElement("product_name")]
		public string Nombre { get; set; }

		[XmlElement("deep_link")]
		public string Enlace { get; set; }

		[XmlElement("image_url")]
		public string Imagen { get; set; }

		[XmlElement("price")]
		public string PrecioRebajado { get; set; }

		[XmlElement("rrp_price")]
		public string PrecioBase { get; set; }

		[XmlElement("drm")]
		public string DRM { get; set; }

		[XmlElement("stock_level")]
		public string Stock { get; set; }
	}

	#endregion

	#region Gold

	public class GreenManGamingGold
	{
		[JsonPropertyName("results")]
		public List<GreenManGamingGoldResultado> Resultados { get; set; }
	}

	public class GreenManGamingGoldResultado
	{
		[JsonPropertyName("hits")]
		public List<GreenManGamingGoldJuego> Juegos { get; set; }
	}

	public class GreenManGamingGoldJuego
	{
		[JsonPropertyName("DisplayName")]
		public string Nombre { get; set; }

		[JsonPropertyName("Url")]
		public string Enlace { get; set; }

		[JsonPropertyName("ImageUrl")]
		public string Imagen { get; set; }

		[JsonPropertyName("DrmName")]
		public string DRM { get; set; }

		[JsonPropertyName("Regions")]
		public GreenManGamingGoldJuegoRegiones Regiones { get; set; }

		[JsonPropertyName("OutOfStockRegions")]
		public GreenManGamingGoldJuegoStocks SinStocks { get; set; }
	}

	public class GreenManGamingGoldJuegoRegiones
	{
		[JsonPropertyName("ES")]
		public GreenManGamingGoldJuegoPrecios ES { get; set; }
	}

	public class GreenManGamingGoldJuegoPrecios
	{
		[JsonPropertyName("Mrp")]
		public decimal PrecioRebajado { get; set; }

		[JsonPropertyName("Rrp")]
		public decimal PrecioBase { get; set; }
	}

	public class GreenManGamingGoldJuegoStocks
	{
		[JsonPropertyName("ES")]
		public bool ES { get; set; }
	}

	#endregion
}
