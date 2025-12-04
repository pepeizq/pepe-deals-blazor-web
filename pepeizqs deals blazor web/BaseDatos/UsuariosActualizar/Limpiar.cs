#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.UsuariosActualizar
{
	public static class Limpiar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Una(UsuarioActualizar usuario, SqlConnection conexion = null)
		{
			if (usuario == null)
			{
				return;
			}

			conexion = CogerConexion(conexion);

			string sqlEliminar = "DELETE FROM usuariosActualizar WHERE idUsuario=@idUsuario AND metodo=@metodo";

			try
			{
				conexion.Execute(sqlEliminar, new
				{
					usuario.IdUsuario,
					usuario.Metodo
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuarios Actualizar Limpiar", ex);
			}
		}
	}
}