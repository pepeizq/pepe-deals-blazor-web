#nullable disable

using Dapper;
using Newtonsoft.Json;

namespace BaseDatos.Bundles
{
	public static class Insertar
	{
		public static async void Ejecutar(Bundles2.Bundle bundle) 
		{
			string sqlInsertar = "INSERT INTO bundles " +
					"(bundleTipo, nombre, tienda, imagen, enlace, fechaEmpieza, fechaTermina, juegos, tiers, pick, imagenNoticia) VALUES " +
					"(@bundleTipo, @nombre, @tienda, @imagen, @enlace, @fechaEmpieza, @fechaTermina, @juegos, @tiers, @pick, @imagenNoticia) ";

			var parametros = new
			{
				bundleTipo = bundle.BundleTipo,
				nombre = bundle.Nombre,
				tienda = bundle.Tienda,
				imagen = bundle.Imagen,
				enlace = bundle.Enlace,
				fechaEmpieza = bundle.FechaEmpieza,
				fechaTermina = bundle.FechaTermina,
				juegos = JsonConvert.SerializeObject(bundle.Juegos),
				tiers = JsonConvert.SerializeObject(bundle.Tiers),
				pick = bundle.Pick.ToString(),
				imagenNoticia = bundle.ImagenNoticia
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteAsync(sqlInsertar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Insertar 1", ex);
			}

			try
			{
				int id = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QuerySingleOrDefaultAsync<int>("SELECT MAX(id) FROM bundles", transaction: sentencia);
				});

				if (id <= 0 || bundle.Juegos == null || bundle.Juegos?.Count == 0)
				{
					return;
				}

				foreach (var juego in bundle.Juegos)
				{
					var juego2 = Juegos.Buscar.UnJuego(juego.JuegoId);

					if (juego2 == null)
					{
						continue;
					}

					if (juego2.Bundles == null)
					{
						juego2.Bundles = new List<global::Juegos.JuegoBundle>();
					}

					var juegoBundle = new global::Juegos.JuegoBundle
					{
						DRM = juego.DRM,
						JuegoId = int.Parse(juego.JuegoId),
						Tier = juego.Tier,
						BundleId = id,
						FechaEmpieza = bundle.FechaEmpieza,
						FechaTermina = bundle.FechaTermina,
						Imagen = juego.Imagen,
						Nombre = juego.Nombre,
						Enlace = bundle.Enlace,
						Tipo = bundle.BundleTipo
					};

					juego2.Bundles.Add(juegoBundle);

					string sqlActualizarJuego = @"
						UPDATE juegos 
						SET bundles = @bundles
						WHERE id = @id;
					";

					var parametrosJuego = new
					{
						id = juego.JuegoId,
						bundles = JsonConvert.SerializeObject(juego2.Bundles)
					};

					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						return await sentencia.Connection.ExecuteAsync(sqlActualizarJuego, parametrosJuego, transaction: sentencia);
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Insertar 2", ex);
			}
		}
	}
}
