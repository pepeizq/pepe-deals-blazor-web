#nullable disable

using APIs.Steam;
using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BaseDatos.Curators
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

		public static Curator Uno(int idSteam, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			var fila = conexion.QueryFirstOrDefault<dynamic>("SELECT * FROM curators WHERE idSteam=@idSteam", new { idSteam }); 
			
			if (fila == null)
			{
				return null;
			}
	
			Curator curator = new Curator
			{
				Id = fila.id,
				IdSteam = fila.idSteam,
				Nombre = fila.nombre,
				Imagen = fila.imagen,
				Descripcion = fila.descripcion,
				Slug = fila.slug, 
				SteamIds = JsonSerializer.Deserialize<List<int>>(fila.steamIds), 
				Web = JsonSerializer.Deserialize<SteamCuratorAPIWeb>(fila.web) 
			}; 
			
			curator.ImagenFondo = fila.imagenFondo != null ? (string)fila.imagenFondo : null;

			if (fila.fecha != null)
			{
				curator.Fecha = (DateTime)fila.fecha;
			}

			return curator;
		}

		public static Curator Uno(string slug, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			var fila = conexion.QueryFirstOrDefault<dynamic>("SELECT * FROM curators WHERE slug=@slug", new { slug });

			if (fila == null)
			{
				return null;
			}

			Curator curator = new Curator
			{
				Id = fila.id,
				IdSteam = fila.idSteam,
				Nombre = fila.nombre,
				Imagen = fila.imagen,
				Descripcion = fila.descripcion,
				Slug = fila.slug,
				SteamIds = JsonSerializer.Deserialize<List<int>>(fila.steamIds),
				Web = JsonSerializer.Deserialize<SteamCuratorAPIWeb>(fila.web)
			};

			curator.ImagenFondo = fila.imagenFondo != null ? (string)fila.imagenFondo : null;

			if (fila.fecha != null)
			{
				curator.Fecha = (DateTime)fila.fecha;
			}

			return curator;
		}
	}

	public class Curator : ComponentBase, IComponent
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int IdSteam { get; set; }
		public string Nombre { get; set; }
		public string Imagen { get; set; }
		public string Descripcion { get; set; }
		public string Slug { get; set; }
		public List<int> SteamIds { get; set; }
		public SteamCuratorAPIWeb Web { get; set; }
		public string ImagenFondo { get; set; }
		public DateTime? Fecha { get; set; }
	}

	public class CuratorFicha
	{
		public Curator Curator { get; set; }
		public int Posicion { get; set; }
		public List<global::Juegos.Juego> JuegosMostrar { get; set; }
	}
}
