#nullable disable

using Bundles2;
using Herramientas;
using Juegos;
using Noticias;
using System.Text;
using System.Text.Json;

namespace Tareas
{

    public class IndexNow : BackgroundService
    {
        private readonly ILogger<IndexNow> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public IndexNow(ILogger<IndexNow> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
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
                string piscinaWeb = _configuracion.GetValue<string>("PoolWeb:Contenido");
                string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

                if (piscinaWeb != piscinaUsada)
                {
					try
					{
						TimeSpan tiempoSiguiente = TimeSpan.FromHours(1);

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("indexNow", tiempoSiguiente) == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("indexNow", DateTime.Now);

							List<Juego> juegos = await BaseDatos.Juegos.Buscar.Aleatorios();
							List<Bundle> bundles = await BaseDatos.Bundles.Buscar.Aleatorios();
							List<Noticia> noticias = await BaseDatos.Noticias.Buscar.Aleatorias();

							var handler = new HttpClientHandler()
							{
								ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
							};

							using (handler)
							{
								using (HttpClient cliente = new HttpClient(handler) { })
								{
									List<string> listaEnlaces = new List<string>();

									if (juegos?.Count > 0)
									{
										foreach (var juego in juegos)
										{
											if (juego.Id > 0 && string.IsNullOrEmpty(juego.Nombre) == false)
											{
												listaEnlaces.Add("https://pepe.deals/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/");
											}
										}
									}

									if (bundles?.Count > 0)
									{
										foreach (var bundle in bundles)
										{
											if (bundle.Id > 0 && string.IsNullOrEmpty(bundle.Nombre) == false)
											{
												listaEnlaces.Add("https://pepe.deals/bundle/" + bundle.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(bundle.Nombre) + "/");
											}
										}
									}

									if (noticias?.Count > 0)
									{
										foreach (var noticia in noticias)
										{
											if (noticia.Id > 0 && string.IsNullOrEmpty(noticia.TituloEn) == false)
											{
												listaEnlaces.Add("https://pepe.deals/news/" + noticia.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(noticia.TituloEn) + "/");
											}
										}
									}

									if (listaEnlaces?.Count > 0)
									{
										var indexNowConfig = _configuracion.GetSection("IndexNow");
										string host = indexNowConfig["Host"];
										string key = indexNowConfig["Key"];
										string keyLocation = indexNowConfig["KeyLocation"];

										var payload = new
										{
											host,
											key,
											keyLocation,
											urlList = listaEnlaces
										};

										string json = JsonSerializer.Serialize(payload);
										StringContent contenido = new StringContent(json, Encoding.UTF8, "application/json");
										await cliente.PostAsync("https://www.bing.com/indexnow", contenido);
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("IndexNow", ex, false);
					}
				}
            }
        }
    }
}
