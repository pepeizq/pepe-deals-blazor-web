#nullable disable

using Gratis2;
using Juegos;
using pepeizqs_deals_blazor_web.Componentes.Interfaz;
using Suscripciones2;

namespace Herramientas
{
    public static class Tooltip
    {
        public static ToolTipDatos Generar(string idioma, CajaJuego.Tipo tipo, Juego juego, JuegoDRM drm, bool usuarioConectado, bool usuarioTieneJuego, bool usuarioDeseaJuego, int idBundleDescartar = 0, GratisTipo gratisTipoActual = GratisTipo.Desconocido, SuscripcionTipo suscripcionTipoActual = SuscripcionTipo.Desconocido)
        {
            ToolTipDatos datos = new ToolTipDatos
            {
                Nombre = juego.Nombre,
                Video = null,
                ReviewsIcono = null,
                ReviewsCantidad = null,
				BundlesActuales = null,
                BundlesPasados = null,
				GratisActuales = null,
                GratisPasados = null,
				SuscripcionesActuales = null,
                SuscripcionesPasadas = null
            };

			if (juego.Etiquetas != null && juego.Etiquetas.Count > 0)
			{
				datos.Etiquetas = new List<string>();

				int i = 0;
				foreach (var etiqueta in juego.Etiquetas)
				{
					if (i < 3)
					{
						datos.Etiquetas.Add(etiqueta);
					}

					i += 1;
				}
			}

			if (juego.Media?.Videos?.Count > 0)
			{
				if (string.IsNullOrEmpty(juego.Media.Videos[0].Micro) == false)
				{
					datos.Video = juego.Media.Videos[0].Micro;

					datos.Video = datos.Video.Replace(".mp4", ".webm");
				}
			}

			if (juego.Analisis != null)
            {
                if (string.IsNullOrEmpty(juego.Analisis.Porcentaje) == false && string.IsNullOrEmpty(juego.Analisis.Cantidad) == false)
                {
					if (juego.Analisis.Cantidad.Length > 1)
					{
						if (int.Parse(juego.Analisis.Porcentaje) > 74)
						{
							datos.ReviewsIcono = "/imagenes/analisis/positivo3.svg";
						}
						else if (int.Parse(juego.Analisis.Porcentaje) > 49 && int.Parse(juego.Analisis.Porcentaje) < 75)
						{
							datos.ReviewsIcono = "/imagenes/analisis/meh3.svg";
						}
						else if (int.Parse(juego.Analisis.Porcentaje) < 50)
						{
							datos.ReviewsIcono = "/imagenes/analisis/negativo3.svg";
						}

						datos.ReviewsCantidad = juego.Analisis.Porcentaje.ToString() + "% • " + Calculadora.RedondearAnalisis(idioma, juego.Analisis.Cantidad);
					}
                }
            }

			if (juego.BundlesActuales?.Count > 0)
			{
				List<JuegoBundlesActuales> bundlesFinales = new List<JuegoBundlesActuales>();

				foreach (var bundle2 in juego.BundlesActuales)
				{
					bool añadir = true;

					if (idBundleDescartar > 0 && bundle2.id == idBundleDescartar)
					{
						añadir = false;
					}

					if (añadir == true)
					{
						bundlesFinales.Add(bundle2);
					}
				}

				if (bundlesFinales?.Count > 0)
				{
					if (bundlesFinales.Count == 1)
					{
						datos.BundlesActuales = Herramientas.Idiomas.BuscarTexto(idioma, "String2", "Tooltip") + " (" + Bundles2.BundlesCargar.DevolverBundle(bundlesFinales[0].bundleTipo).Tienda + ")";
					}
					else
					{
						datos.BundlesActuales = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String3", "Tooltip"), bundlesFinales?.Count.ToString());
					}
				}
			}

			if (juego.BundlesPasados?.Count > 0)
			{
				if (juego.BundlesPasados.Count == 1)
				{
					datos.BundlesPasados = Herramientas.Idiomas.BuscarTexto(idioma, "String2", "Tooltip") + " (" + Bundles2.BundlesCargar.DevolverBundle(juego.BundlesPasados[0].bundleTipo).Tienda + ")";
				}
				else
				{
					datos.BundlesPasados = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String3", "Tooltip"), juego.BundlesPasados?.Count.ToString());
				}
			}

			if (juego.GratisActuales?.Count > 0)
			{
				List<JuegoGratisActuales> gratisFinales = new List<JuegoGratisActuales>();

				foreach (var gratis2 in juego.GratisActuales)
				{
					bool añadir = true;

					if (gratisTipoActual != GratisTipo.Desconocido)
					{
						if (GratisCargar.DevolverGratis(gratis2.gratis).Tipo == gratisTipoActual)
						{
							añadir = false;
						}
					}

					if (añadir == true)
					{
						gratisFinales.Add(gratis2);
					}
				}

				if (gratisFinales?.Count > 0)
				{
					if (gratisFinales.Count == 1)
					{
						datos.GratisActuales = Herramientas.Idiomas.BuscarTexto(idioma, "String4", "Tooltip") + " (" + Gratis2.GratisCargar.DevolverGratis(gratisFinales[0].gratis).Nombre + ")";
					}
					else
					{
						datos.GratisActuales = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String5", "Tooltip"), gratisFinales?.Count.ToString());
					}
				}
			}

			if (juego.GratisPasados?.Count > 0)
			{
				if (juego.GratisPasados.Count == 1)
				{
					datos.GratisPasados = Herramientas.Idiomas.BuscarTexto(idioma, "String4", "Tooltip") + " (" + Gratis2.GratisCargar.DevolverGratis(juego.GratisPasados[0].gratis).Nombre + ")";
				}
				else
				{
					datos.GratisPasados = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String5", "Tooltip"), juego.GratisPasados?.Count.ToString());
				}
			}

			if (juego.SuscripcionesActuales?.Count > 0)
			{
				List<JuegoSuscripcionActuales> suscripcionesFinales = new List<JuegoSuscripcionActuales>();

				foreach (var suscripcion2 in juego.SuscripcionesActuales)
				{
					bool añadir = true;

					if (suscripcionTipoActual != SuscripcionTipo.Desconocido)
					{
						if (SuscripcionesCargar.DevolverSuscripcion(suscripcion2.suscripcion).Id == suscripcionTipoActual)
						{
							añadir = false;
						}
					}

					if (añadir == true)
					{
						suscripcionesFinales.Add(suscripcion2);
					}
				}

				if (suscripcionesFinales?.Count > 0)
				{
					if (suscripcionesFinales.Count == 1)
					{
						datos.SuscripcionesActuales = Herramientas.Idiomas.BuscarTexto(idioma, "String6", "Tooltip") + " (" + SuscripcionesCargar.DevolverSuscripcion(suscripcionesFinales[0].suscripcion).Nombre + ")";
					}
					else
					{
						datos.SuscripcionesActuales = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String7", "Tooltip"), suscripcionesFinales?.Count.ToString());
					}
				}
			}

			if (juego.SuscripcionesPasados?.Count > 0)
			{
				if (juego.SuscripcionesPasados.Count == 1)
				{
					datos.SuscripcionesPasadas = Herramientas.Idiomas.BuscarTexto(idioma, "String6", "Tooltip") + " (" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(juego.SuscripcionesPasados[0].suscripcion).Nombre + ")";
				}
				else
				{
					datos.SuscripcionesPasadas = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String7", "Tooltip"), juego.SuscripcionesPasados?.Count.ToString());
				}
			}

			return datos;
        }
	}

    public class ToolTipDatos
    {
        public string Nombre { get; set; }
		public List<string> Etiquetas { get; set; }
		public string Video { get; set; }
		public string ReviewsIcono { get; set; }
		public string ReviewsCantidad { get; set; }
        public string BundlesActuales { get; set; }
        public string GratisActuales { get; set; }
        public string SuscripcionesActuales { get; set; }
        public string BundlesPasados { get; set; }
		public string GratisPasados { get; set; }
		public string SuscripcionesPasadas { get; set; }
	}
}
