#nullable disable

using Microsoft.AspNetCore.Mvc;
using Noticias;

namespace Herramientas.Redireccionador
{
	public class Noticias : Controller
	{
		[ResponseCache(Duration = 6000)]
		[HttpGet("link/news/{id}/")]
		public async Task<IActionResult> AbrirNoticia(int id)
		{
			Noticia noticia = await global::BaseDatos.Noticias.Buscar.UnaNoticia(id);

			if (string.IsNullOrEmpty(noticia.Enlace) == false)
			{
				if (noticia.NoticiaTipo == NoticiaTipo.Bundles)
				{
					return Redirect(Herramientas.EnlaceAcortador.Generar(noticia.Enlace, noticia.BundleTipo, false, false));
				}
				else if (noticia.NoticiaTipo == NoticiaTipo.Gratis)
				{
					return Redirect(Herramientas.EnlaceAcortador.Generar(noticia.Enlace, noticia.GratisTipo, false, false));
				}
				else if (noticia.NoticiaTipo == NoticiaTipo.Suscripciones)
				{
					return Redirect(Herramientas.EnlaceAcortador.Generar(noticia.Enlace, noticia.SuscripcionTipo, false, false));
				}
				else
				{
					return Redirect(noticia.Enlace);
				}
			}

			return Redirect("~/news/" + id.ToString() + "/");
		}
	}
}
