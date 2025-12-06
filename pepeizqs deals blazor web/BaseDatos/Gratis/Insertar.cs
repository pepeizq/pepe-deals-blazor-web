#nullable disable

using Dapper;
using Juegos;

namespace BaseDatos.Gratis
{
	public static class Insertar
	{
		public static void Ejecutar(JuegoGratis actual)
		{
			string sqlInsertar = @"
				INSERT INTO gratis 
				(gratis, juegoId, nombre, imagen, drm, enlace, fechaEmpieza, fechaTermina, imagenNoticia) 
				VALUES 
				(@Tipo, @JuegoId, @Nombre, @Imagen, @DRM, @Enlace, @FechaEmpieza, @FechaTermina, @ImagenNoticia)
			";

			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Execute(sqlInsertar, new
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
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Insertar", ex);
			}
		}
	}
}
