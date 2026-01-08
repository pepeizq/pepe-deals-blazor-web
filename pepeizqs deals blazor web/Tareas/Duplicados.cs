#nullable disable

using Herramientas;

namespace Tareas
{
	public class Duplicados : BackgroundService
	{
		private readonly ILogger<Duplicados> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Duplicados(ILogger<Duplicados> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
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
						TimeSpan tiempoSiguiente = TimeSpan.FromHours(48);

						if (DateTime.Now.Hour == 2)
						{
							tiempoSiguiente = TimeSpan.FromMinutes(30);
						}

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("duplicados", tiempoSiguiente) == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("duplicados", DateTime.Now);

							List<Juegos.Juego> duplicados = await BaseDatos.Juegos.Buscar.Duplicados();

							await BaseDatos.Admin.Actualizar.Dato("duplicados", duplicados.Count);
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Duplicados", ex);
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