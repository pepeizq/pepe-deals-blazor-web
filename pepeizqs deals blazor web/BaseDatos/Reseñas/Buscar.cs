#nullable disable

using APIs.Steam;
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

		public static JuegoAnalisisAmpliado Cargar(int id, string idioma, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			var fila = conexion.QueryFirstOrDefault($"SELECT contenido{idioma} as Contenido, positivos{idioma} as CantidadPositivos, negativos{idioma} as CantidadNegativos FROM juegosAnalisis WHERE id=@id", new { id });

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

		public static bool DebeModificarse(int id, string idioma, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			DateTime? fechaRegistrada = conexion.ExecuteScalar<DateTime?>($"SELECT fecha{idioma} FROM juegosAnalisis WHERE id=@id", new { id });

			if (fechaRegistrada.HasValue == false)
			{
				return true;
			}
			
			return fechaRegistrada.Value.AddDays(7) < DateTime.Now;
		}
	}
}
