#nullable disable

using Herramientas;

namespace Tareas.Tiendas
{
	public class Muvegames : BackgroundService
	{
		private string id = APIs.Muvegames.Tienda.Generar().Id;

		private readonly ILogger<Muvegames> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Muvegames(ILogger<Muvegames> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			Random azar = new Random();
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(azar.Next(10, 60)));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaTiendas = _configuracion.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					TimeSpan siguienteComprobacion = TimeSpan.FromHours(3);

					if (DateTime.Now.Hour == 19)
					{
						siguienteComprobacion = TimeSpan.FromHours(4);
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

