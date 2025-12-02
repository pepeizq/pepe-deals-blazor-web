#nullable disable

using Juegos;
using pepeizqs_deals_web.Data;
using System.Text.Json;

namespace Herramientas
{
	public static class Deseados
	{
		public static async Task<List<JuegoDeseadoMostrar>> LeerJuegos(string usuarioId)
		{
			await Task.Yield();

			Usuario deseadosUsuario = global::BaseDatos.Usuarios.Buscar.DeseadosTiene(usuarioId);

			List<JuegoDeseadoMostrar> deseadosGestor = new List<JuegoDeseadoMostrar>();

			#region Deseados Steam

			List<string> deseadosSteam = new List<string>();

			if (string.IsNullOrEmpty(deseadosUsuario.SteamWishlist) == false)
			{
				deseadosSteam = Herramientas.Listados.Generar(deseadosUsuario.SteamWishlist);
			}

			if (deseadosSteam?.Count > 0)
			{
				List<Juego> deseadosSteamJuegos = new List<Juego>();

				deseadosSteamJuegos = global::BaseDatos.Juegos.Buscar.MultiplesJuegosSteam2(deseadosSteam.Select(int.Parse).ToList());

				if (deseadosSteamJuegos != null)
				{
					int i = 0;

					foreach (var juego in deseadosSteamJuegos)
					{
						i += 1;

						if (juego != null)
						{
							deseadosGestor = AñadirJuegoMostrar(deseadosGestor, juego, JuegoDRM.Steam, true);
						}
					}
				}
			}

			#endregion

			#region Deseados Web

			List<JuegoDeseado> deseadosWeb = new List<JuegoDeseado>();

			if (string.IsNullOrEmpty(deseadosUsuario.Wishlist) == false)
			{
				deseadosWeb = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosUsuario.Wishlist);
			}

			if (deseadosWeb?.Count > 0)
			{
				List<Juego> deseadosWebJuegos = new List<Juego>();

				deseadosWebJuegos = global::BaseDatos.Juegos.Buscar.MultiplesJuegos(deseadosWeb);

				int i = 0;

				foreach (var deseadoWeb in deseadosWebJuegos)
				{
					i += 1;

					JuegoDRM drmDeseado = JuegoDRM.NoEspecificado;

					foreach (var deseado in deseadosWeb)
					{
						if (deseado.IdBaseDatos == deseadoWeb.Id.ToString())
						{
							drmDeseado = deseado.DRM;
							break;
						}
					}

					deseadosGestor = AñadirJuegoMostrar(deseadosGestor, deseadoWeb, drmDeseado, false);
				}
			}

			#endregion

			#region Deseados GOG

			List<string> deseadosGog = new List<string>();

			if (string.IsNullOrEmpty(deseadosUsuario.GogWishlist) == false)
			{
				deseadosGog = Herramientas.Listados.Generar(deseadosUsuario.GogWishlist);
			}

			if (deseadosGog?.Count > 0)
			{
				List<Juego> deseadosGogJuegos = new List<Juego>();

				deseadosGogJuegos = global::BaseDatos.Juegos.Buscar.MultiplesJuegosGOG(deseadosGog);

				if (deseadosGogJuegos != null)
				{
					int i = 0;

					foreach (var juego in deseadosGogJuegos)
					{
						i += 1;

						if (juego != null)
						{
							deseadosGestor = AñadirJuegoMostrar(deseadosGestor, juego, JuegoDRM.GOG, true);
						}
					}
				}
			}

			#endregion

			return deseadosGestor;
		}

		private static List<JuegoDeseadoMostrar> AñadirJuegoMostrar(List<JuegoDeseadoMostrar> deseadosGestor, Juego juego, JuegoDRM drm, bool importado)
		{
			bool yaEsta = false;

			if (deseadosGestor.Count > 0)
			{
				foreach (var deseado in deseadosGestor)
				{
					if (deseado.Id == juego.Id && deseado.DRM == drm)
					{
						yaEsta = true;
						break;
					}
				}
			}

			if (yaEsta == false)
			{
				bool añadido = false;

				if (juego?.PrecioMinimosHistoricos?.Count > 0)
				{
					foreach (var historico in juego.PrecioMinimosHistoricos)
					{
						if (historico.DRM == drm)
						{
							if (Herramientas.OfertaActiva.Verificar(historico) == true)
							{
								JuegoDeseadoMostrar nuevoDeseado = new JuegoDeseadoMostrar();
								nuevoDeseado.Id = juego.Id;
								nuevoDeseado.IdSteam = juego.IdSteam;
								nuevoDeseado.IdGog = juego.IdGog;
								nuevoDeseado.SlugEpic = juego.SlugEpic;
								nuevoDeseado.Nombre = juego.Nombre;
								nuevoDeseado.Imagen = juego.Imagenes.Header_460x215;
								nuevoDeseado.DRM = drm;
								nuevoDeseado.Precio = historico;
								nuevoDeseado.Historico = true;
								nuevoDeseado.Importado = importado;

								if (juego.Analisis != null)
								{
									if (string.IsNullOrEmpty(juego.Analisis.Porcentaje) == false)
									{
										nuevoDeseado.ReseñasPorcentaje = juego.Analisis.Porcentaje.Replace("%", null);
									}

									if (string.IsNullOrEmpty(juego.Analisis.Cantidad) == false)
									{
										nuevoDeseado.ReseñasCantidad = juego.Analisis.Cantidad.Replace(",", null);
									}
								}
								else
								{
									nuevoDeseado.ReseñasPorcentaje = "0";
									nuevoDeseado.ReseñasCantidad = "0";
								}

								deseadosGestor.Add(nuevoDeseado);

								añadido = true;
							}

							break;
						}
					}
				}

				if (añadido == false)
				{
					if (juego.PrecioActualesTiendas?.Count > 0)
					{
						JuegoPrecio precioFinal = null;
						decimal precioReferencia = 1000000;

						foreach (var actual in juego.PrecioActualesTiendas)
						{
							if (actual != null)
							{
								if (actual.DRM == drm)
								{
									if (Herramientas.OfertaActiva.Verificar(actual) == true)
									{
										if (actual.Precio > 0)
										{
											if (actual.Moneda != Herramientas.JuegoMoneda.Euro && actual.PrecioCambiado == 0)
											{
												actual.PrecioCambiado = Herramientas.Divisas.Cambio(actual.Precio, actual.Moneda);
											}

											if (precioReferencia > actual.Precio && actual.Precio > 0 && actual.Moneda == Herramientas.JuegoMoneda.Euro)
											{
												precioReferencia = actual.Precio;
												precioFinal = actual;
												precioFinal.Precio = actual.Precio;
											}
											else if (precioReferencia > actual.PrecioCambiado && actual.PrecioCambiado > 0 && actual.Moneda != Herramientas.JuegoMoneda.Euro)
											{
												precioReferencia = actual.PrecioCambiado;
												precioFinal = actual;
												precioFinal.Precio = actual.Precio;
												precioFinal.PrecioCambiado = actual.PrecioCambiado;
											}
										}
									}
								}
							}
						}

						if (precioFinal != null)
						{
							JuegoDeseadoMostrar nuevoDeseado = new JuegoDeseadoMostrar();
							nuevoDeseado.Id = juego.Id;
							nuevoDeseado.IdSteam = juego.IdSteam;
							nuevoDeseado.IdGog = juego.IdGog;
							nuevoDeseado.SlugEpic = juego.SlugEpic;
							nuevoDeseado.Nombre = juego.Nombre;
							nuevoDeseado.Imagen = juego.Imagenes.Header_460x215;
							nuevoDeseado.DRM = drm;
							nuevoDeseado.Precio = precioFinal;
							nuevoDeseado.Historico = false;

							foreach (var minimo in juego.PrecioMinimosHistoricos)
							{
								if (drm == minimo.DRM)
								{
									if (minimo.PrecioCambiado > 0 && minimo.Moneda != Herramientas.JuegoMoneda.Euro)
									{
										nuevoDeseado.HistoricoPrecio = Herramientas.Precios.Euro(minimo.PrecioCambiado);
									}
									else if (minimo.PrecioCambiado == 0 && minimo.Moneda != Herramientas.JuegoMoneda.Euro)
									{
										nuevoDeseado.HistoricoPrecio = Herramientas.Precios.Euro(Herramientas.Divisas.Cambio(minimo.Precio, minimo.Moneda));
									}
									else
									{
										nuevoDeseado.HistoricoPrecio = Herramientas.Precios.Euro(minimo.Precio);
									}
								}
							}

							nuevoDeseado.Importado = importado;

							if (juego.Analisis != null)
							{
								nuevoDeseado.ReseñasPorcentaje = juego.Analisis?.Porcentaje?.Replace("%", null);
								nuevoDeseado.ReseñasCantidad = juego.Analisis?.Cantidad?.Replace(",", null);
							}

							if (string.IsNullOrEmpty(nuevoDeseado.ReseñasPorcentaje) == true)
							{
								nuevoDeseado.ReseñasPorcentaje = "0";
							}

							if (string.IsNullOrEmpty(nuevoDeseado.ReseñasCantidad) == true)
							{
								nuevoDeseado.ReseñasCantidad = "0";
							}

							deseadosGestor.Add(nuevoDeseado);
						}
					}
				}
			}

			return deseadosGestor;
		}

		public static bool ComprobarSiEstaImportado(Usuario deseados, Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado, bool usarIdMaestra = false)
		{
			if (deseados == null)
			{
				return false;
			}

			string deseadosSteamEnBruto = deseados.SteamWishlist;
			string deseadosGogEnBruto = deseados.GogWishlist;

			if (usarIdMaestra == true && juego.IdMaestra == 0)
			{
				juego.IdMaestra = juego.Id;
			}

			if (drm == JuegoDRM.Steam || drm == JuegoDRM.NoEspecificado)
			{
				if (juego.IdSteam > 0)
				{
					List<string> deseadosSteam = new List<string>();

					if (string.IsNullOrEmpty(deseadosSteamEnBruto) == false)
					{
						deseadosSteam = Listados.Generar(deseadosSteamEnBruto);
					}

					if (deseadosSteam?.Count > 0)
					{
						return deseadosSteam.Any(d => d == juego.IdSteam.ToString() && (drm == JuegoDRM.Steam || drm == JuegoDRM.NoEspecificado));
					}
				}
			}

			if (drm == JuegoDRM.GOG || drm == JuegoDRM.NoEspecificado)
			{
				if (juego.IdGog > 0)
				{
					List<string> deseadosGog = new List<string>();

					if (string.IsNullOrEmpty(deseadosGogEnBruto) == false)
					{
						deseadosGog = Listados.Generar(deseadosGogEnBruto);
					}

					if (deseadosGog?.Count > 0)
					{
						return deseadosGog.Any(d => d == juego.IdGog.ToString() && (drm == JuegoDRM.GOG || drm == JuegoDRM.NoEspecificado));
					}
				}
			}

			return false;
		}

		public static bool ComprobarSiEstaWeb(Usuario deseados, Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado, bool usarIdMaestra = false)
		{
			if (deseados == null)
			{
				return false;
			}

			string deseadosWebEnBruto = deseados.Wishlist;

			if (usarIdMaestra == true && juego.IdMaestra == 0)
			{
				juego.IdMaestra = juego.Id;
			}

			if (juego.Id > 0)
			{
				List<JuegoDeseado> deseadosWeb = new List<JuegoDeseado>();

				if (string.IsNullOrEmpty(deseadosWebEnBruto) == false)
				{
					deseadosWeb = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosWebEnBruto);
				}

				if (deseadosWeb?.Count > 0)
				{
					if (usarIdMaestra == false)
					{
						return deseadosWeb.Any(d => int.Parse(d.IdBaseDatos) == juego.Id && (drm == d.DRM || drm == JuegoDRM.NoEspecificado));
					}
					else
					{
						return deseadosWeb.Any(d => int.Parse(d.IdBaseDatos) == juego.IdMaestra && (drm == d.DRM || drm == JuegoDRM.NoEspecificado));
					}
				}
			}

			return false;
		}

		public static bool ComprobarSiEsta(string deseadosSteamEnBruto, string deseadosWebEnBruto, string deseadosGogEnBruto, Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado, bool usarIdMaestra = false)
		{
			bool resultado = false;

			if (usarIdMaestra == true && juego.IdMaestra == 0)
			{
				juego.IdMaestra = juego.Id;
			}

			if (drm == JuegoDRM.Steam || drm == JuegoDRM.NoEspecificado)
			{
				if (juego.IdSteam > 0)
				{
					List<string> deseadosSteam = new List<string>();

					if (string.IsNullOrEmpty(deseadosSteamEnBruto) == false)
					{
						deseadosSteam = Listados.Generar(deseadosSteamEnBruto);
					}

					if (deseadosSteam?.Count > 0)
					{
						resultado = deseadosSteam.Any(d => d == juego.IdSteam.ToString() && (drm == JuegoDRM.Steam || drm == JuegoDRM.NoEspecificado));
					
						if (resultado == true)
						{
							return true;
						}
					}
				}
			}

			if (drm == JuegoDRM.GOG || drm == JuegoDRM.NoEspecificado)
			{
				if (juego.IdGog > 0)
				{
					List<string> deseadosGog = new List<string>();

					if (string.IsNullOrEmpty(deseadosGogEnBruto) == false)
					{
						deseadosGog = Listados.Generar(deseadosGogEnBruto);
					}

					if (deseadosGog?.Count > 0)
					{
						resultado = deseadosGog.Any(d => d == juego.IdGog.ToString() && (drm == JuegoDRM.GOG || drm == JuegoDRM.NoEspecificado));

						if (resultado == true)
						{
							return true;
						}
					}
				}
			}

			if (juego.Id > 0 || juego.IdMaestra > 0)
			{
				List<JuegoDeseado> deseadosWeb = new List<JuegoDeseado>();

				if (string.IsNullOrEmpty(deseadosWebEnBruto) == false)
				{
					deseadosWeb = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosWebEnBruto);
				}

				if (deseadosWeb?.Count > 0)
				{
					if (usarIdMaestra == false)
					{
						return deseadosWeb.Any(d => int.Parse(d.IdBaseDatos) == juego.Id && (drm == d.DRM || drm == JuegoDRM.NoEspecificado));
					}
					else
					{
						return deseadosWeb.Any(d => int.Parse(d.IdBaseDatos) == juego.IdMaestra && (drm == d.DRM || drm == JuegoDRM.NoEspecificado));
					}
				}
			}
			
			return false;
		}

		public static bool ComprobarSiEsta(string deseadosWebEnBruto, Juego juego, JuegoDRM drm, bool usarIdMaestra = false)
		{
			List<JuegoDeseado> deseadosWeb = new List<JuegoDeseado>();

			if (string.IsNullOrEmpty(deseadosWebEnBruto) == false)
			{
				deseadosWeb = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosWebEnBruto);
			}

			if (deseadosWeb?.Count > 0)
			{
				if (usarIdMaestra == false)
				{
					return deseadosWeb.Any(d => int.Parse(d.IdBaseDatos) == juego.Id && d.DRM == drm);
				}
				else
				{
					return deseadosWeb.Any(d => int.Parse(d.IdBaseDatos) == juego.IdMaestra && d.DRM == drm);
				}
			}

			return false;
		}

		public static void CambiarEstado(string usuarioId, Juego juego, bool estado, JuegoDRM drm, bool usarIdMaestra)
		{
			List<JuegoDeseado> deseados = new List<JuegoDeseado>();

			Usuario deseadosCargar = global::BaseDatos.Usuarios.Buscar.DeseadosTiene(usuarioId);

			if (string.IsNullOrEmpty(deseadosCargar?.Wishlist) == false)
			{
				deseados = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosCargar.Wishlist);
			}

			if (estado == true)
			{
				bool añadir = true;

				if (deseados.Count > 0)
				{
					if (usarIdMaestra == false)
					{
						añadir = !deseados.Any(d => int.Parse(d.IdBaseDatos) == juego.Id && d.DRM == drm);
					}
					else
					{
						añadir = !deseados.Any(d => int.Parse(d.IdBaseDatos) == juego.IdMaestra && d.DRM == drm);
					}
				}

				if (añadir == true)
				{
					JuegoDeseado deseado = new JuegoDeseado();

					if (usarIdMaestra == false)
					{
						deseado.IdBaseDatos = juego.Id.ToString();
					}
					else
					{
						deseado.IdBaseDatos = juego.IdMaestra.ToString();
					}

					deseado.DRM = drm;

					deseados.Add(deseado);
				}

				global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);
			}
			else
			{
				if (deseados.Count > 0)
				{
					int posicion = -1;

					if (usarIdMaestra == false)
					{
						posicion = deseados.FindIndex(d => int.Parse(d.IdBaseDatos) == juego.Id && d.DRM == drm);
					}
					else
					{
						posicion = deseados.FindIndex(d => int.Parse(d.IdBaseDatos) == juego.IdMaestra && d.DRM == drm);
					}

					if (posicion >= 0)
					{
						deseados.RemoveAt(posicion);
					}
				}

				global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);
			}
		}

		public static List<JuegoTieneDesea> CambiarEstado(List<JuegoTieneDesea> usuarioTieneDesea, string usuarioId, Juego juego, bool estado, JuegoDRM drm)
		{
			List<JuegoDeseado> deseados = new List<JuegoDeseado>();

			Usuario deseadosCargar = global::BaseDatos.Usuarios.Buscar.DeseadosTiene(usuarioId);

			if (string.IsNullOrEmpty(deseadosCargar?.Wishlist) == false)
			{
				deseados = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosCargar.Wishlist);
			}

			if (estado == true)
			{
				bool añadir = true;

				if (deseados.Count > 0)
				{
					añadir = !deseados.Any(d => int.Parse(d.IdBaseDatos) == juego.Id && d.DRM == drm);
				}

				if (añadir == true)
				{
					JuegoDeseado deseado = new JuegoDeseado();
					deseado.IdBaseDatos = juego.Id.ToString();
					deseado.DRM = drm;

					deseados.Add(deseado);

					if (usuarioTieneDesea?.Count > 0)
					{
						usuarioTieneDesea.Where(u => u.DRM == drm).ToList().ForEach(d => d.Desea = true);
					}
					else
					{
						usuarioTieneDesea = new List<JuegoTieneDesea>();

						JuegoTieneDesea deseado2 = new JuegoTieneDesea();
						deseado2.DRM = drm;
						deseado2.Desea = true;
						usuarioTieneDesea.Add(deseado2);
					}
				}

				global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);
			}
			else
			{
				int posicion = -1;

				if (deseados.Count > 0)
				{
					posicion = deseados.FindIndex(d => int.Parse(d.IdBaseDatos) == juego.Id && d.DRM == drm);

					if (posicion >= 0)
					{
						deseados.RemoveAt(posicion);
					}
				}

				global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);

				if (usuarioTieneDesea != null)
				{
					usuarioTieneDesea.Where(u => u.DRM == drm).ToList().ForEach(d => d.Desea = false);
				}
			}

			return usuarioTieneDesea;
		}
	}
}
