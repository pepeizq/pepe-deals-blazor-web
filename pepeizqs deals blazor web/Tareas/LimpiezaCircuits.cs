using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;

namespace Tareas
{
	public class LimpiezaCircuits : BackgroundService
	{
		private readonly CircuitOptions _options;

		public LimpiezaCircuits(IOptions<CircuitOptions> options)
		{
			_options = options.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);

				var valorOriginal = _options.DisconnectedCircuitRetentionPeriod;
				_options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(1);
				await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				_options.DisconnectedCircuitRetentionPeriod = valorOriginal;

				GC.Collect(2, GCCollectionMode.Aggressive, blocking: true, compacting: true);
				GC.WaitForPendingFinalizers();
			}
		}
	}
}