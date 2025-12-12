#nullable disable

using Herramientas;

namespace Tareas
{
	public class Mantenimiento : BackgroundService
	{
		private readonly ILogger<Mantenimiento> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;

		public Mantenimiento(ILogger<Mantenimiento> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp == piscinaUsada)
				{
					try
					{
						TimeSpan tiempoSiguiente = TimeSpan.FromHours(48);

						if (DateTime.Now.Hour == 4)
						{
							tiempoSiguiente = TimeSpan.FromMinutes(40);
						}

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", tiempoSiguiente) == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("mantenimiento", DateTime.Now);

							await BaseDatos.Reseñas.Limpiar.Ejecutar();
							await BaseDatos.Juegos.Limpiar.Minimos();
							await BaseDatos.Portapapeles.Borrar.Limpieza();
							await Divisas.ActualizarDatos();

							//await BaseDatos.Mantenimiento.Encoger.Ejecutar();
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Mantenimiento", ex);
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