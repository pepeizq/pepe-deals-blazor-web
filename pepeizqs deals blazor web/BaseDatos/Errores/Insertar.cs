#nullable disable

using Dapper;
using Herramientas;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Errores
{
    public static class Insertar
	{
		public static async void Mensaje2(string seccion, Exception ex, bool reiniciar = true, string comandoUsado = null)
		{
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

			await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
			{
				return await sentencia.Connection.ExecuteAsync(sqlInsertar, new
				{
					seccion,
					mensaje = ex.Message,
					stacktrace = ex.StackTrace,
					fecha = DateTime.Now,
					sentenciaSQL = comandoUsado
				}, transaction: sentencia);
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

		public static async void Mensaje(string seccion, Exception ex, SqlConnection conexion = null, bool reiniciar = true, SqlCommand comandoUsado = null)
		{
			string sql = @"
                INSERT INTO errores
                (seccion, mensaje, stacktrace, fecha, sentenciaSQL)
                VALUES (@seccion, @mensaje, @stacktrace, @fecha, @sentenciaSQL)
            ";

			string sentencia = comandoUsado != null ? GenerarSentencia(comandoUsado) : null;

			await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
			{
				return await sentencia.Connection.ExecuteAsync(sql, new
				{
					seccion,
					mensaje = ex.Message,
					stacktrace = ex.StackTrace,
					fecha = DateTime.Now,
					sentenciaSQL = sentencia
				}, transaction: sentencia);
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

        public static async void Mensaje(string seccion, string mensaje, string enlace = null, SqlCommand comandoUsado = null, SqlConnection conexion = null)
        {
			string sql = @"
				INSERT INTO errores
				(seccion, mensaje, stacktrace, fecha, enlace, sentenciaSQL)
				VALUES (@seccion, @mensaje, @stacktrace, @fecha, @enlace, @sentenciaSQL);
			";

			await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
			{
				return await sentencia.Connection.ExecuteAsync(sql, new
				{
					seccion,
					mensaje,
					stacktrace = "",
					fecha = DateTime.Now,
					enlace = (object)enlace ?? DBNull.Value,
					sentenciaSQL = comandoUsado != null ? GenerarSentencia(comandoUsado) : null
				}, transaction: sentencia);
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
