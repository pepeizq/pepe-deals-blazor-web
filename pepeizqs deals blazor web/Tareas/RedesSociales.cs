#nullable disable

using Herramientas;
using Microsoft.Data.SqlClient;

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
					if (BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60)) == null)
					{
						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("redessociales", TimeSpan.FromMinutes(5)) == true)
						{
							BaseDatos.Admin.Actualizar.TareaUso("redessociales", DateTime.Now);

							try
							{
								await BaseDatos.RedesSociales.Buscar.PendientesPosteo();
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales", ex);
							}
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