#nullable disable

using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;

namespace Herramientas.Redireccionador
{
	public class Enlace : Controller
	{
		[HttpGet("link/{id}/")]
		public IActionResult CogerAcortador(string id)
		{
			return Redirect(EnlaceAcortador.AlargarEnlace(id));
		}

		[HttpGet("/game/steam/{id}/")]
		public async Task<IActionResult> Steam(string id)
		{
			Juegos.Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(null, id);

			if (juego == null)
			{
				return Redirect("~/");
			}

			return Redirect("/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/");
		}

		[HttpGet("/game/gog/{id}/")]
		public async Task<IActionResult> Gog(string id)
		{
			Juegos.Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(null, null, id);

			if (juego == null)
			{
				return Redirect("~/");
			}

			return Redirect("/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/");
		}

		[HttpGet("/game/epic/{id}/")]
		public async Task<IActionResult> Epic(string id)
		{
			Juegos.Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(null, null, null, id);

			if (juego == null)
			{
				return Redirect("~/");
			}

			return Redirect("/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/");
		}
	}
}
