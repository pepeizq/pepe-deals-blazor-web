#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;
using Tareas;

namespace BaseDatos.Portada
{
	public static class Buscar
	{
		public static async Task<List<JuegoMinimoTarea>> BuscarMinimos(string tienda = null)
		{
			string busqueda = @"SELECT j.*,
       pmh.DRM as DRMElegido
FROM juegos j
CROSS APPLY OPENJSON(j.PrecioMinimosHistoricos)
WITH (
    FechaActualizacion DATETIME2 '$.FechaActualizacion',
    FechaTermina DATETIME2 '$.FechaTermina',
    DRM INT '$.DRM',
    Tienda NVARCHAR(50) '$.Tienda'
) AS pmh
WHERE j.ultimaModificacion >= DATEADD(day, -3, GETDATE())
  AND j.analisis IS NOT NULL
  AND j.analisis <> 'null'
  AND ISJSON(j.analisis) = 1
  AND JSON_VALUE(j.analisis, '$.Cantidad') IS NOT NULL
  AND TRY_CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'), ',', '')) > 99
  AND j.nombre IS NOT NULL
  AND j.imagenes IS NOT NULL
  AND (j.mayorEdad = 'false' OR j.mayorEdad IS NULL)
  AND (j.freeToPlay = 'false' OR j.freeToPlay IS NULL)
  AND j.PrecioMinimosHistoricos IS NOT NULL
  AND j.PrecioMinimosHistoricos <> 'null'
  AND ISJSON(j.PrecioMinimosHistoricos) = 1
  AND (
        (pmh.FechaActualizacion >= DATEADD(hour, -24, GETDATE()) AND (pmh.Tienda = 'steam' OR pmh.Tienda = 'steambundles')) OR
        (pmh.FechaActualizacion >= DATEADD(hour, -25, GETDATE()) AND (pmh.Tienda = 'humblestore' OR pmh.Tienda = 'humblechoice')) OR
        (pmh.FechaActualizacion >= DATEADD(hour, -48, GETDATE()) AND pmh.Tienda = 'epicgamesstore') OR
        (pmh.FechaActualizacion >= DATEADD(hour, -12, GETDATE()))    
      )
AND (
	pmh.FechaTermina IS NULL
	OR pmh.FechaTermina = '0001-01-01'
	OR pmh.FechaTermina > GETDATE()
)";

			if (string.IsNullOrEmpty(tienda) == false)
			{
				busqueda = busqueda + $" AND pmh.Tienda='{tienda}'";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoMinimoTarea>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Buscar Minimos", ex, false);
			}

			return new List<JuegoMinimoTarea>();
		}

		public static async Task<List<Juego>> Destacados()
		{
			string busqueda = @"SELECT TOP 6 idMaestra, nombre, JSON_VALUE(imagenes, '$.Logo') as logo, JSON_VALUE(imagenes, '$.Library_1920x620') as fondo, JSON_VALUE(imagenes, '$.Header_460x215') as header, precioMinimosHistoricos, JSON_VALUE(media, '$.Videos[0].Micro') as video, idSteam FROM seccionMinimos
WHERE tipo = 0 AND 
year(getdate()) < year(JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam')) + 6 AND
CONVERT(float, JSON_VALUE(precioMinimosHistoricos, '$[0].Precio')) > 1.99 AND 
JSON_VALUE(precioMinimosHistoricos, '$[0].DRM') = 0 AND 
CONVERT(datetime2, JSON_VALUE(precioMinimosHistoricos, '$[0].FechaActualizacion')) > DATEADD(HOUR,-24,GetDate()) AND 
(CONVERT(bigint, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',','')) > 1999 AND 
bundles IS NULL AND 
NOT EXISTS (SELECT 1 FROM gratis WHERE gratis.juegoId = seccionMinimos.idMaestra AND gratis.DRM = 0) AND 
NOT EXISTS (SELECT 1 FROM suscripciones WHERE suscripciones.juegoId = seccionMinimos.idMaestra AND suscripciones.DRM = 0) OR CONVERT(bigint, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',','')) > 29999) AND 
(ocultarPortada IS NULL OR ocultarPortada = 'false') 
ORDER BY NEWID()";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					var filas = await conexion.QueryAsync(busqueda);

					var juegos = filas.Select(fila =>
					{
						Juego juego = new Juego
						{
							Id = fila.idMaestra,
							IdMaestra = fila.idMaestra,
							Nombre = fila.nombre,
							IdSteam = fila.idSteam
						};

						if (!string.IsNullOrEmpty(fila.logo) || !string.IsNullOrEmpty(fila.fondo) || !string.IsNullOrEmpty(fila.header))
						{
							juego.Imagenes = new JuegoImagenes
							{
								Logo = fila.logo,
								Library_1920x620 = fila.fondo,
								Header_460x215 = fila.header
							};
						}

						if (!string.IsNullOrEmpty(fila.precioMinimosHistoricos))
						{
							juego.PrecioMinimosHistoricos =
								JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
						}

						if (!string.IsNullOrEmpty(fila.video))
						{
							juego.Media = new JuegoMedia
							{
								Videos = new List<JuegoMediaVideo>
								{
									new JuegoMediaVideo { Micro = fila.video }
								}
							};
						}

						return juego;
					}).ToList();

					return juegos;

				}).ContinueWith(t => t.Result.ToList());
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Destacados", ex, false);
			}

			return null;
		}

		public static async Task<List<Juego>> Minimos(int tipo, int posicion = 0, List<string> categorias = null, List<string> drms = null, int cantidadReseñas = 199)
		{
			string categoria = null;

			if (categorias?.Count > 0)
			{
				int i = 0;
				foreach (var valor in categorias)
				{
					if (i == 0)
					{
						categoria = categoria + " AND (tipo = " + valor;
					}
					else if (i > 0)
					{
						categoria = categoria + " OR tipo = " + valor;
					}

					i += 1;
				}

				if (string.IsNullOrEmpty(categoria) == false)
				{
					categoria = categoria + ")";
				}
			}

			string drm = null;

			if (drms?.Count > 0)
			{
				int i = 0;
				foreach (var valor in drms)
				{
					if (i == 0)
					{
						drm = drm + " AND (JSON_VALUE(precioMinimosHistoricos, '$[0].DRM') = " + valor;
					}
					else if (i > 0)
					{
						drm = drm + " OR JSON_VALUE(precioMinimosHistoricos, '$[0].DRM') = " + valor;
					}

					i += 1;
				}

				if (string.IsNullOrEmpty(drm) == false)
				{
					drm = drm + ")";
				}
			}

			string busqueda = @"SELECT j.idMaestra, j.nombre, j.imagenes, j.precioMinimosHistoricos, JSON_VALUE(j.media, '$.Videos[0].Micro') as video, j.etiquetas, j.bundles, 
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.idMaestra
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.idMaestra
          AND g.fechaTermina < GETDATE()
        FOR JSON PATH
    ) AS GratisPasados,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.idMaestra
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.idMaestra
          AND s.FechaTermina < GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesPasados, j.idSteam, CONVERT(datetime2, JSON_VALUE(j.precioMinimosHistoricos, '$[0].FechaDetectado')) AS Fecha, j.idGog, j.analisis, CONVERT(datetime2, JSON_VALUE(j.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM seccionMinimos j
                                    WHERE CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',','')) > @cantidadAnalisis AND JSON_VALUE(j.precioMinimosHistoricos, '$[0].Descuento') > 0 AND (j.MayorEdad <> 'true' OR j.MayorEdad IS NULL) @categoria @drm";

			if (tipo == 0)
			{
				busqueda = busqueda + " ORDER BY Fecha DESC";
			}
			else if (tipo == 1)
			{
				busqueda = busqueda + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
			}
			else if (tipo == 2)
			{
				busqueda = busqueda + " AND CONVERT(datetime2, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam')) > DATEADD(DAY,-30,GetDate()) ORDER BY CONVERT(datetime2, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam')) DESC";
			}

			busqueda = busqueda.Replace("@categoria", categoria);
			busqueda = busqueda.Replace("@drm", drm);
			busqueda = busqueda.Replace("@cantidadAnalisis", cantidadReseñas.ToString());

			busqueda = busqueda + @$" OFFSET {posicion} ROWS
										FETCH NEXT 100 ROWS ONLY";

			try
			{
				var filas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryAsync(busqueda);
				});

				List<Juego> resultados = new List<Juego>();

				foreach (var fila in filas)
				{
					Juego juego = new Juego
					{
						Id = fila.idMaestra,
						IdMaestra = fila.idMaestra,
						Nombre = fila.nombre,
						IdSteam = fila.idSteam,
						IdGog = fila.idGog,
						Caracteristicas = fila.FechaLanzamiento != null ? new JuegoCaracteristicas { FechaLanzamientoSteam = fila.FechaLanzamiento } : null
					};

					if (string.IsNullOrEmpty(fila.imagenes) == false)
					{
						juego.Imagenes = JsonSerializer.Deserialize<JuegoImagenes>(fila.imagenes);
					}

					if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
					{
						juego.PrecioMinimosHistoricos = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
					}

					if (string.IsNullOrEmpty(fila.video) == false)
					{
						juego.Media = new JuegoMedia
						{
							Videos = new List<JuegoMediaVideo> { new JuegoMediaVideo { Micro = fila.video } }
						};
					}

					if (string.IsNullOrEmpty(fila.bundles) == false)
					{
						juego.Bundles = JsonSerializer.Deserialize<List<JuegoBundle>>(fila.bundles);
					}

					if (string.IsNullOrEmpty(fila.GratisActuales) == false)
					{
						juego.GratisActuales = JsonSerializer.Deserialize<List<JuegoGratisActuales>>(fila.GratisActuales);
					}

					if (string.IsNullOrEmpty(fila.GratisPasados) == false)
					{
						juego.GratisPasados = JsonSerializer.Deserialize<List<JuegoGratisPasados>>(fila.GratisPasados);
					}

					if (string.IsNullOrEmpty(fila.SuscripcionesActuales) == false)
					{
						juego.SuscripcionesActuales = JsonSerializer.Deserialize<List<JuegoSuscripcionActuales>>(fila.SuscripcionesActuales);
					}

					if (string.IsNullOrEmpty(fila.SuscripcionesPasados) == false)
					{
						juego.SuscripcionesPasados = JsonSerializer.Deserialize<List<JuegoSuscripcionPasados>>(fila.SuscripcionesPasados);
					}

					if (string.IsNullOrEmpty(fila.analisis) == false)
					{
						juego.Analisis = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
					}

					resultados.Add(juego);
				}

				return resultados;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Minimos", ex, false);
			}

			return null;
		}

		public static async Task<List<Juego>> Proximamente(int cantidadJuegos, List<string> categorias = null, List<string> drms = null)
		{
			string busqueda = @"SELECT TOP @cantidadJuegos id, nombre, imagenes, precioMinimosHistoricos, JSON_VALUE(media, '$.Videos[0].Micro') as video, idSteam, idGog, CONVERT(datetime2, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM juegos 
                                    WHERE ISJSON(caracteristicas) > 0 AND DATEDIFF(DAY, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam'), GETDATE()) < 0
ORDER BY CONVERT(datetime2, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam'))";

			busqueda = busqueda.Replace("@cantidadJuegos", cantidadJuegos.ToString());

			try
			{
				var filas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(busqueda)).ToList();
				});

				List<Juego> resultados = new List<Juego>();

				foreach (var fila in filas)
				{
					Juego juego = new Juego
					{
						Id = fila.id,
						IdMaestra = fila.id,
						Nombre = fila.nombre,
						IdSteam = fila.idSteam,
						IdGog = fila.idGog,
						Caracteristicas = fila.FechaLanzamiento != null ? new JuegoCaracteristicas { FechaLanzamientoSteam = fila.FechaLanzamiento } : null
					};

					if (string.IsNullOrEmpty(fila.imagenes) == false)
					{
						juego.Imagenes = JsonSerializer.Deserialize<JuegoImagenes>(fila.imagenes);
					}

					if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
					{
						juego.PrecioMinimosHistoricos = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
					}

					if (string.IsNullOrEmpty(fila.video) == false)
					{
						juego.Media = new JuegoMedia
						{
							Videos = new List<JuegoMediaVideo> { new JuegoMediaVideo { Micro = fila.video } }
						};
					}

					resultados.Add(juego);
				}

				return resultados;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Proximamente", ex, false);
			}

			return null;
		}
	}
}
