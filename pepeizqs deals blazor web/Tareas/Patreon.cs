#nullable disable

using Herramientas;

namespace Tareas
{
	public class Patreon : BackgroundService
	{
		private readonly ILogger<Patreon> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Patreon(ILogger<Patreon> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaApp = _configuracion.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp == piscinaUsada)
				{
					try
					{
						TimeSpan tiempoSiguiente = TimeSpan.FromHours(6);

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("patreon", tiempoSiguiente) == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("patreon", DateTime.Now);

							await Herramientas.Patreon.Leer(_configuracion);
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Patreon", ex);
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