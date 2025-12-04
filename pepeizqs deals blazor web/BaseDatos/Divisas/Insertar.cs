#nullable disable

using Dapper;
using Herramientas;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Divisas
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

		public static void Ejecutar(Divisa divisa, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlAñadir = "INSERT INTO divisas " +
                     "(id, cantidad, fecha) VALUES " +
                     "(@id, @cantidad, @fecha) ";

			try
			{
				conexion.Execute(sqlAñadir, new
				{
					id = divisa.Id,
					cantidad = divisa.Cantidad,
					fecha = divisa.Fecha
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Insertar Divisa " + divisa.Id, ex);
			}
		}
	}
}
