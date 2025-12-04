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

		public static void Temporal(string nombreTabla, string enlace, string nombreJuego = "vacio", string imagen = "vacio", SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlBuscar = $"SELECT enlace FROM temporal{nombreTabla} WHERE enlace = @enlace";
			var resultado = conexion.QuerySingleOrDefault<string>(sqlBuscar, new { enlace });

			if (resultado == null)
			{
				string sqlInsertar = $"INSERT INTO temporal{nombreTabla} (enlace, nombre, imagen) " +
									 "VALUES (@enlace, @nombre, @imagen)";

				try
				{
					conexion.Execute(sqlInsertar, new { 
						enlace, 
						nombre = nombreJuego, 
						imagen 
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Insertar Temporal Suscripción", ex);
				}
			}
		}
	}
}
