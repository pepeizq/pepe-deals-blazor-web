#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

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
			conexion = CogerConexion(conexion);

			string sql = @"
                INSERT INTO errores
                (seccion, mensaje, stacktrace, fecha, sentenciaSQL)
                VALUES (@seccion, @mensaje, @stacktrace, @fecha, @sentenciaSQL)
            ";

			string sentencia = comandoUsado != null ? GenerarSentencia(comandoUsado) : null;

			conexion.Execute(sql, new
			{
				seccion,
				mensaje = ex.Message,
				stacktrace = ex.StackTrace,
				fecha = DateTime.Now,
				sentenciaSQL = sentencia
			});

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
			conexion = CogerConexion(conexion);

			string sql = @"
				INSERT INTO errores
				(seccion, mensaje, stacktrace, fecha, enlace, sentenciaSQL)
				VALUES (@seccion, @mensaje, @stacktrace, @fecha, @enlace, @sentenciaSQL);
			";

			conexion.Execute(sql, new
			{
				seccion,
				mensaje,
				stacktrace = "",
				fecha = DateTime.Now,
				enlace = (object)enlace ?? DBNull.Value,
				sentenciaSQL = comandoUsado != null ? GenerarSentencia(comandoUsado) : null
			});
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
