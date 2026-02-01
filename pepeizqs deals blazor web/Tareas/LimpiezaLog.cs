#nullable disable

using Dapper;
using Herramientas;

namespace Tareas
{
	public class LimpiezaLog : BackgroundService
	{
		private readonly ILogger<LimpiezaLog> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public LimpiezaLog(ILogger<LimpiezaLog> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
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

				if (piscinaApp == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
				{
					try
					{
						await Herramientas.BaseDatos.Select(async (conexion) =>
						{
							return await conexion.ExecuteAsync("DBCC SHRINKFILE('pepeizq2_simply__winspace_es_1_log', 200);");
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Limpieza Log", ex, false);
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
