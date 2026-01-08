#nullable disable

using Discord;
using Discord.Webhook;
using System.Net;

namespace Herramientas.RedesSociales
{
	public static class Discord
	{
		public static async Task<bool> Postear(IConfiguration configuracion, Noticias.Noticia noticia, string dominio)
		{
			string ingles = configuracion.GetValue<string>("Discord:Ingles");
			string español = configuracion.GetValue<string>("Discord:Español");

			string enlace = string.Empty;

			if (noticia.Id == 0)
			{
				enlace = "/link/news/" + noticia.IdMaestra.ToString() + "/";
			}
			else
			{
				enlace = "/link/news/" + noticia.Id.ToString() + "/";
			}

			if (string.IsNullOrEmpty(enlace) == false)
			{
				if (enlace.Contains("https://" + dominio) == false)
				{
					enlace = "https://" + dominio + enlace;
				}
			}

			bool enviados = false;

			enviados = await Enviar(noticia.TituloEn, enlace, noticia.Imagen, ingles);

			if (enviados == true)
			{
				enviados = await Enviar(noticia.TituloEs, enlace, noticia.Imagen, español);
			}
			
			return enviados;
		}

		private static async Task<bool> Enviar(string titulo, string enlace, string imagen, string hook)
		{
			using (DiscordWebhookClient cliente = new DiscordWebhookClient(hook))
			{
				EmbedBuilder constructor = new EmbedBuilder();

				constructor.Title = titulo;
				constructor.Url = enlace;

				if (string.IsNullOrEmpty(imagen) == false)
				{
					constructor.ImageUrl = WebUtility.HtmlDecode(imagen);
				}

				List<Embed> lista = new List<Embed>
				{
					constructor.Build()
				};

				ulong resultado = await cliente.SendMessageAsync(titulo + Environment.NewLine + enlace, false, lista, "pepebot5");

				if (resultado > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
