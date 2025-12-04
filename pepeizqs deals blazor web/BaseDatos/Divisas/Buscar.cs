#nullable disable

using Dapper;
using Herramientas;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Divisas
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

		public static Divisa Ejecutar(string id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			if (string.IsNullOrEmpty(id) == false) 
			{
                string sqlBuscar = "SELECT * FROM divisas WHERE id=@id";

				return conexion.QueryFirstOrDefault<Divisa>(sqlBuscar, new { id });
			}

			return null;
		}
	}
}
