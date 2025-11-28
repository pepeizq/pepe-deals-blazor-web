#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Pendientes
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

		public static string IDs(string nombre, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sql1 = @"SELECT id FROM juegos WHERE nombre = @nombre OR nombreCodigo = @nombreLimpio";

			var id = conexion.QueryFirstOrDefault<int?>(sql1, new
			{
				nombre,
				nombreLimpio = Herramientas.Buscador.LimpiarNombre(nombre, false)
			});

			if (id != null)
			{
				return id.Value.ToString();
			}

			string sql2 = @"SELECT ids FROM juegosIDs WHERE nombre = @nombre";

			var ids = conexion.QueryFirstOrDefault<string>(sql2, new { nombre });

			if (ids != null)
			{
				return ids;
			}

			return "0";
		}

        public static int TiendasCantidad(SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

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

			var resultados = conexion.Query<int>(sql).ToList();

			return resultados.Sum();
		}

		public static int SuscripcionCantidad(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			return conexion.Query<int>(sql).Sum();
		}

		public static int StreamingCantidad(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			return conexion.Query<int>(sql).Sum();
		}

        public static int PlataformaCantidad(SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

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

			return conexion.Query<int>(sql).Sum();
		}

		public static List<Pendiente> Tienda(string tiendaId, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string tabla = $"tienda{tiendaId}";
			string sql = $@"SELECT enlace, nombre, imagen FROM {tabla} WHERE idJuegos = '0' AND descartado = 'no'";

			return conexion.Query<Pendiente>(sql).ToList();
		}

        public static List<Pendiente> Suscripcion(Suscripciones2.SuscripcionTipo id, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string tabla = $"temporal{id}";
			string sql = $@"SELECT enlace, nombre, imagen FROM {tabla}";

			return conexion.Query<Pendiente>(sql).ToList();
		}

        public static List<Pendiente> Streaming(Streaming2.StreamingTipo id, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string tabla = $"streaming{id}";
			string sql = $@"SELECT nombreCodigo as Enlace, nombre FROM {tabla} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";

			if (id == Streaming2.StreamingTipo.Boosteroid)
			{
				sql = $@"SELECT id as Enlace, nombre FROM {tabla} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";
			}

			var filas = conexion.Query(sql);

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

        public static List<Pendiente> Plataforma(Plataformas2.PlataformaTipo id, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string tabla = $"temporal{id}juegos";
			string sql = $@"SELECT id as Enlace, Nombre FROM {tabla}";

			return conexion.Query<Pendiente>(sql).ToList();
		}

        public static Pendiente PrimerJuegoTienda(string tiendaId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sql = $"SELECT TOP 1 * FROM tienda{tiendaId} WHERE idJuegos='0' AND descartado='no'";

			return conexion.QueryFirstOrDefault<Pendiente>(sql);
		}

        public static Pendiente PrimerJuegoSuscripcion(string suscripcionId, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string sql = $"SELECT TOP 1 * FROM temporal{suscripcionId}";

			return conexion.QueryFirstOrDefault<Pendiente>(sql);
		}

        public static Pendiente PrimerJuegoStreaming(string streamingId, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string sql = $"SELECT TOP 1 * FROM streaming{streamingId} WHERE idJuego IS NULL AND descartado IS NULL";

			var fila = conexion.QueryFirstOrDefault<dynamic>(sql);

			if (fila == null)
			{
				return null;
			}

			var pendiente = new Pendiente();

			try 
			{ 
				pendiente.Enlace = fila.Enlace; 
			}
			catch
			{
				try 
				{ 
					pendiente.Enlace = fila.Enlace.ToString(); 
				}
				catch 
				{ 
					pendiente.Enlace = null; 
				}
			}

			try 
			{ 
				pendiente.Nombre = fila.Nombre; 
			}
			catch 
			{ 
				pendiente.Nombre = null; 
			}

			pendiente.Imagen = null;

			return pendiente;
		}

		public static Pendiente PrimerJuegoPlataforma(string plataformaId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sql = $"SELECT TOP 1 * FROM temporal{plataformaId}juegos";

			var fila = conexion.QueryFirstOrDefault<dynamic>(sql);

			if (fila == null) 
			{ 
				return null; 
			}

			var pendiente = new Pendiente();

			if (fila.Nombre != null)
			{
				pendiente.Enlace = fila.Enlace;
				pendiente.Nombre = fila.Nombre;
			}
			else
			{
				pendiente.Enlace = fila.Enlace;
				pendiente.Nombre = fila.Enlace;
			}

			pendiente.Imagen = "vacio";

			return pendiente;
		}
	}

	public class Pendiente
	{
		public string Enlace;
		public string Nombre;
		public string Imagen;
	}

	public class PendientesTienda
	{
		public List<Pendiente> Pendientes;
		public Tiendas2.Tienda Tienda;
	}

	public class PendientesSuscripcion
	{
		public List<Pendiente> Pendientes;
		public Suscripciones2.Suscripcion Suscripcion;
	}

	public class PendientesStreaming
    {
        public List<Pendiente> Pendientes;
        public Streaming2.Streaming Streaming;
    }

	public class PendientesPlataforma
	{
		public List<Pendiente> Pendientes;
		public Plataformas2.Plataforma Plataforma;
	}
}
