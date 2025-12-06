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
		public static void Comprobacion(bool cambioPrecio, int id, List<JuegoPrecio> ofertasActuales, List<JuegoPrecio> ofertasHistoricas, List<JuegoHistorico> historicos, string slugGOG = null, string idGOG = null, string slugEpic = null, DateTime? ultimaModificacion = null, JuegoAnalisis reseñas = null)
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
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute(sql.ToString(), parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Juego Actualizar " + id, ex, null, false, sql.ToString());
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

		public static void Media(Juego nuevoJuego, Juego juego, SqlConnection conexion = null)
		{
			if (nuevoJuego != null && juego != null)
			{
				if (juego.Imagenes == null)
				{
					juego.Imagenes = new JuegoImagenes();
				}

				if (string.IsNullOrEmpty(juego.Imagenes.Library_600x900) == false)
				{
					bool cambiar = true;

					if (juego.Imagenes.Library_600x900.Contains("i.imgur.com") == true)
					{
						cambiar = false;
					}
					else if (!Regex.IsMatch(juego.Imagenes.Library_600x900, $"{Regex.Escape("library_capsule.")}.*?{Regex.Escape("/" + juego.IdSteam.ToString() + "/")}"))
					{
						cambiar = false;
					}

					if (cambiar == true)
					{
						juego.Imagenes.Library_600x900 = nuevoJuego.Imagenes.Library_600x900;
					}
                }
				else
				{
                    juego.Imagenes.Library_600x900 = nuevoJuego.Imagenes.Library_600x900;
                }

                if (string.IsNullOrEmpty(juego.Imagenes.Library_1920x620) == false)
                {
					bool cambiar = true;

					if (juego.Imagenes.Library_1920x620.Contains("i.imgur.com") == true)
					{
						cambiar = false;
					}
					else if (!Regex.IsMatch(juego.Imagenes.Library_1920x620, $"{Regex.Escape("library_hero.")}.*?{Regex.Escape("/" + juego.IdSteam.ToString() + "/")}"))
					{
						cambiar = false;
					}

					if (cambiar == true)
					{
						juego.Imagenes.Library_1920x620 = nuevoJuego.Imagenes.Library_1920x620;
					}
				}
                else
                {
                    juego.Imagenes.Library_1920x620 = nuevoJuego.Imagenes.Library_1920x620;
                }

                if (string.IsNullOrEmpty(juego.Imagenes.Logo) == false)
                {
					bool cambiar = true;

					if (juego.Imagenes.Logo.Contains("i.imgur.com") == true)
					{
						cambiar = false;
					}
					else if (!Regex.IsMatch(juego.Imagenes.Logo, $"{Regex.Escape("logo.")}.*?{Regex.Escape("/" + juego.IdSteam.ToString() + "/")}"))
					{
						cambiar = false;
					}

					if (cambiar == true)
					{
						juego.Imagenes.Logo = nuevoJuego.Imagenes.Logo;
					}
                }
                else
                {
                    juego.Imagenes.Logo = nuevoJuego.Imagenes.Logo;
                }

				juego.Imagenes.Capsule_231x87 = nuevoJuego.Imagenes.Capsule_231x87;
				juego.Imagenes.Header_460x215 = nuevoJuego.Imagenes.Header_460x215;

				juego.Nombre = nuevoJuego.Nombre;
				
				if (juego.Caracteristicas == null)
				{
					juego.Caracteristicas = new JuegoCaracteristicas();
				}

				juego.Caracteristicas = nuevoJuego.Caracteristicas;
				juego.Deck = nuevoJuego.Deck;
				juego.SteamOS = nuevoJuego.SteamOS;

				juego.Media = nuevoJuego.Media;
				juego.Categorias = nuevoJuego.Categorias;
				juego.Etiquetas = nuevoJuego.Etiquetas;

				if (juego.Idiomas == null)
				{
					juego.Idiomas = new List<JuegoIdioma>();
				}

				if (nuevoJuego.Idiomas?.Count > 0)
				{
					foreach (var nuevoIdioma in nuevoJuego.Idiomas)
					{
						bool existe = false;

						if (juego.Idiomas != null)
						{
							foreach (var viejoIdioma in juego.Idiomas)
							{
								if (viejoIdioma.DRM == nuevoIdioma.DRM && nuevoIdioma.Idioma == viejoIdioma.Idioma)
								{
									existe = true;

									viejoIdioma.Audio = nuevoIdioma.Audio;
									viejoIdioma.Texto = nuevoIdioma.Texto;

									break;
								}
							}
						}

						if (existe == false)
						{
							juego.Idiomas.Add(nuevoIdioma);
						}
					}
				}

				string añadirReseñas = null;

				if (nuevoJuego.Analisis != null)
				{
					if (string.IsNullOrEmpty(nuevoJuego.Analisis.Porcentaje) == false && string.IsNullOrEmpty(nuevoJuego.Analisis.Cantidad) == false)
					{
						añadirReseñas = ", analisis=@analisis";
					}
				}

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

				using (conexion)
				{
					string sqlActualizar = "UPDATE juegos " +
										"SET nombre=@nombre, imagenes=@imagenes, caracteristicas=@caracteristicas, media=@media, nombreCodigo=@nombreCodigo, categorias=@categorias, etiquetas=@etiquetas, fechaSteamAPIComprobacion=@fechaSteamAPIComprobacion, idiomas=@idiomas, deck=@deck, steamOS=@steamOS" + añadirReseñas + " WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
					{
						comando.Parameters.AddWithValue("@id", juego.Id);
						comando.Parameters.AddWithValue("@nombre", juego.Nombre);
						comando.Parameters.AddWithValue("@imagenes", JsonSerializer.Serialize(juego.Imagenes));
						comando.Parameters.AddWithValue("@caracteristicas", JsonSerializer.Serialize(juego.Caracteristicas));
						comando.Parameters.AddWithValue("@media", JsonSerializer.Serialize(juego.Media));
						comando.Parameters.AddWithValue("@nombreCodigo", Herramientas.Buscador.LimpiarNombre(juego.Nombre));
						comando.Parameters.AddWithValue("@categorias", JsonSerializer.Serialize(juego.Categorias));
						comando.Parameters.AddWithValue("@etiquetas", JsonSerializer.Serialize(juego.Etiquetas));
						comando.Parameters.AddWithValue("@fechaSteamAPIComprobacion", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
						comando.Parameters.AddWithValue("@idiomas", JsonSerializer.Serialize(juego.Idiomas));
						comando.Parameters.AddWithValue("@deck", juego.Deck);
						comando.Parameters.AddWithValue("@steamOS", juego.SteamOS);

						if (string.IsNullOrEmpty(añadirReseñas) == false)
						{
							comando.Parameters.AddWithValue("@analisis", JsonSerializer.Serialize(nuevoJuego.Analisis));
						}
						
						try
						{
							comando.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Actualizar Steam API", ex);
						}
					}
				}
			}		
		}

		public static void Tipo(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET tipo=@tipo WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@tipo", juego.Tipo);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void IdSteam(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idSteam=@idSteam WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@idSteam", juego.IdSteam);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void IdGOG(Juego juego, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idGOG=@idGOG WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@idGOG", juego.IdGog);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void SlugGOG(Juego juego, SqlConnection conexion = null)
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
					"SET slugGOG=@slugGOG WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@slugGOG", juego.SlugGOG);

				comando.ExecuteNonQuery();
				try
				{
					
				}
				catch
				{

				}
			}
		}

		public static void SlugEpic(Juego juego, SqlConnection conexion = null)
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
					"SET slugEpic=@slugEpic WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@slugEpic", juego.SlugEpic);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void ExeEpic(Juego juego, SqlConnection conexion = null)
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
					"SET exeEpic=@exeEpic WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@exeEpic", juego.ExeEpic);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void IdXbox(int idJuego, string idXbox, SqlConnection conexion = null)
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
					"SET idXbox=@idXbox WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", idJuego);
				comando.Parameters.AddWithValue("@idXbox", idXbox);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void ExeUbisoft(Juego juego, SqlConnection conexion = null)
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
					"SET exeUbisoft=@exeUbisoft WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", juego.Id);
				comando.Parameters.AddWithValue("@exeUbisoft", juego.ExeUbisoft);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void Deck(Juego juego, SqlConnection conexion = null)
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

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET deck=@deck, deckTokens=@deckTokens, deckComprobacion=@deckComprobacion, steamOS=@steamOS, steamOSTokens=@steamOSTokens WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", juego.Id);
					comando.Parameters.AddWithValue("@deck", juego.Deck);
					comando.Parameters.AddWithValue("@deckTokens", JsonSerializer.Serialize(juego.DeckTokens));
					comando.Parameters.AddWithValue("@deckComprobacion", juego.DeckComprobacion);
					comando.Parameters.AddWithValue("@steamOS", juego.SteamOS);
					comando.Parameters.AddWithValue("@steamOSTokens", JsonSerializer.Serialize(juego.SteamOSTokens));

					comando.ExecuteNonQuery();
					try
					{

					}
					catch
					{

					}
				}
			}
		}

		public static void DeckVacio(Juego juego, SqlConnection conexion = null)
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

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET deckComprobacion=@deckComprobacion WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", juego.Id);
					comando.Parameters.AddWithValue("@deckComprobacion", juego.DeckComprobacion);

					comando.ExecuteNonQuery();
					try
					{

					}
					catch
					{

					}
				}
			}
		}

		public static void Historicos(Juego juego, SqlConnection conexion = null)
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

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET historicos=@historicos WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", juego.Id);
					comando.Parameters.AddWithValue("@historicos", JsonSerializer.Serialize(juego.Historicos));

					comando.ExecuteNonQuery();
					try
					{

					}
					catch
					{

					}
				}
			}
		}

		public static void GalaxyGOG(Juego juego, SqlConnection conexion = null)
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

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET galaxyGOG=@galaxyGOG, idiomas=@idiomas WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", juego.Id);
					comando.Parameters.AddWithValue("@galaxyGOG", JsonSerializer.Serialize(juego.GalaxyGOG));
					comando.Parameters.AddWithValue("@idiomas", JsonSerializer.Serialize(juego.Idiomas));

					comando.ExecuteNonQuery();
					try
					{

					}
					catch
					{

					}
				}
			}
		}

		public static void EpicGames(Juego juego, SqlConnection conexion = null)
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

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET epicGames=@epicGames, idiomas=@idiomas WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", juego.Id);
					comando.Parameters.AddWithValue("@epicGames", JsonSerializer.Serialize(juego.EpicGames));
					comando.Parameters.AddWithValue("@idiomas", JsonSerializer.Serialize(juego.Idiomas));

					comando.ExecuteNonQuery();
					try
					{

					}
					catch
					{

					}
				}
			}
		}

		public static void Xbox(Juego juego, SqlConnection conexion = null)
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

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET xbox=@xbox, idiomas=@idiomas WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", juego.Id);
					comando.Parameters.AddWithValue("@xbox", JsonSerializer.Serialize(juego.Xbox));
					comando.Parameters.AddWithValue("@idiomas", JsonSerializer.Serialize(juego.Idiomas));

					comando.ExecuteNonQuery();
					try
					{

					}
					catch
					{

					}
				}
			}
		}

		public static void CantidadJugadoresSteam(Juego juego)
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute("UPDATE juegos SET cantidadJugadoresSteam=@cantidadJugadoresSteam WHERE id=@id", new
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
		public static void UltimasActualizacioneseInteligenciaArticial(int idJuego, DateTime? fechaSteam, DateTime? fechaGOG, bool inteligenciaArtificial = false, SqlConnection conexion = null)
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

			string añadirFechaSteam = null;

			if (fechaSteam != null)
			{
				añadirFechaSteam = ", ultimaActualizacionSteam=@ultimaActualizacionSteam";
			}

			string añadirFechaGOG = null;

			if (fechaGOG != null)
			{
				añadirFechaGOG = ", ultimaActualizacionGOG=@ultimaActualizacionGOG";
			}

			using (conexion)
			{
				string sqlActualizar = "UPDATE juegos " +
					"SET inteligenciaArtificial=@inteligenciaArtificial, ultimaActualizacion=@ultimaActualizacion" + añadirFechaSteam + añadirFechaGOG + " WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@id", idJuego);
					comando.Parameters.AddWithValue("@inteligenciaArtificial", inteligenciaArtificial);
					comando.Parameters.AddWithValue("@ultimaActualizacion", DateTime.Now);

					if (fechaSteam != null)
					{
						comando.Parameters.AddWithValue("@ultimaActualizacionSteam", fechaSteam);
					}

					if (fechaGOG != null)
					{
						comando.Parameters.AddWithValue("@ultimaActualizacionGOG", fechaGOG);
					}

					try
					{
						comando.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Actualizar SteamCMD " + idJuego.ToString(), ex);
					}
				}
			}
		}
	}
}
