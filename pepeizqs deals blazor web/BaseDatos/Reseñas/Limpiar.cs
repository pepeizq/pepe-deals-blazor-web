#nullable disable

using Dapper;

namespace BaseDatos.Reseñas
{
	public static class Limpiar
	{
		public static async Task Ejecutar()
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("TRUNCATE TABLE juegosAnalisis", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Limpiar", ex);
			}
		}
	}
}
