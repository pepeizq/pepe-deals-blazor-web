#nullable disable

using Dapper;

namespace BaseDatos.Mantenimiento
{
	public static class Encoger
	{
		public static async Task Ejecutar()
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder();
			string baseDatos = builder.Configuration.GetValue<string>("Mantenimiento:BaseDatos");

			string sqlEncoger = $"DBCC SHRINKDATABASE ({baseDatos})";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlEncoger, commandTimeout: 5000, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Encoger Base Datos", ex, true, sqlEncoger);
			}
		}
	}
}
