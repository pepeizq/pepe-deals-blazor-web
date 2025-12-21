#nullable disable

using Dapper;
using Herramientas;
using System.Globalization;

namespace BaseDatos.Sitemaps
{
	public static class Buscar
	{
		public static async Task<int> Cantidad(string tabla)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {tabla}");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Cantidad", ex);
			}

			return 0;
		}

		public static async Task<List<string>> Juegos(string dominio, int id1, int id2)
		{
			string buscar = "SELECT id, nombre, ultimaModificacion FROM juegos WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { Id1 = id1, Id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Juegos", ex);
			}

			return new List<string>();
		}

		public static async Task<List<string>> Bundles(string dominio, int id1, int id2)
		{
			string buscar = "SELECT id, nombre, fechaEmpieza FROM bundles WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { Id1 = id1, Id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Bundles", ex);
			}

			return new List<string>();
		}

		public static async Task<List<string>> Gratis(string dominio, int id1, int id2)
		{
			string buscar = "SELECT juegoId, nombre, fechaEmpieza FROM gratis WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { Id1 = id1, Id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Gratis", ex);
			}

			return new List<string>();		
		}

		public static async Task<List<string>> Suscripciones(string dominio, int id1, int id2)
		{
			string buscar = "SELECT juegoId, nombre, fechaEmpieza FROM suscripciones WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { id1 = id1, id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Suscripciones", ex);
			}

			return new List<string>();
		}

		public static async Task<List<string>> NoticiasIngles(string dominio, int id1, int id2)
		{
			string buscar = "SELECT id, tituloEn, fechaEmpieza FROM noticias WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { id1 = id1, id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Noticias Ingles", ex);
			}

			return new List<string>();
		}

		public static async Task<List<string>> NoticiasEspañol(string dominio, int id1, int id2)
		{
			string buscar = "SELECT id, tituloEs, fechaEmpieza FROM noticias WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { id1 = id1, id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Noticias Español", ex);
			}

			return new List<string>();
		}

		public static async Task<List<string>> Curators(string dominio, int id1, int id2)
		{
			string buscar = "SELECT slug, fecha FROM curators WHERE id > @id1 AND id < @id2";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(buscar, new { id1 = id1, id2 = id2 })).ToList();
				});

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
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Sitemaps Curators", ex);
			}

			return new List<string>();		
		}
	}
}
