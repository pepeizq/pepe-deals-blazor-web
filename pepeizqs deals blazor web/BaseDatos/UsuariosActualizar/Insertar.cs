#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.UsuariosActualizar
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

		public static void Ejecutar(string idUsuario, string metodo, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(idUsuario) == false && string.IsNullOrEmpty(metodo) == false)
			{
				conexion = CogerConexion(conexion);

				string sqlAñadir = @"
					IF NOT EXISTS (
						SELECT 1 FROM usuariosActualizar 
						WHERE idUsuario = @idUsuario
					)
					BEGIN
						INSERT INTO usuariosActualizar (idUsuario, metodo)
						VALUES (@idUsuario, @metodo)
					END
					";

				try
				{
					conexion.Execute(sqlAñadir, new
					{
						idUsuario,
						metodo
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Actualizar Insertar", ex);
				}
			}
		}
	}
}
