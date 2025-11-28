#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Reseñas
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

		public static void Ejecutar(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			conexion.Execute("TRUNCATE TABLE juegosAnalisis");
		}
	}
}
