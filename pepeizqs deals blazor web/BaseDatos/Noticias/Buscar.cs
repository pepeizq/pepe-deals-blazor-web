#nullable disable

using Dapper;
using Noticias;

namespace BaseDatos.Noticias
{
	public static class Buscar
	{
		public static async Task<Noticia> UnaNoticia(int id)
		{
			string sql = "SELECT * FROM noticias WITH (NOLOCK) WHERE id = @id";

			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync<Noticia>(sql, new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticia Una", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Actuales(NoticiaTipo tipo = NoticiaTipo.Desconocido, int ultimosDias = 0)
		{
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

			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync<Noticia>(busqueda, transaction: sentencia).ContinueWith(t => t.Result.ToList());
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actuales", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Año(string año)
		{
			string busqueda = "SELECT * FROM noticias WHERE YEAR(fechaEmpieza) = " + año + " AND GETDATE() > fechaTermina ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync<Noticia>(busqueda, transaction: sentencia).ContinueWith(t => t.Result.ToList());
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Año", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Ultimas(int cantidad)
		{
			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM noticias ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync<Noticia>(busqueda, transaction: sentencia).ContinueWith(t => t.Result.ToList());
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Ultimas", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Ultimas(int cantidad, NoticiaTipo tipo)
		{
			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM noticias WHERE noticiaTipo=@noticiaTipo ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync<Noticia>(busqueda, new { noticiaTipo = tipo }, transaction: sentencia).ContinueWith(t => t.Result.ToList());
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Ultimas", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Aleatorias()
		{
			string busqueda = @"SELECT TOP 50 id, tituloEn FROM noticias ORDER BY NEWID()";

			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync<Noticia>(busqueda, transaction: sentencia).ContinueWith(t => t.Result.ToList());
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Aleatorias", ex);
			}

			return null;
		}
	}
}
