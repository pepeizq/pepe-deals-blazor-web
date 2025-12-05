#nullable disable

using Dapper;

namespace BaseDatos.CorreosEnviar
{
	public static class Borrar
	{
		public static void Ejecutar(int id)
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Execute("DELETE FROM correosEnviar WHERE id=@id", new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Correos Enviar Borrar", ex);
			}
		}
	}
}
