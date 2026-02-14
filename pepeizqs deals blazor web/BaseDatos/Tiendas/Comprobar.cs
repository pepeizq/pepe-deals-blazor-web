#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Tiendas
{
	public static class Comprobar
	{
		public static async Task Steam(List<JuegoPrecio> ofertas, List<JuegoReseñas> reseñas, bool rapido)
		{
			if (ofertas == null || ofertas.Count == 0)
			{
				return;
			}

			var idsSteam = new List<int>();
			var mapaOfertas = new Dictionary<int, JuegoPrecio>();
			var mapaReseñas = new Dictionary<int, JuegoReseñas>();

			foreach (var oferta in ofertas)
			{
				string idSteam2 = APIs.Steam.Juego.LimpiarID(oferta.Enlace);

				if (string.IsNullOrEmpty(idSteam2) == false)
				{
					idSteam2 = APIs.Steam.Tienda.IdsEspeciales(idSteam2);

					if (int.TryParse(idSteam2, out int idSteam) && idSteam > 0)
					{
						idsSteam.Add(idSteam);
						mapaOfertas[idSteam] = oferta;
					}
				}
			}

			if (reseñas != null && reseñas.Count > 0)
			{
				foreach (var reseña in reseñas)
				{
					mapaReseñas[reseña.IdSteam] = reseña;
				}
			}

			if (idsSteam.Count == 0)
			{
				return;
			}

			string buscarJuegos = "SELECT id, precioMinimosHistoricos, precioActualesTiendas, idSteam, historicos, fechaSteamAPIComprobacion FROM juegos WHERE idSteam IN @idsSteam";

			try
			{
				var datos = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<dynamic>(buscarJuegos, new { idsSteam })).ToList();
				});

				var juegosEncontrados = datos.Cast<dynamic>().ToList();

				foreach (var dato in juegosEncontrados)
				{
					int idSteam = dato.idSteam;
					var oferta = mapaOfertas[idSteam];
					var reseña = mapaReseñas.ContainsKey(idSteam) ? mapaReseñas[idSteam] : null;

					Juego juego = JuegoCrear.Generar();

					if (reseña != null && juego != null)
					{
						if (string.IsNullOrEmpty(reseña.Cantidad) == false && string.IsNullOrEmpty(reseña.Porcentaje) == false)
						{
							JuegoAnalisis nuevasReseñas = new JuegoAnalisis();
							nuevasReseñas.Cantidad = reseña.Cantidad;
							nuevasReseñas.Porcentaje = reseña.Porcentaje;

							juego.Analisis = nuevasReseñas;
						}
					}

					juego.IdSteam = idSteam;

					string fechaAPI = dato.fechaSteamAPIComprobacion;

					if (string.IsNullOrEmpty(fechaAPI) == false)
					{
						bool actualizarAPI = DateTime.Now.Subtract(DateTime.Parse(fechaAPI)) > TimeSpan.FromDays(91);

						if (actualizarAPI == true)
						{
							await BaseDatos.JuegosActualizar.Insertar.Ejecutar(juego.Id, juego.IdSteam, "SteamAPI");
						}

						int id = dato.id ?? 0;

						if (id > 0)
						{
							List<JuegoPrecio> ofertasHistoricas = null;

							if (string.IsNullOrEmpty(dato.precioMinimosHistoricos) == false)
							{
								ofertasHistoricas = JsonSerializer.Deserialize<List<JuegoPrecio>>(dato.precioMinimosHistoricos) ?? new List<JuegoPrecio>();

								if (ofertasHistoricas.Count == 0)
								{
									ofertasHistoricas.Add(oferta);
								}
							}

							List<JuegoPrecio> ofertasActuales = null;

							if (string.IsNullOrEmpty(dato.precioActualesTiendas) == false)
							{
								ofertasActuales = JsonSerializer.Deserialize<List<JuegoPrecio>>(dato.precioActualesTiendas) ?? new List<JuegoPrecio>();

								if (ofertasActuales.Count == 0)
								{
									ofertasActuales.Add(oferta);
								}
							}

							List<JuegoHistorico> historicos = null;

							if (string.IsNullOrEmpty(dato.historicos) == false)
							{
								historicos = JsonSerializer.Deserialize<List<JuegoHistorico>>(dato.historicos) ?? new List<JuegoHistorico>();
							}

							await Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, null, null, null, juego.Analisis);
						}
					}
				}

				var idsSteamEncontrados = juegosEncontrados.Select(d => (int)d.idSteam).ToList();
				var idsSteamNoEncontrados = idsSteam.Where(id => !idsSteamEncontrados.Contains(id)).ToList();

				if (rapido == false && idsSteamNoEncontrados.Count > 0)
				{
					foreach (var idSteam in idsSteamNoEncontrados)
					{
						var oferta = mapaOfertas[idSteam];
						var reseña = mapaReseñas.ContainsKey(idSteam) ? mapaReseñas[idSteam] : null;
						string idSteam2 = idSteam.ToString();

						try
						{
							Juego juego = await APIs.Steam.Juego.CargarDatosJuego(idSteam2);

							if (juego != null)
							{
								if (reseña != null)
								{
									if (string.IsNullOrEmpty(reseña.Cantidad) == false && string.IsNullOrEmpty(reseña.Porcentaje) == false)
									{
										JuegoAnalisis nuevasReseñas = new JuegoAnalisis();
										nuevasReseñas.Cantidad = reseña.Cantidad;
										nuevasReseñas.Porcentaje = reseña.Porcentaje;

										juego.Analisis = nuevasReseñas;
									}
								}

								juego.PrecioActualesTiendas ??= new List<JuegoPrecio>();
								juego.PrecioMinimosHistoricos ??= new List<JuegoPrecio>();

								if (juego.PrecioActualesTiendas.Count == 0)
								{
									juego.PrecioActualesTiendas.Add(oferta);
									juego.PrecioMinimosHistoricos.Add(oferta);
								}

								await Juegos.Insertar.Ejecutar(juego);
							}
						}
						catch { }
					}
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Steam", ex);
			}
		}

		public static async Task Steam(JuegoPrecio oferta, JuegoAnalisis reseñas, bool rapido)
		{
			string idSteam2 = APIs.Steam.Juego.LimpiarID(oferta.Enlace);

			if (string.IsNullOrEmpty(idSteam2) == false)
			{
				idSteam2 = APIs.Steam.Tienda.IdsEspeciales(idSteam2);

				if (!int.TryParse(idSteam2, out int idSteam) || idSteam <= 0)
				{
					return;
				}

				if (idSteam > 0)
				{
					Juego juego = JuegoCrear.Generar();

					if (reseñas != null && juego != null)
					{
						if (string.IsNullOrEmpty(reseñas.Cantidad) == false && string.IsNullOrEmpty(reseñas.Porcentaje) == false)
						{
							JuegoAnalisis nuevasReseñas = new JuegoAnalisis();
							nuevasReseñas.Cantidad = reseñas.Cantidad;
							nuevasReseñas.Porcentaje = reseñas.Porcentaje;

							juego.Analisis = nuevasReseñas;
						}
					}

					string buscarJuego = "SELECT id, precioMinimosHistoricos, precioActualesTiendas, idSteam, historicos, fechaSteamAPIComprobacion FROM juegos WHERE idSteam=@idSteam";

					try
					{
						var datos = await Herramientas.BaseDatos.Select(async conexion =>
						{
							return await conexion.QueryFirstOrDefaultAsync<dynamic>(buscarJuego, new { idSteam });
						});

						if (datos != null)
						{
							juego.IdSteam = idSteam;

							string fechaAPI = datos.fechaSteamAPIComprobacion;

							if (string.IsNullOrEmpty(fechaAPI) == false)
							{
								bool actualizarAPI = DateTime.Now.Subtract(DateTime.Parse(fechaAPI)) > TimeSpan.FromDays(91);

								if (actualizarAPI == true)
								{
									await BaseDatos.JuegosActualizar.Insertar.Ejecutar(juego.Id, juego.IdSteam, "SteamAPI");
								}

								int id = datos.id ?? 0;

								if (id > 0)
								{
									List<JuegoPrecio> ofertasHistoricas = null;

									if (string.IsNullOrEmpty(datos.precioMinimosHistoricos) == false)
									{
										ofertasHistoricas = JsonSerializer.Deserialize<List<JuegoPrecio>>(datos.precioMinimosHistoricos) ?? new List<JuegoPrecio>();

										if (ofertasHistoricas.Count == 0)
										{
											ofertasHistoricas.Add(oferta);
										}
									}

									List<JuegoPrecio> ofertasActuales = null;

									if (string.IsNullOrEmpty(datos.precioActualesTiendas) == false)
									{
										ofertasActuales = JsonSerializer.Deserialize<List<JuegoPrecio>>(datos.precioActualesTiendas) ?? new List<JuegoPrecio>();

										if (ofertasActuales.Count == 0)
										{
											ofertasActuales.Add(oferta);
										}
									}

									List<JuegoHistorico> historicos = null;

									if (string.IsNullOrEmpty(datos.historicos) == false)
									{
										historicos = JsonSerializer.Deserialize<List<JuegoHistorico>>(datos.historicos) ?? new List<JuegoHistorico>();
									}

									await Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, null, null, null, juego.Analisis);
								}
							}
						}
						else
						{
							if (rapido == false)
							{
								try
								{
									juego = await APIs.Steam.Juego.CargarDatosJuego(idSteam2);
								}
								catch { }

								if (juego != null)
								{
									juego.PrecioActualesTiendas ??= new List<JuegoPrecio>();
									juego.PrecioMinimosHistoricos ??= new List<JuegoPrecio>();

									if (juego.PrecioActualesTiendas.Count == 0)
									{
										juego.PrecioActualesTiendas.Add(oferta);
										juego.PrecioMinimosHistoricos.Add(oferta);
									}

									await Juegos.Insertar.Ejecutar(juego);
								}
							}
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Steam", ex);
					}
				}
			}
		}

		public static async Task Resto(JuegoPrecio oferta, string idGog = null, string slugGOG = null, string slugEpic = null)
		{
			bool encontrado = false;

			string esquema = $"tienda{oferta.Tienda}";
			string sqlBuscar = $@"
SELECT j.id,
       j.precioMinimosHistoricos,
       j.precioActualesTiendas,
       j.idSteam,
       j.historicos,
       j.analisis
FROM {esquema} t
CROSS APPLY (
    SELECT TRY_CAST(value AS INT) AS numero
    FROM STRING_SPLIT(t.idJuegos, ',')
) ids
JOIN juegos j ON j.id = ids.numero
WHERE t.enlace = @Enlace
  AND t.descartado = 'no'
  AND ids.numero IS NOT NULL;
";

			List<dynamic> resultados = new List<dynamic>();

			try
			{
				resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(sqlBuscar, new
					{
						Enlace = oferta.Enlace
					})).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 1", ex);
			}

			if (resultados?.Count > 0)
			{
				foreach (var fila in resultados)
				{
					encontrado = true;

					int id = fila.id ?? 0;
					int idSteam = fila.idSteam ?? 0;

					var ofertasHistoricas = string.IsNullOrEmpty(fila.precioMinimosHistoricos) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);

					var ofertasActuales = string.IsNullOrEmpty(fila.precioActualesTiendas) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioActualesTiendas);

					var historicos = string.IsNullOrEmpty(fila.historicos) ? new List<JuegoHistorico>() : JsonSerializer.Deserialize<List<JuegoHistorico>>(fila.historicos);

					JuegoAnalisis reseñas = null;
					if (!string.IsNullOrEmpty(fila.analisis) && fila.analisis != "null")
					{
						reseñas = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
					}

					if (ofertasActuales?.Count == 0)
					{
						ofertasActuales.Add(oferta);
					}

					if (id > 0)
					{
						await Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, slugGOG, idGog, slugEpic, reseñas);
					}
				}
			}

			// -----------------------------------------------------------------

			if (encontrado == false)
			{
				string sqlInsertar = $@"
IF NOT EXISTS (SELECT 1 FROM {esquema} WHERE enlace = @Enlace)
BEGIN
    DECLARE @nuevaId NVARCHAR(MAX); 

    SELECT @nuevaId = id 
    FROM juegos 
    WHERE nombreCodigo = @NombreCodigo;

    IF @nuevaId IS NULL SET @nuevaId = 0;

    INSERT INTO {esquema} (enlace, nombre, imagen, idJuegos, descartado)
    VALUES (@Enlace, @Nombre, @Imagen, @nuevaId, 'no');
END;
";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlInsertar, new
						{
							oferta.Enlace,
							oferta.Nombre,
							oferta.Imagen,
							NombreCodigo = Herramientas.Buscador.LimpiarNombre(oferta.Nombre)
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 2 - " + sqlInsertar, ex);
				}
			}
		}

		public static async Task Resto(List<JuegoPrecio> ofertas)
		{
			if (ofertas == null || ofertas.Count == 0)
			{
				return;
			}

			var ofertasPorTienda = ofertas.GroupBy(o => o.Tienda).ToList();

			foreach (var grupoTienda in ofertasPorTienda)
			{
				string esquema = $"tienda{grupoTienda.Key}";

				var enlacesUnicos = grupoTienda.DistinctBy(o => o.Enlace).ToList();
				var enlaces = enlacesUnicos.Select(o => o.Enlace).ToList();

				string placeholders = string.Join(",", enlaces.Select((_, i) => $"@enlace{i}"));

				string sqlBuscar = $@"
SELECT j.id,
       j.precioMinimosHistoricos,
       j.precioActualesTiendas,
       j.idSteam,
       j.historicos,
       j.analisis,
       t.enlace
FROM {esquema} t
CROSS APPLY (
    SELECT TRY_CAST(value AS INT) AS numero
    FROM STRING_SPLIT(t.idJuegos, ',')
) ids
JOIN juegos j ON j.id = ids.numero
WHERE t.enlace IN ({placeholders})
  AND t.descartado = 'no'
  AND ids.numero IS NOT NULL;
";

				var parametros = new DynamicParameters();
				for (int i = 0; i < enlaces.Count; i++)
				{
					parametros.Add($"@enlace{i}", enlaces[i]);
				}

				List<dynamic> resultados = new List<dynamic>();

				try
				{
					resultados = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync(sqlBuscar, parametros)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 1", ex);
					continue;
				}

				var mapaOfertas = enlacesUnicos.ToDictionary(o => o.Enlace, o => o);
				var enlacesEncontrados = new HashSet<string>();

				if (resultados?.Count > 0)
				{
					foreach (var fila in resultados)
					{
						string enlace = fila.enlace;
						enlacesEncontrados.Add(enlace);

						if (!mapaOfertas.ContainsKey(enlace))
							continue;

						var oferta = mapaOfertas[enlace];

						int id = fila.id ?? 0;
						int idSteam = fila.idSteam ?? 0;

						var ofertasHistoricas = string.IsNullOrEmpty(fila.precioMinimosHistoricos) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);

						var ofertasActuales = string.IsNullOrEmpty(fila.precioActualesTiendas) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioActualesTiendas);

						var historicos = string.IsNullOrEmpty(fila.historicos) ? new List<JuegoHistorico>() : JsonSerializer.Deserialize<List<JuegoHistorico>>(fila.historicos);

						JuegoAnalisis reseñas = null;
						if (!string.IsNullOrEmpty(fila.analisis) && fila.analisis != "null")
						{
							reseñas = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
						}

						if (ofertasActuales?.Count == 0)
						{
							ofertasActuales.Add(oferta);
						}

						if (id > 0)
						{
							await Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, null, null, null, reseñas);
						}
					}
				}

				// -----------------------------------------------------------------

				var enlacesNoEncontrados = mapaOfertas.Where(kv => !enlacesEncontrados.Contains(kv.Key)).ToList();

				if (enlacesNoEncontrados.Count > 0)
				{
					foreach (var kvp in enlacesNoEncontrados)
					{
						var oferta = kvp.Value;

						string sqlInsertar = $@"
IF NOT EXISTS (SELECT 1 FROM {esquema} WHERE enlace = @Enlace)
BEGIN
    DECLARE @nuevaId NVARCHAR(MAX); 

    SELECT @nuevaId = id 
    FROM juegos 
    WHERE nombreCodigo = @NombreCodigo;

    IF @nuevaId IS NULL SET @nuevaId = 0;

    INSERT INTO {esquema} (enlace, nombre, imagen, idJuegos, descartado)
    VALUES (@Enlace, @Nombre, @Imagen, @nuevaId, 'no');
END;
";

						try
						{
							await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
							{
								return await conexion.ExecuteAsync(sqlInsertar, new
								{
									oferta.Enlace,
									oferta.Nombre,
									oferta.Imagen,
									NombreCodigo = Herramientas.Buscador.LimpiarNombre(oferta.Nombre)
								}, transaction: sentencia);
							});
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 2 - " + sqlInsertar, ex);
						}
					}
				}
			}
		}
	}
}