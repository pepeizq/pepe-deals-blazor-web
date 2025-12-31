#nullable disable

using APIs.Steam;
using Herramientas;
using pepeizqs_deals_web.Data;
using System.Text.Json;

namespace Tareas
{
	public class UsuariosActualizar : BackgroundService
	{
		private readonly ILogger<UsuariosActualizar> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;

		public UsuariosActualizar(ILogger<UsuariosActualizar> logger, IServiceScopeFactory factory, IDecompiladores decompilador)
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
					try
					{
						TimeSpan siguienteComprobacion = TimeSpan.FromMinutes(30);

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("usuariosActualizar", siguienteComprobacion) == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("usuariosActualizar", DateTime.Now);

							List<BaseDatos.UsuariosActualizar.UsuarioActualizar> usuarios = await BaseDatos.UsuariosActualizar.Buscar.Todos();
					
							if (usuarios?.Count > 0)
							{
								foreach (var usuario2 in usuarios)
								{
									if (usuario2.Metodo == "Steam")
									{
										Usuario usuario = await BaseDatos.Usuarios.Buscar.OpcionesSteam(usuario2.IdUsuario);
										usuario.Id = usuario2.IdUsuario;

										if (string.IsNullOrEmpty(usuario?.SteamAccount) == false)
										{
											SteamUsuario datos = await APIs.Steam.Cuenta.CargarDatos(usuario.SteamAccount);
											
											if (datos != null)
											{
												bool actualizarJuegos = true;

												if (usuario.SteamGamesAllow != null && usuario.SteamGamesAllow == false)
												{
													actualizarJuegos = false;
												}

												if (actualizarJuegos == true)
												{
													usuario.SteamGames = JsonSerializer.Serialize(datos.Juegos);
												}

												bool actualizarDeseados = true;

												if (usuario.SteamWishlistAllow != null && usuario.SteamWishlistAllow == false)
												{
													actualizarDeseados = false;
												}

												if (actualizarDeseados == true)
												{
													usuario.SteamWishlist = datos.Deseados;
												}

												usuario.Avatar = datos.Avatar;
												usuario.Nickname = datos.Nombre;
												usuario.SteamAccountLastCheck = DateTime.Now.ToString();
												usuario.OfficialGroup = datos.GrupoPremium;
												usuario.OfficialGroup2 = datos.GrupoNormal;

												await BaseDatos.Usuarios.Actualizar.Steam(usuario);
												await BaseDatos.UsuariosActualizar.Limpiar.Una(usuario2);
											}
										}
									}

									if (usuario2.Metodo == "GOG")
									{
										Usuario usuario = await BaseDatos.Usuarios.Buscar.OpcionesGOG(usuario2.IdUsuario);
										usuario.Id = usuario2.IdUsuario;

										if (usuario != null)
										{
											bool actualizarJuegos = true;

											if (usuario.GogGamesAllow != null && usuario.GogGamesAllow == false)
											{
												actualizarJuegos = false;
											}

											if (actualizarJuegos == true)
											{
												usuario.GogGames = JsonSerializer.Serialize(await APIs.GOG.Cuenta.BuscarJuegos(usuario.GogAccount));
											}

											bool actualizarDeseados = true;

											if (usuario.GogWishlistAllow != null && usuario.GogWishlistAllow == false)
											{
												actualizarDeseados = false;
											}

											if (actualizarDeseados == true)
											{
												usuario.GogWishlist = await APIs.GOG.Cuenta.BuscarDeseados(usuario.GogId);
											}

											usuario.GogAccountLastCheck = DateTime.Now;

											await BaseDatos.Usuarios.Actualizar.GOG(usuario);
											await BaseDatos.UsuariosActualizar.Limpiar.Una(usuario2);
										}
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Usuarios Actualizar", ex);
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