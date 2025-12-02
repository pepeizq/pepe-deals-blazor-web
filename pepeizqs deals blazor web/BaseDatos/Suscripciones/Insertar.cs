#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Suscripciones
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

		public static void Ejecutar(int juegoId, JuegoSuscripcion actual, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlInsertar = @"INSERT INTO suscripciones (suscripcion, juegoId, nombre, imagen, drm, enlace, fechaEmpieza, fechaTermina, imagenNoticia) VALUES (@Tipo, @JuegoId, @Nombre, @Imagen, @DRM, @Enlace, @FechaEmpieza, @FechaTermina, @ImagenNoticia); ";

			try
			{
				conexion.Execute(sqlInsertar, new
				{
					Tipo = actual.Tipo,
					JuegoId = actual.JuegoId,
					Nombre = actual.Nombre,
					Imagen = actual.Imagen,
					DRM = actual.DRM,
					Enlace = actual.Enlace,
					FechaEmpieza = actual.FechaEmpieza,
					FechaTermina = actual.FechaTermina,
					ImagenNoticia = actual.ImagenNoticia
				});
			}
			catch
			{
			}
		}

		public static void Temporal(SqlConnection conexion, string nombreTabla, string enlace, string nombreJuego = "vacio", string imagen = "vacio")
		{
			bool encontrado = false;
			string sqlBuscar = "SELECT enlace FROM temporal" + nombreTabla + " WHERE enlace=@enlace";

			using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion))
			{
				comando.Parameters.AddWithValue("@enlace", enlace);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						encontrado = true;
					}
				}
			}

			if (encontrado == false)
			{
				string sqlInsertar = "INSERT INTO temporal" + nombreTabla + " " +
					"(enlace, nombre, imagen) VALUES " +
					"(@enlace, @nombre, @imagen) ";

				using (SqlCommand comando = new SqlCommand(sqlInsertar, conexion))
				{
					comando.Parameters.AddWithValue("@enlace", enlace);
					comando.Parameters.AddWithValue("@nombre", nombreJuego);
					comando.Parameters.AddWithValue("@imagen", imagen);

					try
					{
						comando.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Insertar Temporal Suscripción", ex);
					}
				}
			}
		}
	}
}
