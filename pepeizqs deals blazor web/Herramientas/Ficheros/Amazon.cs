#nullable disable

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.Sqlite;
using pepeizqs_deals_web.Data;

namespace Herramientas.Ficheros
{
	public static class Amazon
	{
		public static async Task<int> Cargar(IBrowserFile fichero, string usuarioId)
		{
			Usuario usuario = await global::BaseDatos.Usuarios.Buscar.OpcionesAmazon(usuarioId);
			usuario.Id = usuarioId;

			int importados = 0;

			int maximoTamaño = 268435456; //256 mb;
			byte[] buffer = new byte[fichero.Size];

			Herramientas.Ficheros.LecturaPerezosa stream = new Herramientas.Ficheros.LecturaPerezosa(fichero, maximoTamaño);
			StreamContent contenido = new StreamContent(stream);

			string ubicacion = Path.GetFullPath("./wwwroot/otros/amazon-" + usuarioId + ".sqlite");
			await File.WriteAllBytesAsync(ubicacion, await contenido.ReadAsByteArrayAsync());

			List<string> listadoIds = new List<string>();

			using (SqliteConnection conexion = new SqliteConnection("Data Source=" + ubicacion))
			{
				conexion.Open();

				SqliteCommand comando = conexion.CreateCommand();
				comando.CommandText = "SELECT * FROM dbset";

				using (SqliteDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(0)) == false)
							{
								listadoIds.Add(lector.GetString(0));
							}
						}
					}
				}
			}

			if (listadoIds.Count > 0)
			{
				string textoIds = string.Empty;

				foreach (string id in listadoIds)
				{
					await global::BaseDatos.Plataformas.Buscar.Amazon(id, null);

					if (string.IsNullOrEmpty(textoIds) == true)
					{
						textoIds = id;
					}
					else
					{
						textoIds = textoIds + "," + id;
					}

					importados += 1;
				}

				if (usuario != null)
				{
					usuario.AmazonGames = textoIds;
					usuario.AmazonLastImport = DateTime.Now;

					await global::BaseDatos.Usuarios.Actualizar.Amazon(usuario);
				}
			}

			return importados;
		}
	}
}
