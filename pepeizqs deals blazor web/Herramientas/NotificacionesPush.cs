#nullable disable

using pepeizqs_deals_web.Data;
using System.Text.Json;
using WebPush;

public class PushSubscription
{
	public int Id { get; set; }
	public string UserId { get; set; }
	public string SubscriptionJson { get; set; }
	public DateTime FechaCreacion { get; set; }
	public bool Activa { get; set; }
	public virtual Usuario User { get; set; }
}

namespace Herramientas
{
	public class NotificacionesPush
	{
		private readonly pepeizqs_deals_webContext _db;
		private readonly IConfiguration _configuracion;

		public NotificacionesPush(pepeizqs_deals_webContext db, IConfiguration configuracion)
		{
			_db = db;
			_configuracion = configuracion;
		}

		public async Task<bool> EnviarNotificacion(string usuarioId, string titulo, string enlace)
		{
			try
			{
				var subscription = _db.PushSubscriptions.FirstOrDefault(s => s.UserId == usuarioId && s.Activa);

				if (subscription == null)
				{
					return false;
				}

				using (JsonDocument doc = JsonDocument.Parse(subscription.SubscriptionJson))
				{
					JsonElement root = doc.RootElement;

					var endpoint = root.GetProperty("endpoint").GetString();
					var p256dh = root.GetProperty("keys").GetProperty("p256dh").GetString();
					var auth = root.GetProperty("keys").GetProperty("auth").GetString();

					WebPush.PushSubscription pushSuscripcion = new WebPush.PushSubscription(endpoint, p256dh, auth);

					var vapidPublicKey = _configuracion["NotificacionesPush:PublicKey"];
					var vapidPrivateKey = _configuracion["NotificacionesPush:PrivateKey"];
					var vapidSubject = _configuracion["NotificacionesPush:Subject"];

					VapidDetails vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);

					string payload = JsonSerializer.Serialize(new
					{
						title = titulo,
						icon = "/logo/logo6.png",
						url = enlace
					});

					WebPushClient webPushClient = new WebPushClient();
					await webPushClient.SendNotificationAsync(pushSuscripcion, payload, vapidDetails);

					return true;
				}
			}
			catch (Exception ex)
			{
				global::BaseDatos.Errores.Insertar.Mensaje("Enviar Notificaciones Push", ex, false);
				return false;
			}
		}

		public async Task<bool> EnviarNoticia(Noticias.Noticia noticia, string dominio)
		{
			if (noticia != null)
			{
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

				var suscripciones = _db.PushSubscriptions
					.Where(s => s.Activa)
					.GroupBy(s => s.UserId)
					.ToList();

				int enviadas = 0;

				foreach (var usuario in suscripciones)
				{
					string usuarioId = usuario.Key;

					if (string.IsNullOrEmpty(usuarioId) == false)
					{
						Usuario usuarioOpciones = await global::BaseDatos.Usuarios.Buscar.OpcionesNotificacionesPush(usuarioId);

						string idioma = usuarioOpciones.Language;

						if (string.IsNullOrEmpty(usuarioOpciones.LanguageOverride) == false)
						{
							idioma = usuarioOpciones.LanguageOverride;
						}

						bool enviar = false;

						if (usuarioOpciones.NotificationPushBundles == true && noticia.NoticiaTipo == Noticias.NoticiaTipo.Bundles)
						{
							enviar = true;
						}
						else if (usuarioOpciones.NotificationPushFree == true && noticia.NoticiaTipo == Noticias.NoticiaTipo.Gratis)
						{
							enviar = true;
						}
						else if (usuarioOpciones.NotificationPushSubscriptions == true && noticia.NoticiaTipo == Noticias.NoticiaTipo.Suscripciones)
						{
							enviar = true;
						}
						else if (usuarioOpciones.NotificationPushWeb == true && noticia.NoticiaTipo == Noticias.NoticiaTipo.Web)
						{
							enviar = true;
						}
						else
						{
							if (usuarioOpciones.NotificationPushOthers == true && (noticia.NoticiaTipo != Noticias.NoticiaTipo.Bundles &&
																					noticia.NoticiaTipo != Noticias.NoticiaTipo.Gratis &&
																					noticia.NoticiaTipo != Noticias.NoticiaTipo.Suscripciones &&
																					noticia.NoticiaTipo != Noticias.NoticiaTipo.Web))
							{
								enviar = true;
							}
						}

						if (enviar == true)
						{
							string titulo = Herramientas.Idiomas.ElegirTexto(idioma, noticia.TituloEn, noticia.TituloEs);

							var exito = await EnviarNotificacion(usuarioId, titulo, enlace);

							if (exito == true)
							{
								enviadas += 1;
							}
						}
					}
				}

				if (enviadas > 0)
				{
					return true;
				}
			}

			return false;
		}
	}
}