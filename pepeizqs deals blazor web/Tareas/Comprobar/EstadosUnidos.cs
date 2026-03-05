#nullable disable

using Herramientas;
using Tiendas2;

namespace Tareas.Comprobar
{
	public class EstadosUnidos : BackgroundService
	{
		private readonly ILogger<EstadosUnidos> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public EstadosUnidos(ILogger<EstadosUnidos> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaTiendas = _configuracion.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
				{
					foreach (var tienda in Tiendas2.TiendasCargar.GenerarListado())
					{
						TimeSpan siguienteComprobacion = TimeSpan.Zero;

						if (tienda.Id == APIs.Steam.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Fanatical.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.GamersGate.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarUk().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarFr().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarDe().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarUs().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.IndieGala.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.WinGameStore.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.GameBillet.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.DLGamer.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Gamesporium.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}

						if (siguienteComprobacion > TimeSpan.Zero)
						{
							bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsarUS(siguienteComprobacion, tienda.Id);

							if (sePuedeUsar == true)
							{
								try
								{
									await Tiendas2.TiendasCargar.TareasGestionador(TiendaRegion.EstadosUnidos, tienda.Id);

									var tareasEnUso = await BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60));

									if (tareasEnUso.Count == 0)
									{
										Environment.Exit(1);
									}
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Comprobador: " + tienda.Id, ex);
								}
							}
						}
					}
				}
			}
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			await base.StopAsync(stoppingToken);
		}
	}
}
