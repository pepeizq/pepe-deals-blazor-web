#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Extension
{
	public class Extension
	{
		public int Id { get; set; }
		public string Nombre { get; set; }
		public List<ExtensionPrecio> MinimosHistoricos { get; set; }
		public List<ExtensionPrecio> PreciosActuales { get; set; }
		public List<ExtensionBundle> Bundles { get; set; }
		public List<ExtensionGratis> Gratis { get; set; }
		public List<ExtensionSuscripcion> Suscripciones { get; set; }
		public int IdSteam { get; set; }
		public int IdGOG { get; set; }
		public string SlugGOG { get; set; }
		public string SlugEpic { get; set; }
	}

	public class ExtensionPrecio
	{
		public JuegoPrecio Datos { get; set; }
		public string Tienda { get; set; }
		public string TiendaIcono { get; set; }
	}

	public class ExtensionBundle
	{
		public JuegoBundle Datos { get; set; }
		public string NombreBundle { get; set; }
		public string TiendaBundle { get; set; }
		public string IconoBundle { get; set; }
	}

	public class ExtensionGratis
	{
		public JuegoGratisJson Datos { get; set; }
		public string NombreGratis { get; set; }
		public string IconoGratis { get; set; }
	}

	public class ExtensionSuscripcion
	{
		public JuegoSuscripcionJson Datos { get; set; }
		public string NombreSuscripcion { get; set; }
		public string IconoSuscripcion { get; set; }
	}


	public static class Buscar
	{
		public static async Task<Extension> Steam2(string id)
		{
			string buscar = @"SELECT j.id, j.nombre, j.precioMinimosHistoricos, j.precioActualesTiendas, j.bundles, 
(
    SELECT g.*, g.gratis AS Tipo
    FROM gratis g
    WHERE g.juegoId = j.id
    FOR JSON PATH
) as gratis2, 
(
    SELECT s.*, s.suscripcion AS Tipo
    FROM suscripciones s
    WHERE s.juegoId = j.id
    FOR JSON PATH
) as suscripciones2, j.idSteam, j.idGOG, j.slugGOG, j.slugEpic FROM juegos j WHERE idSteam='" + id + "'";

			return await GenerarDatos(buscar, "Steam " + id);
		}

		public static async Task<Extension> Gog2(string slug)
		{
			string buscar = @"SELECT j.id, j.nombre, j.precioMinimosHistoricos, j.precioActualesTiendas, j.bundles, 
(
    SELECT g.*, g.gratis AS Tipo
    FROM gratis g
    WHERE g.juegoId = j.id
    FOR JSON PATH
) as gratis2, 
(
    SELECT s.*, s.suscripcion AS Tipo
    FROM suscripciones s
    WHERE s.juegoId = j.id
    FOR JSON PATH
) as suscripciones2, j.idSteam, j.idGOG, j.slugGOG, j.slugEpic FROM juegos j WHERE slugGOG='" + slug + "'";

			return await GenerarDatos(buscar, "GOG " + slug);
		}

		public static async Task<Extension> EpicGames2(string slug)
		{
			string buscar = @"SELECT j.id, j.nombre, j.precioMinimosHistoricos, j.precioActualesTiendas, j.bundles, 
(
    SELECT g.*, g.gratis AS Tipo
    FROM gratis g
    WHERE g.juegoId = j.id
    FOR JSON PATH
) as gratis2, 
(
    SELECT s.*, s.suscripcion AS Tipo
    FROM suscripciones s
    WHERE s.juegoId = j.id
    FOR JSON PATH
) as suscripciones2, j.idSteam, j.idGOG, j.slugGOG, j.slugEpic FROM juegos j WHERE slugEpic='" + slug + "'";

			return await GenerarDatos(buscar, "Epic " + slug);
		}

		private static async Task<Extension> GenerarDatos(string buscar, string id)
		{
			if (buscar == null)
			{
				return null;
			}

			try
			{
				var fila2 = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync<dynamic>(buscar, transaction: sentencia);
				});

				IDictionary<string, object> fila = (IDictionary<string, object>)fila2;

				Extension extension = new Extension();

				if (fila == null)
				{
					return null;
				}

				string CogerString(IDictionary<string, object> fila, string columna)
				{
					return (fila).ContainsKey(columna) && fila[columna] != null ? fila[columna].ToString() : null;
				}

				int? CogerInt(IDictionary<string, object> fila, string columna)
				{
					return (fila).ContainsKey(columna) && fila[columna] != null ? Convert.ToInt32(fila[columna]) : (int?)null;
				}

				extension.Id = CogerInt(fila, "id") ?? 0;
				extension.Nombre = CogerString(fila, "nombre");

				string jsonMinimos = CogerString(fila, "precioMinimosHistoricos");
				if (string.IsNullOrEmpty(jsonMinimos) == false)
				{
					var lista = JsonSerializer.Deserialize<List<JuegoPrecio>>(jsonMinimos);

					if (lista?.Count > 0)
					{
						extension.MinimosHistoricos = new List<ExtensionPrecio>();

						foreach (var precio in lista)
						{
							precio.Enlace = Herramientas.EnlaceAcortador.Generar(precio.Enlace, precio.Tienda, false, false);

							extension.MinimosHistoricos.Add(new ExtensionPrecio
							{
								Datos = precio,
								Tienda = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda).Nombre,
								TiendaIcono = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda).ImagenIcono
							});
						}
					}
				}

				string jsonActuales = CogerString(fila, "precioActualesTiendas");
				if (string.IsNullOrEmpty(jsonActuales) == false)
				{
					var lista = JsonSerializer.Deserialize<List<JuegoPrecio>>(jsonActuales);

					if (lista?.Count > 0)
					{
						extension.PreciosActuales = new List<ExtensionPrecio>();

						foreach (var precio in lista)
						{
							precio.Enlace = Herramientas.EnlaceAcortador.Generar(precio.Enlace, precio.Tienda, false, false);

							if (precio != null && precio.Tienda != null)
							{
								if (extension.PreciosActuales == null)
								{
									extension.PreciosActuales = new List<ExtensionPrecio>();
								}

								extension.PreciosActuales?.Add(new ExtensionPrecio
								{
									Datos = precio,
									Tienda = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda).Nombre,
									TiendaIcono = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda).ImagenIcono
								});
							}
						}
					}
				}

				var jsonBundles = CogerString(fila, "bundles");
				if (string.IsNullOrEmpty(jsonBundles) == false)
				{
					var lista = JsonSerializer.Deserialize<List<JuegoBundle>>(jsonBundles);

					if (lista?.Count > 0)
					{
						extension.Bundles = new List<ExtensionBundle>();

						foreach (var bundle in lista)
						{
							var datosBundle = await BaseDatos.Bundles.Buscar.UnBundle(bundle.BundleId);
							if (datosBundle != null)
							{
								bundle.Enlace = Herramientas.EnlaceAcortador.Generar(bundle.Enlace, bundle.Tipo, false, false);

								extension.Bundles.Add(new ExtensionBundle
								{
									Datos = bundle,
									NombreBundle = datosBundle.Nombre,
									TiendaBundle = Bundles2.BundlesCargar.DevolverBundle(bundle.Tipo).Tienda,
									IconoBundle = Bundles2.BundlesCargar.DevolverBundle(bundle.Tipo).ImagenIcono
								});
							}
						}
					}
				}

				var jsonGratis = CogerString(fila, "gratis2");
				if (string.IsNullOrEmpty(jsonGratis) == false)
				{
					var lista = JsonSerializer.Deserialize<List<JuegoGratisJson>>(jsonGratis);

					if (lista?.Count > 0)
					{
						extension.Gratis = new List<ExtensionGratis>();

						foreach (var gratis in lista)
						{
							gratis.Enlace = Herramientas.EnlaceAcortador.Generar(gratis.Enlace, gratis.Tipo, false, false);

							extension.Gratis.Add(new ExtensionGratis
							{
								Datos = gratis,
								NombreGratis = Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre,
								IconoGratis = Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).ImagenIcono
							});
						}
					}
				}

				var jsonSuscripciones = CogerString(fila, "suscripciones2");
				if (string.IsNullOrEmpty(jsonSuscripciones) == false)
				{
					var lista = JsonSerializer.Deserialize<List<JuegoSuscripcionJson>>(jsonSuscripciones);

					if (lista?.Count > 0)
					{
						extension.Suscripciones = new List<ExtensionSuscripcion>();

						foreach (var suscripcion in lista)
						{
							suscripcion.Enlace = Herramientas.EnlaceAcortador.Generar(suscripcion.Enlace, suscripcion.Tipo, false, false);

							extension.Suscripciones.Add(new ExtensionSuscripcion
							{
								Datos = suscripcion,
								NombreSuscripcion = Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre,
								IconoSuscripcion = Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).ImagenIcono
							});
						}
					}
				}

				extension.IdSteam = CogerInt(fila, "idSteam") ?? 0;
				extension.IdGOG = CogerInt(fila, "idGOG") ?? 0;

				extension.SlugGOG = CogerString(fila, "slugGOG");
				extension.SlugEpic = CogerString(fila, "slugEpic");

				return extension;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Extension Generar " + id, ex, false);
			}

			return null;
		}
	}
}
