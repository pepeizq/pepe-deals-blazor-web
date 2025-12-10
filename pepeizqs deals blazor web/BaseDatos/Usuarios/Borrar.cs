#nullable disable

using Dapper;

namespace BaseDatos.Usuarios
{
	public static class Borrar
	{
		public static async Task NotificacionesPush(string usuarioId)
		{
			try
			{
				string borrar = "DELETE FROM usuariosNotificaciones WHERE usuarioId=@usuarioId";

				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(borrar, new { usuarioId }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Borrar Notificaciones Push", ex);
			}
		}
	}
}
