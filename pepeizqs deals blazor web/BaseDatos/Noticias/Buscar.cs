#nullable disable

using Bundles2;
using Dapper;
using Gratis2;
using Microsoft.Data.SqlClient;
using Noticias;
using Suscripciones2;

namespace BaseDatos.Noticias
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

		public static Noticia UnaNoticia(int id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sql = "SELECT * FROM noticias WHERE id = @id";

			return conexion.QueryFirstOrDefault<Noticia>(sql, new { id });
		}

		public static Noticia Ultimo(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT TOP 1 * FROM noticias ORDER BY id DESC";

			return conexion.QueryFirstOrDefault<Noticia>(busqueda);
		}

		public static List<Noticia> Actuales(NoticiaTipo tipo = NoticiaTipo.Desconocido, int ultimosDias = 0, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM noticias WHERE (GETDATE() BETWEEN fechaEmpieza AND fechaTermina)";

			if (ultimosDias > 0)
			{
				busqueda = busqueda + " AND (GETDATE()-" + ultimosDias.ToString() + " < fechaEmpieza)";
			}

			if (tipo != NoticiaTipo.Desconocido)
			{
				busqueda = busqueda + " AND (noticiaTipo=" + (int)tipo + ")";
			}

			busqueda = busqueda + " ORDER BY id DESC";

			return conexion.Query<Noticia>(busqueda).ToList();
		}

		public static List<Noticia> Año(string año, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM noticias WHERE YEAR(fechaEmpieza) = " + año + " AND GETDATE() > fechaTermina ORDER BY id DESC";

			return conexion.Query<Noticia>(busqueda).ToList();
		}

		public static List<Noticia> Ultimas(int cantidad, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM noticias ORDER BY id DESC";

			return conexion.Query<Noticia>(busqueda).ToList();
		}

		public static List<Noticia> Ultimas(int cantidad, NoticiaTipo tipo, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM noticias WHERE noticiaTipo=@noticiaTipo ORDER BY id DESC";

			return conexion.Query<Noticia>(busqueda, new {noticiaTipo = tipo}).ToList();
		}

		public static List<Noticia> Aleatorias(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT TOP 50 id, tituloEn FROM noticias ORDER BY NEWID()";

			return conexion.Query<Noticia>(busqueda).ToList();
		}
	}
}
