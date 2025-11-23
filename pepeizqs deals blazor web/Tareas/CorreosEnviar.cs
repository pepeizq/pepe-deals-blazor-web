#nullable disable

using Herramientas;
using Herramientas.Correos;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace Tareas
{
	public class CorreosEnviar : BackgroundService
	{
		private readonly ILogger<CorreosEnviar> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;

		public CorreosEnviar(ILogger<CorreosEnviar> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
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
				string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp == piscinaUsada)
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
							TimeSpan siguienteComprobacion = TimeSpan.FromMinutes(1);

							if (BaseDatos.Admin.Buscar.TareaPosibleUsar("correosEnviar", siguienteComprobacion, conexion) == true)
							{
								BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", DateTime.Now, conexion);

								List<BaseDatos.CorreosEnviar.CorreoPendienteEnviar> pendientes = BaseDatos.CorreosEnviar.Buscar.PendientesEnviar(conexion);

								if (pendientes?.Count > 0)
								{
									List<CorreoMinimoFinal> correosMinimosFinal = new List<CorreoMinimoFinal>();
									List<CorreoDeseadoBundleFinal> correosDeseadosBundleFinal = new List<CorreoDeseadoBundleFinal>();

									foreach (var pendiente in pendientes.ToList())
									{
										if (pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimo)
										{
											if (correosMinimosFinal.Any(c => c.CorreoHacia == pendiente.CorreoHacia) == false)
											{
												CorreoMinimoFinal correoMinimoFinal = new CorreoMinimoFinal()
												{
													CorreoHacia = pendiente.CorreoHacia,
													Jsons = new List<CorreoMinimoJson>() { JsonSerializer.Deserialize<CorreoMinimoJson>(pendiente.Json) },
													HoraOriginal = pendiente.Fecha
												};

												correosMinimosFinal.Add(correoMinimoFinal);
											}
											else
											{
												correosMinimosFinal.Where(c => c.CorreoHacia == pendiente.CorreoHacia).FirstOrDefault().Jsons.Add(JsonSerializer.Deserialize<CorreoMinimoJson>(pendiente.Json));
											}
										}
										else if (pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimos)
										{
											if (string.IsNullOrEmpty(pendiente.Json) == false)
											{
												if (correosMinimosFinal.Any(c => c.CorreoHacia == pendiente.CorreoHacia) == false)
												{
													CorreoMinimoFinal correoMinimoFinal = new CorreoMinimoFinal()
													{
														CorreoHacia = pendiente.CorreoHacia,
														Jsons = new List<CorreoMinimoJson>(),
														HoraOriginal = pendiente.Fecha
													};

													correoMinimoFinal.Jsons.AddRange(JsonSerializer.Deserialize<List<CorreoMinimoJson>>(pendiente.Json));
													correosMinimosFinal.Add(correoMinimoFinal);
												}
												else
												{
													correosMinimosFinal.Where(c => c.CorreoHacia == pendiente.CorreoHacia).FirstOrDefault().Jsons.AddRange(JsonSerializer.Deserialize<List<CorreoMinimoJson>>(pendiente.Json));
												}
											}
										}
										else if (pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadoBundle)
										{
											if (correosDeseadosBundleFinal.Any(c => c.CorreoHacia == pendiente.CorreoHacia) == false)
											{
												CorreoDeseadoBundleFinal correoDeseadosBundleFinal = new CorreoDeseadoBundleFinal()
												{
													CorreoHacia = pendiente.CorreoHacia,
													Jsons = new List<CorreoDeseadoBundleJson>() { JsonSerializer.Deserialize<CorreoDeseadoBundleJson>(pendiente.Json) },
													HoraOriginal = pendiente.Fecha
												};

												correosDeseadosBundleFinal.Add(correoDeseadosBundleFinal);
											}
											else
											{
												correosDeseadosBundleFinal.Where(c => c.CorreoHacia == pendiente.CorreoHacia).FirstOrDefault().Jsons.Add(JsonSerializer.Deserialize<CorreoDeseadoBundleJson>(pendiente.Json));
											}
										}
										else if (pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadosBundle)
										{
											if (string.IsNullOrEmpty(pendiente.Json) == false)
											{
												if (correosDeseadosBundleFinal.Any(c => c.CorreoHacia == pendiente.CorreoHacia) == false)
												{
													CorreoDeseadoBundleFinal correoMinimoFinal = new CorreoDeseadoBundleFinal()
													{
														CorreoHacia = pendiente.CorreoHacia,
														Jsons = new List<CorreoDeseadoBundleJson>(),
														HoraOriginal = pendiente.Fecha
													};

													correoMinimoFinal.Jsons.AddRange(JsonSerializer.Deserialize<List<CorreoDeseadoBundleJson>>(pendiente.Json));
													correosDeseadosBundleFinal.Add(correoMinimoFinal);
												}
												else
												{
													correosDeseadosBundleFinal.Where(c => c.CorreoHacia == pendiente.CorreoHacia).FirstOrDefault().Jsons.AddRange(JsonSerializer.Deserialize<List<CorreoDeseadoBundleJson>>(pendiente.Json));
												}
											}
										}
										else
										{
											bool enviado = Herramientas.Correos.Enviar.Ejecutar(pendiente.Html, pendiente.Titulo, pendiente.CorreoDesde, pendiente.CorreoHacia);

											if (enviado == true)
											{
												BaseDatos.CorreosEnviar.Borrar.Ejecutar(pendiente.Id, conexion);
											}
											else
											{
												break;
											}
										}
									}

									#region Minimos Historicos

									foreach (var correoMinimoFinal in correosMinimosFinal)
									{ 
										if (correoMinimoFinal.Jsons.Count > 1)
										{
											Herramientas.Correos.DeseadoMinimo.Nuevos(correoMinimoFinal.Jsons, correoMinimoFinal.CorreoHacia, correoMinimoFinal.HoraOriginal);

											foreach (var pendiente2 in pendientes.ToList())
											{
												if (pendiente2.CorreoHacia == correoMinimoFinal.CorreoHacia && (pendiente2.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimo || pendiente2.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimos))
												{
													BaseDatos.CorreosEnviar.Borrar.Ejecutar(pendiente2.Id, conexion);
												}
											}
										}
									}

									pendientes = BaseDatos.CorreosEnviar.Buscar.PendientesEnviar();

									foreach (var pendiente in pendientes.ToList())
									{
										if (pendiente.Fecha.AddMinutes(15) < DateTime.Now && (pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimo || pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimos))
										{
											bool enviado = Herramientas.Correos.Enviar.Ejecutar(pendiente.Html, pendiente.Titulo, pendiente.CorreoDesde, pendiente.CorreoHacia);

											BaseDatos.Errores.Insertar.Mensaje("Correo " + pendiente.Tipo.ToString() + " " + pendiente.Json?.Length.ToString() + " se borra ID: " + pendiente.Id.ToString(), enviado.ToString());

											if (enviado == true)
											{
												BaseDatos.CorreosEnviar.Borrar.Ejecutar(pendiente.Id, conexion);
											}
											else
											{
												break;
											}
										}
									}

									#endregion

									#region Deseados Bundle 

									foreach (var correoDeseadoBundle in correosDeseadosBundleFinal)
									{
										if (correoDeseadoBundle.Jsons.Count > 1)
										{
											Herramientas.Correos.DeseadoBundle.Nuevos(correoDeseadoBundle.Jsons, correoDeseadoBundle.CorreoHacia, correoDeseadoBundle.HoraOriginal);

											foreach (var pendiente2 in pendientes.ToList())
											{
												if (pendiente2.CorreoHacia == correoDeseadoBundle.CorreoHacia && (pendiente2.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimo || pendiente2.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimos))
												{
													BaseDatos.CorreosEnviar.Borrar.Ejecutar(pendiente2.Id, conexion);
												}
											}
										}
									}

									pendientes = BaseDatos.CorreosEnviar.Buscar.PendientesEnviar();

									foreach (var pendiente in pendientes.ToList())
									{
										if (pendiente.Fecha.AddMinutes(5) < DateTime.Now && (pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadoBundle || pendiente.Tipo == BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadosBundle))
										{
											bool enviado = Herramientas.Correos.Enviar.Ejecutar(pendiente.Html, pendiente.Titulo, pendiente.CorreoDesde, pendiente.CorreoHacia);

											if (enviado == true)
											{
												BaseDatos.CorreosEnviar.Borrar.Ejecutar(pendiente.Id, conexion);
											}
											else
											{
												break;
											}
										}
									}

									#endregion
								}
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Correos Enviar", ex, conexion);
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

	public class CorreoMinimoFinal
	{
		public List<CorreoMinimoJson> Jsons { get; set; }
		public string CorreoHacia { get; set; }
		public DateTime HoraOriginal { get; set; }
	}

	public class CorreoDeseadoBundleFinal
	{
		public List<CorreoDeseadoBundleJson> Jsons { get; set; }
		public string CorreoHacia { get; set; }
		public DateTime HoraOriginal { get; set; }
	}
}
