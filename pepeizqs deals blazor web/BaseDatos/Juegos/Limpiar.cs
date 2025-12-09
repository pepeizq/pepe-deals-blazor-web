#nullable disable

using Dapper;
namespace BaseDatos.Juegos
{
	public static class Limpiar
	{
		public static async Task Minimos()
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteAsync("TRUNCATE TABLE seccionMinimos", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("Limpiar Minimos", ex);
			}
		}
	}
}
