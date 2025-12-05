#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Usuarios
{
	public static class Borrar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void NotificacionesPush(string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string borrar = "DELETE FROM usuariosNotificaciones WHERE usuarioId=@usuarioId";

				conexion.Execute(borrar, new { usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Borrar Notificaciones Push", ex);
			}
		}
	}
}
