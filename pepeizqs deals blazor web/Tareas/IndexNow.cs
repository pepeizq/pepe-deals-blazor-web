#nullable disable

using Bundles2;
using Herramientas;
using Juegos;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using Noticias;
using System.Text;

namespace Tareas
{

    public class IndexNow : BackgroundService
    {
        private readonly ILogger<IndexNow> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;

        public IndexNow(ILogger<IndexNow> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
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
                string piscinaWeb = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
                string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

                if (piscinaWeb != piscinaUsada)
                {
                    SqlConnection conexion = new SqlConnection();

                    try
                    {
                        conexion = Herramientas.BaseDatos.Conectar();
                    }
                    catch { }

                    if (conexion.State == System.Data.ConnectionState.Open)
                    {
                        try
                        {
                            TimeSpan tiempoSiguiente = TimeSpan.FromHours(1);

                            if (BaseDatos.Admin.Buscar.TareaPosibleUsar("indexNow", tiempoSiguiente, conexion) == true)
                            {
                                BaseDatos.Admin.Actualizar.TareaUso("indexNow", DateTime.Now, conexion);

                                List<Juego> juegos = BaseDatos.Juegos.Buscar.Aleatorios();
								List<Bundle> bundles = BaseDatos.Bundles.Buscar.Aleatorios();
                                List<Noticia> noticias = BaseDatos.Noticias.Buscar.Aleatorias();

								var handler = new HttpClientHandler()
                                {
                                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                };

                                using (handler)
                                {
                                    using (HttpClient cliente = new HttpClient(handler) { })
                                    {
                                        string peticionJuegos = null;

                                        if (juegos?.Count > 0)
                                        {
                                            int i = 0;

                                            foreach (var juego in juegos)
                                            {
                                                if (juego.Id > 0 && string.IsNullOrEmpty(juego.Nombre) == false)
                                                {
                                                    peticionJuegos = peticionJuegos + Strings.ChrW(34) + "https://pepe.deals/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/" + Strings.ChrW(34);
                                                }

                                                i += 1;

                                                if (i < juegos.Count && juego.Id > 0 && string.IsNullOrEmpty(juego.Nombre) == false)
                                                {
                                                    peticionJuegos = peticionJuegos + "," + Environment.NewLine + Environment.NewLine;
                                                }
                                            }
                                        }

                                        string peticionBundles = null;

                                        if (bundles?.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var bundle in bundles)
                                            {
                                                if (bundle.Id > 0 && string.IsNullOrEmpty(bundle.NombreBundle) == false)
                                                {
                                                    peticionBundles = peticionBundles + Strings.ChrW(34) + "https://pepe.deals/bundle/" + bundle.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(bundle.NombreBundle) + "/" + Strings.ChrW(34);
                                                }

                                                i += 1;

                                                if (i < bundles.Count && bundle.Id > 0 && string.IsNullOrEmpty(bundle.NombreBundle) == false)
                                                {
                                                    peticionBundles = peticionBundles + "," + Environment.NewLine + Environment.NewLine;
                                                }
                                            }
										}

                                        string peticionNoticias = null;

                                        if (noticias?.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var noticia in noticias)
                                            {
                                                if (noticia.Id > 0 && string.IsNullOrEmpty(noticia.TituloEn) == false)
                                                {
                                                    peticionNoticias = peticionNoticias + Strings.ChrW(34) + "https://pepe.deals/news/" + noticia.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(noticia.TituloEn) + "/" + Strings.ChrW(34);
                                                }

                                                i += 1;

                                                if (i < noticias.Count && noticia.Id > 0 && string.IsNullOrEmpty(noticia.TituloEn) == false)
                                                {
                                                    peticionNoticias = peticionNoticias + "," + Environment.NewLine + Environment.NewLine;
                                                }
                                            }
										}

										if (string.IsNullOrEmpty(peticionJuegos) == false && string.IsNullOrEmpty(peticionBundles) == false && string.IsNullOrEmpty(peticionNoticias) == false)
                                        {
                                            string peticionBing = @"{
                      ""host"": ""pepe.deals"",
                      ""key"": ""fe53dc4ecd644e32bc89b3c73ac45940"",
                      ""keyLocation"": ""https://pepe.deals/fe53dc4ecd644e32bc89b3c73ac45940.txt"",
                      ""urlList"": [";
                                            peticionBing = peticionBing + peticionJuegos + ", ";
											peticionBing = peticionBing + peticionBundles + ", ";
											peticionBing = peticionBing + peticionNoticias;
											peticionBing = peticionBing + "]\r\n                    }";

                                            StringContent contenidoBing = new StringContent(peticionBing, Encoding.UTF8, "application/json");
                                            await cliente.PostAsync("https://www.bing.com/indexnow", contenidoBing);
										}
                                    }
                                }
                            }
                        }
                        catch (Exception ex) 
                        {
                            BaseDatos.Errores.Insertar.Mensaje("IndexNow", ex, null, false);
                        }
                    }
                }
            }
        }
    }
}
