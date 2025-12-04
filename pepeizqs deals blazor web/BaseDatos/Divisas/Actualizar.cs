#nullable disable

using Dapper;
using Herramientas;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Divisas
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

		public static void Ejecutar(Divisa divisa, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlActualizar = "UPDATE divisas " +
                    "SET id=@id, cantidad=@cantidad, fecha=@fecha WHERE id=@id";

			try
			{
				conexion.Execute(sqlActualizar, new
				{
					id = divisa.Id,
					cantidad = divisa.Cantidad,
					fecha = divisa.Fecha
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Divisa " + divisa.Id, ex);
			}
		}
	}
}
