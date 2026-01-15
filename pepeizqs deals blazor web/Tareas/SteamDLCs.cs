#nullable disable

using APIs.Steam;
using Herramientas;

namespace Tareas
{
	public class SteamDLCs : BackgroundService
	{
		private readonly ILogger<SteamDLCs> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public SteamDLCs(ILogger<SteamDLCs> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaTiendas = _configuracion.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					if (DateTime.Now.Hour != 19 && DateTime.Now.Hour != 20)
					{
						try
						{
							TimeSpan siguienteComprobacion = TimeSpan.FromMinutes(30);

							if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("steamDLCs", siguienteComprobacion) == true && await BaseDatos.Admin.Buscar.TiendasLibre() == true)
							{
								await BaseDatos.Admin.Actualizar.TareaUso("steamDLCs", DateTime.Now);

								var dlcs = await BaseDatos.Juegos.Buscar.DLCs(null, Juegos.JuegoTipo.DLC);

								if (dlcs?.Count > 0)
								{
									foreach (var dlc in dlcs)
									{
										int dlcMaestro = await APIs.Steam.Juego.CargarDLCMaestro(dlc.IdSteam.ToString());

										if (dlcMaestro > 0)
										{
											Juegos.Juego juegoMaestro = await BaseDatos.Juegos.Buscar.UnJuego(null, dlcMaestro.ToString());

											if (juegoMaestro != null)
											{
												dlc.Maestro = juegoMaestro.Id.ToString();
												await BaseDatos.Juegos.Actualizar.DlcMaestro(dlc);
											}
											else
											{
												Juegos.Juego nuevoJuego = await APIs.Steam.Juego.CargarDatosJuego(dlcMaestro.ToString());

												if (nuevoJuego != null)
												{
													await BaseDatos.Juegos.Insertar.Ejecutar(nuevoJuego);
													Juegos.Juego nuevoJuego2 = await BaseDatos.Juegos.Buscar.UnJuego(null, dlcMaestro.ToString());

													dlc.Maestro = nuevoJuego2.Id.ToString();
													await BaseDatos.Juegos.Actualizar.DlcMaestro(dlc);
												}
												else
												{
													dlc.Maestro = "descartado";
													await BaseDatos.Juegos.Actualizar.DlcMaestro(dlc);
												}
											}
										}
									}
								}
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Steam DLCs", ex);
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