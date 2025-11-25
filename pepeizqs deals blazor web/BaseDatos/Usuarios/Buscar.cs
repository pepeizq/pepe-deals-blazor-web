#nullable disable

using Herramientas;
using Juegos;
using Microsoft.Data.SqlClient;
using pepeizqs_deals_web.Data;

namespace BaseDatos.Usuarios
{
	public static class Buscar
	{
		public static bool RolDios(string id)
		{
			bool esDios = false;

			if (string.IsNullOrEmpty(id) == false)
			{
				SqlConnection conexion = Herramientas.BaseDatos.Conectar();

				using (conexion)
				{
					string busqueda = "SELECT Role FROM AspNetUsers WHERE Id=@Id";

					using (SqlCommand comando = new SqlCommand(busqueda, conexion))
					{
						comando.Parameters.AddWithValue("@Id", id);

						using (SqlDataReader lector = comando.ExecuteReader())
						{
							while (lector.Read())
							{
								if (lector.GetString(0) == "God")
								{
									esDios = true;
								}
							}
						}
					}
				}
			}

			return esDios;
		}

		public static string IdiomaSobreescribir(string usuarioId, SqlConnection conexion = null)
		{
			string idioma = "en";

			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT LanguageOverride FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								idioma = lector.GetString(0);
							}
						}
					}
				}
			}

			return idioma;
		}

		public static string IdiomaJuegos(string usuarioId, SqlConnection conexion = null)
		{
			string idioma = "en";

			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT LanguageGames FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								idioma = lector.GetString(0);
							}
						}
					}
				}
			}

			return idioma;
		}

		public static Usuario JuegosTiene(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario juegos = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT SteamGames, GogGames, AmazonGames, EpicGames, UbisoftGames, EaGames FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								juegos.SteamGames = lector.GetString(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								juegos.GogGames = lector.GetString(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								juegos.AmazonGames = lector.GetString(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								juegos.EpicGames = lector.GetString(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								juegos.UbisoftGames = lector.GetString(4);
							}

							if (lector.IsDBNull(5) == false)
							{
								juegos.EaGames = lector.GetString(5);
							}
						}
					}
				}

				return juegos;
			}

			return null;
		}

		public static DateTime? FechaPatreon(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT PatreonLastCheck FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								return lector.GetDateTime(0);
							}
						}
					}
				}
			}

			return null;
		}

		public static Usuario DeseadosTiene(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario deseados = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT SteamWishlist, Wishlist, GogWishlist, WishlistData FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								deseados.SteamWishlist = lector.GetString(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								deseados.Wishlist = lector.GetString(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								deseados.GogWishlist = lector.GetString(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								deseados.WishlistData = lector.GetString(3);
							}
						}
					}
				}

				return deseados;
			}

			return null;
		}

		public static Usuario OpcionesCabecera(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario opciones = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT Avatar, Email, Nickname, PatreonCoins FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								opciones.Avatar = lector.GetString(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								opciones.Email = lector.GetString(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								opciones.Nickname = lector.GetString(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								opciones.PatreonCoins = lector.GetInt32(3);
							}
						}
					}
				}

				return opciones;
			}

			return null;
		}

		public static Usuario OpcionesPortada(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario opciones = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT IndexOption1, IndexOption2, IndexDRMs, IndexCategories, ForumIndex FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								opciones.IndexOption1 = lector.GetBoolean(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								opciones.IndexOption2 = lector.GetBoolean(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								opciones.IndexDRMs = lector.GetString(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								opciones.IndexCategories = lector.GetString(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								opciones.ForumIndex = lector.GetBoolean(4);
							}
						}
					}
				}

				return opciones;
			}

			return null;
		}

		public static Usuario OpcionesDeseados(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario opciones = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT WishlistSort, WishlistOption3, WishlistOption4, WishlistPosition, WishlistData FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								opciones.WishlistSort = lector.GetInt32(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								opciones.WishlistOption3 = lector.GetInt32(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								opciones.WishlistOption4 = lector.GetDecimal(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								opciones.WishlistPosition = lector.GetInt32(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								opciones.WishlistData = lector.GetString(4);
							}
						}
					}
				}

				return opciones;
			}

			return null;
		}

		public static Usuario OpcionesMinimos(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario opciones = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT HistoricalLowsOption1, HistoricalLowsOption4, HistoricalLowsOption2, HistoricalLowsOption3, HistoricalLowsDRMs, HistoricalLowsStores, HistoricalLowsCategories, HistoricalLowsSteamDeck, HistoricalLowsSort, HistoricalLowsRelease, HistoricalLowsAI FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								opciones.HistoricalLowsOption1 = lector.GetBoolean(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								opciones.HistoricalLowsOption4 = lector.GetBoolean(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								opciones.HistoricalLowsOption2 = lector.GetInt32(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								opciones.HistoricalLowsOption3 = lector.GetDecimal(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								opciones.HistoricalLowsDRMs = lector.GetString(4);
							}

							if (lector.IsDBNull(5) == false)
							{
								opciones.HistoricalLowsStores = lector.GetString(5);
							}

							if (lector.IsDBNull(6) == false)
							{
								opciones.HistoricalLowsCategories = lector.GetString(6);
							}

							if (lector.IsDBNull(7) == false)
							{
								opciones.HistoricalLowsSteamDeck = lector.GetString(7);
							}

							if (lector.IsDBNull(8) == false)
							{
								opciones.HistoricalLowsSort = lector.GetInt32(8);
							}

							if (lector.IsDBNull(9) == false)
							{
								opciones.HistoricalLowsRelease = lector.GetInt32(9);
							}

							if (lector.IsDBNull(10) == false)
							{
								opciones.HistoricalLowsAI = lector.GetInt32(10);
							}
						}
					}
				}

				return opciones;
			}

			return null;
		}

		public static Usuario OpcionesCuenta(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				Usuario opciones = new Usuario();

				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT EmailConfirmed, PatreonCoins, SteamAccountLastCheck, GogAccountLastCheck, AmazonLastImport, EpicGamesLastImport, UbisoftLastImport, EaLastImport FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								opciones.EmailConfirmed = lector.GetBoolean(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								opciones.PatreonCoins = lector.GetInt32(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								opciones.SteamAccountLastCheck = lector.GetString(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								opciones.GogAccountLastCheck = lector.GetDateTime(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								opciones.AmazonLastImport = lector.GetDateTime(4);
							}

							if (lector.IsDBNull(5) == false)
							{
								opciones.EpicGamesLastImport = lector.GetDateTime(5);
							}

							if (lector.IsDBNull(6) == false)
							{
								opciones.UbisoftLastImport = lector.GetDateTime(6);
							}

							if (lector.IsDBNull(7) == false)
							{
								opciones.EaLastImport = lector.GetDateTime(7);
							}
						}
					}
				}

				return opciones;
			}

			return null;
		}

		public static List<Usuario> UsuariosNotificacionesCorreo(SqlConnection conexion = null)
		{
			List<Usuario> usuarios = new List<Usuario>();

			if (conexion == null)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
			}

			string busqueda = "SELECT Id, NotificationBundles, NotificationFree, NotificationSubscriptions, NotificationOthers, NotificationWeb, NotificationDelisted, Email, Language FROM AspNetUsers";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							bool añadir = false;

							Usuario usuario = new Usuario
							{
								Id = lector.GetString(0)
							};

							if (lector.IsDBNull(1) == false)
							{
								añadir = true;
								usuario.NotificationBundles = lector.GetBoolean(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								añadir = true;
								usuario.NotificationFree = lector.GetBoolean(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								añadir = true;
								usuario.NotificationSubscriptions = lector.GetBoolean(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								añadir = true;
								usuario.NotificationOthers = lector.GetBoolean(4);
							}

							if (lector.IsDBNull(5) == false)
							{
								añadir = true;
								usuario.NotificationWeb = lector.GetBoolean(5);
							}

							if (lector.IsDBNull(6) == false)
							{
								añadir = true;
								usuario.NotificationDelisted = lector.GetBoolean(6);
							}

							if (lector.IsDBNull(7) == false)
							{
								añadir = true;
								usuario.Email = lector.GetString(7);
							}

							if (lector.IsDBNull(8) == false)
							{
								añadir = true;
								usuario.Language = lector.GetString(8);
							}

							if (añadir == true)
							{
								usuarios.Add(usuario);
							}
						}
					}
				}
			}

			return usuarios;
		}

		public static List<Usuario> UsuariosNotificacionesPush(SqlConnection conexion = null)
		{
			List<Usuario> usuarios = new List<Usuario>();

			if (conexion == null)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
			}

			string busqueda = "SELECT Id, NotificationPushBundles, NotificationPushFree, NotificationPushSubscriptions, NotificationPushOthers, Language FROM AspNetUsers";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							bool añadir = false;

							Usuario usuario = new Usuario
							{
								Id = lector.GetString(0)
							};

							if (lector.IsDBNull(1) == false)
							{
								añadir = true;
								usuario.NotificationPushBundles = lector.GetBoolean(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								añadir = true;
								usuario.NotificationPushFree = lector.GetBoolean(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								añadir = true;
								usuario.NotificationPushSubscriptions = lector.GetBoolean(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								añadir = true;
								usuario.NotificationPushOthers = lector.GetBoolean(4);
							}

							if (lector.IsDBNull(5) == false)
							{
								añadir = true;
								usuario.Language = lector.GetString(5);
							}

							if (añadir == true)
							{
								usuarios.Add(usuario);
							}
						}
					}
				}
			}

			return usuarios;
		}

		public static bool CorreoYaUsado(string correo, SqlConnection conexion = null)
		{
			bool yaUsado = false;

			if (string.IsNullOrEmpty(correo) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT Id FROM AspNetUsers WHERE Email=@Email";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Email", correo);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								yaUsado = true;
							}
						}
					}
				}
			}

			return yaUsado;
		}

		public static bool UsuarioTieneJuego(string usuarioId, int juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
			bool yaTiene = false;

			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = "DECLARE @idSteam nvarchar(256);\r\n\r\nSET @idSteam = CONCAT('\"Id\":', (SELECT idSteam FROM juegos WHERE id=@juegoId), ',');\r\n\r\nSELECT id FROM AspNetUsers WHERE CHARINDEX(@idSteam, SteamGames) > 0 AND id=@id";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = "DECLARE @idGog nvarchar(256);\r\n\r\nSET @idGog = CONCAT('\"Id\":', (SELECT idGog FROM juegos WHERE id=@juegoId), ',');\r\n\r\nSELECT id FROM AspNetUsers WHERE CHARINDEX(@idGog, GogGames) > 0 AND id=@id";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(AmazonGames, ',') WHERE VALUE IN ((SELECT idAmazon FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EpicGames, ',') WHERE VALUE IN ((SELECT exeEpic FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(UbisoftGames, ',') WHERE VALUE IN ((SELECT exeUbisoft FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EaGames, ',') WHERE VALUE IN ((SELECT exeEa FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@id", usuarioId);
					comando.Parameters.AddWithValue("@juegoId", juegoId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								yaTiene = true;
							}
						}
					}
				}
			}

			return yaTiene;
		}

		public static string UsuarioQuiereCorreos(string usuarioId, string seccion, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT " + seccion + ", EmailConfirmed, Email FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						while (lector.Read())
						{
							bool notificaciones = false;
							bool correo = false;

							//Enseñar Notificaciones Minimos
							if (lector.IsDBNull(0) == false)
							{
								if (lector.GetBoolean(0) == true)
								{
									notificaciones = true;
								}
							}

							//Correo Confirmado
							if (lector.IsDBNull(1) == false)
							{
								if (lector.GetBoolean(1) == true)
								{
									correo = true;
								}
							}

							if (correo == true && notificaciones == true)
							{
								//Correo
								if (lector.IsDBNull(2) == false)
								{
									if (string.IsNullOrEmpty(lector.GetString(2)) == false)
									{
										return lector.GetString(2);
									}
								}
							}
						}
					}
				}
			}

			return null;
		}

		public static bool CuentaSteamUsada(string id64Steam, string idUsuario)
		{
			SqlConnection conexion = Herramientas.BaseDatos.Conectar();

			using (conexion)
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE SteamId=@SteamId";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@SteamId", id64Steam);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						while (lector.Read())
						{
							if (lector.IsDBNull(0) == false)
							{
								if (lector.GetString(0) != idUsuario)
								{
									return true;
								}
							}
						}
					}
				}
			}

			return false;
		}

		public static bool CuentaGogUsada(string idGog, string idUsuario, SqlConnection conexion = null)
		{
			if (conexion == null)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
			}

			string busqueda = "SELECT Id FROM AspNetUsers WHERE GogId=@GogId";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				comando.Parameters.AddWithValue("@GogId", idGog);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							if (lector.GetString(0) != idUsuario)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public static NotificacionSuscripcion UnUsuarioNotificacionesPush(string usuarioId, SqlConnection conexion = null)
		{
			if (conexion == null)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
			}

			string busqueda = "SELECT * FROM usuariosNotificaciones WHERE usuarioId=@usuarioId";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				comando.Parameters.AddWithValue("@usuarioId", usuarioId);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						NotificacionSuscripcion usuario = new NotificacionSuscripcion();
						usuario.UserId = lector.GetString(0);
						usuario.NotificationSubscriptionId = lector.GetInt32(1);
						usuario.Url = lector.GetString(2);
						usuario.P256dh = lector.GetString(3);
						usuario.Auth = lector.GetString(4);
						usuario.UserAgent = lector.GetString(5);

						return usuario;
					}
				}
			}

			return null;
		}

		public static bool UsuarioQuiereNotificacionesPushMinimos(string usuarioId, SqlConnection conexion = null)
		{
			if (conexion == null)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
			}

			string busqueda = "SELECT NotificationPushLows FROM AspNetUsers WHERE Id=@Id";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				comando.Parameters.AddWithValue("@Id", usuarioId);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						if (lector.IsDBNull(0) == false)
						{
							return lector.GetBoolean(0);
						}
					}
				}
			}

			return false;
		}

		public static bool UsuarioNombreRepetido(string nombre, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(nombre) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT Id FROM AspNetUsers WHERE Nickname=@Nickname";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Nickname", nombre);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public static bool PerfilYaUsado(string nombre, SqlConnection conexion = null)
		{
			bool yaUsado = false;

			if (string.IsNullOrEmpty(nombre) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT Id FROM AspNetUsers WHERE ProfileNickname=@ProfileNickname";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@ProfileNickname", nombre);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								yaUsado = true;
							}
						}
					}
				}
			}

			return yaUsado;
		}

		public static Usuario PerfilCargar(string nombre, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(nombre) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT Id, ProfileNickname2, ProfileAvatar, ProfileSteamAccount, ProfileGogAccount, ProfileLastPlayed, ProfileGames, ProfileWishlist FROM AspNetUsers WHERE ProfileNickname=@ProfileNickname AND ProfileShow='true'";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@ProfileNickname", nombre);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							Usuario perfil = new Usuario();

							if (lector.IsDBNull(0) == false)
							{
								perfil.Id = lector.GetString(0);
							}

							if (lector.IsDBNull(1) == false)
							{
								perfil.ProfileNickname2 = lector.GetString(1);
							}

							if (lector.IsDBNull(2) == false)
							{
								perfil.ProfileAvatar = lector.GetString(2);
							}

							if (lector.IsDBNull(3) == false)
							{
								perfil.ProfileSteamAccount = lector.GetBoolean(3);
							}

							if (lector.IsDBNull(4) == false)
							{
								perfil.ProfileGogAccount = lector.GetBoolean(4);
							}

							if (lector.IsDBNull(5) == false)
							{
								perfil.ProfileLastPlayed = lector.GetBoolean(5);
							}

							if (lector.IsDBNull(6) == false)
							{
								perfil.ProfileGames = lector.GetBoolean(6);
							}

							if (lector.IsDBNull(7) == false)
							{
								perfil.ProfileWishlist = lector.GetBoolean(7);
							}

							return perfil;
						}
					}
				}
			}

			return null;
		}

		public static string PerfilSteamCuenta(string id, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(id) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT SteamAccount FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", id);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								return lector.GetString(0);
							}
						}
					}
				}
			}

			return null;
		}

		public static string PerfilGogCuenta(string id, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(id) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT GogAccount FROM AspNetUsers WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", id);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								return lector.GetString(0);
							}
						}
					}
				}
			}

			return null;
		}

		public static int CuantosUsuariosTienenJuego(string juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
			int cuantos = 0;

			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(CONCAT('\"Id\":', (SELECT idSteam FROM juegos WHERE id=@juegoId), ','), SteamGames) > 0";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(CONCAT('\"Id\":', (SELECT idGog FROM juegos WHERE id=@juegoId), ','), GogGames) > 0";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(AmazonGames, ',') WHERE VALUE IN ((SELECT idAmazon FROM juegos WHERE id=@juegoId)))";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EpicGames, ',') WHERE VALUE IN ((SELECT exeEpic FROM juegos WHERE id=@juegoId)))";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(UbisoftGames, ',') WHERE VALUE IN ((SELECT exeUbisoft FROM juegos WHERE id=@juegoId)))";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EaGames, ',') WHERE VALUE IN ((SELECT exeEa FROM juegos WHERE id=@juegoId)))";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@juegoId", juegoId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								cuantos = lector.GetInt32(0);
							}
						}
					}
				}
			}

			return cuantos;
		}

		public static int CuantosUsuariosTienenDeseado(string juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
			int cuantos = 0;

			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = "DECLARE @idSteam nvarchar(256);\r\n\r\nSET @idSteam = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":0}');\r\n\r\nDECLARE @num1 int;\r\nSET @num1 = (SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idSteam, Wishlist) > 0);\r\n\r\nDECLARE @num2 int;\r\nSET @num2 = (SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(SteamWishlist, ',') WHERE VALUE IN (SELECT idSteam FROM juegos WHERE id=@juegoId)));\r\n\r\nSELECT @num1 + @num2";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = "DECLARE @idGOG nvarchar(256);\r\n\r\nSET @idGOG = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":8}');\r\n\r\nDECLARE @num1 int;\r\nSET @num1 = (SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idGOG, Wishlist) > 0);\r\n\r\nDECLARE @num2 int;\r\nSET @num2 = (SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(GogWishlist, ',') WHERE VALUE IN (SELECT idGog FROM juegos WHERE id=@juegoId)));\r\n\r\nSELECT @num1 + @num2";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = "DECLARE @idAmazon nvarchar(256);\r\n\r\nSET @idAmazon = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":9}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idAmazon, Wishlist) > 0;";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = "DECLARE @idEpic nvarchar(256);\r\n\r\nSET @idEpic = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":6}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idEpic, Wishlist) > 0;";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = "DECLARE @idUbisoft nvarchar(256);\r\n\r\nSET @idUbisoft = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":2}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idUbisoft, Wishlist) > 0;";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = "DECLARE @idEA nvarchar(256);\r\n\r\nSET @idEA = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":3}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idEA, Wishlist) > 0;";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@juegoId", juegoId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								cuantos = lector.GetInt32(0);
							}
						}
					}
				}
			}

			return cuantos;
		}

		public static List<string> ListaUsuariosTienenDeseado(int juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
			List<string> usuarios = new List<string>();

			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = @"DECLARE @idSteam nvarchar(256);
SET @idSteam = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":0}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idSteam, Wishlist) > 0
UNION
SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(SteamWishlist, ',') WHERE VALUE IN (SELECT idSteam FROM juegos WHERE id=@juegoId))";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = @"DECLARE @idGOG nvarchar(256);
SET @idGOG = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":8}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idGOG, Wishlist) > 0
UNION
SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(GogWishlist, ',') WHERE VALUE IN (SELECT idGOG FROM juegos WHERE id=@juegoId))";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = @"DECLARE @idAmazon nvarchar(256);
SET @idAmazon = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":9}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idAmazon, Wishlist) > 0";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = @"DECLARE @idEpic nvarchar(256);
SET @idEpic = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":6}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idEpic, Wishlist) > 0";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = @"DECLARE @idUbisoft nvarchar(256);
SET @idUbisoft = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":2}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idUbisoft, Wishlist) > 0";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = @"DECLARE @idEA nvarchar(256);
SET @idEA = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":3}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idEA, Wishlist) > 0";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@juegoId", juegoId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						while (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								usuarios.Add(lector.GetString(0));
							}
						}
					}
				}
			}

			return usuarios;
		}

		public static string Opcion(string usuarioId, string valor, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								return lector.GetString(0);
							}
						}
					}
				}
			}

			return null;
		}

		public static int BundlesOrden(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == false)
			{
				if (conexion == null)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
				else
				{
					if (conexion.State != System.Data.ConnectionState.Open)
					{
						conexion = Herramientas.BaseDatos.Conectar();
					}
				}

				string busqueda = "SELECT BundlesSort FROM AspNetUsers WHERE id=@Id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								return lector.GetInt32(0);
							}
						}
					}
				}
			}

			return 0;
		}
	}
}
