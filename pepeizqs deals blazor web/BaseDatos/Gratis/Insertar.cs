#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Gratis
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

		public static void Ejecutar(JuegoGratis actual, SqlConnection conexion = null)
		{
            conexion = CogerConexion(conexion);

			string sqlInsertar = @"
				INSERT INTO gratis 
				(gratis, juegoId, nombre, imagen, drm, enlace, fechaEmpieza, fechaTermina, imagenNoticia) 
				VALUES 
				(@Tipo, @JuegoId, @Nombre, @Imagen, @DRM, @Enlace, @FechaEmpieza, @FechaTermina, @ImagenNoticia)
			";

			try
			{
				conexion.Execute(sqlInsertar, new
				{
					actual.Tipo,
					actual.JuegoId,
					actual.Nombre,
					actual.Imagen,
					actual.DRM,
					actual.Enlace,
					actual.FechaEmpieza,
					actual.FechaTermina,
					actual.ImagenNoticia
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Insertar juego gratis", ex);
			}
		}
	}
}
