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

		public static UsuarioListadosJuegos Cargar(Usuario usuario)
		{
			UsuarioListadosJuegos listados = new UsuarioListadosJuegos();

			if (string.IsNullOrEmpty(usuario.SteamGames) == false)
			{
				try
				{
					listados.Steam = JsonSerializer.Deserialize<List<SteamUsuarioJuego>>(usuario.SteamGames);
				}
				catch { }
			}

			if (string.IsNullOrEmpty(usuario.GogGames) == false)
			{
				try
				{
					listados.Gog = JsonSerializer.Deserialize<List<GOGUsuarioJuego>>(usuario.GogGames);
				}
				catch { }
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

		public static bool ComprobarSiTiene(UsuarioListadosJuegos listados, Juegos.Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado)
		{
			if (juego != null && listados != null)
			{
				if (juego.IdSteam > 0)
				{
					bool drmValidoSteam = false;

					if (drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Steam)
					{
						drmValidoSteam = true;
					}

					if (drmValidoSteam == true)
					{
						if (listados.Steam?.Count > 0)
						{
							if (juego.Tipo == JuegoTipo.Game)
							{
								foreach (var juegoUsuario in listados.Steam)
								{
									if (juegoUsuario.Id == juego.IdSteam)
									{
										return true;
									}
								}
							}
						}
					}
				}
				
				if (juego.IdGog > 0)
				{
					bool drmValidoGog = false;

					if (drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.GOG)
					{
						drmValidoGog = true;
					}

					if (drmValidoGog == true)
					{
						if (listados.Gog?.Count > 0)
						{
							if (juego.Tipo == JuegoTipo.Game)
							{
								foreach (var juegoUsuario in listados.Gog)
								{
									if (juegoUsuario.Id == juego.IdGog)
									{
										return true;
									}
								}
							}
						}
					}
				}
				
				if (string.IsNullOrEmpty(juego.IdAmazon) == false)
				{
					bool drmValidoAmazon = false;

					if (drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Amazon)
					{
						drmValidoAmazon = true;
					}

					if (drmValidoAmazon == true)
					{
						if (listados.Amazon?.Count > 0)
						{
							if (juego.Tipo == JuegoTipo.Game)
							{
								foreach (var juegoUsuario in listados.Amazon)
								{
									if (juegoUsuario == juego.IdAmazon.ToString())
									{
										return true;
									}
								}
							}
						}
					}
				}

				if (string.IsNullOrEmpty(juego.ExeEpic) == false)
				{
					bool drmValidoEpic = false;

					if (drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Epic)
					{
						drmValidoEpic = true;
					}

					if (drmValidoEpic == true)
					{
						if (listados.Epic?.Count > 0)
						{
							if (juego.Tipo == JuegoTipo.Game)
							{
								foreach (var juegoUsuario in listados.Epic)
								{
									if (juegoUsuario == juego.ExeEpic)
									{
										return true;
									}
								}
							}
						}
					}
				}

				if (string.IsNullOrEmpty(juego.ExeUbisoft) == false)
				{
					bool drmValidoUbisoft = false;

					if (drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Ubisoft)
					{
						drmValidoUbisoft = true;
					}

					if (drmValidoUbisoft == true)
					{
						if (listados.Ubisoft?.Count > 0)
						{
							if (juego.Tipo == JuegoTipo.Game)
							{
								foreach (var juegoUsuario in listados.Ubisoft)
								{
									if (juegoUsuario == juego.ExeUbisoft)
									{
										return true;
									}
								}
							}
						}
					}
				}

				if (string.IsNullOrEmpty(juego.ExeEA) == false)
				{
					bool drmValidoEa = false;

					if (drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.EA)
					{
						drmValidoEa = true;
					}

					if (drmValidoEa == true)
					{
						if (listados.Ea?.Count > 0)
						{
							if (juego.Tipo == JuegoTipo.Game)
							{
								foreach (var juegoUsuario in listados.Ea)
								{
									if (juegoUsuario == juego.ExeEA)
									{
										return true;
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

		public static UsuarioJuegosIndex CrearIndex(UsuarioListadosJuegos listados)
		{
			var index = new UsuarioJuegosIndex();

			if (listados.Steam != null)
				index.Steam = listados.Steam.Select(x => x.Id).ToHashSet();

			if (listados.Gog != null)
				index.Gog = listados.Gog.Select(x => x.Id).ToHashSet();

			if (listados.Amazon != null)
				index.Amazon = listados.Amazon.ToHashSet();

			if (listados.Epic != null)
				index.Epic = listados.Epic.ToHashSet();

			if (listados.Ubisoft != null)
				index.Ubisoft = listados.Ubisoft.ToHashSet();

			if (listados.Ea != null)
				index.Ea = listados.Ea.ToHashSet();

			return index;
		}

		public static bool ComprobarSiTiene(UsuarioJuegosIndex index, Juegos.Juego juego, JuegoDRM drm = JuegoDRM.NoEspecificado)
		{
			if (juego == null || juego.Tipo != JuegoTipo.Game)
				return false;

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Steam) &&
				juego.IdSteam > 0 &&
				index.Steam.Contains(juego.IdSteam))
				return true;

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.GOG) &&
				juego.IdGog > 0 &&
				index.Gog.Contains(juego.IdGog))
				return true;

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Amazon) &&
				!string.IsNullOrEmpty(juego.IdAmazon) &&
				index.Amazon.Contains(juego.IdAmazon))
				return true;

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Epic) &&
				!string.IsNullOrEmpty(juego.ExeEpic) &&
				index.Epic.Contains(juego.ExeEpic))
				return true;

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.Ubisoft) &&
				!string.IsNullOrEmpty(juego.ExeUbisoft) &&
				index.Ubisoft.Contains(juego.ExeUbisoft))
				return true;

			if ((drm == JuegoDRM.NoEspecificado || drm == JuegoDRM.EA) &&
				!string.IsNullOrEmpty(juego.ExeEA) &&
				index.Ea.Contains(juego.ExeEA))
				return true;

			return false;
		}
	}

	public class UsuarioJuegosIndex
	{
		public HashSet<int> Steam = new();
		public HashSet<int> Gog = new();
		public HashSet<string> Amazon = new();
		public HashSet<string> Epic = new();
		public HashSet<string> Ubisoft = new();
		public HashSet<string> Ea = new();
    }

	public class UsuarioListadosJuegos
	{
		public List<SteamUsuarioJuego> Steam { get; set; }
		public List<GOGUsuarioJuego> Gog { get; set; }
		public List<string> Amazon { get; set; }
		public List<string> Epic { get; set; }
		public List<string> Ubisoft { get; set; }
		public List<string> Ea { get; set; }
	}
}
