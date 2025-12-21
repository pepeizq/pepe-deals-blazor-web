#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Data;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace Tareas
{
	public class JuegoMinimoTarea : Juego
	{
		public JuegoDRM DRMElegido { get; set; }
	}

	public class Minimos : BackgroundService
    {
        private readonly ILogger<Minimos> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;
		private readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1);

		public Minimos(ILogger<Minimos> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
        {
            _logger = logger;
            _factoria = factory;
            _decompilador = decompilador;
        }

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
        {
			WebApplicationBuilder builder = WebApplication.CreateBuilder();
			string piscinaWeb = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
			string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            while (await timer.WaitForNextTickAsync(tokenParar))
            {
				if (await _semaforo.WaitAsync(0) == false)
				{
					continue;
				}

				try
				{
					if (piscinaWeb == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
					{
						await BaseDatos.Portada.Limpiar.Total();

						foreach (var tienda in Tiendas2.TiendasCargar.GenerarListado())
						{
							List<Juego> juegosParaInsertar = new List<Juego>();

							List<JuegoMinimoTarea> juegos = await BaseDatos.Portada.Buscar.BuscarMinimos(tienda.Id);

							if (juegos == null) continue;

							foreach (var juego in juegos)
							{
								juego.IdMaestra = juego.Id;
								juego.PrecioMinimosHistoricos = juego.PrecioMinimosHistoricos.Where(x => x.DRM == juego.DRMElegido).ToList();

								if (juego.PrecioMinimosHistoricos?.Count > 0 && (juego.PrecioMinimosHistoricos[0].Precio > 0 || juego.PrecioMinimosHistoricos[0].PrecioCambiado > 0))
								{
									juegosParaInsertar.Add(juego);
								}
							}

							DataTable tabla = new DataTable();
							tabla.Columns.Add("idSteam", typeof(long));
							tabla.Columns.Add("idGog", typeof(long));
							tabla.Columns.Add("nombre", typeof(string));
							tabla.Columns.Add("tipo", typeof(int));
							tabla.Columns.Add("fechaSteamAPIComprobacion", typeof(DateTime));
							tabla.Columns.Add("imagenes", typeof(string));
							tabla.Columns.Add("precioMinimosHistoricos", typeof(string));
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
							tabla.Columns.Add("precioActualesTiendas", typeof(string));
							tabla.Columns.Add("categorias", typeof(string));
							tabla.Columns.Add("etiquetas", typeof(string));
							tabla.Columns.Add("idiomas", typeof(string));
							tabla.Columns.Add("deck", typeof(int));
							tabla.Columns.Add("steamOS", typeof(int));
							tabla.Columns.Add("inteligenciaArtificial", typeof(bool));
							tabla.Columns.Add("idMaestra", typeof(long));
							tabla.Columns.Add("ocultarPortada", typeof(bool));

							foreach (var juego in juegosParaInsertar)
							{
								tabla.Rows.Add(
									juego.IdSteam,
									juego.IdGog,
									juego.Nombre,
									(int)juego.Tipo,
									juego.FechaSteamAPIComprobacion,
									JsonSerializer.Serialize(juego.Imagenes),
									JsonSerializer.Serialize(juego.PrecioMinimosHistoricos),
									JsonSerializer.Serialize(juego.Analisis),
									JsonSerializer.Serialize(juego.Caracteristicas),
									JsonSerializer.Serialize(juego.Media),
									Herramientas.Buscador.LimpiarNombre(juego.Nombre),
									juego.Bundles != null ? JsonSerializer.Serialize(juego.Bundles) : null,
									juego.Gratis != null ? JsonSerializer.Serialize(juego.Gratis) : null,
									juego.Suscripciones != null ? JsonSerializer.Serialize(juego.Suscripciones) : null,
									juego.Maestro,
									juego.FreeToPlay,
									juego.MayorEdad,
									juego.PrecioActualesTiendas != null ? JsonSerializer.Serialize(juego.PrecioActualesTiendas) : null,
									juego.Categorias != null ? JsonSerializer.Serialize(juego.Categorias) : null,
									juego.Etiquetas != null ? JsonSerializer.Serialize(juego.Etiquetas) : null,
									juego.Idiomas != null ? JsonSerializer.Serialize(juego.Idiomas) : null,
									(int)juego.Deck,
									(int)juego.SteamOS,
									juego.InteligenciaArtificial,
									juego.IdMaestra,
									juego.OcultarPortada
								);
							}

							DynamicParameters parametros = new DynamicParameters();
							parametros.Add("@Datos", tabla.AsTableValuedParameter("dbo.SeccionMinimosType"));

							await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
							{
								return await conexion.ExecuteAsync("dbo.UpsertSeccionMinimos",
								parametros,
								transaction: sentencia,
								commandType: CommandType.StoredProcedure,
								commandTimeout: 600);
							});
						}
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tarea - Minimos", ex, false);
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
