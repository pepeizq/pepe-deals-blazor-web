#nullable disable

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace Herramientas.Ficheros
{
	public static class Imagenes
	{
		public static string[] hostsPermitidos = new[]
		{
			"i.imgur.com",
			"shared.cloudflare.steamstatic.com",
			"shared.fastly.steamstatic.com",
			"avatars.steamstatic.com",
			"fanatical.imgix.net"
		};

		public static string ServidorExterno(string enlace, int ancho = 0, int alto = 0)
		{
			if (string.IsNullOrEmpty(enlace) == false)
			{
				if (enlace.Contains("https://pepe.deals") == true)
				{
					enlace = enlace.Replace("https://pepe.deals", null);
				}

				//bool añadirServidor = true;

				//if (enlace.Contains("https://hb.imgix.net/") == true)
				//{
				//	añadirServidor = false;
				//}
				//else if (enlace.Contains("https://i.imgur.com/") == true)
				//{
				//	añadirServidor = false;
				//}
    //            else if (enlace.Contains("https://media.greenmangamingbundles.com/") == true)
    //            {
    //                añadirServidor = false;
    //            }

    //            if (añadirServidor == true)
				//{
				//	//enlace = "https://wsrv.nl/?n=-1&output=webp&url=" + enlace;

				//	//if (ancho > 0 && alto > 0)
				//	//{
				//	//	enlace = enlace + "&w=" + ancho + "&h=" + alto + "&dpr=2";
				//	//}
				//}
			}

			return enlace;
		}
	}

	public static class Constructor
	{
		public enum LayoutModo
		{
			Vertical,  
			Horizontal 
		}

		public static async Task<CombinarImagenResultado> Imagen(List<string> imagenesEnlaces, string encabezadoEnlace = null, LayoutModo modo = LayoutModo.Vertical)
		{
			try
			{
				Image encabezadoImagen = null;
				int encabezadoAncho = 0;
				int encabezadoAltura = 0;
				int encabezadoPadding = 40;
				int encabezadoAlturaMaxima = 150;
				int encabezadoAnchoMaximo = 250;
				int separadorAltura = 2;
				int separadorAncho = 2;

				if (string.IsNullOrEmpty(encabezadoEnlace) == false)
				{
					try
					{
						var httpClient2 = new HttpClient();
						var respuesta = await httpClient2.GetAsync(encabezadoEnlace);
						respuesta.EnsureSuccessStatusCode();
						var stream = await respuesta.Content.ReadAsStreamAsync();
						encabezadoImagen = Image.Load(stream);

						double aspectRatio = (double)encabezadoImagen.Width / encabezadoImagen.Height;

						if (modo == LayoutModo.Vertical)
						{
							int finalEncabezadoAltura = Math.Min(encabezadoImagen.Height, encabezadoAlturaMaxima);
							encabezadoAltura = finalEncabezadoAltura + (encabezadoPadding * 2) + separadorAltura;
						}
						else
						{
							int finalEncabezadoAncho = Math.Min(encabezadoImagen.Width, encabezadoAnchoMaximo);
							encabezadoAncho = finalEncabezadoAncho + (encabezadoPadding * 2) + separadorAncho;
						}
					}
					catch (Exception ex)
					{
						return new CombinarImagenResultado
						{
							Exito = false,
							ErrorMensaje = $"Error al descargar imagen de encabezado: {ex.Message}",
							RequestMalo = true
						};
					}
				}

				List<Image> imagenes = new List<Image>();
				HttpClient httpClient = new HttpClient();

				foreach (string imagenEnlace in imagenesEnlaces)
				{
					try
					{
						var response = await httpClient.GetAsync(imagenEnlace);
						response.EnsureSuccessStatusCode();
						var stream = await response.Content.ReadAsStreamAsync();
						imagenes.Add(Image.Load(stream));
					}
					catch (Exception ex)
					{
						return new CombinarImagenResultado
						{
							Exito = false,
							ErrorMensaje = $"Error al descargar imagen de {imagenEnlace}: {ex.Message}",
							RequestMalo = true
						};
					}
				}

				if (imagenes.Count == 0)
				{
					return new CombinarImagenResultado
					{
						Exito = false,
						ErrorMensaje = "No se pudieron descargar las imágenes",
						RequestMalo = true
					};
				}

				int imagenAncho = 300;
				int imagenAltura = 440;
				int padding = 30;
				int columnasPorFila = 4;

				int filas = (int)Math.Ceiling((double)imagenes.Count / columnasPorFila);
				int gridAncho = (imagenAncho * columnasPorFila) + (padding * (columnasPorFila + 1));
				int gridAltura = (imagenAltura * filas) + (padding * (filas + 1));

				int totalAncho = modo == LayoutModo.Vertical ? gridAncho : gridAncho + encabezadoAncho;
				int totalAltura = modo == LayoutModo.Vertical ? gridAltura + encabezadoAltura : gridAltura;

				var imagenCombinada = new Image<Rgba32>(totalAncho, totalAltura);

				var startColorRgba = new Rgba32(0, 32, 51, 255); // #002033
				var endColorRgba = new Rgba32(0, 44, 71, 255);   // #002c47

				double angleRadians = 66 * Math.PI / 180;
				double cosAngle = Math.Cos(angleRadians);
				double sinAngle = Math.Sin(angleRadians);
				double maxDistance = Math.Sqrt(totalAncho * totalAncho + totalAltura * totalAltura);

				imagenCombinada.Mutate(ctx =>
				{
					for (int y = 0; y < totalAltura; y++)
					{
						for (int x = 0; x < totalAncho; x++)
						{
							double distance = (x * cosAngle) + (y * sinAngle);
							float ratio = (float)Math.Clamp(distance / maxDistance, 0, 1);

							var blendedColor = new Rgba32(
								(byte)((1 - ratio) * startColorRgba.R + ratio * endColorRgba.R),
								(byte)((1 - ratio) * startColorRgba.G + ratio * endColorRgba.G),
								(byte)((1 - ratio) * startColorRgba.B + ratio * endColorRgba.B),
								255
							);

							imagenCombinada[x, y] = blendedColor;
						}
					}
				});

				if (encabezadoImagen != null)
				{
					if (modo == LayoutModo.Vertical)
					{
						ProcesarEncabezadoVertical(ref imagenCombinada, encabezadoImagen, totalAncho, encabezadoPadding, encabezadoAlturaMaxima, separadorAltura);
					}
					else
					{
						ProcesarEncabezadoHorizontal(ref imagenCombinada, encabezadoImagen, gridAltura, encabezadoPadding, encabezadoAnchoMaximo, separadorAncho);
					}

					encabezadoImagen.Dispose();
				}

				int gridArranqueX = modo == LayoutModo.Horizontal ? encabezadoAncho : 0;
				int gridArranqueY = modo == LayoutModo.Vertical ? encabezadoAltura : 0;

				int xPosicion = gridArranqueX + padding;
				int yPosicion = gridArranqueY + padding;

				for (int i = 0; i < imagenes.Count; i++)
				{
					var img = imagenes[i];
					var imagenRedimensionada = img.Clone(x => x.Resize(imagenAncho, imagenAltura));
					imagenCombinada.Mutate(ctx => ctx.DrawImage(imagenRedimensionada, new SixLabors.ImageSharp.Point(xPosicion, yPosicion), 1));

					xPosicion += imagenAncho + padding;
					imagenRedimensionada.Dispose();

					if ((i + 1) % columnasPorFila == 0)
					{
						xPosicion = gridArranqueX + padding;
						yPosicion += imagenAltura + padding;
					}
				}

				string nombreFichero = $"{Guid.NewGuid()}.jpeg";
				string rutaDirectorio = Path.Combine(AppContext.BaseDirectory, "imagenes", "noticias");

				if (Directory.Exists(rutaDirectorio) == false)
				{
					Directory.CreateDirectory(rutaDirectorio);
				}

				string rutaFichero = Path.Combine(rutaDirectorio, nombreFichero);

				try
				{
					await imagenCombinada.SaveAsJpegAsync(rutaFichero);

					foreach (var img in imagenes)
					{
						img.Dispose();
					}
						
					imagenCombinada.Dispose();

					return new CombinarImagenResultado
					{
						Exito = true,
						DirectorioFichero = $"/imagenes/noticias/{nombreFichero}"
					};
				}
				catch (Exception ex)
				{
					return new CombinarImagenResultado
					{
						Exito = false,
						ErrorMensaje = $"Error al guardar la imagen: {ex.Message}",
						RequestMalo = false
					};
				}
			}
			catch (Exception ex)
			{
				return new CombinarImagenResultado
				{
					Exito = false,
					ErrorMensaje = $"Error al combinar imágenes: {ex.Message}",
					RequestMalo = false
				};
			}
		}

		private static void ProcesarEncabezadoVertical(ref Image<Rgba32> imagenCombinada, Image encabezadoImagen, int totalWidth, int padding, int alturaMaxima, int separadorAltura)
		{
			int headerMaxWidth = totalWidth - (padding * 2);
			double aspectRatio = (double)encabezadoImagen.Width / encabezadoImagen.Height;

			int newHeaderWidth = headerMaxWidth;
			int newHeaderHeight = (int)(headerMaxWidth / aspectRatio);

			if (newHeaderHeight > alturaMaxima)
			{
				newHeaderHeight = alturaMaxima;
				newHeaderWidth = (int)(alturaMaxima * aspectRatio);
			}

			var resizedHeader = encabezadoImagen.Clone(x => x.Resize(newHeaderWidth, newHeaderHeight));
			int offsetX = (totalWidth - newHeaderWidth) / 2;
			int offsetY = padding;

			imagenCombinada.Mutate(ctx => ctx.DrawImage(resizedHeader, new SixLabors.ImageSharp.Point(offsetX, offsetY), 1));

			int separatorY = offsetY + newHeaderHeight + padding;
			int separatorMargin = 40;
			for (int x = separatorMargin; x < totalWidth - separatorMargin; x++)
			{
				imagenCombinada[x, separatorY] = new Rgba32(100, 100, 100, 255);
			}

			resizedHeader.Dispose();
		}

		private static void ProcesarEncabezadoHorizontal(ref Image<Rgba32> imagenCombinada, Image encabezadoImagen, int gridHeight, int padding, int anchoMaximo, int separadorAncho)
		{
			int headerMaxHeight = gridHeight - (padding * 2);
			double aspectRatio = (double)encabezadoImagen.Width / encabezadoImagen.Height;

			int newHeaderWidth = (int)(headerMaxHeight * aspectRatio);
			int newHeaderHeight = headerMaxHeight;

			if (newHeaderWidth > anchoMaximo)
			{
				newHeaderWidth = anchoMaximo;
				newHeaderHeight = (int)(anchoMaximo / aspectRatio);
			}

			var resizedHeader = encabezadoImagen.Clone(x => x.Resize(newHeaderWidth, newHeaderHeight));
			int offsetX = padding;
			int offsetY = (gridHeight - newHeaderHeight) / 2;

			imagenCombinada.Mutate(ctx => ctx.DrawImage(resizedHeader, new SixLabors.ImageSharp.Point(offsetX, offsetY), 1));

			resizedHeader.Dispose();
		}
	}

	public class CombinarImagenResultado
	{
		public bool Exito { get; set; }
		public string ErrorMensaje { get; set; } = string.Empty;
		public bool RequestMalo { get; set; }
		public string DirectorioFichero { get; set; }
	}
}
