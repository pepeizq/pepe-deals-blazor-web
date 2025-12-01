#nullable disable

using Dapper;
using Herramientas;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace BaseDatos.Sitemaps
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

		public static int Cantidad(string tabla, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			return conexion.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tabla}");
		}

		public static List<string> Juegos(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT id, nombre, ultimaModificacion FROM juegos WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				int id = fila.id;
				string nombre = fila.nombre;
				DateTime? fecha = fila.ultimaModificacion;

				if (id > 0 && string.IsNullOrEmpty(nombre) == false)
				{
					string texto = "<url>" + Environment.NewLine +
						 "<loc>https://" + dominio + "/game/" + id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(nombre) + "/</loc>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<lastmod>" + fecha.Value.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine;
					}

					texto = texto + "</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}

		public static List<string> Bundles(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT id, nombre, fechaEmpieza FROM bundles WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				int id = fila.id;
				string nombre = fila.nombre;
				DateTime? fecha = fila.fechaEmpieza != null ? DateTime.Parse(fila.fechaEmpieza.ToString(), CultureInfo.InvariantCulture) : (DateTime?)null;

				if (id > 0 && string.IsNullOrEmpty(nombre) == false)
				{
					string texto = "<url>" + Environment.NewLine +
						 "<loc>https://" + dominio + "/bundle/" + id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(nombre) + "/</loc>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<lastmod>" + fecha.Value.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine;
					}

					texto = texto + "</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}

		public static List<string> Gratis(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT juegoId, nombre, fechaEmpieza FROM gratis WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				int id = fila.juegoId;
				string nombre = fila.nombre;
				DateTime? fecha = fila.fechaEmpieza != null ? DateTime.Parse(fila.fechaEmpieza.ToString(), CultureInfo.InvariantCulture) : (DateTime?)null;

				if (id > 0 && string.IsNullOrEmpty(nombre) == false)
				{
					string texto = "<url>" + Environment.NewLine +
						 "<loc>https://" + dominio + "/game/" + id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(nombre) + "/</loc>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<lastmod>" + fecha.Value.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine;
					}

					texto = texto + "</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}

		public static List<string> Suscripciones(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT juegoId, nombre, fechaEmpieza FROM suscripciones WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				int id = fila.juegoId;
				string nombre = fila.nombre;
				DateTime? fecha = fila.fechaEmpieza != null ? DateTime.Parse(fila.fechaEmpieza.ToString(), CultureInfo.InvariantCulture) : (DateTime?)null;

				if (id > 0 && string.IsNullOrEmpty(nombre) == false)
				{
					string texto = "<url>" + Environment.NewLine +
						 "<loc>https://" + dominio + "/game/" + id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(nombre) + "/</loc>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<lastmod>" + fecha.Value.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine;
					}

					texto = texto + "</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}

		public static List<string> NoticiasIngles(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT id, tituloEn, fechaEmpieza FROM noticias WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				int id = fila.id;
				string tituloEn = fila.tituloEn;
				DateTime? fecha = fila.fechaEmpieza != null ? (DateTime?)fila.fechaEmpieza : null;

				if (id > 0 && string.IsNullOrEmpty(tituloEn) == false)
				{
					tituloEn = tituloEn.Replace("&", "&amp;");

					string texto = "<url>" + Environment.NewLine +
						"<loc>https://" + dominio + "/news/" + id.ToString() + "/" + EnlaceAdaptador.Nombre(tituloEn) + "/</loc>" + Environment.NewLine +
						"<news:news>" + Environment.NewLine +
						"<news:publication>" + Environment.NewLine +
						"<news:name>pepe's deals</news:name>" + Environment.NewLine +
						"<news:language>en</news:language>" + Environment.NewLine +
						"</news:publication>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<news:publication_date>" + fecha.Value.ToString("yyyy-MM-dd") + "</news:publication_date>" + Environment.NewLine;
					}

					texto = texto + "<news:title>" + tituloEn + "</news:title>" + Environment.NewLine +
						"</news:news>" + Environment.NewLine +
						"</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}

		public static List<string> NoticiasEspañol(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT id, tituloEs, fechaEmpieza FROM noticias WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				int id = fila.id;
				string tituloEs = fila.tituloEs;
				DateTime? fecha = fila.fechaEmpieza != null ? (DateTime?)fila.fechaEmpieza : null;

				if (id > 0 && string.IsNullOrEmpty(tituloEs) == false)
				{
					tituloEs = tituloEs.Replace("&", "&amp;");

					string texto = "<url>" + Environment.NewLine +
						"<loc>https://" + dominio + "/news/" + id.ToString() + "/" + EnlaceAdaptador.Nombre(tituloEs) + "/</loc>" + Environment.NewLine +
						"<news:news>" + Environment.NewLine +
						"<news:publication>" + Environment.NewLine +
						"<news:name>pepe's deals</news:name>" + Environment.NewLine +
						"<news:language>es</news:language>" + Environment.NewLine +
						"</news:publication>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<news:publication_date>" + fecha.Value.ToString("yyyy-MM-dd") + "</news:publication_date>" + Environment.NewLine;
					}

					texto = texto + "<news:title>" + tituloEs + "</news:title>" + Environment.NewLine +
						"</news:news>" + Environment.NewLine +
						"</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}

		public static List<string> Curators(string dominio, int id1, int id2, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string buscar = "SELECT slug, fecha FROM curators WHERE id > @id1 AND id < @id2";

			var resultados = conexion.Query(buscar, new { Id1 = id1, Id2 = id2 });

			List<string> lineas = new List<string>();

			foreach (var fila in resultados)
			{
				string slug = fila.slug;
				DateTime? fecha = fila.fecha != null ? (DateTime?)fila.fecha : null;

				if (string.IsNullOrEmpty(slug) == false)
				{
					string texto = "<url>" + Environment.NewLine +
						 "<loc>https://" + dominio + "/curator/" + slug + "/</loc>" + Environment.NewLine;

					if (fecha.HasValue && fecha.Value.Year > 1)
					{
						texto = texto + "<lastmod>" + fecha.Value.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine;
					}

					texto = texto + "</url>";

					lineas.Add(texto);
				}
			}

			return lineas;
		}
	}
}
