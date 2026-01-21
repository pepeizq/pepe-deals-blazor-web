#nullable disable

using System.IO.Compression;
using System.Net;
using System.Text;

namespace Herramientas
{

	public interface IDecompiladores
    {
        Task<string> Estandar(string enlace);
    }

	public class Decompiladores2 : IDecompiladores
	{
        private readonly IHttpClientFactory fabrica;

        public Decompiladores2(IHttpClientFactory _fabrica)
		{
			fabrica = _fabrica;
		}

		public async Task<string> Estandar(string enlace)
		{
            HttpClient cliente = fabrica.CreateClient();
		
            string contenido = string.Empty;

            cliente.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0");

            try
            {
                HttpResponseMessage respuesta = await cliente.GetAsync(enlace);
                
				if (respuesta.IsSuccessStatusCode == true)
				{
                    contenido = await respuesta.Content.ReadAsStringAsync();
                }
				
                respuesta.Dispose();
            }
            catch { }

            return contenido;
        }
	}

	public static class Decompiladores
	{
		private static readonly HttpClient _cliente;

		static Decompiladores()
		{
			var handler = new SocketsHttpHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				PooledConnectionLifetime = TimeSpan.FromMinutes(15),
				PooledConnectionIdleTimeout = TimeSpan.FromMinutes(10),
				MaxConnectionsPerServer = 2
			};

			_cliente = new HttpClient(handler, disposeHandler: false);
			_cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0");
			_cliente.Timeout = TimeSpan.FromSeconds(60);
		}

		public static async Task<string> Estandar(string enlace)
		{
			HttpResponseMessage respuesta = null;

			try
			{
				respuesta = await _cliente.GetAsync(
					enlace,
					HttpCompletionOption.ResponseHeadersRead);

				respuesta.EnsureSuccessStatusCode();

				return await respuesta.Content.ReadAsStringAsync();
			}
			catch (Exception ex)
			{
				string cuerpo = null;

				if (respuesta != null)
				{
					cuerpo = await respuesta.Content.ReadAsStringAsync();
				}

				global::BaseDatos.Errores.Insertar.Mensaje(
					"Decompilador",
					"Estado: " + ((int?)respuesta?.StatusCode)?.ToString() + Environment.NewLine + Environment.NewLine +
					"Razon: " + respuesta?.ReasonPhrase + Environment.NewLine + Environment.NewLine +
					"Cuerpo: " + cuerpo + Environment.NewLine + Environment.NewLine +
					"Error: " + ex.Message,
					enlace
				); 
				
				return null;
			}
		}

		public static async Task<string> GZipFormato(string enlace) 
        {
            await Task.Yield();

			HttpRequestMessage mensaje = new HttpRequestMessage();
			mensaje.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true, NoStore = true };
			mensaje.Headers.Pragma.ParseAdd("no-cache");
			mensaje.RequestUri = new Uri(enlace);
			mensaje.Headers.Accept.ParseAdd("application/json, text/plain, */*");
			mensaje.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br, zstd");
			mensaje.Headers.AcceptLanguage.ParseAdd("es-ES,es;q=0.8,en-US;q=0.5,en;q=0.3");
			mensaje.Headers.Connection.ParseAdd("keep-alive");
			mensaje.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 10; Generic Android-x86_64 Build/QD1A.190821.014.C2; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/79.0.3945.36 Safari/537.36");
			mensaje.Headers.Add("Sec-Fetch-Dest", "empty");
			mensaje.Headers.Add("Sec-Fetch-Mode", "cors");
			mensaje.Headers.Add("Sec-Fetch-Site", "same-origin");
			mensaje.Headers.TE.ParseAdd("trailers");

			CookieContainer cookieContainer = new CookieContainer();

			if (enlace.Contains("https://2game.com") == true)
			{
				cookieContainer.Add(new Uri(enlace), new Cookie("store", "es_es"));
			}
			
			using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer, UseCookies = true, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
			{
				using (HttpClient cliente = new HttpClient(handler) { BaseAddress = new Uri(enlace) })
				{
					cliente.DefaultRequestHeaders.Accept.Clear();

					using (HttpResponseMessage respuesta = await cliente.GetAsync(enlace, HttpCompletionOption.ResponseContentRead))
					{
						Stream stream = await respuesta.Content.ReadAsStreamAsync();

						using (GZipStream descompresion = new GZipStream(stream, CompressionMode.Decompress, false))
						{
							using (StreamReader lector = new StreamReader(stream, Encoding.UTF8))
							{
								return await lector.ReadToEndAsync();
							}
						}
					}				
				}
			}
		}

		public static async Task<string> GZipFormato2(string enlace)
		{
			HttpRequestMessage mensaje = new HttpRequestMessage();
			mensaje.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true, NoStore = true };
			mensaje.Headers.Pragma.ParseAdd("no-cache");
			mensaje.RequestUri = new Uri(enlace);
			mensaje.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
			mensaje.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
			mensaje.Headers.AcceptLanguage.ParseAdd("es,en-US;q=0.7,en;q=0.3");
			mensaje.Headers.Connection.ParseAdd("keep-alive");
			mensaje.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 10; Generic Android-x86_64 Build/QD1A.190821.014.C2; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/79.0.3945.36 Safari/537.36");

			CookieContainer cookieContainer = new CookieContainer();
			
			using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer, UseCookies = true, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
			{
				using (HttpClient cliente = new HttpClient(handler) { BaseAddress = new Uri(enlace) })
				{
                    HttpResponseMessage respuesta = await cliente.SendAsync(mensaje);

					Stream stream = await respuesta.Content.ReadAsStreamAsync();
					respuesta.Dispose();

					using (GZipStream descompresion = new GZipStream(stream, CompressionMode.Decompress, false))
					{
						using (StreamReader lector = new StreamReader(stream, Encoding.UTF8))
						{
							return await lector.ReadToEndAsync();
						}
					}
				}
			}
		}

		public static async Task<string> NoSeguro(string enlace)
		{
			HttpRequestMessage mensaje = new HttpRequestMessage();
			mensaje.RequestUri = new Uri(enlace);
			mensaje.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
			mensaje.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
			mensaje.Headers.AcceptLanguage.ParseAdd("es,en-US;q=0.7,en;q=0.3");
			mensaje.Headers.Connection.ParseAdd("keep-alive");
			mensaje.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 10; Generic Android-x86_64 Build/QD1A.190821.014.C2; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/79.0.3945.36 Safari/537.36");

			CookieContainer cookieContainer = new CookieContainer();

			var handler = new HttpClientHandler();
			handler.ClientCertificateOptions = ClientCertificateOption.Manual;
			handler.ServerCertificateCustomValidationCallback =
				(mensaje, cert, cetChain, policyErrors) =>
				{
					return true;
				};

			using (handler)
			{
				using (HttpClient cliente = new HttpClient(handler) { BaseAddress = new Uri(enlace) })
				{
					HttpResponseMessage respuesta = await cliente.SendAsync(mensaje);

					Stream stream = await respuesta.Content.ReadAsStreamAsync();

					using (GZipStream descompresion = new GZipStream(stream, CompressionMode.Decompress))
					{
						using (StreamReader lector = new StreamReader(stream))
						{
							return await lector.ReadToEndAsync();
						}
					}
				}
			}
		}

		private static readonly HttpClient _http = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(20)
		};

		public static async Task<string> GZipFormato3(string enlace)
		{
			for (int i = 0; i < 3; i++)
			{
				try
				{
					using var request = new HttpRequestMessage(HttpMethod.Get, enlace);
					request.Headers.AcceptEncoding.ParseAdd("gzip");
					request.Headers.AcceptEncoding.ParseAdd("deflate");

					using var respuesta = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

					if (respuesta.IsSuccessStatusCode == false)
					{
						continue;
					}

					var stream = await respuesta.Content.ReadAsStreamAsync();
					Stream finalStream = stream;

					if (respuesta.Content.Headers.ContentEncoding.Contains("gzip"))
					{
						finalStream = new GZipStream(stream, CompressionMode.Decompress);
					}

					using var lector = new StreamReader(finalStream);
					string resultado = await lector.ReadToEndAsync();

					if (string.IsNullOrWhiteSpace(resultado) == false)
					{
						return resultado;
					}			
				}
				catch
				{
					await Task.Delay(500); 
				}
			}

			return "{}";
		}
	}
}
