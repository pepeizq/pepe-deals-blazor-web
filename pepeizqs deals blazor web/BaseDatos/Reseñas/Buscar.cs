#nullable disable

using APIs.Steam;
using BaseDatos.Pendientes;
using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BaseDatos.Reseñas
{
	public static class Buscar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static JuegoAnalisisAmpliado Cargar(int id, string idioma)
		{
			try
			{
				var fila = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault($"SELECT contenido{idioma} as Contenido, positivos{idioma} as CantidadPositivos, negativos{idioma} as CantidadNegativos FROM juegosAnalisis WHERE id=@id", new { id }, transaction: sentencia);
				});

				JuegoAnalisisAmpliado reseñas = new JuegoAnalisisAmpliado();

				if (fila != null)
				{
					if (!string.IsNullOrEmpty(fila.Contenido))
					{
						reseñas.Contenido = JsonSerializer.Deserialize<List<SteamAnalisisAPIAnalisis>>(fila.Contenido);
					}

					reseñas.CantidadPositivos = fila.CantidadPositivos ?? 0;
					reseñas.CantidadNegativos = fila.CantidadNegativos ?? 0;
				}

				return reseñas;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Cargar", ex);
			}

			return null;
		}

		public static bool DebeModificarse(int id, string idioma)
		{
			try
			{
				DateTime? fechaRegistrada = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.ExecuteScalar<DateTime?>($"SELECT fecha{idioma} FROM juegosAnalisis WHERE id=@id", new { id }, transaction: sentencia);
				});

				if (fechaRegistrada.HasValue == false)
				{
					return true;
				}

				return fechaRegistrada.Value.AddDays(7) < DateTime.Now;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Debe Modificarse", ex);
			}

			return false;
		}
	}
}
