#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Recompensas
{
	public class RecompensaJuego
	{
		public int Id;
		public int JuegoId;
		public string Clave;
		public int Coins;
		public DateTime FechaEmpieza;
		public string UsuarioId;
		public JuegoDRM DRM;
		public string JuegoNombre;
		public DateTime? FechaCaduca;
	}

	public static class Juegos
	{
		public static RecompensaJuego Cargar(SqlDataReader lector)
		{
			RecompensaJuego juego = new RecompensaJuego
			{
				Id = lector.GetInt32(0),
				JuegoId = lector.GetInt32(1),
				Clave = lector.GetString(2),
				Coins = lector.GetInt32(3),
				FechaEmpieza = lector.GetDateTime(5)
			};

			if (lector.IsDBNull(4) == false)
			{
				juego.UsuarioId = lector.GetString(4);
			}

			if (lector.IsDBNull(6) == false)
			{
				juego.DRM = Enum.Parse<JuegoDRM>(lector.GetInt32(6).ToString());
			}
			else
			{
				juego.DRM = JuegoDRM.Steam;
			}

			if (lector.IsDBNull(7) == false)
			{
				juego.JuegoNombre = lector.GetString(7);
			}

			if (lector.IsDBNull(8) == false)
			{
				juego.FechaCaduca = lector.GetDateTime(8);
			}

			return juego;
		}

		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Insertar(RecompensaJuego recompensa, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlInsertar = @"
				INSERT INTO recompensasJuegos 
				(juegoId, clave, coins, fecha, juegoNombre, drm" + (recompensa.FechaCaduca != null ? ", fechaCaduca" : "") + @")
				VALUES 
				(@juegoId, @clave, @coins, @fecha, @juegoNombre, @drm" + (recompensa.FechaCaduca != null ? ", @fechaCaduca" : "") + @")";

			try
			{
				conexion.Execute(sqlInsertar, new
				{
					juegoId = recompensa.JuegoId.ToString(),
					clave = recompensa.Clave,
					coins = recompensa.Coins,
					fecha = recompensa.FechaEmpieza,
					juegoNombre = recompensa.JuegoNombre,
					drm = recompensa.DRM,
					fechaCaduca = recompensa.FechaCaduca
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensa Insertar", ex);
			}
		}

		public static List<RecompensaJuego> Disponibles(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT * FROM recompensasJuegos WHERE usuarioId IS NULL AND (fechaCaduca IS NULL OR fechaCaduca > GETDATE()) ORDER BY juegoNombre";

				return conexion.Query<RecompensaJuego>(busqueda).ToList();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Disponibles", ex);
			}

			return new List<RecompensaJuego>();
		}

		public static void Actualizar(int id, string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string sqlActualizar = "UPDATE recompensasJuegos " +
					"SET usuarioId=@usuarioId WHERE id=@id";

				conexion.Execute(sqlActualizar, new { id, usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Actualizar", ex);
			}			
		}

        public static List<RecompensaJuego> LeerJuegosUsuario(string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT * FROM recompensasJuegos WHERE usuarioId=@usuarioId ORDER BY juegoNombre";

				return conexion.Query<RecompensaJuego>(busqueda, new { usuarioId }).ToList();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Leer Juegos Usuario", ex);
			}

			return new List<RecompensaJuego>();
        }
    }
}
