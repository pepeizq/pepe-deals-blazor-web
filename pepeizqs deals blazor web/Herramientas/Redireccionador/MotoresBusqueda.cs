#nullable disable

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;

namespace Herramientas.Redireccionador
{

	public class MotoresBusqueda : Controller
	{
		[HttpGet("BingSiteAuth.xml")]
		public IActionResult BingVerificacion()
		{
			string dominio = HttpContext.Request.Host.Value;

			if (dominio == "pepe.deals")
			{
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string clave = builder.Configuration.GetValue<string>("WebmasterDeals:BingWebmasterTools");

				XmlWriterSettings opciones = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					NewLineHandling = NewLineHandling.Entitize,
					NewLineOnAttributes = true,
					Indent = true
				};

				using (MemoryStream stream = new MemoryStream())
				{
					using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
					{
						xmlEscritor.WriteStartDocument();
						xmlEscritor.WriteStartElement("users");

						xmlEscritor.WriteStartElement("user");
						xmlEscritor.WriteString(clave);
						xmlEscritor.WriteEndElement();

						xmlEscritor.WriteEndElement();
						xmlEscritor.WriteEndDocument();

						xmlEscritor.Flush();
					}

					return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
				}
			}

			return null;
		}
	}
}
