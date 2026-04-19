using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Herramientas
{
	public static class ImagenesOptimizador
	{
		private static readonly int[] Tamaños = { 48, 96, 256 };
		private static readonly string[] Sufijos = { "-sm", "-md", "-lg" };
		private static readonly string[] ExtensionesValidas = { ".webp", ".png", ".jpg", ".jpeg" };

		public static void GenerarImagenesResponsive(string rutaBaseDatos)
		{
			try
			{
				var carpetas = new[]
				{
					Path.Combine(rutaBaseDatos, "favicons"),
					Path.Combine(rutaBaseDatos, "imagenes", "bundles"),
					Path.Combine(rutaBaseDatos, "imagenes", "drm"),
					Path.Combine(rutaBaseDatos, "imagenes", "otros"),
					Path.Combine(rutaBaseDatos, "imagenes", "streaming"),
					Path.Combine(rutaBaseDatos, "imagenes", "suscripciones"),
					Path.Combine(rutaBaseDatos, "imagenes", "tiendas"),
					Path.Combine(rutaBaseDatos, "imagenes", "webs")
				};

				int imagenesProcesadas = 0;
				int imagenesYaOptimizadas = 0;

				foreach (var carpeta in carpetas)
				{
					if (Directory.Exists(carpeta) == false)
					{
						continue;
					}

					List<string>? archivos = Directory.GetFiles(carpeta).Where(f => ExtensionesValidas.Contains(Path.GetExtension(f).ToLower())).ToList();

					foreach (var archivo in archivos)
					{
						if (EsImagenRedimensionada(archivo) == true)
						{
							imagenesYaOptimizadas += 1;
							continue;
						}

						if (YaExistenVariantes(archivo) == true)
						{
							imagenesYaOptimizadas += 1;
							continue;
						}

						try
						{
							GenerarVariantes(archivo);
							imagenesProcesadas += 1;
						}
						catch (Exception ex)
						{
							
						}
					}
				}

			}
			catch (Exception ex)
			{

			}
		}

		private static bool EsImagenRedimensionada(string archivo)
		{
			var nombre = Path.GetFileNameWithoutExtension(archivo);
			return nombre.EndsWith("-sm") || nombre.EndsWith("-md") || nombre.EndsWith("-lg");
		}

		private static bool YaExistenVariantes(string archivo)
		{
			var directorio = Path.GetDirectoryName(archivo);
			var nombre = Path.GetFileNameWithoutExtension(archivo);

			var sm = Path.Combine(directorio!, $"{nombre}-sm.webp");
			var md = Path.Combine(directorio!, $"{nombre}-md.webp");
			var lg = Path.Combine(directorio!, $"{nombre}-lg.webp");

			bool existenTodas = File.Exists(sm) && File.Exists(md) && File.Exists(lg);

			return existenTodas;
		}

		private static void GenerarVariantes(string rutaOriginal)
		{
			using (var imagen = Image.Load(rutaOriginal))
			{
				var directorio = Path.GetDirectoryName(rutaOriginal);
				var nombre = Path.GetFileNameWithoutExtension(rutaOriginal);

				for (int i = 0; i < Tamaños.Length; i++)
				{
					var redimensionada = imagen.Clone(x =>
						x.Resize(Tamaños[i], Tamaños[i], KnownResamplers.Lanczos3)
					);

					var rutaNueva = Path.Combine(directorio!, $"{nombre}{Sufijos[i]}.webp");

					int calidad = Tamaños[i] switch
					{
						48 => 70,   // -sm: más compresión
						96 => 75,   // -md: medio
						256 => 85,  // -lg: mejor calidad
						_ => 80
					};

					redimensionada.SaveAsWebp(rutaNueva, new WebpEncoder { Quality = calidad });
				}
			}
		}
	}
}