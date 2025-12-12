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
				try
				{
					conexion.Open();
				}
				catch (SqlException ex) when (EsErrorDeTransporte(ex))
				{
					SqlConnection.ClearPool(conexion);
					conexion.Open();
				}
			}

			return conexion;	
        }

		private static bool EsErrorDeTransporte(SqlException ex)
		{
			int[] errores =
			{
				-1,      // General network error
				18,      // Connection has been closed by peer
				19,      // Physical connection is not usable
				10053,   // Connection aborted
				10054,   // Connection reset by peer
				10060    // Timeout
			};

			return ex.Errors.Cast<SqlError>().Any(e => errores.Contains(e.Number));
		}

		public static async Task EjecutarConConexionAsync(Func<SqlTransaction, Task> sentencia, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;
			int intentos = 2; 

			while (true)
			{
				try
				{
					if (conexion == null)
					{
						conexion = Conectar();
					}						

					if (conexion.State != ConnectionState.Open)
					{
						await conexion.OpenAsync();
					}			

					await using var transaccion = await conexion.BeginTransactionAsync();
					{
						try
						{
							await sentencia((SqlTransaction)transaccion);
							await transaccion.CommitAsync();
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

					return;
				}
				catch (SqlException ex) when (EsErrorDeTransporte(ex) && intentos-- > 0)
				{
					SqlConnection.ClearPool(conexion);

					if (cerrar == true)
					{
						await conexion.DisposeAsync();
						conexion = null;
					}

					await Task.Delay(150);
				}
			}
		}

		public static async Task<T> EjecutarConConexionAsync<T>(Func<SqlTransaction, Task<T>> sentencia, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;
			int intentos = 2;

			while (true)
			{
				try
				{
					if (conexion == null)
					{
						conexion = Conectar();
					}

					if (conexion.State != ConnectionState.Open)
					{
						await conexion.OpenAsync();
					}

					await using var transaccion = await conexion.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

					try
					{
						T resultado = await sentencia((SqlTransaction)transaccion);
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
				catch (SqlException ex) when (EsErrorDeTransporte(ex) && intentos-- > 0)
				{
					SqlConnection.ClearPool(conexion);

					if (cerrar == true)
					{
						await conexion.DisposeAsync();
						conexion = null;
					}

					await Task.Delay(150);
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
