using Dapper;

namespace BaseDatos.Juegos
{
	public static class Borrar
	{
		public static async void Ejecutar(string id)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("DELETE FROM juegos WHERE id=@id", new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Borrar", ex);
			}
		}
	}
}
