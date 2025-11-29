#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BaseDatos.Tiendas
{
	public static class Comprobar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static async void Steam(JuegoPrecio oferta, JuegoAnalisis reseñas, bool rapido, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

					var datos = await conexion.QueryFirstOrDefaultAsync<dynamic>(buscarJuego, new { idSteam });

					if (datos != null)
					{
						juego.IdSteam = idSteam;

						string fechaAPI = datos.fechaSteamAPIComprobacion;

						if (string.IsNullOrEmpty(fechaAPI) == false)
						{
							bool actualizarAPI = DateTime.Now.Subtract(DateTime.Parse(fechaAPI)) > TimeSpan.FromDays(91);

							if (actualizarAPI == true)
							{
								BaseDatos.JuegosActualizar.Insertar.Ejecutar(juego.Id, juego.IdSteam, "SteamAPI");
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

									Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, conexion, null, null, null, juego.Analisis);
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

								Juegos.Insertar.Ejecutar(juego, conexion);
							}
						}
					}
				}
			}
		}

		public static async void Resto(JuegoPrecio oferta, SqlConnection conexion = null, string idGog = null, string slugGOG = null, string slugEpic = null)
		{
			conexion = CogerConexion(conexion);

			bool encontrado = false;

			string esquema = $"tienda{oferta.Tienda}";
			string sqlBuscar = $@"
DECLARE @ids NVARCHAR(MAX);

SELECT @ids = idJuegos 
FROM {esquema}
WHERE enlace = @Enlace
  AND descartado = 'no';

IF @ids IS NOT NULL AND @ids <> '0'
BEGIN
    DECLARE @tabla TABLE (numero INT NOT NULL);

    INSERT INTO @tabla (numero)
    SELECT TRY_CAST(value AS INT)
    FROM STRING_SPLIT(@ids, ',')
    WHERE value <> '';

    SELECT id, precioMinimosHistoricos, precioActualesTiendas, 
           idSteam, historicos, analisis
    FROM juegos
    WHERE id IN (SELECT numero FROM @tabla);
END;
";

			var resultados = (await conexion.QueryAsync(sqlBuscar, new
			{
				Enlace = oferta.Enlace
			})).ToList();

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

				if (ofertasHistoricas.Count == 0)
				{
					ofertasHistoricas.Add(oferta);
				}

				if (ofertasActuales.Count == 0)
				{
					ofertasActuales.Add(oferta);
				}

				if (id > 0)
				{
					Juegos.Precios.Actualizar(id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, conexion, slugGOG, idGog, slugEpic, reseñas);
				}
			}

			// -----------------------------------------------------------------

			if (encontrado == false)
			{
				conexion = CogerConexion(conexion);

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
					await conexion.ExecuteAsync(sqlInsertar, new
					{
						Enlace = oferta.Enlace,
						Nombre = oferta.Nombre,
						Imagen = oferta.Imagen,
						NombreCodigo = Herramientas.Buscador.LimpiarNombre(oferta.Nombre)
					});
				}
				catch (Exception ex)
				{
					Errores.Insertar.Mensaje("Insertar Tienda: " + oferta?.Enlace, ex);
				}
			}
		}
	}
}