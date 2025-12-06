#nullable disable

using Herramientas;

namespace Tareas.Tiendas
{
	public class WinGameStore : BackgroundService
	{
		private string id = APIs.WinGameStore.Tienda.Generar().Id;

		private readonly ILogger<WinGameStore> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;

		public WinGameStore(ILogger<WinGameStore> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			Random azar = new Random();
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(azar.Next(20, 60)));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string piscinaTiendas = builder.Configuration.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					TimeSpan siguienteComprobacion = TimeSpan.FromHours(3);

					if (DateTime.Now.Hour == 18)
					{
						siguienteComprobacion = TimeSpan.FromMinutes(20);
					}

					if (DateTime.Now.Hour == 19)
					{
						siguienteComprobacion = TimeSpan.FromHours(4);
					}

					bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, id);

					if (sePuedeUsar == true && BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60))?.Result.Count == 0)
					{
						try
						{
							await Tiendas2.TiendasCargar.TareasGestionador(null, id);

							Environment.Exit(1);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(id, ex);
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