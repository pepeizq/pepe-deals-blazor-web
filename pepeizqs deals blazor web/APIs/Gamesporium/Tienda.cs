#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.Gamesporium
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesporium",
				Nombre = "Gamesporium",
				ImagenLogo = "/imagenes/tiendas/gamesporium_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesporium_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesporium_icono.webp",
				Color = "#141414",
				AdminEnseñar = true,
				AdminInteractuar = true
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			if (enlace.Contains("?") == true)
			{
				return enlace + "&utm_source=affiliate&utm_medium=pepedeals";
			}
			else
			{
				return enlace + "?utm_source=affiliate&utm_medium=pepedeals";
			}
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				//enlace = "https://feed.mulwi.com/f/b74893-2/feed.xml";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://feed.mulwi.com/f/b74893-2/general_us.xml";
			}

			if (string.IsNullOrEmpty(enlace) == true)
			{
				return;
			}

			string html = await Decompiladores.Estandar(enlace);

			if (string.IsNullOrEmpty(html) == false)
			{
				GamesporiumJuegos listaJuegos = new GamesporiumJuegos();
				XmlSerializer xml = new XmlSerializer(typeof(GamesporiumJuegos));

				using (StringReader lector = new StringReader(html))
				{
					listaJuegos = (GamesporiumJuegos)xml.Deserialize(lector);
				}

				if (listaJuegos?.Juegos?.Count > 0)
				{
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					foreach (GamesporiumJuego juego in listaJuegos.Juegos)
					{
						bool buscar = true;

						if (string.IsNullOrEmpty(juego.PaisesAprobados) == false)
						{
							List<string> listaPaisesAprobados = new List<string>();

							string[] datosPartidos = juego.PaisesAprobados.Split(',');
							listaPaisesAprobados.AddRange(datosPartidos);

							if (listaPaisesAprobados.Count > 0)
							{
								bool encontrado = false;
								foreach (var pais in listaPaisesAprobados)
								{
									if (region == TiendaRegion.Europa && pais.ToLower() == "es")
									{
										encontrado = true;
										break;
									}
									else if (region == TiendaRegion.EstadosUnidos && pais.ToLower() == "us")
									{
										encontrado = true;
										break;
									}
								}

								if (encontrado == false)
								{
									buscar = false;
								}
							}
						}

						if (region == TiendaRegion.Europa && juego.Moneda.ToLower() != "eur")
						{
							buscar = false;
						}
						else if (region == TiendaRegion.EstadosUnidos && juego.Moneda.ToLower() != "usd")
						{
							buscar = false;
						}

						if (juego.StockEstado.ToLower() == "false")
						{
							buscar = false;
						}

						if (string.IsNullOrEmpty(juego.PrecioRebajado) == true)
						{
							buscar = false;
						}

						if (buscar == true)
						{
							decimal precioBase = decimal.Parse(juego.PrecioBase);
							decimal precioRebajado = decimal.Parse(juego.PrecioRebajado);

							int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

							if (descuento > 0)
							{
								string nombre = WebUtility.HtmlDecode(juego.Nombre);

								string enlaceJuego = juego.Enlace;

								string imagen = juego.Imagen;

								JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlaceJuego,
									Imagen = imagen,
									Precio = precioRebajado,
									Descuento = descuento,
									Tienda = Generar().Id,
									DRM = drm,
									FechaDetectado = DateTime.Now,
									FechaActualizacion = DateTime.Now
								};

								if (region == TiendaRegion.Europa)
								{
									oferta.Moneda = JuegoMoneda.Euro;
								}
								else if (region == TiendaRegion.EstadosUnidos)
								{
									oferta.Moneda = JuegoMoneda.Dolar;
								}

								ofertas.Add(oferta);
							}
						}
					}

					if (ofertas?.Count > 0)
					{
						int juegos2 = 0;

						int tamaño = 500;
						var lotes = ofertas
							.Select((oferta, indice) => new { oferta, indice })
							.GroupBy(x => x.indice / tamaño)
							.Select(g => g.Select(x => x.oferta).ToList())
							.ToList();

						foreach (var lote in lotes)
						{
							try
							{
								await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}

							juegos2 += lote.Count;

							try
							{
								await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, juegos2);
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

	[XmlRoot("rss")]
	public class GamesporiumJuegos
	{
		[XmlArray("products")]
		[XmlArrayItem("product")]
		public List<GamesporiumJuego> Juegos { get; set; }
	}

	public class GamesporiumJuego
	{
		[XmlElement("ProductName")]
		public string Nombre { get; set; }

		[XmlElement("ProductURL")]
		public string Enlace { get; set; }

		[XmlElement("ImageURL")]
		public string Imagen { get; set; }

		[XmlElement("Currency")]
		public string Moneda { get; set; }

		[XmlElement("CompareAtPrice")]
		public string PrecioRebajado { get; set; }

		[XmlElement("Price")]
		public string PrecioBase { get; set; }

		[XmlElement("DRM")]
		public string DRM { get; set; }

		[XmlElement("WhitelistCountries")]
		public string PaisesAprobados { get; set; }

		[XmlElement("StockStatus")]
		public string StockEstado { get; set; }
	}
}
