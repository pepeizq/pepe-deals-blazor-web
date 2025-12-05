#nullable disable

using APIs.Steam;
using Herramientas;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
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
							TimeSpan siguienteComprobacion = TimeSpan.FromMinutes(30);

							if (BaseDatos.Admin.Buscar.TareaPosibleUsar("usuariosActualizar", siguienteComprobacion) == true && BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60))?.Count == 0)
							{
								BaseDatos.Admin.Actualizar.TareaUso("usuariosActualizar", DateTime.Now);

								List<BaseDatos.UsuariosActualizar.UsuarioActualizar> usuarios = BaseDatos.UsuariosActualizar.Buscar.Todos(conexion);

								if (usuarios.Count > 0)
								{
									foreach (var usuario2 in usuarios)
									{
										if (usuario2.Metodo == "Steam")
										{
											UserManager<Usuario> UserManager = _factoria.CreateScope().ServiceProvider.GetRequiredService<UserManager<Usuario>>();

											Usuario usuario = await UserManager.FindByIdAsync(usuario2.IdUsuario);
											SteamUsuario datos = await APIs.Steam.Cuenta.CargarDatos(usuario.SteamAccount);

											usuario.SteamGames = JsonSerializer.Serialize(datos.Juegos);
											usuario.SteamWishlist = datos.Deseados;
											usuario.Avatar = datos.Avatar;
											usuario.Nickname = datos.Nombre;
											usuario.SteamAccountLastCheck = DateTime.Now.ToString();
											usuario.OfficialGroup = datos.GrupoPremium;
											usuario.OfficialGroup2 = datos.GrupoNormal;

											await UserManager.UpdateAsync(usuario);

											BaseDatos.UsuariosActualizar.Limpiar.Una(usuario2, conexion);
										}

										if (usuario2.Metodo == "GOG")
										{
											UserManager<Usuario> UserManager = _factoria.CreateScope().ServiceProvider.GetRequiredService<UserManager<Usuario>>();

											Usuario usuario = await UserManager.FindByIdAsync(usuario2.IdUsuario);

											usuario.GogGames = JsonSerializer.Serialize(await APIs.GOG.Cuenta.BuscarJuegos(usuario.GogAccount));
											usuario.GogWishlist = await APIs.GOG.Cuenta.BuscarDeseados(usuario.GogId);
											usuario.GogAccountLastCheck = DateTime.Now;

											await UserManager.UpdateAsync(usuario);

											BaseDatos.UsuariosActualizar.Limpiar.Una(usuario2, conexion);
										}
									}
								}
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Usuarios Actualizar", ex, conexion);
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