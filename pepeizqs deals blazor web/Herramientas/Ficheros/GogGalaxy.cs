#nullable disable

using Juegos;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using pepeizqs_deals_web.Data;

namespace Herramientas.Ficheros
{
	public static class GogGalaxy
	{
		public static async Task<int> Cargar(JuegoDRM drm, IBrowserFile fichero, string usuarioId)
		{
			Usuario usuario = await global::BaseDatos.Usuarios.Buscar.OpcionesGOGGalaxy(usuarioId);
			usuario.Id = usuarioId;

			int importados = 0;

			int maximoTamaño = 268435456; //256 mb
			byte[] buffer = new byte[fichero.Size];

			LecturaPerezosa stream = new LecturaPerezosa(fichero, maximoTamaño);
			StreamContent contenido = new StreamContent(stream);

			string ubicacion = Path.GetFullPath("./wwwroot/otros/goggalaxy-" + usuarioId + ".db");
			await File.WriteAllBytesAsync(ubicacion, await contenido.ReadAsByteArrayAsync());

			List<GogGalaxyJuego> listadoJuegos = new List<GogGalaxyJuego>();

			using (SqliteConnection conexion = new SqliteConnection("Data Source=" + ubicacion))
			{
				conexion.Open();

				SqliteCommand comando = conexion.CreateCommand();
				comando.CommandText = "SELECT releaseKey, value FROM GamePieces WHERE gamePieceTypeId = 16";

				using (SqliteDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(0)) == false)
							{
								if (drm == JuegoDRM.Amazon && lector.GetString(0).Contains("amazon_") == true)
								{
									string id = lector.GetString(0);
									id = id.Replace("amazon_", null);

									string nombre = lector.GetString(1);
									nombre = nombre.Replace("{" + Strings.ChrW(34) + "title" + Strings.ChrW(34) + ":" + Strings.ChrW(34), null);
									nombre = nombre.Replace("}", null);
									nombre = nombre.Replace("\"", null);

									GogGalaxyJuego nuevo = new GogGalaxyJuego
									{
										Id = id,
										Nombre = nombre
									};

									listadoJuegos.Add(nuevo);
								}

								if (drm == JuegoDRM.Epic && lector.GetString(0).Contains("epic_") == true)
								{
									string id = lector.GetString(0);
									id = id.Replace("epic_", null);

									string nombre = lector.GetString(1);
									nombre = nombre.Replace("{" + Strings.ChrW(34) + "title" + Strings.ChrW(34) + ":" + Strings.ChrW(34), null);
									nombre = nombre.Replace("}", null);
									nombre = nombre.Replace("\"", null);

									GogGalaxyJuego nuevo = new GogGalaxyJuego
									{
										Id = id,
										Nombre = nombre
									};

									listadoJuegos.Add(nuevo);
								}
							}
						}
					}
				}
			}

			if (listadoJuegos.Count > 0)
			{
				string textoIds = string.Empty;

				foreach (GogGalaxyJuego juego in listadoJuegos)
				{
					if (drm == JuegoDRM.Amazon)
					{
						await global::BaseDatos.Plataformas.Buscar.Amazon(juego.Id, juego.Nombre);
					}

					if (drm == JuegoDRM.Epic)
					{
						await global::BaseDatos.Plataformas.Buscar.Epic(juego.Id, juego.Nombre);
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

					if (drm == JuegoDRM.Epic)
					{
						usuario.EpicGames = textoIds;
						usuario.EpicGamesLastImport = DateTime.Now;

						await global::BaseDatos.Usuarios.Actualizar.Epic(usuario);
					}

					if (drm == JuegoDRM.EA)
					{
						usuario.EaGames = textoIds;
						usuario.EaLastImport = DateTime.Now;

						await global::BaseDatos.Usuarios.Actualizar.EA(usuario);
					}
				}
			}

			return importados;
		}
	}

	public class GogGalaxyJuego
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
	}
}
