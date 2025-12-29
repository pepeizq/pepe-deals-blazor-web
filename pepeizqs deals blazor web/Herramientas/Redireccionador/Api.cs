#nullable disable

using Bundles2;
using Juegos;
using Microsoft.AspNetCore.Mvc;
using Noticias;

namespace Herramientas.Redireccionador
{
	public class Api : Controller
	{
		[ResponseCache(Duration = 6000)]
		[HttpGet("api/game/{id}")]
		public async Task<IActionResult> Juego(int id)
		{
			Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(id.ToString());

			if (juego != null)
			{
				return Ok(juego);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/steam/{id}")]
		public async Task<IActionResult> JuegoSteam(int id)
		{
			Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(null, id.ToString());

			if (juego != null)
			{
				return Ok(juego);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/gog/{id}")]
		public async Task<IActionResult> JuegoGog(string id)
		{
			Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(null, null, id);

			if (juego != null)
			{
				return Ok(juego);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/bundle/{id}/")]
		public async Task<IActionResult> Bundle(int id)
		{
			Bundle bundle = await global::BaseDatos.Bundles.Buscar.UnBundle(id);

			if (bundle != null)
			{
				return Ok(bundle);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/bundle/{id}/{juegos}")]
		public async Task<IActionResult> Bundle(int id, string juegos)
		{
			Bundle bundle = await global::BaseDatos.Bundles.Buscar.UnBundle(id);

			if (bundle != null)
			{
				if (string.IsNullOrEmpty(juegos) == false)
				{
					if (juegos.ToLower() == "games")
					{
						foreach (var juego in bundle.Juegos)
						{
							juego.Juego = await global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
						}
					}
				}

				return Ok(bundle);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-bundles/")]
		public async Task<IActionResult> BundlesUltimos()
		{
			List<Bundle> bundles = await global::BaseDatos.Bundles.Buscar.Ultimos(5);

			if (bundles?.Count > 0)
			{
				return Ok(bundles);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-bundles/{cantidad}/")]
		public async Task<IActionResult> BundlesUltimos(int cantidad)
		{
			int cantidadFinal = 5;

			if (cantidad > 0)
			{
				cantidadFinal = cantidad;
			}

			if (cantidadFinal > 25)
			{
				cantidadFinal = 25;
			}

			List<Bundle> bundles = await global::BaseDatos.Bundles.Buscar.Ultimos(cantidadFinal);

			if (bundles?.Count > 0)
			{
				return Ok(bundles);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/news/{id}")]
		public async Task<IActionResult> Noticia(int id)
		{
			Noticia noticia = await global::BaseDatos.Noticias.Buscar.UnaNoticia(id);

			if (noticia != null)
			{
				return Ok(noticia);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-news/")]
		public async Task<IActionResult> NoticiasUltimas()
		{
			List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Ultimas(5);

			if (noticias != null)
			{
				return Ok(noticias);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-news/{cantidad}/")]
		public async Task<IActionResult> NoticiasUltimas(int cantidad)
		{
			int cantidadFinal = 5;

			if (cantidad > 0)
			{
				cantidadFinal = cantidad;
			}

			if (cantidadFinal > 25)
			{
				cantidadFinal = 25;
			}

			List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Ultimas(cantidadFinal);

			if (noticias != null)
			{
				return Ok(noticias);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-free/")]
		public async Task<IActionResult> GratisUltimos()
		{
			List<JuegoGratis> gratis = await global::BaseDatos.Gratis.Buscar.Ultimos(5);

			if (gratis != null)
			{
				return Ok(gratis);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-free/{cantidad}/")]
		public async Task<IActionResult> GratisUltimos(int cantidad)
		{
			int cantidadFinal = 5;

			if (cantidad > 0)
			{
				cantidadFinal = cantidad;
			}

			if (cantidadFinal > 25)
			{
				cantidadFinal = 25;
			}

			List<JuegoGratis> gratis = await global::BaseDatos.Gratis.Buscar.Ultimos(cantidadFinal);

			if (gratis != null)
			{
				return Ok(gratis);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-subscriptions/")]
		public async Task<IActionResult> SuscripcionesUltimos()
		{
			List<JuegoSuscripcion> suscripciones = await global::BaseDatos.Suscripciones.Buscar.Ultimos("5");

			if (suscripciones != null)
			{
				return Ok(suscripciones);
			}

			return Redirect("~/");
		}

		[ResponseCache(Duration = 6000)]
		[HttpGet("api/last-subscriptions/{Cantidad}/")]
		public async Task<IActionResult> SuscripcionesUltimos(int Cantidad)
		{
			int cantidadFinal = 5;

			if (Cantidad > 0)
			{
				cantidadFinal = Cantidad;
			}

			if (cantidadFinal > 25)
			{
				cantidadFinal = 25;
			}

			List<JuegoSuscripcion> suscripciones = await global::BaseDatos.Suscripciones.Buscar.Ultimos(cantidadFinal.ToString());

			if (suscripciones != null)
			{
				return Ok(suscripciones);
			}

			return Redirect("~/");
		}
	}
}
