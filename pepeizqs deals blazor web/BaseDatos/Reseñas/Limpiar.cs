#nullable disable

using Dapper;

namespace BaseDatos.Reseñas
{
	public static class Limpiar
	{
		public static void Ejecutar()
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute("TRUNCATE TABLE juegosAnalisis", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Limpiar", ex);
			}
		}
	}
}
