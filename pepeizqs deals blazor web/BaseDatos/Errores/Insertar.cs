#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Errores
{
    public static class Insertar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Mensaje2(string seccion, Exception ex, SqlConnection conexion = null, bool reiniciar = true, string comandoUsado = null)
		{
			conexion = CogerConexion(conexion);

			string sqlInsertar = @"
        INSERT INTO errores (seccion, mensaje, stacktrace, fecha {0})
        VALUES (@seccion, @mensaje, @stacktrace, @fecha {1});
    ";

			string columnasExtra = "";
			string valoresExtra = "";

			if (string.IsNullOrEmpty(comandoUsado) == false)
			{
				columnasExtra = ", sentenciaSQL";
				valoresExtra = ", @sentenciaSQL";
			}

			sqlInsertar = string.Format(sqlInsertar, columnasExtra, valoresExtra);

			try
			{
				conexion.Execute(sqlInsertar,
					new
					{
						seccion,
						mensaje = ex.Message,
						stacktrace = ex.StackTrace,
						fecha = DateTime.Now,
						sentenciaSQL = comandoUsado
					}
				);
			}
			catch
			{
				
			}

			if (reiniciar == true)
			{
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp != piscinaUsada)
				{
					Environment.Exit(1);
				}
			}
		}

		public static void Mensaje(string seccion, Exception ex, SqlConnection conexion = null, bool reiniciar = true, SqlCommand comandoUsado = null)
		{
            if (conexion == null)
            {
                conexion = Herramientas.BaseDatos.Conectar();
            }
            else
            {
                if (conexion.State != System.Data.ConnectionState.Open)
                {
                    conexion = Herramientas.BaseDatos.Conectar();
                }
            }

            string añadirComando1 = string.Empty;
            string añadirComando2 = string.Empty;

            if (comandoUsado != null)
            {
                añadirComando1 = ", sentenciaSQL";
                añadirComando2 = ", @sentenciaSQL";
            }

            using (conexion)
            {
                string sqlInsertar = "INSERT INTO errores " +
                                "(seccion, mensaje, stacktrace, fecha" + añadirComando1 + ") VALUES " +
                                "(@seccion, @mensaje, @stacktrace, @fecha" + añadirComando2 + ") ";

                using (SqlCommand comando = new SqlCommand(sqlInsertar, conexion))
                {
                    comando.Parameters.AddWithValue("@seccion", seccion);
                    comando.Parameters.AddWithValue("@mensaje", ex.Message);
                    comando.Parameters.AddWithValue("@stacktrace", ex.StackTrace);
                    comando.Parameters.AddWithValue("@fecha", DateTime.Now);

                    if (comandoUsado != null)
                    {
                        comando.Parameters.AddWithValue("@sentenciaSQL", GenerarSentencia(comandoUsado));
                    }

                    try
                    {
                        comando.ExecuteNonQuery();
                    }
                    catch
                    {

                    }
				}
			}
            
            if (reiniciar == true)
            {
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp != piscinaUsada)
                {
					Environment.Exit(1);
				}
			}  
        }

        public static void Mensaje(string seccion, string mensaje, string enlace = null, SqlCommand comandoUsado = null, SqlConnection conexion = null)
        {
            if (conexion == null)
            {
                conexion = Herramientas.BaseDatos.Conectar();
            }
            else
            {
                if (conexion.State != System.Data.ConnectionState.Open)
                {
                    conexion = Herramientas.BaseDatos.Conectar();
                }
            }

            string añadirEnlace1 = string.Empty;
            string añadirEnlace2 = string.Empty;

            if (string.IsNullOrEmpty(enlace) == false)
            {
                añadirEnlace1 = ", enlace";
                añadirEnlace2 = ", @enlace";
            }

            string añadirComando1 = string.Empty;
            string añadirComando2 = string.Empty;

            if (comandoUsado != null)
            {
                añadirComando1 = ", sentenciaSQL";
                añadirComando2 = ", @sentenciaSQL";
            }

            string sqlInsertar = "INSERT INTO errores " +
                               "(seccion, mensaje, stacktrace, fecha" + añadirEnlace1 + añadirComando1 + ") VALUES " +
                               "(@seccion, @mensaje, @stacktrace, @fecha" + añadirEnlace2 + añadirComando2 + ") ";

            using (SqlCommand comando = new SqlCommand(sqlInsertar, conexion))
            {
                comando.Parameters.AddWithValue("@seccion", seccion);
                comando.Parameters.AddWithValue("@mensaje", "nada");
                comando.Parameters.AddWithValue("@stacktrace", mensaje);
                comando.Parameters.AddWithValue("@fecha", DateTime.Now);

                if (string.IsNullOrEmpty(enlace) == false)
                {
                    comando.Parameters.AddWithValue("@enlace", enlace);
                }

                if (comandoUsado != null)
                {
                    comando.Parameters.AddWithValue("@sentenciaSQL", GenerarSentencia(comandoUsado));
                }

                try
                {
                    comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Mensaje("Errores", ex);
                }
			}
		}

        public static string GenerarSentencia(SqlCommand comandoUsado)
        {
            string comandoSQL = comandoUsado.CommandText;

            foreach (SqlParameter parametro in comandoUsado.Parameters)
            {
                string valorParametro = parametro.Value.ToString();

                if (parametro.DbType == System.Data.DbType.String || parametro.DbType == System.Data.DbType.DateTime)
                {
                    valorParametro = "'" + valorParametro.Replace("'", "''") + "'";
                }
                else if (parametro.DbType == System.Data.DbType.Boolean)
                {
                    if (valorParametro == "True")
                    {
                        valorParametro = "1";
                    }
                    else
                    {
                        valorParametro = "0";
                    }
                }
                else if (parametro.Value == null)
                {
                    valorParametro = "NULL";
                }

                comandoSQL = comandoSQL.Replace(parametro.ParameterName, valorParametro);
            }

            return comandoSQL;
        }
    }
}
