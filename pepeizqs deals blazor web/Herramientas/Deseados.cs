#nullable disable

using Juegos;
using pepeizqs_deals_web.Data;
using System.Text.Json;
using Tiendas2;

namespace Herramientas
{
	public static class Deseados
	{
		

		public static async Task<List<JuegoDeseadoMostrar>> LeerJuegos(TiendaRegion region, string usuarioId)
		{
			Usuario deseadosUsuario = await global::BaseDatos.Usuarios.Buscar.DeseadosTiene(usuarioId);

			Task<List<JuegoDeseadoMostrar>> tareaSteam = CargarDeseadosSteam(deseadosUsuario, region);
			Task<List<JuegoDeseadoMostrar>> tareaWeb = CargarDeseadosWeb(deseadosUsuario, region);
			Task<List<JuegoDeseadoMostrar>> tareaGog = CargarDeseadosGog(deseadosUsuario, region);

			await Task.WhenAll(tareaSteam, tareaWeb, tareaGog);

			return tareaSteam.Result
				.Concat(tareaWeb.Result)
				.Concat(tareaGog.Result)
				.ToList();
		}

		private static async Task<List<JuegoDeseadoMostrar>> CargarDeseadosSteam(Usuario deseadosUsuario, TiendaRegion region)
		{
			List<JuegoDeseadoMostrar> resultado = new List<JuegoDeseadoMostrar>();
			HashSet<(string, JuegoDRM)> deseadosHash = new HashSet<(string, JuegoDRM)>();

			if (string.IsNullOrEmpty(deseadosUsuario.SteamWishlist) == true)
			{
				return resultado;
			}

			List<int> ids = Herramientas.Listados.Generar(deseadosUsuario.SteamWishlist).Select(int.Parse).ToList();

			if (ids.Count == 0)
			{
				return resultado;
			}

			List<Juego> juegos = await global::BaseDatos.Juegos.Buscar.MultiplesJuegosSteam2(region, ids);

			foreach (var juego in juegos.Where(j => j != null))
			{
				AñadirJuegoMostrar(resultado, deseadosHash, juego, JuegoDRM.Steam, true, region);
			}

			return resultado;
		}

		private static async Task<List<JuegoDeseadoMostrar>> CargarDeseadosWeb(Usuario deseadosUsuario, TiendaRegion region)
		{
			List<JuegoDeseadoMostrar> resultado = new List<JuegoDeseadoMostrar>();
			HashSet<(string, JuegoDRM)> deseadosHash = new HashSet<(string, JuegoDRM)>();

			if (string.IsNullOrEmpty(deseadosUsuario.Wishlist) == true)
			{
				return resultado;
			}

			List<JuegoDeseado> deseadosWeb = JsonSerializer.Deserialize<List<JuegoDeseado>>(deseadosUsuario.Wishlist);

			if (deseadosWeb?.Count == 0)
			{
				return resultado;
			}

			Dictionary<string, JuegoDeseado> deseadosWebDicc = deseadosWeb.ToDictionary(d => d.IdBaseDatos);

			List<Juego> juegos = await global::BaseDatos.Juegos.Buscar.MultiplesJuegos(region, deseadosWeb);

			foreach (var juego in juegos.Where(j => j != null))
			{
				JuegoDRM drm = deseadosWebDicc.TryGetValue(juego.Id.ToString(), out var deseado) ? deseado.DRM : JuegoDRM.NoEspecificado;

				AñadirJuegoMostrar(resultado, deseadosHash, juego, drm, false, region);
			}

			return resultado;
		}

		private static async Task<List<JuegoDeseadoMostrar>> CargarDeseadosGog(Usuario deseadosUsuario, TiendaRegion region)
		{
			List<JuegoDeseadoMostrar> resultado = new List<JuegoDeseadoMostrar>();
			HashSet<(string, JuegoDRM)> deseadosHash = new HashSet<(string, JuegoDRM)>();

			if (string.IsNullOrEmpty(deseadosUsuario.GogWishlist) == true)
			{
				return resultado;
			}

			List<string> ids = Herramientas.Listados.Generar(deseadosUsuario.GogWishlist);

			if (ids.Count == 0)
			{
				return resultado;
			}

			List<Juego> juegos = await global::BaseDatos.Juegos.Buscar.MultiplesJuegosGOG(region, ids);

			foreach (var juego in juegos.Where(j => j != null))
			{
				AñadirJuegoMostrar(resultado, deseadosHash, juego, JuegoDRM.GOG, true, region);
			}

			return resultado;
		}

		private static void AñadirJuegoMostrar(List<JuegoDeseadoMostrar> deseadosGestor, HashSet<(string id, JuegoDRM DRM)> deseadosHash,
			Juego juego, JuegoDRM drm, bool importado, TiendaRegion region)
		{
			if (deseadosHash.Add((juego.Id.ToString(), drm)) == false)
			{
				return;
			}

			JuegoDeseadoMostrar nuevoDeseado = null;
			JuegoPrecio historico = null;

			if (region == TiendaRegion.Europa && juego.PrecioMinimosHistoricos?.Count > 0)
			{
				historico = juego.PrecioMinimosHistoricos.FirstOrDefault(h => h.DRM == drm);			
			}
			else if (region == TiendaRegion.EstadosUnidos && juego.PrecioMinimosHistoricosUS?.Count	> 0)
			{
				historico = juego.PrecioMinimosHistoricosUS.FirstOrDefault(h => h.DRM == drm);
			}

			if (historico != null && Herramientas.OfertaActiva.Verificar(historico))
			{
				nuevoDeseado = new JuegoDeseadoMostrar
				{
					Id = juego.Id,
					IdSteam = juego.IdSteam,
					IdGog = juego.IdGog,
					SlugEpic = juego.SlugEpic,
					Nombre = juego.Nombre,
					Imagen = juego.Imagenes.Header_460x215,
					DRM = drm,
					Precio = historico,
					Historico = true,
					Importado = importado
				};
			}

			JuegoPrecio precioFinal = null;

			if (nuevoDeseado == null && region == TiendaRegion.Europa && juego.PrecioActualesTiendas?.Count > 0)
			{
				precioFinal = juego.PrecioActualesTiendas
					.Where(p => p != null && p.DRM == drm && Herramientas.OfertaActiva.Verificar(p) && p.Precio > 0)
					.Select(p =>
					{
						if (p.Moneda != Herramientas.JuegoMoneda.Euro && p.PrecioCambiado == 0)
							p.PrecioCambiado = Herramientas.Divisas.CambioEuro(p.Precio, p.Moneda);
						return p;
					})
					.Where(p => p.Moneda == Herramientas.JuegoMoneda.Euro ? p.Precio > 0 : p.PrecioCambiado > 0)
					.OrderBy(p => p.Moneda == Herramientas.JuegoMoneda.Euro ? p.Precio : p.PrecioCambiado)
					.FirstOrDefault();
			}
			else if (nuevoDeseado == null && region == TiendaRegion.EstadosUnidos && juego.PrecioActualesTiendasUS?.Count > 0)
			{
				precioFinal = juego.PrecioActualesTiendasUS
					.Where(p => p != null && p.DRM == drm && Herramientas.OfertaActiva.Verificar(p) && p.Precio > 0)
					.Select(p =>
					{
						if (p.Moneda != Herramientas.JuegoMoneda.Dolar && p.PrecioCambiado == 0)
							p.PrecioCambiado = Herramientas.Divisas.CambioDolar(p.Precio, p.Moneda);
						return p;
					})
					.Where(p => p.Moneda == Herramientas.JuegoMoneda.Dolar ? p.Precio > 0 : p.PrecioCambiado > 0)
					.OrderBy(p => p.Moneda == Herramientas.JuegoMoneda.Dolar ? p.Precio : p.PrecioCambiado)
					.FirstOrDefault();
			}

			if (precioFinal != null)
			{
				nuevoDeseado = new JuegoDeseadoMostrar
				{
					Id = juego.Id,
					IdSteam = juego.IdSteam,
					IdGog = juego.IdGog,
					SlugEpic = juego.SlugEpic,
					Nombre = juego.Nombre,
					Imagen = juego.Imagenes.Header_460x215,
					DRM = drm,
					Precio = precioFinal,
					Historico = false,
					Importado = importado
				};

				var minimo = juego.PrecioMinimosHistoricos?.FirstOrDefault(m => m.DRM == drm);

				if (minimo != null)
				{
					if (region == TiendaRegion.Europa)
					{
						nuevoDeseado.HistoricoPrecio = minimo.PrecioCambiado > 0 && minimo.Moneda != Herramientas.JuegoMoneda.Euro
							? Herramientas.Precios.Euro(minimo.PrecioCambiado)
							: minimo.PrecioCambiado == 0 && minimo.Moneda != Herramientas.JuegoMoneda.Euro
								? Herramientas.Precios.Euro(Herramientas.Divisas.CambioEuro(minimo.Precio, minimo.Moneda))
								: Herramientas.Precios.Euro(minimo.Precio);
					}
					else if (region == TiendaRegion.EstadosUnidos)
					{
						nuevoDeseado.HistoricoPrecio = minimo.PrecioCambiado > 0 && minimo.Moneda != Herramientas.JuegoMoneda.Dolar
							? Herramientas.Precios.Dolar(minimo.PrecioCambiado)
							: minimo.PrecioCambiado == 0 && minimo.Moneda != Herramientas.JuegoMoneda.Dolar
								? Herramientas.Precios.Dolar(Herramientas.Divisas.CambioDolar(minimo.Precio, minimo.Moneda))
								: Herramientas.Precios.Dolar(minimo.Precio);
					}
				}
			}

			if (nuevoDeseado == null)
			{
				return;
			}

			AsignarReseñas(nuevoDeseado, juego.Analisis);

			deseadosGestor.Add(nuevoDeseado);
		}

		private static void AsignarReseñas(JuegoDeseadoMostrar deseado, JuegoAnalisis analisis)
		{
			deseado.ReseñasPorcentaje = analisis?.Porcentaje?.Replace("%", null) ?? "0";
			deseado.ReseñasCantidad = analisis?.Cantidad?.Replace(",", null) ?? "0";
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

		public static async Task CambiarEstado(string usuarioId, Juego juego, bool estado, JuegoDRM drm, bool usarIdMaestra)
		{
			List<JuegoDeseado> deseados = new List<JuegoDeseado>();

			Usuario deseadosCargar = await global::BaseDatos.Usuarios.Buscar.DeseadosTiene(usuarioId);

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

				await global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);
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

				await global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);
			}
		}

		public static async Task<List<JuegoTieneDesea>> CambiarEstado(List<JuegoTieneDesea> usuarioTieneDesea, string usuarioId, Juego juego, bool estado, JuegoDRM drm)
		{
			List<JuegoDeseado> deseados = new List<JuegoDeseado>();

			Usuario deseadosCargar = await global::BaseDatos.Usuarios.Buscar.DeseadosTiene(usuarioId);

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

				await global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);
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

				await global::BaseDatos.Usuarios.Actualizar.Opcion("Wishlist", JsonSerializer.Serialize(deseados), usuarioId);

				if (usuarioTieneDesea != null)
				{
					usuarioTieneDesea.Where(u => u.DRM == drm).ToList().ForEach(d => d.Desea = false);
				}
			}

			return usuarioTieneDesea;
		}

		public static UsuarioDeseadosImportadosIndex CrearImportadosIndex(Usuario usuario)
		{
			if (usuario == null)
			{
				return null;
			}

			var index = new UsuarioDeseadosImportadosIndex();

			if (string.IsNullOrEmpty(usuario.SteamWishlist) == false)
			{
				index.Steam = Listados.Generar(usuario.SteamWishlist).Select(int.Parse).ToHashSet();
			}
				

			if (string.IsNullOrEmpty(usuario.GogWishlist) == false)
			{
				index.Gog = Listados.Generar(usuario.GogWishlist).Select(int.Parse).ToHashSet();
			}
				
			return index;
		}

		public static UsuarioDeseadosWebIndex CrearWebIndex(Usuario usuario)
		{
			var index = new UsuarioDeseadosWebIndex();

			if (!string.IsNullOrEmpty(usuario.Wishlist))
			{
				var lista = JsonSerializer.Deserialize<List<JuegoDeseado>>(usuario.Wishlist);

				index.Juegos = lista
					.Select(d => (int.Parse(d.IdBaseDatos), d.DRM))
					.ToHashSet();
			}

			return index;
		}

		public static bool ComprobarSiEstaImportado(UsuarioDeseadosImportadosIndex index, Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado)
		{
			if (juego == null)
				return false;

			if ((drm == JuegoDRM.Steam || drm == JuegoDRM.NoEspecificado) &&
				juego.IdSteam > 0 &&
				index.Steam.Contains(juego.IdSteam))
				return true;

			if ((drm == JuegoDRM.GOG || drm == JuegoDRM.NoEspecificado) &&
				juego.IdGog > 0 &&
				index.Gog.Contains(juego.IdGog))
				return true;

			return false;
		}

		public static bool ComprobarSiEstaWeb(UsuarioDeseadosWebIndex index, Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado, bool usarIdMaestra = false)
		{
			if (juego == null)
				return false;

			int idCheck = usarIdMaestra && juego.IdMaestra > 0 ? juego.IdMaestra : juego.Id;

			if (drm == JuegoDRM.NoEspecificado)
				return index.Juegos.Any(x => x.id == idCheck);

			return index.Juegos.Contains((idCheck, drm));
		}
	}

	public class UsuarioDeseadosImportadosIndex
	{
		public HashSet<int> Steam = new();
		public HashSet<int> Gog = new();
	}

	public class UsuarioDeseadosWebIndex
	{
		public HashSet<(int id, JuegoDRM drm)> Juegos = new();
	}
}
