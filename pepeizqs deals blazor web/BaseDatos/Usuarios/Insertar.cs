#nullable disable

using Dapper;
using Herramientas;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Usuarios
{
	public static class Insertar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void NotificacionesPush(NotificacionSuscripcion datos, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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
				conexion.Execute(sql, new
				{
					usuarioId = datos.UserId,
					notificacionId = datos.NotificationSubscriptionId,
					enlace = datos.Url,
					p256dh = datos.P256dh,
					auth = datos.Auth,
					userAgent = datos.UserAgent
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Insertar Notificaciones Push", ex);
			}	
		}
	}
}
