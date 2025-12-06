#nullable disable

using Dapper;

namespace BaseDatos.Pendientes
{
	public static class Buscar
	{
		public static string IDs(string nombre)
		{
			try
			{
				string busqueda1 = "SELECT id FROM juegos WHERE nombre = @nombre OR nombreCodigo = @nombreLimpio";

				var id = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<int?>(busqueda1, new
					{
						nombre,
						nombreLimpio = Herramientas.Buscador.LimpiarNombre(nombre, false)
					}, transaction: sentencia);
				});

				if (id != null)
				{
					return id.Value.ToString();
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Buscar IDs 1", ex);
			}

			try
			{
				string busqueda2 = "SELECT ids FROM juegosIDs WHERE nombre = @nombre";

				var ids = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<string>(busqueda2, new { nombre }, transaction: sentencia);
				});

				if (ids != null)
				{
					return ids;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Buscar IDs 2", ex);
			}

			return "0";
		}

        public static int TiendasCantidad()
        {
			List<string> sentencias = new List<string>();

			foreach (var tienda in Tiendas2.TiendasCargar.GenerarListado())
			{
				if (tienda.Id != "steam")
				{
					string tabla = $"tienda{tienda.Id}";
					sentencias.Add($"SELECT COUNT(*) FROM {tabla} WHERE idJuegos = '0' AND descartado = 'no'");
				}
			}

			if (sentencias.Count == 0)
			{
				return 0;
			}

			string sql = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, sentencias) + " OPTION (MAXDOP 8);";

			try
			{
				var resultados = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<int>(sql, transaction: sentencia).ToList();
				});

				return resultados.Sum();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Tiendas Cantidad", ex);
			}

			return 0;
		}

		public static int SuscripcionCantidad()
		{
			List<string> sentencias = new List<string>();

			foreach (var suscripcion in Suscripciones2.SuscripcionesCargar.GenerarListado())
			{
				if (suscripcion.AdminPendientes)
				{
					string tabla = $"temporal{suscripcion.Id}";
					sentencias.Add($"SELECT COUNT(*) FROM {tabla}");
				}
			}

			if (sentencias.Count == 0)
			{
				return 0;
			}

			string sql = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, sentencias) + " OPTION (MAXDOP 8);";

			try
			{
				var resultados = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<int>(sql, transaction: sentencia).ToList();
				});

				return resultados.Sum();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Suscripcion Cantidad", ex);
			}

			return 0;
		}

		public static int StreamingCantidad()
		{
			List<string> sentencias = new List<string>();

			foreach (var streaming in Streaming2.StreamingCargar.GenerarListado())
			{
				string tabla = $"streaming{streaming.Id}";
				string where = "WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";

				sentencias.Add($"SELECT COUNT(*) FROM {tabla} {where}");
			}

			if (sentencias.Count == 0)
			{
				return 0;
			}

			string sql = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, sentencias) + " OPTION (MAXDOP 8);";

			try
			{
				var resultados = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<int>(sql, transaction: sentencia).ToList();
				});

				return resultados.Sum();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Streaming Cantidad", ex);
			}

			return 0;
		}

        public static int PlataformaCantidad()
        {
			List<string> sentencias = new List<string>();

			foreach (var plataforma in Plataformas2.PlataformasCargar.GenerarListado())
			{
				string tabla = $"temporal{plataforma.Id}juegos";
				sentencias.Add($"SELECT COUNT(*) FROM {tabla}");
			}

			if (sentencias.Count == 0)
			{
				return 0;
			}

			string sql = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, sentencias) + " OPTION (MAXDOP 8);";

			try
			{
				var resultados = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<int>(sql, transaction: sentencia).ToList();
				});

				return resultados.Sum();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Plataforma Cantidad", ex);
			}

			return 0;
		}

		public static List<Pendiente> Tienda(string tiendaId)
        {
			string tabla = $"tienda{tiendaId}";
			string sql = $@"SELECT enlace, nombre, imagen FROM {tabla} WHERE idJuegos = '0' AND descartado = 'no'";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<Pendiente>(sql, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Tienda " + tiendaId, ex);
			}

			return new List<Pendiente>();
		}

        public static List<Pendiente> Suscripcion(Suscripciones2.SuscripcionTipo id)
        {
			string tabla = $"temporal{id}";
			string sql = $@"SELECT enlace, nombre, imagen FROM {tabla}";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<Pendiente>(sql, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Suscripcion " + id.ToString(), ex);
			}

			return new List<Pendiente>();
		}

        public static List<Pendiente> Streaming(Streaming2.StreamingTipo id)
        {
			string tabla = $"streaming{id}";
			string sql = $@"SELECT nombreCodigo as Enlace, nombre FROM {tabla} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";

			if (id == Streaming2.StreamingTipo.Boosteroid)
			{
				sql = $@"SELECT id as Enlace, nombre FROM {tabla} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";
			}

			try
			{
				var filas = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<Pendiente>(sql, transaction: sentencia).ToList();
				});

				List<Pendiente> lista = new List<Pendiente>();

				foreach (var fila in filas)
				{
					lista.Add(new Pendiente
					{
						Enlace = fila.Enlace?.ToString(),
						Nombre = fila.Nombre
					});
				}

				return lista;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Streaming " + id.ToString(), ex);
			}

			return new List<Pendiente>();			
		}

        public static List<Pendiente> Plataforma(Plataformas2.PlataformaTipo id)
        {
			string tabla = $"temporal{id}juegos";
			string sql = $@"SELECT id as Enlace, Nombre FROM {tabla}";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<Pendiente>(sql, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Plataforma " + id.ToString(), ex);
			}

			return new List<Pendiente>();
		}

        public static Pendiente PrimerJuegoTienda(string tiendaId)
		{
			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<Pendiente>($"SELECT TOP 1 * FROM tienda{tiendaId} WHERE idJuegos='0' AND descartado='no'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Tienda Primer Juego " + tiendaId, ex);
			}

			return null;
		}

        public static Pendiente PrimerJuegoSuscripcion(string suscripcionId)
        {
			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<Pendiente>($"SELECT TOP 1 * FROM temporal{suscripcionId}", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Suscripcion Primer Juego " + suscripcionId, ex);
			}

			return null;
		}

        public static Pendiente PrimerJuegoStreaming(string streamingId)
        {
			string sql = $"SELECT TOP 1 nombreCodigo as Enlace, nombre FROM streaming{streamingId} WHERE idJuego IS NULL AND descartado IS NULL";

			if (streamingId == Streaming2.StreamingTipo.Boosteroid.ToString())
			{
				sql = $@"SELECT TOP 1 id as Enlace, nombre FROM streaming{streamingId} WHERE idJuego IS NULL AND descartado IS NULL";
			}

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<Pendiente>(sql, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Streaming Primer Juego " + streamingId, ex);
			}

			return null;
		}

		public static Pendiente PrimerJuegoPlataforma(string plataformaId)
		{
			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<Pendiente>($"SELECT TOP 1 * FROM temporal{plataformaId}juegos", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Plataforma Primer Juego " + plataformaId, ex);
			}

			return null;
		}
	}

	public class Pendiente
	{
		public string Enlace { get; set; }
		public string Nombre { get; set; }
		public string Imagen { get; set; }
	}

	public class PendientesTienda
	{
		public List<Pendiente> Pendientes { get; set; }
		public Tiendas2.Tienda Tienda { get; set; }
	}

	public class PendientesSuscripcion
	{
		public List<Pendiente> Pendientes { get; set; }
		public Suscripciones2.Suscripcion Suscripcion { get; set; }
	}

	public class PendientesStreaming
    {
        public List<Pendiente> Pendientes { get; set; }
		public Streaming2.Streaming Streaming { get; set; }
	}

	public class PendientesPlataforma
	{
		public List<Pendiente> Pendientes { get; set; }
		public Plataformas2.Plataforma Plataforma { get; set; }
	}
}
