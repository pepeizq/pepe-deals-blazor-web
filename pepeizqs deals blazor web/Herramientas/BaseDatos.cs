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

		public static async Task EjecutarConConexionAsync(Func<SqlTransaction, Task> sentencia, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null)
			{
				conexion = Conectar();
			}

			if (conexion.State != ConnectionState.Open)
			{
				await conexion.OpenAsync();
			}

			await using (SqlTransaction transaccion = (SqlTransaction)await conexion.BeginTransactionAsync())
			{
				try
				{
					await sentencia(transaccion);
					await transaccion.CommitAsync();
				}
				catch
				{
					try { await transaccion.RollbackAsync(); } catch { }
					throw;
				}
				finally
				{
					if (cerrar)
					{
						await conexion.CloseAsync();
					}
				}
			}
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

		public static async Task<T> EjecutarConConexionAsync<T>(Func<SqlTransaction, Task<T>> sentencia, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null)
			{
				conexion = Conectar();
			}

			if (conexion.State != ConnectionState.Open)
			{
				await conexion.OpenAsync();
			}

			await using (SqlTransaction transaccion = (SqlTransaction)await conexion.BeginTransactionAsync())
			{
				try
				{
					T resultado = await sentencia(transaccion);
					await transaccion.CommitAsync();

					return resultado;
				}
				catch
				{
					try { await transaccion.RollbackAsync(); } catch { }
					throw;
				}
				finally
				{
					if (cerrar == true)
					{
						await conexion.CloseAsync();
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
