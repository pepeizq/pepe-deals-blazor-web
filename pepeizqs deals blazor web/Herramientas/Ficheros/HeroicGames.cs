#nullable disable

using Juegos;
using Microsoft.AspNetCore.Components.Forms;
using pepeizqs_deals_web.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Herramientas.Ficheros
{
	public static class HeroicGames
	{
		public static async Task<int> Cargar(JuegoDRM drm, IBrowserFile fichero, string usuarioId)
		{
			Usuario usuario = await global::BaseDatos.Usuarios.Buscar.OpcionesAmazon(usuarioId);
			usuario.Id = usuarioId;

			int importados = 0;

			int maximoTamaño = 268435456; //256 mb
			byte[] buffer = new byte[fichero.Size];

			LecturaPerezosa stream = new LecturaPerezosa(fichero, maximoTamaño);
			StreamContent contenido = new StreamContent(stream);

			string contenido2 = await contenido.ReadAsStringAsync();

			List<HeroicGamesJuego> listadoJuegos = new List<HeroicGamesJuego>();

			if (string.IsNullOrEmpty(contenido2) == false)
			{
				List<HeroicGamesBD> bd = JsonSerializer.Deserialize<List<HeroicGamesBD>>(contenido2);

				if (bd != null)
				{
					if (bd.Count > 0)
					{
						foreach (HeroicGamesBD registro in bd)
						{
							if (drm == JuegoDRM.Amazon && registro.Tipo.Contains("com.amazon") == true)
							{
								HeroicGamesJuego juego = new HeroicGamesJuego
								{
									Nombre = registro.Producto.Nombre,
									Id = registro.Producto.Id
								};

								juego.Id = juego.Id.Replace("amzn1.resource.", null);

								listadoJuegos.Add(juego);
							}
						}
					}
				}
			}

			if (listadoJuegos.Count > 0)
			{
				string textoIds = string.Empty;

				foreach (HeroicGamesJuego juego in listadoJuegos)
				{
					if (drm == JuegoDRM.Amazon)
					{
						await global::BaseDatos.Plataformas.Buscar.Amazon(juego.Id, juego.Nombre);
					}

					if (string.IsNullOrEmpty(textoIds) == true)
					{
						textoIds = juego.Id;
					}
					else
					{
						textoIds = textoIds + "," + juego.Id;
					}

					importados += 1;
				}

				if (usuario != null)
				{
					if (drm == JuegoDRM.Amazon)
					{
						usuario.AmazonGames = textoIds;
						usuario.AmazonLastImport = DateTime.Now;

						await global::BaseDatos.Usuarios.Actualizar.Amazon(usuario);
					}
				}
			}

			return importados;
		}
	}

	public class HeroicGamesBD
	{
		[JsonPropertyName("product")]
		public HeroicGamesDBProducto Producto { get; set; }

		[JsonPropertyName("__type")]
		public string Tipo { get; set; }
	}

	public class HeroicGamesDBProducto
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("title")]
		public string Nombre { get; set; }
	}

	public class HeroicGamesJuego
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
	}
}
