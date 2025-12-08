#nullable disable

using BlazorNotification;
using Juegos;
using Microsoft.VisualBasic;

namespace Herramientas
{
	public class NotificacionesPush
	{
		public async static Task<bool> EnviarNoticia(IBlazorNotificationService servicio, Noticias.Noticia noticia)
		{
			string enlace = string.Empty;

			if (noticia.Id == 0)
			{
				enlace = "https://pepe.deals/link/news/" + noticia.IdMaestra.ToString() + "/";
			}
			else
			{
				enlace = "https://pepe.deals/link/news/" + noticia.Id.ToString() + "/";
			}

			string titulo = noticia.TituloEn;
			titulo = titulo.Replace("'", null);

			NotificationOptions opciones = new NotificationOptions
			{
				Icon = "https://pepe.deals/logo/logo6.png",
				Image = noticia.Imagen,
				Data = "url: " + Strings.ChrW(34) + noticia.Enlace + Strings.ChrW(34) + ", " + Environment.NewLine + "status: " + Strings.ChrW(34) + "open"
			};

			try
			{
				await servicio.SendAsync(titulo, opciones);
			}
			catch (Exception ex)
			{
				global::BaseDatos.Errores.Insertar.Mensaje("Notificaciones Push", ex);
				return false;
			}

			return true;
		}

		public static void EnviarMinimo(string usuarioId, Juego juego, JuegoPrecio minimo, JuegoDRM drm)
		{
			if (juego != null)
			{
				//WebApplicationBuilder builder = WebApplication.CreateBuilder();
				//string publicKey = builder.Configuration.GetValue<string>("NotificacionesPush:PublicKey");
				//string privateKey = builder.Configuration.GetValue<string>("NotificacionesPush:PrivateKey");

				//VapidDetails vapidDetalles = new VapidDetails("https://pepe.deals", publicKey, privateKey);
				//WebPushClient webPushCliente = new WebPushClient();

				//NotificacionSuscripcion usuario = global::BaseDatos.Usuarios.Buscar.UnUsuarioNotificacionesPush(usuarioId);

				//if (usuario != null)
				//{
				//	string titulo = minimo.Nombre + " - " + Herramientas.Precios.Euro(minimo.Precio);

				//	PushSubscription suscripcion = new PushSubscription(usuario.Url, usuario.P256dh, usuario.Auth);

				//	try
				//	{
				//		var payload = JsonSerializer.Serialize(new
				//		{
				//			message = titulo,
				//			url = $"/game/{juego.Id.ToString()}/{Herramientas.EnlaceAdaptador.Nombre(juego.Nombre)}/"
				//		});

				//		await webPushCliente.SendNotificationAsync(suscripcion, payload, vapidDetalles);
				//	}
				//	catch (Exception ex)
				//	{
				//		global::BaseDatos.Errores.Insertar.Mensaje("Notificaciones Push", ex);
				//	}
				//}
			}
		}
	}

	public class NotificacionSuscripcion
	{
		public int NotificationSubscriptionId { get; set; }
		public string UserId { get; set; }
		public string Url { get; set; }
		public string P256dh { get; set; }
		public string Auth { get; set; }
		public string UserAgent { get; set; }
	}
}
