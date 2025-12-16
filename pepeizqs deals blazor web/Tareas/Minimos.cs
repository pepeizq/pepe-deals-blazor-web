#nullable disable

using Herramientas;
using Juegos;

namespace Tareas
{
	public class JuegoMinimoTarea : Juego
	{
		public JuegoDRM DRMElegido { get; set; }
	}

	public class Minimos : BackgroundService
    {
        private readonly ILogger<Minimos> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;
		private readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1);

		public Minimos(ILogger<Minimos> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
        {
            _logger = logger;
            _factoria = factory;
            _decompilador = decompilador;
        }

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
        {
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            while (await timer.WaitForNextTickAsync(tokenParar))
            {
				if (await _semaforo.WaitAsync(0) == false)
				{
					continue;
				}

				try
				{
					WebApplicationBuilder builder = WebApplication.CreateBuilder();
					string piscinaWeb = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
					string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

					if (piscinaWeb == piscinaUsada)
					{
						List<JuegoMinimoTarea> juegos = await BaseDatos.Portada.Buscar.BuscarMinimos();

						if (juegos?.Count > 0)
						{
							await BaseDatos.Portada.Limpiar.Total();

							foreach (var juego in juegos)
							{
								juego.IdMaestra = juego.Id;
								juego.PrecioMinimosHistoricos = juego.PrecioMinimosHistoricos.Where(x => x.DRM == juego.DRMElegido).ToList();

								if (juego.PrecioMinimosHistoricos?.Count > 0)
								{
									if (juego.PrecioMinimosHistoricos[0].Precio > 0 || juego.PrecioMinimosHistoricos[0].PrecioCambiado > 0)
									{
										await BaseDatos.Juegos.Insertar.Ejecutar(juego, "seccionMinimos", true);
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tarea - Minimos", ex);
				}
				finally
				{
					_semaforo.Release();
				}
			}
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
