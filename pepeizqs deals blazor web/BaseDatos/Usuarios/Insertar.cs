#nullable disable

using Dapper;
using Herramientas;

namespace BaseDatos.Usuarios
{
	public static class Insertar
	{
		public static async Task NotificacionesPush(NotificacionSuscripcion datos)
		{
			string sql = @"
			IF NOT EXISTS (SELECT 1 FROM usuariosNotificaciones WHERE usuarioId = @usuarioId)
			BEGIN
				INSERT INTO usuariosNotificaciones
				(usuarioId, notificacionId, enlace, p256dh, auth, userAgent)
				VALUES
				(@usuarioId, @notificacionId, @enlace, @p256dh, @auth, @userAgent)
			END";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sql, new
					{
						usuarioId = datos.UserId,
						notificacionId = datos.NotificationSubscriptionId,
						enlace = datos.Url,
						p256dh = datos.P256dh,
						auth = datos.Auth,
						userAgent = datos.UserAgent
					}, transaction: sentencia);
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Insertar Notificaciones Push", ex);
			}	
		}
	}
}
