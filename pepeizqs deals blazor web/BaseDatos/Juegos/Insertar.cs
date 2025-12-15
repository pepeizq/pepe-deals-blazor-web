#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Juegos
{
	public static class Insertar
	{
		public static async Task Ejecutar(Juego juego, string tabla = "juegos", bool noExiste = false)
		{
			if (string.IsNullOrEmpty(juego.Nombre) == true)
			{
				return;
			}

			List<string> columnas = new() { "idSteam", "idGog", "nombre", "tipo", "fechaSteamAPIComprobacion",
								"imagenes", "precioMinimosHistoricos", "precioActualesTiendas",
								"analisis", "caracteristicas", "media", "nombreCodigo", "categorias" };

			List<string> valores = new() { "@idSteam", "@idGog", "@nombre", "@tipo", "@fechaSteamAPIComprobacion",
								"@imagenes", "@precioMinimosHistoricos", "@precioActualesTiendas",
								"@analisis", "@caracteristicas", "@media", "@nombreCodigo", "@categorias" };

			DynamicParameters parametros = new();

			parametros.Add("@idSteam", juego.IdSteam);
			parametros.Add("@idGog", juego.IdGog);
			parametros.Add("@nombre", juego.Nombre);
			parametros.Add("@tipo", juego.Tipo);
			parametros.Add("@fechaSteamAPIComprobacion", juego.FechaSteamAPIComprobacion.ToString());
			parametros.Add("@imagenes", JsonSerializer.Serialize(juego.Imagenes));
			parametros.Add("@precioMinimosHistoricos", JsonSerializer.Serialize(juego.PrecioMinimosHistoricos));
			parametros.Add("@precioActualesTiendas", JsonSerializer.Serialize(juego.PrecioActualesTiendas));
			parametros.Add("@analisis", JsonSerializer.Serialize(juego.Analisis));
			parametros.Add("@caracteristicas", JsonSerializer.Serialize(juego.Caracteristicas));
			parametros.Add("@media", JsonSerializer.Serialize(juego.Media));
			parametros.Add("@nombreCodigo", Herramientas.Buscador.LimpiarNombre(juego.Nombre));
			parametros.Add("@categorias", JsonSerializer.Serialize(juego.Categorias));

			void AñadirSi(string columna, string parametro, object valor)
			{
				columnas.Add(columna);
				valores.Add(parametro);
				parametros.Add(parametro, valor);
			}

			if (juego.Bundles != null)
			{
				AñadirSi("bundles", "@bundles", JsonSerializer.Serialize(juego.Bundles));
			}

			if (juego.Gratis != null)
			{
				AñadirSi("gratis", "@gratis", JsonSerializer.Serialize(juego.Gratis));
			}

			if (juego.Suscripciones != null)
			{
				AñadirSi("suscripciones", "@suscripciones", JsonSerializer.Serialize(juego.Suscripciones));
			}

			if (string.IsNullOrEmpty(juego.Maestro) == false && juego.Maestro.Length > 1)
			{
				AñadirSi("maestro", "@maestro", juego.Maestro);
			}

			if (string.IsNullOrEmpty(juego.FreeToPlay) == false)
			{
				AñadirSi("freeToPlay", "@freeToPlay", juego.FreeToPlay);
			}

			if (string.IsNullOrEmpty(juego.MayorEdad) == false)
			{
				AñadirSi("mayorEdad", "@mayorEdad", juego.MayorEdad);
			}

			if (juego.Etiquetas?.Count > 0)
			{
				AñadirSi("etiquetas", "@etiquetas", JsonSerializer.Serialize(juego.Etiquetas));
			}

			if (juego.Idiomas?.Count > 0)
			{
				AñadirSi("idiomas", "@idiomas", JsonSerializer.Serialize(juego.Idiomas));
			}

			if (juego.Deck != JuegoDeck.Desconocido)
			{
				AñadirSi("deck", "@deck", juego.Deck);
			}

			if (juego.SteamOS != JuegoSteamOS.Desconocido)
			{
				AñadirSi("steamOS", "@steamOS", juego.SteamOS);
			}

			if (juego.InteligenciaArtificial == true)
			{
				AñadirSi("inteligenciaArtificial", "@inteligenciaArtificial", true);
			}

			if (tabla == "seccionMinimos")
			{
				AñadirSi("idMaestra", "@idMaestra", juego.IdMaestra);
			}

			if (juego.OcultarPortada)
			{
				AñadirSi("ocultarPortada", "@ocultarPortada", true);
			}

			string sqlInsertar = $"INSERT INTO {tabla} ({string.Join(", ", columnas)}) VALUES ({string.Join(", ", valores)})";

			if (noExiste == true)
			{
				sqlInsertar = $@"IF EXISTS (SELECT 1 FROM {tabla} 
								WHERE JSON_VALUE(precioMinimosHistoricos, '$[0].Enlace')='{juego.PrecioMinimosHistoricos[0].Enlace}'
								AND idMaestra={juego.IdMaestra} AND JSON_VALUE(precioMinimosHistoricos, '$[0].DRM')={(int)juego.PrecioMinimosHistoricos[0].DRM}
								AND JSON_QUERY(precioMinimosHistoricos, '$[0]') <> '{juego.PrecioMinimosHistoricos[0]}')
				BEGIN 
					DELETE FROM seccionMinimos 
					WHERE idMaestra={juego.IdMaestra}
					AND JSON_VALUE(precioMinimosHistoricos, '$[0].DRM')={(int)juego.PrecioMinimosHistoricos[0].DRM}
				END
				IF NOT EXISTS (SELECT 1 FROM {tabla} 
								WHERE JSON_VALUE(precioMinimosHistoricos, '$[0].Enlace')='{juego.PrecioMinimosHistoricos[0].Enlace}'
								AND idMaestra={juego.IdMaestra} AND JSON_VALUE(precioMinimosHistoricos, '$[0].DRM')={(int)juego.PrecioMinimosHistoricos[0].DRM}
								AND JSON_QUERY(precioMinimosHistoricos, '$[0]') <> '{juego.PrecioMinimosHistoricos[0]}')
				BEGIN
					{sqlInsertar}
				END";
			}

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteAsync(sqlInsertar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				string añadido = string.IsNullOrEmpty(juego.Nombre) ? juego.IdSteam + " (Id Steam)" : juego.Nombre;
				Errores.Insertar.Mensaje($"Añadir Juego en {tabla} - {sqlInsertar}", ex);
			}
		}

		public static async Task<string> GogReferencia(string idReferencia)
		{
			try
			{
				int? idJuegoBD = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync<int?>("SELECT * FROM gogReferencias2 WHERE idReferencia=@idReferencia", new { idReferencia }, transaction: sentencia);
				}); 

				if (idJuegoBD.HasValue == true && idJuegoBD.Value > 0)
				{
					return idJuegoBD.Value.ToString();
				}
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("GOG Referencia 1", ex);
			}

			string idJuego = await APIs.GOG.Juego.BuscarReferencia(idReferencia);

			if (string.IsNullOrEmpty(idJuego) == false)
			{
				string sqlInsert = @"
                INSERT INTO gogReferencias2 (idReferencia, idJuego)
                VALUES (@idReferencia, @idJuego)";

				try
				{
					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						return await sentencia.Connection.ExecuteAsync(sqlInsert, new
						{
							idReferencia,
							idJuego
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					Errores.Insertar.Mensaje("GOG Referencia 2 " + idJuego, ex);
				}
			}

			return idReferencia;
		}
	}
}
