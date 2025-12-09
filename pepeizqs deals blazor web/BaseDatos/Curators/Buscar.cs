#nullable disable

using APIs.Steam;
using Dapper;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BaseDatos.Curators
{
	public static class Buscar
	{
		public static async Task<Curator> Uno(int idSteam)
		{
			try
			{
				var fila = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync("SELECT * FROM curators WITH (NOLOCK) WHERE idSteam=@idSteam", new { idSteam }, transaction: sentencia);
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Curator Buscar Uno", ex);
			}

			return null;
		}

		public static async Task<Curator> Uno(string slug)
		{
			try
			{
				var fila = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync("SELECT * FROM curators WITH (NOLOCK) WHERE slug=@slug", new { slug }, transaction: sentencia);
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Curator Buscar Uno", ex);
			}

			return null;
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
