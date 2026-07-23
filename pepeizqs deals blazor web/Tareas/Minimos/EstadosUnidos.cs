//TRUNCATE TABLE seccionMinimosUS;

//ALTER TABLE seccionMinimosUS DROP COLUMN DRM;

//ALTER TABLE seccionMinimosUS
//ADD DRM AS (
//    CAST(JSON_VALUE(precioMinimosHistoricosUS, '$[0].DRM') AS NVARCHAR(200))
//) PERSISTED;

//CREATE UNIQUE INDEX UX_seccionMinimosUS_idMaestra_DRM
//ON seccionMinimosUS (idMaestra, DRM);

#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;
using static Dapper.SqlMapper;

namespace Tareas.Minimos
{
	public class EstadosUnidos : BackgroundService
	{
		private static readonly JsonSerializerOptions _opcionesJsonSinNulls = new JsonSerializerOptions
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		private readonly ILogger<EstadosUnidos> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;
		private readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1);

		public EstadosUnidos(ILogger<EstadosUnidos> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				if (await _semaforo.WaitAsync(0) == false)
				{
					continue;
				}

				try
				{
					string piscinaMinimos = _configuracion.GetValue<string>("PoolMinimos:Contenido");
					string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

					if (piscinaMinimos == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
					{
						await BaseDatos.Portada.Limpiar.Total(TiendaRegion.EstadosUnidos);

						foreach (var tienda in TiendasCargar.GenerarListado())
						{
							List<Juego> juegosParaInsertar = new List<Juego>();

							List<JuegoMinimoTarea> juegos = await BaseDatos.Portada.Buscar.BuscarMinimos(TiendaRegion.EstadosUnidos, tienda.Id);

							if (juegos == null)
							{
								continue;
							}

							foreach (var juego in juegos)
							{
								juego.IdMaestra = juego.Id;
								juego.PrecioMinimosHistoricosUS = juego.PrecioMinimosHistoricosUS.Where(x => x.DRM == juego.DRMElegido).ToList();

								if (juego.Imagenes != null)
								{
									juego.Imagenes.Capsule_231x87 = null;
									juego.Imagenes.Library_600x900 = null;
									juego.Imagenes.Library_1920x620 = null;
									juego.Imagenes.Logo = null;
								}

								if (juego.Caracteristicas != null)
								{
									juego.Caracteristicas.Descripcion = null;
									juego.Caracteristicas.Enlaces = null;
								}

								if (juego.Media != null)
								{
									juego.Media.Capturas2 = null;
								}

								if (juego.PrecioMinimosHistoricosUS?.Count > 0 && juego.PrecioMinimosHistoricosUS[0].Precio > 0)
								{
									juegosParaInsertar.Add(juego);
								}
							}

							int remesaTamaño = 500;
							for (int i = 0; i < juegosParaInsertar.Count; i += remesaTamaño)
							{
								var chunk = juegosParaInsertar.Skip(i).Take(remesaTamaño).ToList();
								DataTable tabla = new DataTable();
								tabla.Columns.Add("idSteam", typeof(long));
								tabla.Columns.Add("idGog", typeof(long));
								tabla.Columns.Add("nombre", typeof(string));
								tabla.Columns.Add("tipo", typeof(int));
								tabla.Columns.Add("fechaSteamAPIComprobacion", typeof(DateTime));
								tabla.Columns.Add("imagenes", typeof(string));
								tabla.Columns.Add("precioMinimosHistoricosUS", typeof(string));
								tabla.Columns.Add("analisis", typeof(string));
								tabla.Columns.Add("caracteristicas", typeof(string));
								tabla.Columns.Add("media", typeof(string));
								tabla.Columns.Add("nombreCodigo", typeof(string));
								tabla.Columns.Add("bundles", typeof(string));
								tabla.Columns.Add("gratis", typeof(string));
								tabla.Columns.Add("suscripciones", typeof(string));
								tabla.Columns.Add("maestro", typeof(string));
								tabla.Columns.Add("freeToPlay", typeof(string));
								tabla.Columns.Add("mayorEdad", typeof(string));
								tabla.Columns.Add("categorias", typeof(string));
								tabla.Columns.Add("etiquetas", typeof(string));
								tabla.Columns.Add("idiomas", typeof(string));
								tabla.Columns.Add("deck", typeof(int));
								tabla.Columns.Add("steamOS", typeof(int));
								tabla.Columns.Add("steamMachine", typeof(int));
								tabla.Columns.Add("steamFrame", typeof(int));
								tabla.Columns.Add("inteligenciaArtificial", typeof(bool));
								tabla.Columns.Add("idMaestra", typeof(long));
								tabla.Columns.Add("ocultarPortada", typeof(bool));

								foreach (var juego in chunk)
								{
									tabla.Rows.Add(
										juego.IdSteam,
										juego.IdGog,
										juego.Nombre,
										(int)juego.Tipo,
										juego.FechaSteamAPIComprobacion,
										JsonSerializer.Serialize(juego.Imagenes, _opcionesJsonSinNulls),
										JsonSerializer.Serialize(juego.PrecioMinimosHistoricosUS),
										JsonSerializer.Serialize(juego.Analisis),
										JsonSerializer.Serialize(juego.Caracteristicas, _opcionesJsonSinNulls),
										JsonSerializer.Serialize(juego.Media, _opcionesJsonSinNulls),
										Herramientas.Buscador.LimpiarNombre(juego.Nombre),
										juego.Bundles != null ? JsonSerializer.Serialize(juego.Bundles) : null,
										juego.Gratis != null ? JsonSerializer.Serialize(juego.Gratis) : null,
										juego.Suscripciones != null ? JsonSerializer.Serialize(juego.Suscripciones) : null,
										juego.Maestro,
										juego.FreeToPlay,
										juego.MayorEdad,
										juego.Categorias != null ? JsonSerializer.Serialize(juego.Categorias) : null,
										juego.Etiquetas != null ? JsonSerializer.Serialize(juego.Etiquetas) : null,
										juego.Idiomas != null ? JsonSerializer.Serialize(juego.Idiomas) : null,
										(int)juego.Deck,
										(int)juego.SteamOS,
										(int)juego.SteamMachine,
										(int)juego.SteamFrame,
										juego.InteligenciaArtificial,
										juego.IdMaestra,
										juego.OcultarPortada
									);
								}

								DynamicParameters parametros = new DynamicParameters();
								parametros.Add("@Datos", tabla.AsTableValuedParameter("dbo.SeccionMinimosUSType"));

								await Herramientas.BaseDatos.Select(async conexion =>
								{
									return await conexion.ExecuteAsync(
										"dbo.UpsertSeccionMinimosUSBatch",
										parametros,
										commandType: CommandType.StoredProcedure,
										commandTimeout: 600
									);
								});
							}
						}
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tarea - Minimos US", ex, false);
				}
				finally
				{
					_semaforo.Release();
				}
			}
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			await base.StopAsync(stoppingToken);
		}
	}
}