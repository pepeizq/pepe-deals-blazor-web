#nullable disable

using Juegos;
using Microsoft.Data.SqlClient;
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
		public JuegoGratis Datos { get; set; }
		public string NombreGratis { get; set; }
		public string IconoGratis { get; set; }
	}

	public class ExtensionSuscripcion
	{
		public JuegoSuscripcion Datos { get; set; }
		public string NombreSuscripcion { get; set; }
		public string IconoSuscripcion { get; set; }
	}


	public static class Buscar
	{
		public static Extension Steam2(string id, SqlConnection conexion = null)
		{
			string buscar = "SELECT id, nombre, precioMinimosHistoricos, precioActualesTiendas, bundles, gratis, suscripciones, idSteam, idGOG, slugGOG, slugEpic FROM juegos WHERE idSteam=@idSteam";

			return GenerarDatos(buscar, "@idSteam", id, conexion);
		}

		public static Extension Gog2(string slug, SqlConnection conexion = null)
		{
			string buscar = "SELECT id, nombre, precioMinimosHistoricos, precioActualesTiendas, bundles, gratis, suscripciones, idSteam, idGOG, slugGOG, slugEpic FROM juegos WHERE slugGOG=@slugGOG";

			return GenerarDatos(buscar, "@slugGOG", slug, conexion);
		}

		public static Extension EpicGames2(string slug, SqlConnection conexion = null)
		{
			string buscar = "SELECT id, nombre, precioMinimosHistoricos, precioActualesTiendas, bundles, gratis, suscripciones, idSteam, idGOG, slugGOG, slugEpic FROM juegos WHERE slugEpic=@slugEpic";

			return GenerarDatos(buscar, "@slugEpic", slug, conexion);
		}

		private static Extension GenerarDatos(string buscar, string parametro, string valor, SqlConnection conexion = null)
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

			Extension extension = new Extension();

			using (SqlCommand comando = new SqlCommand(buscar, conexion))
			{
				comando.Parameters.AddWithValue(parametro, valor);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						if (lector.IsDBNull(0) == false)
						{
							extension.Id = lector.GetInt32(0);
						}

						if (lector.IsDBNull(1) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(1)) == false)
							{
								extension.Nombre = lector.GetString(1);
							}
						}

						if (lector.IsDBNull(2) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(2)) == false)
							{
								var listaTemporal = JsonSerializer.Deserialize<List<JuegoPrecio>>(lector.GetString(2));

								if (listaTemporal != null)
								{
									foreach (var precio in listaTemporal)
									{
										if (extension.MinimosHistoricos == null)
										{
											extension.MinimosHistoricos = new List<ExtensionPrecio>();
										}

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
						}

						if (lector.IsDBNull(3) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(3)) == false)
							{
								var listaTemporal = JsonSerializer.Deserialize<List<JuegoPrecio>>(lector.GetString(3));

								if (listaTemporal != null)
								{
									foreach (var precio in listaTemporal)
									{
										if (extension.PreciosActuales == null)
										{
											extension.PreciosActuales = new List<ExtensionPrecio>();
										}

										precio.Enlace = Herramientas.EnlaceAcortador.Generar(precio.Enlace, precio.Tienda, false, false);

										extension.PreciosActuales.Add(new ExtensionPrecio
										{
											Datos = precio,
											Tienda = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda).Nombre,
											TiendaIcono = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda).ImagenIcono
										});
									}
								}
							}
						}

						if (lector.IsDBNull(4) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(4)) == false)
							{
								var listaTemporal = JsonSerializer.Deserialize<List<JuegoBundle>>(lector.GetString(4));

								if (listaTemporal != null)
								{
									foreach (var bundle in listaTemporal)
									{
										var datosBundle = BaseDatos.Bundles.Buscar.UnBundle(bundle.BundleId);

										if (datosBundle != null)
										{
											if (extension.Bundles == null)
											{
												extension.Bundles = new List<ExtensionBundle>();
											}

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
						}

						if (lector.IsDBNull(5) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(5)) == false)
							{
								var listaTemporal = JsonSerializer.Deserialize<List<JuegoGratis>>(lector.GetString(5));

								if (listaTemporal != null)
								{
									foreach (var gratis in listaTemporal)
									{
										if (extension.Gratis == null)
										{
											extension.Gratis = new List<ExtensionGratis>();
										}

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
						}

						if (lector.IsDBNull(6) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(6)) == false)
							{
								var listaTemporal = JsonSerializer.Deserialize<List<JuegoSuscripcion>>(lector.GetString(6));

								if (listaTemporal != null)
								{
									foreach (var suscripcion in listaTemporal)
									{
										if (extension.Suscripciones == null)
										{
											extension.Suscripciones = new List<ExtensionSuscripcion>();
										}

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
						}

						if (lector.IsDBNull(7) == false)
						{
							extension.IdSteam = lector.GetInt32(7);
						}

						if (lector.IsDBNull(8) == false)
						{
							extension.IdGOG = lector.GetInt32(8);
						}

						if (lector.IsDBNull(9) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(9)) == false)
							{
								extension.SlugGOG = lector.GetString(9);
							}
						}

						if (lector.IsDBNull(10) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(10)) == false)
							{
								extension.SlugEpic = lector.GetString(10);
							}
						}
					}
				}
			}

			return extension;
		}
	}
}
