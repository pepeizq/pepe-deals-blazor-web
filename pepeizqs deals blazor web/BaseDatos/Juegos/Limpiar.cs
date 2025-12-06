#nullable disable

using Dapper;
namespace BaseDatos.Juegos
{
	public static class Limpiar
	{
		public static void Minimos()
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Execute("TRUNCATE TABLE seccionMinimos", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("Limpiar Minimos", ex);
			}
		}
	}
}
