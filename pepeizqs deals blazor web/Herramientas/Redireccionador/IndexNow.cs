#nullable disable

using Microsoft.AspNetCore.Mvc;

namespace Herramientas.Redireccionador
{
	public class IndexNow : Controller
	{
		private readonly IConfiguration _configuracion;

		public IndexNow(IConfiguration configuracion)
		{
			_configuracion = configuracion;
		}

		[HttpGet("{clave}.txt")]
		public IActionResult Bing(string clave)
		{
			string claveIndexNow = _configuracion.GetValue<string>("IndexNow:Key");

			if (string.IsNullOrEmpty(claveIndexNow))
			{
				return NotFound();
			}

			if (clave != claveIndexNow)
			{
				return NotFound();
			}

			return Ok(claveIndexNow);
		}
	}
}
