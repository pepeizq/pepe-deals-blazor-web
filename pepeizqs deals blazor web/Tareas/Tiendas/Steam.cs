#nullable disable

using Herramientas;

namespace Tareas.Tiendas
{
	public class Steam : BackgroundService
	{
		private string id = APIs.Steam.Tienda.Generar().Id;

		private readonly ILogger<Steam> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Steam(ILogger<Steam> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			Random azar = new Random();
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(azar.Next(5, 30)));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaTiendas = _configuracion.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					TimeSpan siguienteComprobacion = TimeSpan.FromHours(2);

					if (DateTime.Now.Hour == 17 || DateTime.Now.Hour == 18)
					{
						siguienteComprobacion = TimeSpan.FromHours(8);
					}

					if (DateTime.Now.Hour == 19)
					{
						if (DateTime.Now.Minute > 3 && DateTime.Now.Minute < 30)
						{
							siguienteComprobacion = TimeSpan.FromSeconds(10);
						}
						else
						{
							siguienteComprobacion = TimeSpan.FromMinutes(5);
						}
					}

					if (DateTime.Now.Hour == 20)
					{
						if (DateTime.Now.Minute < 30)
						{
							siguienteComprobacion = TimeSpan.FromMinutes(10);
						}
						else
						{
							siguienteComprobacion = TimeSpan.FromMinutes(20);
						}
					}

					bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, id);

					if (sePuedeUsar == true && (await BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60)))?.Count == 0)
					{
						try
						{
							await Tiendas2.TiendasCargar.TareasGestionador(id);

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