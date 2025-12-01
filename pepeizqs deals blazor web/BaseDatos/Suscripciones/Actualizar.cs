#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Suscripciones
{
	public static class Actualizar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void FechaTermina(JuegoSuscripcion suscripcion, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlActualizar = "UPDATE suscripciones " +
					"SET fechaTermina=@fechaTermina WHERE enlace=@enlace";

			try
			{
				conexion.Execute(sqlActualizar, new
				{
					Enlace = suscripcion.Enlace,
					FechaTermina = suscripcion.FechaTermina
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Suscripcion FechaTermina", ex);
			}
		}
	}
}
