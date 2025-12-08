#nullable disable

using Gratis2;
using Herramientas;
using Microsoft.VisualBasic;
using Suscripciones2;

namespace Noticias
{
    public static class Plantillas
    {
        public static async Task<Plantilla> Bundles(int bundleId)
        {
            Plantilla plantilla = new Plantilla();

            Bundles2.Bundle bundle = await BaseDatos.Bundles.Buscar.UnBundle(bundleId);

            if (bundle != null)
            {
                if (bundle.Id > 0)
                {
					plantilla.BundleId = bundleId.ToString();
					plantilla.Juegos = null;

					for (int i = 0; i < bundle.Juegos.Count; i += 1)
					{
						if (i == 0)
						{
							plantilla.Juegos = bundle.Juegos[i].JuegoId;
						}
						else
						{
							plantilla.Juegos = plantilla.Juegos + "," + bundle.Juegos[i].JuegoId;
						}
					}

					plantilla.Fecha = bundle.FechaTermina;
					//plantilla.Enlace = EnlaceAcortador.Generar(bundle.Enlace, bundle.Tipo);

					#region Titulo

					Random azarTitulo = new Random();
					int azarTitulo2 = azarTitulo.Next(1, 5);

					if (bundle.Nombre.ToLower().Contains("bundle") == false)
					{
						plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "BundleTitle" + azarTitulo2.ToString() + ".1", "NewsTemplates"), bundle.Nombre, bundle.Tienda);
						plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "BundleTitle" + azarTitulo2.ToString() + ".1", "NewsTemplates"), bundle.Nombre, bundle.Tienda);
					}
					else
					{
						plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "BundleTitle" + azarTitulo2.ToString() + ".2", "NewsTemplates"), bundle.Nombre, bundle.Tienda);
						plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "BundleTitle" + azarTitulo2.ToString() + ".2", "NewsTemplates"), bundle.Nombre, bundle.Tienda);
					}

					#endregion

					#region Contenido

					if (bundle.Pick == false)
					{
						plantilla.ContenidoEn = "<div style=" + Strings.ChrW(34) + "margin-bottom: 15px;" + Strings.ChrW(34) + ">" + string.Format(Idiomas.BuscarTexto("en", "Bundle2", "NewsTemplates"), bundle.Tienda) + "</div>";
						plantilla.ContenidoEs = "<div style=" + Strings.ChrW(34) + "margin-bottom: 15px;" + Strings.ChrW(34) + ">" + string.Format(Idiomas.BuscarTexto("es", "Bundle2", "NewsTemplates"), bundle.Tienda) + "</div>";

						foreach (var tier in bundle.Tiers.OrderBy(x => x.Posicion))
						{
							string precio = tier.Precio;
							precio = precio.Replace(".", ",");
							precio = precio + "€";

							plantilla.ContenidoEn = plantilla.ContenidoEn + "<div>Tier " + tier.Posicion + " (" + precio + "):<ul>";
							plantilla.ContenidoEs = plantilla.ContenidoEs + "<div>Tier " + tier.Posicion + " (" + precio + "):<ul>";

							foreach (var juego in bundle.Juegos)
							{
								if (juego.Tier.Posicion == tier.Posicion)
								{
									plantilla.ContenidoEn = plantilla.ContenidoEn + "<li>" + juego.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ")</li>";
									plantilla.ContenidoEs = plantilla.ContenidoEs + "<li>" + juego.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ")</li>";
								}
							}

							plantilla.ContenidoEn = plantilla.ContenidoEn + "</ul></div>";
							plantilla.ContenidoEs = plantilla.ContenidoEs + "</ul></div>";
						}
					}
					else
					{
						plantilla.ContenidoEn = "<div style=" + Strings.ChrW(34) + "margin-bottom: 15px;" + Strings.ChrW(34) + ">" + string.Format(Idiomas.BuscarTexto("en", "Bundle3", "NewsTemplates"), bundle.Tienda) + "</div>";
						plantilla.ContenidoEs = "<div style=" + Strings.ChrW(34) + "margin-bottom: 15px;" + Strings.ChrW(34) + ">" + string.Format(Idiomas.BuscarTexto("es", "Bundle3", "NewsTemplates"), bundle.Tienda) + "</div>";

						foreach (var tier in bundle.Tiers)
						{
							string precio = tier.Precio;
							precio = precio.Replace(".", ",");
							precio = precio + "€";

							plantilla.ContenidoEn = plantilla.ContenidoEn + "<div style=" + Strings.ChrW(34) + "margin-bottom: 15px;" + Strings.ChrW(34) + ">" + tier.CantidadJuegos.ToString() + " " + Idiomas.BuscarTexto("en", "Bundle4", "NewsTemplates") + " (" + precio + ")</div>";
							plantilla.ContenidoEs = plantilla.ContenidoEs + "<div style=" + Strings.ChrW(34) + "margin-bottom: 15px;" + Strings.ChrW(34) + ">" + tier.CantidadJuegos.ToString() + " " + Idiomas.BuscarTexto("es", "Bundle4", "NewsTemplates") + " (" + precio + ")</div>";
						}

						plantilla.ContenidoEn = plantilla.ContenidoEn + "<div><ul>";
						plantilla.ContenidoEs = plantilla.ContenidoEs + "<div><ul>";

						foreach (var juego in bundle.Juegos)
						{
							plantilla.ContenidoEn = plantilla.ContenidoEn + "<li>" + juego.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ")</li>";
							plantilla.ContenidoEs = plantilla.ContenidoEs + "<li>" + juego.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ")</li>";
						}

						plantilla.ContenidoEn = plantilla.ContenidoEn + "</ul></div>";
						plantilla.ContenidoEs = plantilla.ContenidoEs + "</ul></div>";
					}

					#endregion

					#region Imagen

					if (bundle.ImagenNoticia != null)
					{
						plantilla.Imagen = bundle.ImagenNoticia;
					}

					#endregion
				}
			}

            return plantilla;
        }

        public static async Task<Plantilla> Gratis(Plantilla plantilla, int juegoId, int id, GratisTipo tipoSeleccionado)
        {
            if (string.IsNullOrEmpty(plantilla.Juegos) == true)
            {
                plantilla.Juegos = juegoId.ToString();
            }
            else
            {
                if (plantilla.Juegos.Contains(juegoId.ToString()) == false)
                {
                    plantilla.Juegos = plantilla.Juegos + "," + juegoId.ToString();
                }
                else
                {
                    int int1 = plantilla.Juegos.IndexOf(juegoId.ToString() + ",");

                    if (int1 == -1)
                    {
                        int1 = plantilla.Juegos.IndexOf(juegoId.ToString());
                        plantilla.Juegos = plantilla.Juegos.Remove(int1, juegoId.ToString().Length);
                    }
                    else
                    {
                        plantilla.Juegos = plantilla.Juegos.Remove(int1, juegoId.ToString().Length + 1);
                    }

                    if (plantilla.Juegos.Trim().Length == 1)
                    {
                        plantilla.Juegos = null;
                    }
                }
            }

            List<string> lista = Listados.Generar(plantilla.Juegos);

			if (lista?.Count > 0)
			{
				Juegos.JuegoGratis juegoGratis1 = await BaseDatos.Gratis.Buscar.UnJuego(lista[0]);

				if (juegoGratis1.Juego == null)
				{
					juegoGratis1.Juego = await BaseDatos.Juegos.Buscar.UnJuego(juegoGratis1.JuegoId);
				}

				if (lista.Count == 1)
				{
					Random azarTitulo = new Random();
					int azarTitulo2 = azarTitulo.Next(1, 4);

					plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "FreeTitleOneItem" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
					plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "FreeTitleOneItem" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);

					if (juegoGratis1.Juego.Tipo == Juegos.JuegoTipo.Game)
					{
						plantilla.ContenidoEn = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("en", "Free5", "NewsTemplates");
						plantilla.ContenidoEs = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("es", "Free5", "NewsTemplates");
					}
					else if (juegoGratis1.Juego.Tipo == Juegos.JuegoTipo.DLC)
					{
						plantilla.ContenidoEn = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("en", "Free7", "NewsTemplates");
						plantilla.ContenidoEs = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("es", "Free7", "NewsTemplates");
					}
				}
				else if (lista.Count > 1)
				{
					Juegos.JuegoGratis juegoGratis2 = await BaseDatos.Gratis.Buscar.UnJuego(lista[1]);

					if (juegoGratis2.Juego == null)
					{
						juegoGratis2.Juego = await BaseDatos.Juegos.Buscar.UnJuego(juegoGratis2.JuegoId);
					}

					if (lista.Count == 2)
					{
						Random azarTitulo = new Random();
						int azarTitulo2 = azarTitulo.Next(1, 2);

						plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "FreeTitleTwoItems" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
						plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "FreeTitleTwoItems" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
					}
					else if (lista.Count == 3)
					{
						Juegos.JuegoGratis juegoGratis3 = await BaseDatos.Gratis.Buscar.UnJuego(lista[2]);

						if (juegoGratis3.Juego == null)
						{
							juegoGratis3.Juego = await BaseDatos.Juegos.Buscar.UnJuego(juegoGratis3.JuegoId);
						}

						Random azarTitulo = new Random();
						int azarTitulo2 = azarTitulo.Next(1, 2);

						plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "FreeTitleThreeItems" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, juegoGratis3.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
						plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "FreeTitleThreeItems" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, juegoGratis3.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
					}
					else if (lista.Count > 3)
					{
						if (juegoGratis1.Juego.Tipo == Juegos.JuegoTipo.Game && juegoGratis2.Juego.Tipo == Juegos.JuegoTipo.Game)
						{
							Random azarTitulo = new Random();
							int azarTitulo2 = azarTitulo.Next(1, 2);

							plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "FreeTitleMoreItems" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
							plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "FreeTitleMoreItems" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
						}
						else if (juegoGratis1.Juego.Tipo == Juegos.JuegoTipo.DLC && juegoGratis2.Juego.Tipo == Juegos.JuegoTipo.DLC)
						{
							Random azarTitulo = new Random();
							int azarTitulo2 = azarTitulo.Next(1, 2);

							plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "FreeTitleMoreDLCs" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
							plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "FreeTitleMoreDLCs" + azarTitulo2, "NewsTemplates"), juegoGratis1.Nombre, juegoGratis2.Nombre, GratisCargar.DevolverGratis(tipoSeleccionado).Nombre);
						}
					}

					if (juegoGratis1.Juego.Tipo == Juegos.JuegoTipo.Game && juegoGratis2.Juego.Tipo == Juegos.JuegoTipo.Game)
					{
						plantilla.ContenidoEn = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("en", "Free6", "NewsTemplates");
						plantilla.ContenidoEs = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("es", "Free6", "NewsTemplates");
					}
					else if (juegoGratis1.Juego.Tipo == Juegos.JuegoTipo.DLC && juegoGratis2.Juego.Tipo == Juegos.JuegoTipo.DLC)
					{
						plantilla.ContenidoEn = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("en", "Free8", "NewsTemplates");
						plantilla.ContenidoEs = GratisCargar.DevolverGratis(tipoSeleccionado).Nombre + " " + Idiomas.BuscarTexto("es", "Free8", "NewsTemplates");
					}
				}

				plantilla.ContenidoEn = plantilla.ContenidoEn + Environment.NewLine + "<ul>" + Environment.NewLine;
				plantilla.ContenidoEs = plantilla.ContenidoEs + Environment.NewLine + "<ul>" + Environment.NewLine;

				foreach (var juego in lista)
				{
					Juegos.JuegoGratis gratis = await BaseDatos.Gratis.Buscar.UnJuego(juego);

					if (gratis != null)
					{
						if (GratisCargar.DevolverGratis(tipoSeleccionado).DRMEnseñar == true)
						{
							plantilla.ContenidoEn = plantilla.ContenidoEn + "<li>" + gratis.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(gratis.DRM) + ")</li>" + Environment.NewLine;
							plantilla.ContenidoEs = plantilla.ContenidoEs + "<li>" + gratis.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(gratis.DRM) + ")</li>" + Environment.NewLine;
						}
						else
						{
							plantilla.ContenidoEn = plantilla.ContenidoEn + "<li>" + gratis.Nombre + "</li>" + Environment.NewLine;
							plantilla.ContenidoEs = plantilla.ContenidoEs + "<li>" + gratis.Nombre + "</li>" + Environment.NewLine;
						}
					}
				}

				plantilla.ContenidoEn = plantilla.ContenidoEn + "</ul>";
				plantilla.ContenidoEs = plantilla.ContenidoEs + "</ul>";

				if (juegoGratis1.ImagenNoticia != null)
				{
					plantilla.Imagen = juegoGratis1.ImagenNoticia;
				}
				else
				{
					plantilla.Imagen = juegoGratis1.Juego.Imagenes.Library_1920x620;
				}

				plantilla.Fecha = juegoGratis1.FechaTermina;

				if (string.IsNullOrEmpty(plantilla.GratisIds) == true)
				{
					plantilla.GratisIds = id.ToString();
				}
				else
				{
					plantilla.GratisIds = plantilla.GratisIds + "," + id.ToString();
				}
			}
			else
			{
				plantilla = new Plantilla();
			}

			return plantilla;
        }

        public static async Task<Plantilla> Suscripciones(Plantilla plantilla, int juegoId, int id, SuscripcionTipo tipoSeleccionado)
        {
            if (string.IsNullOrEmpty(plantilla.Juegos) == true)
            {
                plantilla.Juegos = juegoId.ToString();
            }
            else
            {
                if (plantilla.Juegos.Contains(juegoId.ToString()) == false)
                {
                    plantilla.Juegos = plantilla.Juegos + "," + juegoId.ToString();
                }
                else
                {
                    int int1 = plantilla.Juegos.IndexOf(juegoId.ToString() + ",");

                    if (int1 == -1)
                    {
                        int1 = plantilla.Juegos.IndexOf(juegoId.ToString());
                        plantilla.Juegos = plantilla.Juegos.Remove(int1, juegoId.ToString().Length);
                    }
                    else
                    {
                        plantilla.Juegos = plantilla.Juegos.Remove(int1, juegoId.ToString().Length + 1);
                    }

                    if (plantilla.Juegos.Trim().Length == 1)
                    {
                        plantilla.Juegos = null;
                    }
                }
            }

            if (string.IsNullOrEmpty(plantilla.SuscripcionesIds) == true)
            {
                plantilla.SuscripcionesIds = id.ToString();
            }
            else
            {
                if (plantilla.SuscripcionesIds.Contains(id.ToString()) == false)
                {
                    plantilla.SuscripcionesIds = plantilla.SuscripcionesIds + "," + id.ToString();
                }
                else
                {
                    int int1 = plantilla.SuscripcionesIds.IndexOf(id.ToString() + ",");

                    if (int1 == -1)
                    {
                        int1 = plantilla.SuscripcionesIds.IndexOf(id.ToString());
                        plantilla.SuscripcionesIds = plantilla.SuscripcionesIds.Remove(int1, id.ToString().Length);
                    }
                    else
                    {
                        plantilla.SuscripcionesIds = plantilla.SuscripcionesIds.Remove(int1, id.ToString().Length + 1);
                    }

                    if (plantilla.SuscripcionesIds.Trim().Length == 1)
                    {
                        plantilla.SuscripcionesIds = null;
                    }
                }
            }

            List<string> lista = Listados.Generar(plantilla.SuscripcionesIds);

            if (lista != null)
            {
                #region Titulo

                if (lista.Count == 1)
                {
                    plantilla.TituloEn = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).Nombre + " " + string.Format(Idiomas.BuscarTexto("en", "Subscription1", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre);
                    plantilla.TituloEs = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).Nombre + " " + string.Format(Idiomas.BuscarTexto("es", "Subscription1", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre);
                }
                else if (lista.Count == 2)
                {
                    plantilla.TituloEn = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).Nombre + " " + Idiomas.BuscarTexto("en", "Subscription2", "NewsTemplates") + " " +
                        BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[1])).Nombre + " " + string.Format(Idiomas.BuscarTexto("en", "Subscription3", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre);
                    plantilla.TituloEs = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).Nombre + " " + Idiomas.BuscarTexto("es", "Subscription2", "NewsTemplates") + " " +
                        BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[1])).Nombre + " " + string.Format(Idiomas.BuscarTexto("es", "Subscription3", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre);
                }
                else if (lista.Count > 2)
                {
                    plantilla.TituloEn = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).Nombre + ", " +
                        BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[1])).Nombre + " " + string.Format(Idiomas.BuscarTexto("en", "Subscription4", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre);
                    plantilla.TituloEs = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).Nombre + ", " +
                        BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[1])).Nombre + " " + string.Format(Idiomas.BuscarTexto("es", "Subscription4", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre);
                }

                #endregion

                #region Contenido

                plantilla.ContenidoEn = Idiomas.BuscarTexto("en", "Subscription5", "NewsTemplates") + " " + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre + " ";
                plantilla.ContenidoEs = Idiomas.BuscarTexto("es", "Subscription5", "NewsTemplates") + " " + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(tipoSeleccionado).Nombre + " ";

                if (lista.Count == 1)
                {
                    plantilla.ContenidoEn = plantilla.ContenidoEn + Idiomas.BuscarTexto("en", "Subscription6", "NewsTemplates");
                    plantilla.ContenidoEs = plantilla.ContenidoEs + Idiomas.BuscarTexto("es", "Subscription6", "NewsTemplates");
                }
                else if (lista.Count > 1)
                {
                    plantilla.ContenidoEn = plantilla.ContenidoEn + Idiomas.BuscarTexto("en", "Subscription7", "NewsTemplates");
                    plantilla.ContenidoEs = plantilla.ContenidoEs + Idiomas.BuscarTexto("es", "Subscription7", "NewsTemplates");
                }

                plantilla.ContenidoEn = plantilla.ContenidoEn + Environment.NewLine + "<ul>" + Environment.NewLine;
                plantilla.ContenidoEs = plantilla.ContenidoEs + Environment.NewLine + "<ul>" + Environment.NewLine;

                foreach (var juego in lista)
                {
                    Juegos.JuegoSuscripcion suscripcion = BaseDatos.Suscripciones.Buscar.Id(int.Parse(juego));

                    if (suscripcion != null)
                    {
                        plantilla.ContenidoEn = plantilla.ContenidoEn + "<li>" + suscripcion.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(suscripcion.DRM) + ")</li>" + Environment.NewLine;
                        plantilla.ContenidoEs = plantilla.ContenidoEs + "<li>" + suscripcion.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(suscripcion.DRM) + ")</li>" + Environment.NewLine;
                    }
                }

                plantilla.ContenidoEn = plantilla.ContenidoEn + "</ul>";
                plantilla.ContenidoEs = plantilla.ContenidoEs + "</ul>";

                #endregion

                #region Imagen

                if (BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).ImagenNoticia != null)
                {
                    plantilla.Imagen = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).ImagenNoticia;
                }
                else
                {
                    Juegos.Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).JuegoId.ToString());
                    plantilla.Imagen = juego.Imagenes.Library_1920x620;
                }

                #endregion

                #region Fecha

                plantilla.Fecha = BaseDatos.Suscripciones.Buscar.Id(int.Parse(lista[0])).FechaTermina;

                #endregion
            }
            else
            {
                plantilla = new Plantilla();
            }

            return plantilla;
        }

		public static async Task<Plantilla> Despedidas(string enlace, DateTime fecha, int juegoId, string dominio)
		{
			Plantilla plantilla = new Plantilla();

			Juegos.Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(juegoId.ToString());

			if (juego != null)
			{
				plantilla.Juegos = juegoId.ToString();
				plantilla.Fecha = fecha;

				string diaIngles = fecha.Day.ToString();

				if (fecha.Day == 1 || fecha.Day == 21 || fecha.Day == 31)
				{
					diaIngles = diaIngles + "st";
				}
				else if (fecha.Day == 2 || fecha.Day == 22)
				{
					diaIngles = diaIngles + "nd";
				}
				else if (fecha.Day == 3 || fecha.Day == 23)
				{
					diaIngles = diaIngles + "rd";
				}
				else
				{
					diaIngles = diaIngles + "th";
				}

				plantilla.TituloEn = string.Format(Idiomas.BuscarTexto("en", "Goodbye1", "NewsTemplates"), juego.Nombre, Idiomas.BuscarTexto("en", "Month." + fecha.Month.ToString(), "Months"), diaIngles);
				plantilla.TituloEs = string.Format(Idiomas.BuscarTexto("es", "Goodbye1", "NewsTemplates"), juego.Nombre, Idiomas.BuscarTexto("es", "Month." + fecha.Month.ToString(), "Months"), fecha.Day);

				plantilla.ContenidoEn = "<div>" + string.Format(Idiomas.BuscarTexto("en", "Goodbye2", "NewsTemplates"), juego.Nombre, enlace) + "</div>";
				plantilla.ContenidoEs = "<div>" + string.Format(Idiomas.BuscarTexto("es", "Goodbye2", "NewsTemplates"), juego.Nombre, enlace) + "</div>";

				plantilla.ContenidoEn = plantilla.ContenidoEn + Environment.NewLine + Environment.NewLine + "<div style=" + Strings.ChrW(34) + "margin-top: 20px;" + Strings.ChrW(34) + ">" + string.Format(Idiomas.BuscarTexto("en", "Goodbye3", "NewsTemplates"), "https://" + dominio + "/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/");
				plantilla.ContenidoEs = plantilla.ContenidoEs + Environment.NewLine + Environment.NewLine + "<div style=" + Strings.ChrW(34) + "margin-top: 20px;" + Strings.ChrW(34) + ">" + string.Format(Idiomas.BuscarTexto("es", "Goodbye3", "NewsTemplates"), "https://" + dominio + "/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/");

				if (juego.Imagenes.Header_460x215 != null)
				{
					plantilla.Imagen = juego.Imagenes.Header_460x215;
				}
			}

			return plantilla;
		}
    }

    public class Plantilla
    {
        public string TituloEn;
        public string TituloEs;
        public string ContenidoEn;
        public string ContenidoEs;
        public string Juegos;
        public DateTime Fecha;
        public string Enlace;
        public string Imagen;
        public string BundleId;
        public string GratisIds;
        public string SuscripcionesIds;
    }
}
