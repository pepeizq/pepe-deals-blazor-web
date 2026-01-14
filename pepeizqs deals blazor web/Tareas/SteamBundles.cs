#nullable disable

using APIs.Steam;
using Herramientas;

namespace Tareas
{
	public class SteamBundles : BackgroundService
	{
		private readonly ILogger<SteamBundles> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public SteamBundles(ILogger<SteamBundles> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
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

							if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("steamBundles", siguienteComprobacion) == true && await BaseDatos.Admin.Buscar.TiendasLibre() == true)
							{
								await BaseDatos.Admin.Actualizar.TareaUso("steamBundles", DateTime.Now);

								var pendientes = await BaseDatos.Pendientes.Buscar.Tienda(Tienda.GenerarBundles().Id);

								if (pendientes?.Count > 0)
								{
									foreach (var pendiente in pendientes)
									{
										var ids = await Bundle.CargarDatosBundle(pendiente.Enlace);

										if (string.IsNullOrEmpty(ids) == false)
										{
											await BaseDatos.Pendientes.Actualizar.Tienda(Tienda.GenerarBundles().Id, pendiente.Enlace, ids);
										}
										else
										{
											await BaseDatos.Pendientes.Actualizar.DescartarTienda(Tienda.GenerarBundles().Id, pendiente.Enlace);
										}
									}
								}
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Steam Bundles", ex);
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