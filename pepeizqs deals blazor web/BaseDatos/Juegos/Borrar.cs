using Dapper;

namespace BaseDatos.Juegos
{
	public static class Borrar
	{
		public static async Task Ejecutar(string id)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("DELETE FROM juegos WHERE id=@id", new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Borrar", ex);
			}
		}
	}
}
