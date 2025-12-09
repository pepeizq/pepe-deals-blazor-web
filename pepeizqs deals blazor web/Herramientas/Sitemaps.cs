#nullable disable

using Microsoft.AspNetCore.Mvc;
using Noticias;
using System.Text;

namespace Herramientas
{
	public class Sitemaps : Controller
	{
		[HttpGet("sitemap.xml")]
		public async Task<IActionResult> Maestro()
		{
			string dominio = HttpContext.Request.Host.Value;

			List<string> sitemaps = ["https://" + dominio + "/sitemap-main.xml"];

			int cantidadJuegos = await global::BaseDatos.Sitemaps.Buscar.Cantidad("juegos");

			if (cantidadJuegos > 0)
			{
				int segmentacion = cantidadJuegos / 1000;

				int i = 0;
				while (i <= segmentacion)
				{
					sitemaps.Add("https://" + dominio + "/sitemap-games-" + i.ToString() + ".xml");

					i += 1;
				}
			}

			int cantidadBundles = await global::BaseDatos.Sitemaps.Buscar.Cantidad("bundles");

			if (cantidadBundles > 0)
			{
				int segmentacion = cantidadBundles / 100;

				int i = 0;
				while (i <= segmentacion)
				{
					sitemaps.Add("https://" + dominio + "/sitemap-bundles-" + i.ToString() + ".xml");

					i += 1;
				}
			}

			int cantidadGratis = await global::BaseDatos.Sitemaps.Buscar.Cantidad("gratis");

			if (cantidadGratis > 0)
			{
				int segmentacion = cantidadGratis / 1000;

				int i = 0;
				while (i <= segmentacion)
				{
					sitemaps.Add("https://" + dominio + "/sitemap-free-" + i.ToString() + ".xml");

					i += 1;
				}
			}

			int cantidadSuscripciones = await global::BaseDatos.Sitemaps.Buscar.Cantidad("suscripciones");

			if (cantidadSuscripciones > 0)
			{
				int segmentacion = cantidadSuscripciones / 1000;

				int i = 0;
				while (i <= segmentacion)
				{
					sitemaps.Add("https://" + dominio + "/sitemap-subscriptions-" + i.ToString() + ".xml");

					i += 1;
				}
			}

			int cantidadNoticias = await global::BaseDatos.Sitemaps.Buscar.Cantidad("noticias");

			if (cantidadNoticias > 0)
			{
				int segmentacion = cantidadNoticias / 100;

				int i = 0;
				while (i <= segmentacion)
				{
					sitemaps.Add("https://" + dominio + "/sitemap-news-en-" + i.ToString() + ".xml");

					i += 1;
				}

				//i = 0;
				//while (i <= segmentacion)
				//{
				//	sitemaps.Add("https://" + dominio + "/sitemap-news-es-" + i.ToString() + ".xml");

				//	i += 1;
				//}
			}

			int cantidadCurators = await global::BaseDatos.Sitemaps.Buscar.Cantidad("curators");

			if (cantidadCurators > 0)
			{
				int segmentacion = cantidadCurators / 1000;

				int i = 0;
				while (i <= segmentacion)
				{
					sitemaps.Add("https://" + dominio + "/sitemap-curators-" + i.ToString() + ".xml");

					i += 1;
				}
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			sb.Append("<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

			foreach (var sitemap in sitemaps)
			{
				sb.Append("<sitemap>");
				sb.Append("<loc>" + sitemap + "</loc>");
				sb.Append("</sitemap>");
			}

			sb.Append("</sitemapindex>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-main.xml")]
		public async Task<IActionResult> Principal()
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\">\r\n");

			string textoIndex = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/</loc>" + Environment.NewLine +
					"<changefreq>hourly</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoIndex);

			string textoBundles = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/bundles/</loc>" + Environment.NewLine +
					"<changefreq>daily</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoBundles);

			string textoGratis = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/free/</loc>" + Environment.NewLine +
					"<changefreq>daily</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoGratis);

			string textoSuscripciones = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/subscriptions/</loc>" + Environment.NewLine +
					"<changefreq>daily</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoSuscripciones);

			foreach (var suscripcion in Suscripciones2.SuscripcionesCargar.GenerarListado())
			{
				if (suscripcion.AdminInteractuar == true)
				{
					string textoSuscripcion = "<url>" + Environment.NewLine +
						"<loc>https://" + dominio + "/subscriptions/" + Herramientas.EnlaceAdaptador.Nombre(suscripcion.Nombre.ToLower()) + "/</loc>" + Environment.NewLine +
						"<changefreq>daily</changefreq>" + Environment.NewLine +
						"</url>";

					sb.Append(textoSuscripcion);
				}
			}

			string textoMinimos = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/historical-lows/</loc>" + Environment.NewLine +
					"<changefreq>hourly</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoMinimos);

			string textoNoticias = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/last-news/</loc>" + Environment.NewLine +
					"<changefreq>hourly</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoNoticias);

			string textoActualizaciones = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/last-updates/</loc>" + Environment.NewLine +
					"<changefreq>weekly</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoActualizaciones);

			string textoAñadidos = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/last-added/</loc>" + Environment.NewLine +
					"<changefreq>hourly</changefreq>" + Environment.NewLine +
					"</url>";

			sb.Append(textoAñadidos);

			string textoPatreon = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/patreon/</loc>" + Environment.NewLine +
					"</url>";

			sb.Append(textoPatreon);

			string textoComparador = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/compare/</loc>" + Environment.NewLine +
					"</url>";

			sb.Append(textoComparador);

			string textoApi = "<url>" + Environment.NewLine +
					"<loc>https://" + dominio + "/api/</loc>" + Environment.NewLine +
					"</url>";

			sb.Append(textoApi);

			List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Ultimas(20);

			if (noticias.Count > 0)
			{
				foreach (Noticia noticia in noticias)
				{
					DateTime fechaTemporal = noticia.FechaEmpieza;
					fechaTemporal = fechaTemporal.AddDays(7);

					if (fechaTemporal > DateTime.Now)
					{
						string titulo = noticia.TituloEn;
						titulo = titulo.Replace("&", "&amp;");

						string texto = "<url>" + Environment.NewLine +
						"<loc>https://" + dominio + "/news/" + noticia.Id.ToString() + "/" + EnlaceAdaptador.Nombre(noticia.TituloEn) + "/</loc>" + Environment.NewLine +
						"<news:news>" + Environment.NewLine +
						"<news:publication>" + Environment.NewLine +
						"<news:name>pepe's deals</news:name>" + Environment.NewLine +
						"<news:language>en</news:language>" + Environment.NewLine +
						"</news:publication>" + Environment.NewLine +
						"<news:publication_date>" + noticia.FechaEmpieza.ToString("yyyy-MM-dd") + "</news:publication_date>" + Environment.NewLine +
						"<news:title>" + titulo + "</news:title>" + Environment.NewLine +
						"</news:news>" + Environment.NewLine +
						"</url>";

						sb.Append(texto);
					}
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-games-{i:int}.xml")]
		public async Task<IActionResult> Juegos(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\">\r\n");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 1000;
			maximo = (i + 1) * 1000;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.Juegos(dominio, minimo - 1, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
                    sb.Append(linea);
                }
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

        [HttpGet("sitemap-bundles-{i:int}.xml")]
		public async Task<IActionResult> Bundles(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\">\r\n");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 100;
			maximo = (i + 1) * 100;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.Bundles(dominio, minimo - 1, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
					sb.Append(linea);
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-free-{i:int}.xml")]
		public async Task<IActionResult> Gratis(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\">\r\n");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 1000;
			maximo = (i + 1) * 1000;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.Gratis(dominio, minimo - 1, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
					sb.Append(linea);
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-subscriptions-{i:int}.xml")]
		public async Task<IActionResult> Suscripciones(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\">\r\n");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 1000;
			maximo = (i + 1) * 1000;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.Suscripciones(dominio, minimo - 1, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
					sb.Append(linea);
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-news-en-{i:int}.xml")]
		public async Task<IActionResult> NoticiasIngles(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\"\r\n    xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 100;
			maximo = (i + 1) * 100;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.NoticiasIngles(dominio, minimo - 1, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
					sb.Append(linea);
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-news-es-{i:int}.xml")]
		public async Task<IActionResult> NoticiasEspañol(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\"\r\n    xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 100;
			maximo = (i + 1) * 100;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.NoticiasEspañol(dominio, minimo - 1, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
					sb.Append(linea);
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}

		[HttpGet("sitemap-curators-{i:int}.xml")]
		public async Task<IActionResult> Curators(int i)
		{
			string dominio = HttpContext.Request.Host.Value;

			StringBuilder sb = new StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"\r\n        xmlns:news=\"http://www.google.com/schemas/sitemap-news/0.9\">\r\n");

			int minimo = 0;
			int maximo = 0;

			minimo = i * 1000;
			maximo = (i + 1) * 1000;

			if (i == 0)
			{
				minimo = 0;
			}

			List<string> lineas = await global::BaseDatos.Sitemaps.Buscar.Curators(dominio, minimo, maximo);

			if (lineas.Count > 0)
			{
				foreach (var linea in lineas)
				{
					sb.Append(linea);
				}
			}

			sb.Append("</urlset>");

			return new ContentResult
			{
				ContentType = "application/xml",
				Content = sb.ToString(),
				StatusCode = 200
			};
		}
	}
}
