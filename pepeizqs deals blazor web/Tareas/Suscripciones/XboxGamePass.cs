#nullable disable

using Herramientas;

namespace Tareas.Suscripciones
{
	public class XboxGamePass : BackgroundService
	{
		private string id = "pcgamepass";

		private readonly ILogger<XboxGamePass> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public XboxGamePass(ILogger<XboxGamePass> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
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
				string piscinaTiendas = _configuracion.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					TimeSpan siguienteComprobacion = TimeSpan.FromHours(4);

					bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, id);

					if (sePuedeUsar == true && (await BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60)))?.Count == 0)
					{
						try
						{
							await APIs.Xbox.Suscripcion.Buscar();

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
	}
}

