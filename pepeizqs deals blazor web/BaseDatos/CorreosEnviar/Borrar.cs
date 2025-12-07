#nullable disable

using Dapper;

namespace BaseDatos.CorreosEnviar
{
	public static class Borrar
	{
		public static async void Ejecutar(int id)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("DELETE FROM correosEnviar WHERE id=@id", new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Correos Enviar Borrar", ex);
			}
		}
	}
}
