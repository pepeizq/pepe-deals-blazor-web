#nullable disable

using Herramientas;

namespace Tareas.Suscripciones
{
    public class UbisoftPlusClassics : BackgroundService
    {
        private string id = "ubisoftplusclassics";

        private readonly ILogger<UbisoftPlusClassics> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;

        public UbisoftPlusClassics(ILogger<UbisoftPlusClassics> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
        {
            _logger = logger;
            _factoria = factory;
            _decompilador = decompilador;
        }

        protected override async Task ExecuteAsync(CancellationToken tokenParar)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

            while (await timer.WaitForNextTickAsync(tokenParar))
            {
                WebApplicationBuilder builder = WebApplication.CreateBuilder();
                string piscinaTiendas = builder.Configuration.GetValue<string>("PoolTiendas:Contenido");
                string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

                if (piscinaTiendas == piscinaUsada)
                {
					TimeSpan siguienteComprobacion = TimeSpan.FromHours(4);

					bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, id);

					if (sePuedeUsar == true && BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60))?.Result.Count == 0)
					{
						try
						{
							await APIs.Ubisoft.Suscripcion.Buscar();

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