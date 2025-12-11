#nullable disable

using Herramientas;

namespace Tareas
{
    public class RedesSociales : BackgroundService
    {
        private readonly ILogger<RedesSociales> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;

        public RedesSociales(ILogger<RedesSociales> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
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
                string piscinaApp = builder.Configuration.GetValue<string>("PoolTiendas:Contenido");
                string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

                if (piscinaApp == piscinaUsada)
                {
					TimeSpan tiempoSiguiente = TimeSpan.FromHours(48);

					if (DateTime.Now.Hour == 21)
					{
						tiempoSiguiente = TimeSpan.FromMinutes(120);
					}

					if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("redessociales", tiempoSiguiente) == true &&
						(await BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60)))?.Count == 0)
                    {
						await BaseDatos.Admin.Actualizar.TareaUso("redessociales", DateTime.Now);

						try
						{
							List<Juegos.Juego> juegosDia = await BaseDatos.RedesSociales.Buscar.OfertasDelDia();

							if (juegosDia?.Count > 0)
							{
								await Herramientas.RedesSociales.Reddit.PostearOfertasDia(juegosDia);
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales", ex);
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