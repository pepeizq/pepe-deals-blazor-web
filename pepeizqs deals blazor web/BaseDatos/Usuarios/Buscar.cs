#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using Microsoft.Data.SqlClient;
using pepeizqs_deals_web.Data;

namespace BaseDatos.Usuarios
{
	public static class Buscar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static bool RolDios(string id, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return false;
			}

			conexion = CogerConexion(conexion);

			string sql = "SELECT Role FROM AspNetUsers WHERE Id = @Id";

			try
			{
				string rol = conexion.QueryFirstOrDefault<string>(sql, new { Id = id });

				if (string.IsNullOrEmpty(rol) == false)
				{
					if (rol == "God")
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Rol Dios", ex);
			}
			
			return false;
		}

		public static string IdiomaSobreescribir(string usuarioId, SqlConnection conexion = null)
		{
			string idioma = "en";

			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return idioma;
			}

			conexion = CogerConexion(conexion);

			string sql = "SELECT LanguageOverride FROM AspNetUsers WHERE Id = @Id";

			try
			{
				string idiomaBD = conexion.QueryFirstOrDefault<string>(sql, new { Id = usuarioId });

				if (string.IsNullOrEmpty(idiomaBD) == false)
				{
					idioma = idiomaBD;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Idioma Sobreescribir", ex);
			}

			return idioma;
		}

		public static string IdiomaJuegos(string usuarioId, SqlConnection conexion = null)
		{
			string idioma = "en";

			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return idioma;
			}

			conexion = CogerConexion(conexion);

			string sql = "SELECT LanguageGames FROM AspNetUsers WHERE Id = @Id";

			try
			{
				string idiomaBD = conexion.QueryFirstOrDefault<string>(sql, new { Id = usuarioId });

				if (string.IsNullOrEmpty(idiomaBD) == false)
				{
					idioma = idiomaBD;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Idioma Juegos", ex);
			}

			return idioma;
		}

		public static Usuario JuegosTiene(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = @"
				SELECT 
					SteamGames, 
					GogGames, 
					AmazonGames, 
					EpicGames, 
					UbisoftGames, 
					EaGames
				FROM AspNetUsers 
				WHERE Id = @Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Juegos Tiene", ex);
			}

			return null;
		}

		public static DateTime? FechaPatreon(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT PatreonLastCheck FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return conexion.QueryFirstOrDefault<DateTime?>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Fecha Patreon", ex);
			}

			return null;
		}

		public static Usuario DeseadosTiene(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{ 
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = @"
				SELECT 
					SteamWishlist,
					Wishlist,
					GogWishlist,
					WishlistData
				FROM AspNetUsers
				WHERE Id = @Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Deseados Tiene", ex);
			}
				
			return null;
		}

		public static Usuario OpcionesCabecera(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT Avatar, Email, Nickname, PatreonCoins FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Cabecera", ex);
			}

			return null;
		}

		public static Usuario OpcionesPortada(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT IndexOption1, IndexOption2, IndexDRMs, IndexCategories, ForumIndex FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Portada", ex);
			}

			return null;
		}

		public static Usuario OpcionesDeseados(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT WishlistSort, WishlistOption3, WishlistOption4, WishlistPosition, WishlistData FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Deseados", ex);
			}

			return null;
		}

		public static Usuario OpcionesMinimos(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT HistoricalLowsOption1, HistoricalLowsOption4, HistoricalLowsOption2, HistoricalLowsOption3, HistoricalLowsDRMs, HistoricalLowsStores, HistoricalLowsCategories, HistoricalLowsSteamDeck, HistoricalLowsSort, HistoricalLowsRelease, HistoricalLowsAI FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Deseados", ex);
			}

			return null;
		}

		public static Usuario OpcionesCuenta(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT EmailConfirmed, PatreonCoins, SteamAccountLastCheck, GogAccountLastCheck, AmazonLastImport, EpicGamesLastImport, UbisoftLastImport, EaLastImport FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Cuenta", ex);
			}

			return null;
		}

		public static List<Usuario> UsuariosNotificacionesCorreo(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT Id, NotificationBundles, NotificationFree, NotificationSubscriptions, NotificationOthers, NotificationWeb, NotificationDelisted, Email, Language FROM AspNetUsers";

			try
			{
				List<Usuario> usuarios = conexion.Query<Usuario>(busqueda).ToList();

				return usuarios
				   .Where(usuario =>
					   usuario.NotificationBundles != null ||
					   usuario.NotificationFree != null ||
					   usuario.NotificationSubscriptions != null ||
					   usuario.NotificationOthers != null ||
					   usuario.NotificationWeb != null ||
					   usuario.NotificationDelisted != null ||
					   string.IsNullOrEmpty(usuario.Email) == false ||
					   string.IsNullOrEmpty(usuario.Language) == false
				   ).ToList();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Notificaciones Correo", ex);
			}

			return null;
		}

		public static bool CorreoYaUsado(string correo, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(correo) == true)
			{
				return false;
			}

			conexion = CogerConexion(conexion);

			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email=@Email";

			try
			{
				int resultados = conexion.ExecuteScalar<int>(busqueda, new { Email = correo });

				return resultados > 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Correo Ya Usado", ex);
			}

			return false;
		}

		public static bool UsuarioTieneJuego(string usuarioId, int juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return false;
			}

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
				conexion = CogerConexion(conexion);

				try
				{
					string resultado = conexion.QueryFirstOrDefault<string>(
						busqueda,
						new { id = usuarioId, juegoId }
					);

					return resultado != null;
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Tiene Juego", ex);
				}
			}

			return false;
		}

		public static string UsuarioQuiereCorreos(string usuarioId, string seccion, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT " + seccion + " AS Notificaciones, EmailConfirmed, Email FROM AspNetUsers WHERE Id=@Id";

				var datos = conexion.QueryFirstOrDefault(busqueda, new { Id = usuarioId });

				if (datos == null)
				{
					return null;
				}

				bool quiereNotificaciones = datos.Notificaciones ?? false;
				bool correoConfirmado = datos.EmailConfirmed ?? false;

				if (quiereNotificaciones == true && correoConfirmado == true)
				{
					return datos.Email;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Quiere Correos", ex);
			}

			return null;
		}

		public static bool CuentaSteamUsada(string id64Steam, string idUsuario, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE SteamId=@SteamId";

				var ids = conexion.Query<string>(busqueda, new { SteamId = id64Steam });

				foreach (var id in ids)
				{
					if (id != idUsuario)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Cuenta Steam Usada", ex);
			}

			return false;
		}

		public static bool CuentaGogUsada(string idGog, string idUsuario, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE GogId=@GogId";

				var ids = conexion.Query<string>(busqueda, new { GogId = idGog });

				foreach (var id in ids)
				{
					if (id != idUsuario)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Cuenta GOG Usada", ex);
			}

			return false;
		}

		public static NotificacionSuscripcion UnUsuarioNotificacionesPush(string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT * FROM usuariosNotificaciones WHERE usuarioId=@usuarioId";

				return conexion.QueryFirstOrDefault<NotificacionSuscripcion>(busqueda, new { usuarioId });

			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Notificaciones Push", ex);
			}

			return null;
		}

		public static bool UsuarioNombreRepetido(string nombre, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return false;
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE Nickname=@Nickname";

				var id = conexion.QueryFirstOrDefault<string>(busqueda, new { Nickname = nombre });

				return id != null;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Nombre Repetido", ex);
			}

			return false;
		}

		public static bool PerfilYaUsado(string nombre, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return false;
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE ProfileNickname=@ProfileNickname";

				var id = conexion.QueryFirstOrDefault<string>(busqueda, new { ProfileNickname = nombre });

				return id != null;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil Ya Usado", ex);
			}

			return false;
		}

		public static Usuario PerfilCargar(string nombre, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT Id, ProfileNickname2, ProfileAvatar, ProfileSteamAccount, ProfileGogAccount, ProfileLastPlayed, ProfileGames, ProfileWishlist FROM AspNetUsers WHERE ProfileNickname=@ProfileNickname AND ProfileShow='true'";

				return conexion.QueryFirstOrDefault<Usuario>(busqueda, new { ProfileNickname = nombre });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil Cargar", ex);
			}

			return null;
		}

		public static string PerfilSteamCuenta(string id, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT SteamAccount FROM AspNetUsers WHERE Id=@Id";

				return conexion.QueryFirstOrDefault<string>(busqueda, new { Id = id });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil Steam Cuenta", ex);
			}

			return null;
		}

		public static string PerfilGogCuenta(string id, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT GogAccount FROM AspNetUsers WHERE Id=@Id";

				return conexion.QueryFirstOrDefault<string>(busqueda, new { Id = id });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil GOG Cuenta", ex);
			}

			return null;
		}

		public static int CuantosUsuariosTienenJuego(string juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
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
				conexion = CogerConexion(conexion);

				try
				{
					return conexion.ExecuteScalar<int>(busqueda, new { juegoId });
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Cuantos Tienen Juego", ex);
				}
			}

			return 0;
		}

		public static int CuantosUsuariosTienenDeseado(string juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
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
				conexion = CogerConexion(conexion);

				try
				{
					return conexion.ExecuteScalar<int>(busqueda, new { juegoId });
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Cuantos Desean Juego", ex);
				}
			}

			return 0;
		}

		public static List<string> ListaUsuariosTienenDeseado(int juegoId, JuegoDRM drm, SqlConnection conexion = null)
		{
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
				conexion = CogerConexion(conexion);

				try
				{
					return conexion.Query<string>(busqueda, new { juegoId }).ToList();
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Lista Tienen Deseado", ex);
				}
			}

			return new List<string>();
		}

		public static string Opcion(string usuarioId, string valor, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true && string.IsNullOrEmpty(valor) == true)
			{
				return null;
			}
				
			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				conexion = CogerConexion(conexion);

				try
				{
					string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id";

					return conexion.QueryFirstOrDefault<string>(busqueda, new { Id = usuarioId });
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Opcion", ex);
				}
			}

			return null;
		}

		public static int BundlesOrden(string usuarioId, SqlConnection conexion = null)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return 0; 
			}

			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT BundlesSort FROM AspNetUsers WHERE id=@Id";

				return conexion.QueryFirstOrDefault<int>(busqueda, new { Id = usuarioId });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Bundles Orden", ex);
			}

			return 0;
		}
	}
}
