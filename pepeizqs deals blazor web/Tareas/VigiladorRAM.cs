namespace Tareas
{
	public class VigiladorRAM : BackgroundService
	{
		private readonly IHostApplicationLifetime _lifetime;
		private readonly ILogger<VigiladorRAM> _logger;
		private const long LimiteBytes = 1000 * 1024 * 1024; // 1000 MB

		public VigiladorRAM(IHostApplicationLifetime lifetime, ILogger<VigiladorRAM> logger)
		{
			_lifetime = lifetime;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

				var ram = GC.GetTotalMemory(false);

				if (ram > LimiteBytes)
				{
					BaseDatos.Errores.Insertar.Mensaje("Vigilador RAM", $"Uso de RAM excesivo: {ram / (1024 * 1024)} MB");
					_lifetime.StopApplication(); 
				}
			}
		}
	}
}
	