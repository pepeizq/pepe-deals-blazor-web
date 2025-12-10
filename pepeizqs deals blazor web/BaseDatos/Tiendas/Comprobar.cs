#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Tiendas
{
	public static class Comprobar
	{
		public static async void Steam(JuegoPrecio oferta, JuegoAnalisis reseñas, bool rapido)
		{
			string idSteam2 = APIs.Steam.Juego.LimpiarID(oferta.Enlace);

			if (string.IsNullOrEmpty(idSteam2) == false)
			{
				idSteam2 = APIs.Steam.Tienda.IdsEspeciales(idSteam2);

				int idSteam = 0;

				try
				{
					idSteam = int.Parse(idSteam2);
				}
				catch
				{

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
						var datos = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
						{
							return await sentencia.Connection.QueryFirstOrDefaultAsync<dynamic>(buscarJuego, new { idSteam }, transaction: sentencia);
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
								else
								{
									int id = datos.id ?? 0;

									if (id > 0)
									{
										List<JuegoPrecio> ofertasHistoricas = string.IsNullOrEmpty((string)datos.precioMinimosHistoricos) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(datos.precioMinimosHistoricos);

										if (ofertasHistoricas.Count == 0)
										{
											ofertasHistoricas.Add(oferta);
										}

										List<JuegoPrecio> ofertasActuales = string.IsNullOrEmpty((string)datos.precioActualesTiendas) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(datos.precioActualesTiendas);

										if (ofertasActuales.Count == 0)
										{
											ofertasActuales.Add(oferta);
										}

										List<JuegoHistorico> historicos = string.IsNullOrEmpty((string)datos.historicos) ? new List<JuegoHistorico>() : JsonSerializer.Deserialize<List<JuegoHistorico>>(datos.historicos);

										await Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, null, null, null, juego.Analisis);
									}
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
				resultados = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync(sqlBuscar, new
					{
						Enlace = oferta.Enlace
					}, transaction: sentencia).ContinueWith(t => t.Result.ToList());
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
					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						await sentencia.Connection.ExecuteAsync(sqlInsertar, new
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