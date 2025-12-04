#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.UsuariosActualizar
{
	public static class Buscar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static List<UsuarioActualizar> Todos(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM usuariosActualizar";

			return conexion.Query<UsuarioActualizar>(busqueda).ToList();
		}
	}

	public class UsuarioActualizar
	{
		public string IdUsuario { get; set; }
		public string Metodo { get; set; }
	}
}
