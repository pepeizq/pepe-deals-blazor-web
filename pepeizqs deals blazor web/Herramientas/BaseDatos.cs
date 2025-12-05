#nullable disable

using Microsoft.Data.SqlClient;
using System.Data;

namespace Herramientas
{
	public static class BaseDatos
	{
		public static string cadenaConexion;

		public static SqlConnection Conectar(bool usarEstado = true)
		{
            SqlConnection conexion = new SqlConnection(cadenaConexion);
			
            ConnectionState estado = conexion.State;
            
			if (usarEstado == true && estado != ConnectionState.Open)
			{
				conexion.Open();
			}

			return conexion;	
        }

		public static void EjecutarConConexion(Action<SqlTransaction> sentencia, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null || conexion.State != ConnectionState.Open)
			{
				conexion = Conectar();
			}

			using (SqlTransaction transaccion = conexion.BeginTransaction())
			{
				try
				{
					sentencia(transaccion);
					transaccion.Commit();
				}
				catch
				{
					try { transaccion.Rollback(); } catch { }
					throw;
				}
				finally
				{
					if (cerrar == true)
					{
						conexion.Close();
					}
				}
			}
		}

		public static T EjecutarConConexion<T>(Func<SqlTransaction, T> sentencia, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null || conexion.State != ConnectionState.Open)
			{
				conexion = Conectar();
			}

			using (SqlTransaction transaccion = conexion.BeginTransaction())
			{
				try
				{
					T resultado = sentencia(transaccion);
					transaccion.Commit();

					return resultado;
				}
				catch
				{
					try { transaccion.Rollback(); } catch { }
					throw;
				}
				finally
				{
					if (cerrar == true)
					{
						conexion.Close();
					}
				}
			}
		}
	}
}
