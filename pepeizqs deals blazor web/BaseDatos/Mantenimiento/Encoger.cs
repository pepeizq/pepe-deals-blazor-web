#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Mantenimiento
{
	public static class Encoger
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

			WebApplicationBuilder builder = WebApplication.CreateBuilder();
			string baseDatos = builder.Configuration.GetValue<string>("Mantenimiento:BaseDatos");

			string sqlEncoger = $"DBCC SHRINKDATABASE ({baseDatos})";

			try
			{
				conexion.Execute(sqlEncoger, commandTimeout: 5000);
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Encoger Base Datos", ex, null, true, sqlEncoger);
			}
		}
	}
}
