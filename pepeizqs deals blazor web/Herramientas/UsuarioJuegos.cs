#nullable disable

using APIs.GOG;
using APIs.Steam;
using Juegos;
using pepeizqs_deals_web.Data;
using System.Text.Json;

namespace Herramientas
{
	public static class UsuarioJuegos
	{
		public static async Task<UsuarioListadosJuegos> Cargar(string usuarioId)
		{
			Usuario usuario = await global::BaseDatos.Usuarios.Buscar.JuegosTiene(usuarioId);

			if (usuario == null)
			{
				return null;
			}

			UsuarioListadosJuegos listados = new UsuarioListadosJuegos();

			if (string.IsNullOrEmpty(usuario.GamesExcluded) == false)
			{
				try
				{
					string[] datosPartidos = usuario.GamesExcluded.Split(',');
					foreach (var dato in datosPartidos)
					{
						if (int.TryParse(dato.Trim(), out int numero))
						{
							if (listados.Excluidos == null)
							{
								listados.Excluidos = new List<int>();
							}

							listados.Excluidos.Add(numero);
						}
					}
				}
				catch { }
			}

			if (string.IsNullOrEmpty(usuario.SteamGames) == false)
			{
				try
				{
					listados.Steam = JsonSerializer.Deserialize<List<SteamUsuarioJuego>>(usuario.SteamGames);
				} catch { }
			}

			if (string.IsNullOrEmpty(usuario.GogGames) == false)
			{
				try
				{
					listados.Gog = JsonSerializer.Deserialize<List<GOGUsuarioJuego>>(usuario.GogGames);
				} catch { }				
			}

			if (string.IsNullOrEmpty(usuario.AmazonGames) == false)
			{
				listados.Amazon = Herramientas.Listados.Generar(usuario.AmazonGames);
			}

			if (string.IsNullOrEmpty(usuario.EpicGames) == false)
			{
				listados.Epic = Herramientas.Listados.Generar(usuario.EpicGames);
			}

			if (string.IsNullOrEmpty(usuario.UbisoftGames) == false)
			{
				listados.Ubisoft = Herramientas.Listados.Generar(usuario.UbisoftGames);
			}

			if (string.IsNullOrEmpty(usuario.EaGames) == false)
			{
				listados.Ea = Herramientas.Listados.Generar(usuario.EaGames);
			}

			return listados;
		}

		public static bool ComprobarSiEstaExcluido(HashSet<int> excluidos, int juegoId)
		{
			if (juegoId > 0 && excluidos?.Count > 0)
			{
				if (excluidos.Contains(juegoId) == true)
				{
					return true;
				}
			}

			return false;
		}

		public static UsuarioJuegosIndex CrearIndex(UsuarioListadosJuegos listados)
		{
			UsuarioJuegosIndex index = new UsuarioJuegosIndex();

			if (listados.Excluidos != null)
			{
				index.Excluidos = listados.Excluidos.ToHashSet();
			}

			if (listados.Steam != null)
			{
				index.Steam = listados.Steam.Select(x => x.Id).ToHashSet();
			}

			if (listados.Gog != null)
			{
				index.Gog = listados.Gog.Select(x => x.Id).ToHashSet();
			}

			if (listados.Amazon != null)
			{
				index.Amazon = listados.Amazon.ToHashSet();
			}

			if (listados.Epic != null)
			{
				index.Epic = listados.Epic.ToHashSet();
			}

			if (listados.Ubisoft != null)
			{
				index.Ubisoft = listados.Ubisoft.ToHashSet();
			}

			if (listados.Ea != null)
			{	
				index.Ea = listados.Ea.ToHashSet();
			}

			return index;
		}

		public static bool ComprobarSiTiene(UsuarioJuegosIndex index, Juegos.Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado)
		{
			if (index == null || juego == null || juego.Tipo != JuegoTipo.Game)
			{
				return false;
			}

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Steam) && juego.IdSteam > 0 && index.Steam.Contains(juego.IdSteam) == true)
			{
				return true;
			}

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.GOG) && juego.IdGog > 0 && index.Gog.Contains(juego.IdGog) == true)
			{
				return true;
			}

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Amazon) && string.IsNullOrEmpty(juego.IdAmazon) == false && index.Amazon.Contains(juego.IdAmazon) == true)
			{
				return true;
			}

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Epic) && string.IsNullOrEmpty(juego.ExeEpic) == false && index.Epic.Contains(juego.ExeEpic) == true)
			{
				return true;
			}

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Ubisoft) && string.IsNullOrEmpty(juego.ExeUbisoft) == false && index.Ubisoft.Contains(juego.ExeUbisoft) == true)
			{
				return true;
			}

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.EA) && string.IsNullOrEmpty(juego.ExeEA) == false && index.Ea.Contains(juego.ExeEA) == true)
			{
				return true;
			}

			return false;
		}
	}

	public class UsuarioJuegosIndex
	{
		public HashSet<int> Excluidos = new();
		public HashSet<int> Steam = new();
		public HashSet<int> Gog = new();
		public HashSet<string> Amazon = new();
		public HashSet<string> Epic = new();
		public HashSet<string> Ubisoft = new();
		public HashSet<string> Ea = new();

		public bool EstaVacia =>
			Steam.Count == 0 &&
			Gog.Count == 0 &&
			Amazon.Count == 0 &&
			Epic.Count == 0 &&
			Ubisoft.Count == 0 &&
			Ea.Count == 0;
	}

	public class UsuarioListadosJuegos
	{
		public List<int> Excluidos { get; set; }
		public List<SteamUsuarioJuego> Steam { get; set; }
		public List<GOGUsuarioJuego> Gog { get; set; }
		public List<string> Amazon { get; set; }
		public List<string> Epic { get; set; }
		public List<string> Ubisoft { get; set; }
		public List<string> Ea { get; set; }
	}
}
