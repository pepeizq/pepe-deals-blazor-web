//https://www.fanatical.com/api/products-group/killer-bundle/en
//https://www.fanatical.com/api/all/en
//https://www.fanatical.com/api/algolia/bundles?altRank=false
//https://www.fanatical.com/api/algolia/onsaleresults

#nullable disable

using Herramientas;
using Juegos;
using Microsoft.VisualBasic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APIs.Fanatical
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "fanatical",
				Nombre = "Fanatical",
				ImagenLogo = "/imagenes/tiendas/fanatical_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/fanatical_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/fanatical_icono.webp",
				Color = "#ffcf89",
				AdminEnseñar = true,
				AdminInteractuar = true
            };

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?ref=pepeizq";
		}

		public static async Task BuscarOfertas()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(Generar().Id, DateTime.Now, 0);

			string html = await Decompiladores.Estandar("https://feed.fanatical.com/feed");

			if (html != null)
			{
				html = html.Replace("{" + Strings.ChrW(34) + "title" + Strings.ChrW(34), ",{" + Strings.ChrW(34) + "title" + Strings.ChrW(34));

				html = html.Remove(0, 1);
				html = "[" + html + "]";

				List<FanaticalJuego> juegos = JsonSerializer.Deserialize<List<FanaticalJuego>>(html);

				if (juegos?.Count > 0)
				{
					int juegos2 = 0;

					foreach (FanaticalJuego juego in juegos)
					{
						bool autorizar = true;

						if (juego.Regiones?.Count > 0)
						{
							autorizar = false;

							foreach (string region in juego.Regiones)
							{
								if (region == "ES")
								{
									autorizar = true;
								}
							}
						}

						if (juego.Disponible == false)
						{
							autorizar = false;
						}

						//if (juego.Tipo == "bundle")
						//{
						//	autorizar = false;
						//}

						if (autorizar == true)
						{
							string descuentoTexto = juego.Descuento?.ToString();

							if (descuentoTexto != null)
							{
								if (descuentoTexto.Contains(".") == true)
								{
									int int1 = descuentoTexto.IndexOf(".");
									descuentoTexto = descuentoTexto.Remove(int1, descuentoTexto.Length - int1);
								}

								int descuento = 0;

								try
								{
									descuento = int.Parse(descuentoTexto);
								}
								catch { }

								if (descuento > 0)
								{
									string nombre = WebUtility.HtmlDecode(juego.Nombre);

									string imagen = juego.Imagen;

									string enlace = juego.Enlace;

									decimal precioRebajado = juego.PrecioRebajado.EUR ?? 0;

									if (juego.DRMs?.Count > 0 && precioRebajado > 0)
									{
										string drmTexto = juego.DRMs[0];
										JuegoDRM drm = JuegoDRM2.Traducir(drmTexto, Generar().Id);

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

										if (juego.FechaTermina != null)
										{
											if (Convert.ToDouble(juego.FechaTermina) > 0)
											{
												DateTime fechaTermina = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
												fechaTermina = fechaTermina.AddSeconds(Convert.ToDouble(juego.FechaTermina));
												fechaTermina = fechaTermina.ToLocalTime();

												oferta.FechaTermina = fechaTermina;
											}
										}

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
	}

    #region Clases

    public class FanaticalJuego
	{
		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("sku")]
		public string Id { get; set; }

		[JsonPropertyName("drm")]
		public List<string> DRMs { get; set; }

		[JsonPropertyName("image")]
		public string Imagen { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("discount_percent")]
		public decimal? Descuento { get; set; }

		[JsonPropertyName("expiry")]
		public string FechaTermina { get; set; }

		[JsonPropertyName("current_price")]
		public FanaticalJuegoPrecio PrecioRebajado { get; set; }

		[JsonPropertyName("regular_price")]
		public FanaticalJuegoPrecio PrecioBase { get; set; }

		[JsonPropertyName("regions")]
		public List<string> Regiones { get; set; }

		[JsonPropertyName("bundle_games")]
		public FanaticalJuegoBundle Bundle { get; set; }

		[JsonPropertyName("type")]
		public string Tipo { get; set; }

		[JsonPropertyName("in_stock")]
		public bool Disponible { get; set; }
	}

	public class FanaticalJuegoPrecio
	{
		[JsonPropertyName("USD")]
		public decimal? USD { get; set; }

		[JsonPropertyName("GBP")]
		public decimal? GBP { get; set; }

		[JsonPropertyName("EUR")]
		public decimal? EUR { get; set; }
	}

	public class FanaticalJuegoBundle
	{
		[JsonPropertyName("1")]
		public FanaticalJuegoBundleTier Tier1 { get; set; }

		[JsonPropertyName("2")]
		public FanaticalJuegoBundleTier Tier2 { get; set; }

		[JsonPropertyName("3")]
		public FanaticalJuegoBundleTier Tier3 { get; set; }
	}

	public class FanaticalJuegoBundleTier
	{
		[JsonPropertyName("items")]
		public List<FanaticalJuegoBundleJuego> Juegos { get; set; }
	}

	public class FanaticalJuegoBundleJuego
	{
		[JsonPropertyName("steam_id")]
		public int? SteamId { get; set; }
	}

	//--------------------------------------------------------------

	public class FanaticalBundle
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("cover")]
		public string Imagen { get; set; }

		[JsonPropertyName("price")]
		public FanaticalJuegoPrecio Precio { get; set; }

		[JsonPropertyName("available_valid_until")]
		public string FechaTermina { get; set; }
	}

    #endregion
}
