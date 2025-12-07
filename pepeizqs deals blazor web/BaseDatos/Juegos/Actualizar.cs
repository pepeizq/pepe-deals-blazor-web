#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BaseDatos.Juegos
{
	public static class Actualizar
	{
		public static async void Comprobacion(bool cambioPrecio, int id, List<JuegoPrecio> ofertasActuales, List<JuegoPrecio> ofertasHistoricas, List<JuegoHistorico> historicos, string slugGOG = null, string idGOG = null, string slugEpic = null, DateTime? ultimaModificacion = null, JuegoAnalisis reseñas = null)
		{
			var sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");
			sql.Append("precioActualesTiendas=@precioActualesTiendas, ");
			sql.Append("precioMinimosHistoricos=@precioMinimosHistoricos ");

			if (cambioPrecio == true)
			{
				sql.Append(", historicos=@historicos ");
			}

			if (string.IsNullOrEmpty(slugGOG) == false)
			{
				sql.Append(", idGog=@idGog, slugGOG=@slugGOG ");
			}

			if (string.IsNullOrEmpty(slugEpic) == false)
			{
				sql.Append(", slugEpic=@slugEpic ");
			}

			if (ultimaModificacion != null)
			{
				sql.Append(", ultimaModificacion=@ultimaModificacion ");
			}

			sql.Append("WHERE id=@id");

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add("@id", id);
			parametros.Add("@precioActualesTiendas", JsonSerializer.Serialize(ofertasActuales));
			parametros.Add("@precioMinimosHistoricos", JsonSerializer.Serialize(ofertasHistoricas));

			if (cambioPrecio == true)
			{
				parametros.Add("@historicos", JsonSerializer.Serialize(historicos));
			}

			if (string.IsNullOrEmpty(slugGOG) == false)
			{
				parametros.Add("@idGog", idGOG);
				parametros.Add("@slugGOG", slugGOG);
			}

			if (string.IsNullOrEmpty(slugEpic) == false)
			{
				parametros.Add("@slugEpic", slugEpic);
			}

			if (ultimaModificacion != null)
			{
				parametros.Add("@ultimaModificacion", ultimaModificacion);
			}

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sql.ToString(), parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Juego Actualizar " + id, ex,false, sql.ToString());
			}
		}

		public static void Imagenes(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET imagenes=@imagenes WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@imagenes", JsonSerializer.Serialize(juego.Imagenes));

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}

			string sqlActualizar2 = "UPDATE seccionMinimos " +
					"SET imagenes=@imagenes WHERE idMaestra=@id";

			using (SqlCommand comando2 = new SqlCommand(sqlActualizar2, conexion))
			{
				comando2.Parameters.AddWithValue("@id", juego.Id);
				comando2.Parameters.AddWithValue("@imagenes", JsonSerializer.Serialize(juego.Imagenes));

				try
				{
					comando2.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void PreciosActuales(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET precioActualesTiendas=@precioActualesTiendas WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@precioActualesTiendas", JsonSerializer.Serialize(juego.PrecioActualesTiendas));

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void PreciosHistoricos(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET precioMinimosHistoricos=@precioMinimosHistoricos WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@precioMinimosHistoricos", JsonSerializer.Serialize(juego.PrecioMinimosHistoricos));

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

        public static void PreciosHistoricos(int id, List<JuegoPrecio> historicos, SqlConnection conexion = null)
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

            string sqlActualizar = "UPDATE juegos " +
                    "SET precioMinimosHistoricos=@precioMinimosHistoricos WHERE id=@id";

            using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
            {
                comando.Parameters.AddWithValue("@id", id);
                comando.Parameters.AddWithValue("@precioMinimosHistoricos", JsonSerializer.Serialize(historicos));

                try
                {
                    comando.ExecuteNonQuery();
                }
                catch
                {

                }
            }
        }

        public static void Bundles(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET bundles=@bundles WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);

				if (juego.Bundles != null)
				{
					comando.Parameters.AddWithValue("@bundles", JsonSerializer.Serialize(juego.Bundles));
				}
				else
				{
					comando.Parameters.AddWithValue("@bundles", "null");
				}

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void Gratis(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET gratis=@gratis WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);

				if (juego.Gratis != null)
				{
					comando.Parameters.AddWithValue("@gratis", JsonSerializer.Serialize(juego.Gratis));
				}
				else
				{
					comando.Parameters.AddWithValue("@gratis", "null");
				}

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void Suscripciones(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET suscripciones=@suscripciones WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@suscripciones", JsonSerializer.Serialize(juego.Suscripciones));

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}

			string sqlActualizar2 = "UPDATE seccionMinimos " +
					"SET suscripciones=@suscripciones WHERE idMaestra=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar2, conexion))
			{
				comando.Parameters.AddWithValue("@idMaestra", juego.Id);
				comando.Parameters.AddWithValue("@suscripciones", JsonSerializer.Serialize(juego.Suscripciones));

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{
				
				}
			}
		}

		public static void DlcMaestro(Juego juego, SqlConnection conexion = null)
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

            string sqlActualizar = "UPDATE juegos " +
					"SET maestro=@maestro WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@maestro", juego.Maestro);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void FreeToPlay(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET freeToPlay=@freeToPlay WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@freeToPlay", juego.FreeToPlay);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void MayorEdad(Juego juego, SqlConnection conexion = null)
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

			string sqlActualizar = "UPDATE juegos " +
					"SET mayorEdad=@mayorEdad WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@mayorEdad", juego.MayorEdad);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}

			bool actualizar = false;
			string buscarMinimos = "SELECT * FROM seccionMinimos WHERE idMaestra=@idMaestra";

			using (SqlCommand comando2 = new SqlCommand(buscarMinimos, conexion))
			{
				comando2.Parameters.AddWithValue("@idMaestra", juego.Id);

				using (SqlDataReader lector = comando2.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						actualizar = true;
					}
				}
			}

			if (actualizar == true)
			{
				string actualizarMinimos = "UPDATE seccionMinimos " +
					"SET mayorEdad=@mayorEdad WHERE idMaestra=@idMaestra";

				using (SqlCommand comando3 = new SqlCommand(actualizarMinimos, conexion))
				{
					comando3.Parameters.AddWithValue("@idMaestra", juego.Id);
					comando3.Parameters.AddWithValue("@mayorEdad", juego.MayorEdad);

					try
					{
						comando3.ExecuteNonQuery();
					}
					catch
					{

					}
				}
			}
		}

		public static void OcultarPortada(Juego juego, SqlConnection conexion = null)
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

			string sqlActualizar = "UPDATE juegos " +
					"SET ocultarPortada=@ocultarPortada WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@ocultarPortada", juego.OcultarPortada);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}

			bool actualizar = false;
			string buscarMinimos = "SELECT * FROM seccionMinimos WHERE idMaestra=@idMaestra";

			using (SqlCommand comando2 = new SqlCommand(buscarMinimos, conexion))
			{
				comando2.Parameters.AddWithValue("@idMaestra", juego.Id);

				using (SqlDataReader lector = comando2.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						actualizar = true;
					}
				}
			}

			if (actualizar == true)
			{
				string actualizarMinimos = "UPDATE seccionMinimos " +
					"SET ocultarPortada=@ocultarPortada WHERE idMaestra=@idMaestra";

				using (SqlCommand comando3 = new SqlCommand(actualizarMinimos, conexion))
				{
					comando3.Parameters.AddWithValue("@idMaestra", juego.Id);
					comando3.Parameters.AddWithValue("@ocultarPortada", juego.OcultarPortada);

					try
					{
						comando3.ExecuteNonQuery();
					}
					catch
					{

					}
				}
			}
		}

		public static async Task Media(Juego nuevoJuego, Juego juego)
		{
			if (nuevoJuego == null || juego == null)
			{
				return;
			}

			if (juego.Imagenes == null)
			{ 
				juego.Imagenes = new JuegoImagenes();
			}

			juego.Imagenes.Library_600x900 = ProcesarImagen(juego.Imagenes.Library_600x900,
						   nuevoJuego.Imagenes.Library_600x900,
						   juego.IdSteam,
						   "library_capsule.");

			juego.Imagenes.Library_1920x620 = ProcesarImagen(juego.Imagenes.Library_1920x620,
						   nuevoJuego.Imagenes.Library_1920x620,
						   juego.IdSteam,
						   "library_hero.");

			juego.Imagenes.Logo = ProcesarImagen(juego.Imagenes.Logo,
						   nuevoJuego.Imagenes.Logo,
						   juego.IdSteam,
						   "logo.");

			juego.Imagenes.Capsule_231x87 = nuevoJuego.Imagenes.Capsule_231x87;
			juego.Imagenes.Header_460x215 = nuevoJuego.Imagenes.Header_460x215;

			juego.Nombre = nuevoJuego.Nombre;
			juego.Caracteristicas = nuevoJuego.Caracteristicas ?? new JuegoCaracteristicas();
			juego.Deck = nuevoJuego.Deck;
			juego.SteamOS = nuevoJuego.SteamOS;
			juego.Media = nuevoJuego.Media;
			juego.Categorias = nuevoJuego.Categorias;
			juego.Etiquetas = nuevoJuego.Etiquetas;

			if (juego.Idiomas == null)
			{
				juego.Idiomas = new List<JuegoIdioma>();
			}

			if (nuevoJuego.Idiomas != null)
			{
				foreach (var nuevo in nuevoJuego.Idiomas)
				{
					var existente = juego.Idiomas.FirstOrDefault(x => x.DRM == nuevo.DRM && x.Idioma == nuevo.Idioma);

					if (existente != null)
					{
						existente.Audio = nuevo.Audio;
						existente.Texto = nuevo.Texto;
					}
					else
					{
						juego.Idiomas.Add(nuevo);
					}
				}
			}

			string columnasAnalisis = "";
			object paramReseñas = null;

			if (string.IsNullOrEmpty(nuevoJuego.Analisis?.Porcentaje) == false &&
				string.IsNullOrEmpty(nuevoJuego.Analisis?.Cantidad) == false)
			{
				columnasAnalisis = ", analisis=@analisis";
				paramReseñas = JsonSerializer.Serialize(nuevoJuego.Analisis);
			}

			string sql = $@"
				UPDATE juegos SET
					nombre=@nombre,
					imagenes=@imagenes,
					caracteristicas=@caracteristicas,
					media=@media,
					nombreCodigo=@nombreCodigo,
					categorias=@categorias,
					etiquetas=@etiquetas,
					fechaSteamAPIComprobacion=@fechaSteamAPIComprobacion,
					idiomas=@idiomas,
					deck=@deck,
					steamOS=@steamOS
					{columnasAnalisis}
				WHERE id=@id;";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sql, new
					{
						id = juego.Id,
						nombre = juego.Nombre,
						imagenes = JsonSerializer.Serialize(juego.Imagenes),
						caracteristicas = JsonSerializer.Serialize(juego.Caracteristicas),
						media = JsonSerializer.Serialize(juego.Media),
						nombreCodigo = Herramientas.Buscador.LimpiarNombre(juego.Nombre),
						categorias = JsonSerializer.Serialize(juego.Categorias),
						etiquetas = JsonSerializer.Serialize(juego.Etiquetas),
						fechaSteamAPIComprobacion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"),
						idiomas = JsonSerializer.Serialize(juego.Idiomas),
						deck = juego.Deck,
						steamOS = juego.SteamOS,
						analisis = paramReseñas
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Steam API", ex);
			}
		}

		private static string ProcesarImagen(string vieja, string nueva, long idSteam, string patron)
		{
			if (string.IsNullOrEmpty(vieja) == false)
			{
				bool cambiar = true;

				if (vieja.Contains("i.imgur.com") == true)
				{
					cambiar = false;
				}
				else if (!Regex.IsMatch(vieja, $"{Regex.Escape(patron)}.*?/{idSteam}/"))
				{
					cambiar = false;
				}

				if (cambiar == true)
				{ 
					vieja = nueva; 
				}
			}
			else
			{
				vieja = nueva;
			}

			return vieja;
		}

		public static async Task Tipo(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET tipo=@tipo WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						tipo = juego.Tipo
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Tipo " + juego.Id.ToString(), ex);
			}
		}

		public static async Task IdSteam(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idSteam=@idSteam WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						idSteam = juego.IdSteam
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Id Steam " + juego.Id.ToString(), ex);
			}
		}

		public static async Task IdGOG(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idGOG=@idGOG WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						idGOG = juego.IdGog
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Id GOG " + juego.Id.ToString(), ex);
			}
		}

		public static async Task SlugGOG(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET slugGOG=@slugGOG WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						slugGOG = juego.SlugGOG
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Slug GOG " + juego.Id.ToString(), ex);
			}
		}

		public static async Task SlugEpic(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET slugEpic=@slugEpic WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						slugEpic = juego.SlugEpic
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Slug Epic " + juego.Id.ToString(), ex);
			}
		}

		public static async Task ExeEpic(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET exeEpic=@exeEpic WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						exeEpic = juego.ExeEpic
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Exe Epic " + juego.Id.ToString(), ex);
			}
		}

		public static async Task IdXbox(int idJuego, string idXbox)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idXbox=@idXbox WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = idJuego,
						exeUbisoft = idXbox
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Id Xbox " + idJuego.ToString(), ex);
			}
		}

		public static async Task ExeUbisoft(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET exeUbisoft=@exeUbisoft WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						exeUbisoft = juego.ExeUbisoft
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Exe Ubisoft " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Deck(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET deck=@deck, deckTokens=@deckTokens, deckComprobacion=@deckComprobacion, steamOS=@steamOS, steamOSTokens=@steamOSTokens WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						deck = juego.Deck,
						deckTokens = JsonSerializer.Serialize(juego.DeckTokens),
						deckComprobacion = juego.DeckComprobacion,
						steamOS = juego.SteamOS,
						steamOSTokens = JsonSerializer.Serialize(juego.SteamOSTokens)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Deck " + juego.Id.ToString(), ex);
			}
		}

		public static async Task DeckVacio(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET deckComprobacion=@deckComprobacion WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				deckComprobacion = juego.DeckComprobacion
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Deck Comprobacion " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Historicos(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET historicos=@historicos WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				historicos = JsonSerializer.Serialize(juego.Historicos)
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Historicos " + juego.Id.ToString(), ex);
			}
		}

		public static async Task GalaxyGOG(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET galaxyGOG=@galaxyGOG, idiomas=@idiomas WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				galaxyGOG = JsonSerializer.Serialize(juego.GalaxyGOG),
				idiomas = JsonSerializer.Serialize(juego.Idiomas)
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar GOG Galaxy " + juego.Id.ToString(), ex);
			}
		}

		public static async Task EpicGames(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET epicGames=@epicGames, idiomas=@idiomas WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				epicGames = JsonSerializer.Serialize(juego.EpicGames),
				idiomas = JsonSerializer.Serialize(juego.Idiomas)
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Epic Games " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Xbox(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET xbox=@xbox, idiomas=@idiomas WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				xbox = JsonSerializer.Serialize(juego.Xbox),
				idiomas = JsonSerializer.Serialize(juego.Idiomas)
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Xbox " + juego.Id.ToString(), ex);
			}
		}

		public static async Task CantidadJugadoresSteam(Juego juego)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE juegos SET cantidadJugadoresSteam=@cantidadJugadoresSteam WHERE id=@id", new
					{
						id = juego.Id,
						cantidadJugadoresSteam = JsonSerializer.Serialize(juego.CantidadJugadores)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Cantidad Jugadores " + juego.Id.ToString(), ex);
			}
		}

		public static async void UltimasActualizacioneseInteligenciaArticial(int idJuego, DateTime? fechaSteam, DateTime? fechaGOG, bool inteligenciaArtificial = false, SqlConnection conexion = null)
		{
			var sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");
			sql.Append("inteligenciaArtificial = @inteligenciaArtificial, ");
			sql.Append("ultimaActualizacion = @ultimaActualizacion ");

			if (fechaSteam != null)
			{
				sql.Append(", ultimaActualizacionSteam = @ultimaActualizacionSteam ");
			}

			if (fechaGOG != null)
			{
				sql.Append(", ultimaActualizacionGOG = @ultimaActualizacionGOG ");
			}

			sql.Append("WHERE id = @id;");

			var parametros = new
			{
				id = idJuego,
				inteligenciaArtificial,
				ultimaActualizacion = DateTime.Now,
				ultimaActualizacionSteam = fechaSteam,
				ultimaActualizacionGOG = fechaGOG
			};

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sql.ToString(), parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar SteamCMD " + idJuego.ToString(), ex);
			}
		}
	}
}
