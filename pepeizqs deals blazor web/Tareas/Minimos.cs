#nullable disable

using Herramientas;
using Juegos;
using Microsoft.Data.SqlClient;

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

        public Minimos(ILogger<Minimos> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
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
				string piscinaTiendas = builder.Configuration.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					try
					{
						List<JuegoMinimoTarea> juegos = BaseDatos.Portada.Buscar.BuscarMinimos();

						if (juegos?.Count > 0)
						{
							BaseDatos.Portada.Limpiar.Total();

							foreach (var juego in juegos)
							{
								juego.IdMaestra = juego.Id;
								juego.PrecioMinimosHistoricos = juego.PrecioMinimosHistoricos.Where(x => x.DRM == juego.DRMElegido && Herramientas.OfertaActiva.Verificar(x) == true).ToList();

								if (juego.PrecioMinimosHistoricos?.Count > 0)
								{
									BaseDatos.Juegos.Insertar.Ejecutar(juego, "seccionMinimos", false);
								}
							}
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Minimos", ex);
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
