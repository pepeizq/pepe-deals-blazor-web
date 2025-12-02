#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace APIs.AmazonLuna
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion GenerarClaims()
		{
			Suscripciones2.Suscripcion amazon = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.LunaClaims,
				Nombre = "Luna",
				ImagenLogo = "/imagenes/suscripciones/lunaclaims_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp",
				Enlace = "https://luna.amazon.es/claims/",
				DRMDefecto = JuegoDRM.Amazon,
				AdminInteractuar = true,
				UsuarioEnlacesEspecificos = true,
				ParaSiempre = true,
				Precio = 4.99
			};

			DateTime fechaPrime = DateTime.Now;
			fechaPrime = fechaPrime.AddMonths(1);
			fechaPrime = new DateTime(fechaPrime.Year, fechaPrime.Month, fechaPrime.Day, 19, 0, 0);

			amazon.FechaSugerencia = fechaPrime;

			return amazon;
		}

		public static Suscripciones2.Suscripcion GenerarStandard()
		{
			Suscripciones2.Suscripcion amazon = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.LunaStandard,
				Nombre = "Luna Standard",
				ImagenLogo = "/imagenes/suscripciones/lunastandard_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp",
				Enlace = "https://luna.amazon.es/subscription/luna-standard",
				DRMDefecto = JuegoDRM.AmazonLuna,
				AdminInteractuar = false,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 4.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionlunapremium"
			};

			return amazon;
		}

		public static Suscripciones2.Suscripcion GenerarPremium()
		{
			Suscripciones2.Suscripcion amazon = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.LunaPremium,
				Nombre = "Luna Premium",
				ImagenLogo = "/imagenes/suscripciones/lunapremium_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp",
				Enlace = "https://luna.amazon.es/subscription/luna-premium/B085TRCCT6",
				DRMDefecto = JuegoDRM.AmazonLuna,
				AdminInteractuar = true,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 9.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionlunapremium"
			};

			return amazon;
		}

		public static string Referido(string enlace)
		{
			if (enlace.Contains("?") == true)
			{
				enlace = enlace + "&tag=ofedeunpan-21";
			}
			else
			{
				enlace = enlace + "?tag=ofedeunpan-21";
			}

			return enlace;
		}

		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static async Task Buscar(SqlConnection conexion = null)
		{
			await Task.Yield();

			conexion = CogerConexion(conexion);

			int cantidad = 0;

			#region Standard

			BaseDatos.Admin.Actualizar.Tiendas(GenerarStandard().Id.ToString().ToLower(), DateTime.Now, 0);

			List<AmazonLunaJuego> juegosStandard = null;

			try 
			{ 
				var filas = conexion.Query<AmazonLunaFila>("SELECT contenido FROM temporallunastandardjson");

				foreach (var fila in filas)
				{
					juegosStandard = JsonSerializer.Deserialize<List<AmazonLunaJuego>>(fila.Contenido);
				}
			}
			catch (Exception ex) 
			{ 
				BaseDatos.Errores.Insertar.Mensaje("Luna Standard Suscripcion 1", ex); 
			}

			if (juegosStandard?.Count > 0)
			{
				foreach (var juego in juegosStandard)
				{
					if (string.IsNullOrEmpty(juego.Id) == false)
					{
						bool encontrado = false;

						conexion = CogerConexion(conexion);

						string idJuegosTexto = conexion.QueryFirstOrDefault<string>($"SELECT idJuegos FROM {GenerarPremium().TablaPendientes} WHERE enlace=@enlace", new { enlace = juego.Id });

						if (string.IsNullOrEmpty(idJuegosTexto) == false)
						{
							encontrado = true;

							if (idJuegosTexto != "0")
							{
								List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

								if (idJuegos.Count > 0)
								{
									foreach (var id in idJuegos)
									{
										cantidad += 1;
										BaseDatos.Admin.Actualizar.Tiendas(GenerarStandard().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.LunaStandard)
												{
													insertar = false;

													suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(2);

													BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
												}
											}
										}

										if (insertar == true)
										{
											DateTime nuevaFecha = DateTime.Now;
											nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

											JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
											{
												DRM = JuegoDRM.AmazonLuna,
												Nombre = juego.Nombre,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.Id,
												Tipo = Suscripciones2.SuscripcionTipo.LunaStandard
											};

											BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
							BaseDatos.Suscripciones.Insertar.Temporal(conexion, GenerarStandard().Id.ToString().ToLower(), juego.Id, juego.Nombre);
						}
					}
				}

				conexion = CogerConexion(conexion);

				try 
				{ 
					conexion.Execute("DELETE FROM temporallunastandardjson WHERE enlace='1'"); 
				} 
				catch (Exception ex) 
				{ 
					BaseDatos.Errores.Insertar.Mensaje(GenerarStandard().Id.ToString().ToLower(), ex); 
				}
			}

			#endregion

			#region Premium

			BaseDatos.Admin.Actualizar.Tiendas(GenerarPremium().Id.ToString().ToLower(), DateTime.Now, 0);

			cantidad = 0;

			List<AmazonLunaJuego> juegosPremium = null;

			try
			{
				var filas = conexion.Query<AmazonLunaFila>("SELECT contenido FROM temporallunapremiumjson");

				foreach (var fila in filas)
				{
					juegosPremium = JsonSerializer.Deserialize<List<AmazonLunaJuego>>(fila.Contenido);
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Luna Premium Suscripcion 1", ex);
			}

			if (juegosPremium?.Count > 0)
			{
				foreach (var juego in juegosPremium)
				{
					if (string.IsNullOrEmpty(juego.Id) == false)
					{
						bool encontrado = false;

						conexion = CogerConexion(conexion);

						string idJuegosTexto = conexion.QueryFirstOrDefault<string>($"SELECT idJuegos FROM {GenerarPremium().TablaPendientes} WHERE enlace=@enlace", new { enlace = juego.Id });

						if (string.IsNullOrEmpty(idJuegosTexto) == false)
						{
							encontrado = true;

							if (idJuegosTexto != "0")
							{
								List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

								if (idJuegos.Count > 0)
								{
									foreach (var id in idJuegos)
									{
										cantidad += 1;
										BaseDatos.Admin.Actualizar.Tiendas(GenerarPremium().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.LunaPremium)
												{
													insertar = false;

													suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(2);

													BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
												}
											}
										}

										if (insertar == true)
										{
											DateTime nuevaFecha = DateTime.Now;
											nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

											JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
											{
												DRM = JuegoDRM.AmazonLuna,
												Nombre = juego.Nombre,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.Id,
												Tipo = Suscripciones2.SuscripcionTipo.LunaPremium
											};

											BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
							BaseDatos.Suscripciones.Insertar.Temporal(conexion, GenerarPremium().Id.ToString().ToLower(), juego.Id, juego.Nombre);
						}
					}
				}

				conexion = CogerConexion(conexion);

				try
				{
					conexion.Execute("DELETE FROM temporallunapremiumjson WHERE enlace='1'");
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(GenerarPremium().Id.ToString().ToLower(), ex);
				}
			}

			#endregion
		}
	}

	public class AmazonLunaFila
	{
		public string Contenido { get; set; }
	}

	public class AmazonLunaJuego
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
	}
}
