
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

public class NotificacionesPush
{
	private readonly pepeizqs_deals_webContext _db;
	private readonly IConfiguration _config;

	public NotificacionesPush(pepeizqs_deals_webContext db, IConfiguration config)
	{
		_db = db;
		_config = config;
	}

	public async Task<bool> SendNotificationAsync(
	   string userId,
	   string titulo,
	   string mensaje,
	   string enlace)
	{
		try
		{
			var subscription = _db.PushSubscriptions.FirstOrDefault(s => s.UserId == userId && s.Activa);

			if (subscription == null)
			{
				Console.WriteLine($"No hay suscripción activa para el usuario {userId}");
				return false;
			}

			using (JsonDocument doc = JsonDocument.Parse(subscription.SubscriptionJson))
			{
				JsonElement root = doc.RootElement;

				var endpoint = root.GetProperty("endpoint").GetString();
				var p256dh = root.GetProperty("keys").GetProperty("p256dh").GetString();
				var auth = root.GetProperty("keys").GetProperty("auth").GetString();

				var pushSubscription = new WebPush.PushSubscription(endpoint, p256dh, auth);

				var vapidPublicKey = _config["NotificacionesPush:PublicKey"];
				var vapidPrivateKey = _config["NotificacionesPush:PrivateKey"];
				var vapidSubject = _config["NotificacionesPush:Subject"];

				var vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);

				var payload = JsonSerializer.Serialize(new
				{
					title = titulo,
					body = mensaje,
					icon = "/logo/logo6.png",
					url = enlace
				});

				var webPushClient = new WebPushClient();
				await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);

				return true;
			}
		}
		catch (Exception ex)
		{
			BaseDatos.Errores.Insertar.Mensaje("Enviar Notificaciones Push", ex, false);
			return false;
		}
	}

	public async Task<int> SendNotificationToAllAsync(
		string titulo,
		string mensaje,
		string enlace)
	{
		try
		{
			var subscriptions = _db.PushSubscriptions
				.Where(s => s.Activa)
				.GroupBy(s => s.UserId)
				.ToList();

			int enviadas = 0;

			foreach (var userGroup in subscriptions)
			{
				var userId = userGroup.Key;
				var success = await SendNotificationAsync(userId, titulo, mensaje, enlace);
				if (success) enviadas++;
			}

			Console.WriteLine($"✓ {enviadas} notificaciones enviadas");
			return enviadas;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"❌ Error: {ex.Message}");
			return 0;
		}
	}
}