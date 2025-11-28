#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BaseDatos.Curators
{
	public static class Actualizar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Ejecutar(Curator curator, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string añadirImagenFondo = null;

			if (string.IsNullOrEmpty(curator.ImagenFondo) == false)
			{
				añadirImagenFondo = ", imagenFondo=@imagenFondo";
			}
			
			conexion.Execute($@"UPDATE curators SET nombre=@nombre, imagen=@imagen, descripcion=@descripcion, slug=@slug, steamIds=@steamIds, web=@web, fecha=@fecha {añadirImagenFondo} WHERE idSteam=@idSteam", new
			{
				idSteam = curator.IdSteam,
				nombre = curator.Nombre,
				imagen = curator.Imagen,
				descripcion = curator.Descripcion,
				slug = curator.Slug,
				steamIds = JsonSerializer.Serialize(curator.SteamIds),
				web = JsonSerializer.Serialize(curator.Web),
				fecha = DateTime.Now,
				imagenFondo = curator.ImagenFondo
			});
		}

		public static void ImagenFondo(string imagenFondo, int id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			conexion.Execute("UPDATE curators SET imagenFondo=@imagenFondo WHERE idSteam=@idSteam", new 
			{ 
				imagenFondo, 
				idSteam = id 
			});
		}
	}
}
