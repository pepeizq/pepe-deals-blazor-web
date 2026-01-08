#nullable disable

using Herramientas;

namespace Tareas
{
	public class Memoria : BackgroundService
	{
		private readonly ILogger<Memoria> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Memoria(ILogger<Memoria> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaApp = _configuracion.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp == piscinaUsada)
				{
					GC.Collect(2, GCCollectionMode.Aggressive | GCCollectionMode.Forced, blocking: true, compacting: true);
				}
			}
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			await base.StopAsync(stoppingToken);
		}
	}
}