#nullable disable

using Juegos;

namespace Herramientas.RedesSociales
{
	public static class BBCode
	{
		public static string Bundle(string idioma, Bundles2.Bundle bundle, bool elotrolado)
		{
			string texto = null;

			if (bundle != null)
			{
				if (string.IsNullOrEmpty(bundle.ImagenNoticia) == false)
				{
                    texto = "[url=" + bundle.Enlace + "][img]" + bundle.ImagenNoticia + "[/img][/url]" + Environment.NewLine + Environment.NewLine;
                }
                
				texto = texto + "[url=" + bundle.Enlace + "]" + bundle.Enlace + "[/url]" + Environment.NewLine + Environment.NewLine;

                if (bundle.Tiers != null)
                {
                    bundle.Tiers.Sort(delegate (Bundles2.BundleTier t1, Bundles2.BundleTier t2)
                    {
                        return t1.Posicion.CompareTo(t2.Posicion);
                    });

                    decimal totalMinimos = 0;

                    foreach (var tier in bundle.Tiers)
                    {
                        List<Bundles2.BundleJuego> juegosTier = new List<Bundles2.BundleJuego>();

						if (bundle.Juegos?.Count > 0)
						{
							foreach (var juego in bundle.Juegos)
							{
								if (juego.Tier != null)
								{
									if (juego.Tier.Posicion == tier.Posicion)
									{
										juegosTier.Add(juego);
									}
								}
							}
						}

						if (juegosTier.Count > 0)
                        {
                            juegosTier = juegosTier.OrderBy(x => x.Nombre).ToList();
                        }

                        foreach (var juego in juegosTier)
                        {
                            if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
                            {
                                if (string.IsNullOrEmpty(juego.Juego?.Maestro) == false)
                                {
                                    if (juego.Juego?.Maestro != "no")
                                    {
                                        foreach (var juego2 in juegosTier)
                                        {
                                            if (juego2.JuegoId == juego.Juego.Maestro)
                                            {
                                                if (juego2.DLCs == null)
                                                {
                                                    juego2.DLCs = new List<string>();
                                                }

                                                bool añadir = true;

                                                if (juego2.DLCs.Count > 0)
                                                {
                                                    foreach (var dlc in juego2.DLCs)
                                                    {
                                                        if (dlc == juego.JuegoId)
                                                        {
                                                            añadir = false;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (añadir == true)
                                                {
                                                    juego2.DLCs.Add(juego.JuegoId);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (bundle.Pick == true)
                        {
                            if (tier.Posicion == 1)
                            {
                                foreach (var tier2 in bundle.Tiers)
                                {
                                    if (tier2.CantidadJuegos == 1)
                                    {
                                        texto = texto + "[b]" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto(idioma, "String21", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "[/b]" + Environment.NewLine;
                                    }
                                    else if (tier2.CantidadJuegos > 1)
                                    {
                                        texto = texto + "[b]" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto(idioma, "String8", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "[/b] / " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio) / tier2.CantidadJuegos) + " (" + Herramientas.Idiomas.BuscarTexto(idioma, "String20", "Bundle") + ")" + Environment.NewLine;
                                    }
                                }

                                texto = texto + Environment.NewLine;
                            }
                        }
                        else
                        {
                            texto = texto + "[b]Tier " + tier.Posicion.ToString() + ": " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio)) + "[/b]" + Environment.NewLine;

                            foreach (var juego in juegosTier)
                            {
                                if (juego.Juego == null)
                                {
                                    juego.Juego = global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
                                }

                                if (juego.Juego?.PrecioMinimosHistoricos != null)
                                {
                                    foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
                                    {
                                        if (historico.DRM == juego.DRM)
                                        {
                                            if (historico.PrecioCambiado > 0)
                                            {
                                                totalMinimos = totalMinimos + historico.PrecioCambiado;
                                            }
                                            else
                                            {
                                                totalMinimos = totalMinimos + historico.Precio;
                                            }

                                            break;
                                        }
                                    }
                                }
                            }

                            texto = texto + Herramientas.Idiomas.BuscarTexto(idioma, "String14", "Bundle") + ": " + Herramientas.Precios.Euro(totalMinimos) + Environment.NewLine + Environment.NewLine;
                        }

                        if (juegosTier.Count > 0)
                        {
                            texto = texto + "[list]";

                            foreach (var juego in juegosTier)
                            {
                                if (juego.Juego == null)
                                {
                                    juego.Juego = global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
                                }

                                if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
                                {
                                    if (string.IsNullOrEmpty(juego.Juego?.Maestro) == false)
                                    {
                                        if (juego.Juego?.Maestro != "no")
                                        {
                                            foreach (var juego2 in juegosTier)
                                            {
                                                if (juego2.JuegoId == juego.Juego?.Maestro)
                                                {
                                                    if (juego2.DLCs == null)
                                                    {
                                                        juego2.DLCs = new List<string>();
                                                    }

                                                    bool añadir = true;

                                                    if (juego2.DLCs.Count > 0)
                                                    {
                                                        foreach (var dlc in juego2.DLCs)
                                                        {
                                                            if (dlc == juego.JuegoId)
                                                            {
                                                                añadir = false;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (añadir == true)
                                                    {
                                                        juego2.DLCs.Add(juego.JuegoId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (var juego in juegosTier)
                            {
                                bool mostrar = true;

                                if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
                                {
                                    if (string.IsNullOrEmpty(juego.Juego.Maestro) == false)
                                    {
                                        if (juego.Juego.Maestro != "no")
                                        {
                                            foreach (var juego2 in juegosTier)
                                            {
                                                if (juego2.JuegoId == juego.Juego.Maestro)
                                                {
                                                    mostrar = false;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (mostrar == true)
                                {
                                    string nombre = juego.Nombre;
                                    string precioMinimo = null;

                                    if (juego.Juego != null)
                                    {
                                        if (juego.DRM == Juegos.JuegoDRM.Steam && juego.Juego.IdSteam > 0)
                                        {
                                            nombre = "[url=https://store.steampowered.com/app/" + juego.Juego.IdSteam + "]" + nombre + "[/url]";
                                        }
                                        else if (juego.DRM == Juegos.JuegoDRM.GOG && string.IsNullOrEmpty(juego.Juego.SlugGOG) == false)
                                        {
                                            nombre = "[url=https://www.gog.com/game/" + juego.Juego.SlugGOG + "]" + nombre + "[/url]";
                                        }
                                        else if (juego.DRM == Juegos.JuegoDRM.Epic && string.IsNullOrEmpty(juego.Juego.SlugEpic) == false)
                                        {
                                            nombre = "[url=https://www.epicgames.com/store/p/" + juego.Juego.SlugEpic + "]" + nombre + "[/url]";
                                        }

                                        if (juego.Juego.PrecioMinimosHistoricos != null)
                                        {
                                            decimal precioMinimoDecimal = 0;
                                            decimal precioMinimoComparar = 10000;

                                            foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
                                            {
                                                if (historico.DRM == juego.DRM)
                                                {
                                                    if (historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoComparar)
                                                    {
                                                        precioMinimoComparar = historico.PrecioCambiado;
                                                    }
                                                    else if (historico.Precio > 0 && historico.Precio < precioMinimoComparar)
                                                    {
                                                        precioMinimoComparar = historico.Precio;
                                                    }
                                                }
                                            }

                                            if (precioMinimoComparar < 10000)
                                            {
                                                precioMinimoDecimal = precioMinimoComparar;
                                            }     

                                            if (juego.DLCs?.Count > 0)
                                            {
                                                foreach (var dlc in juego.DLCs)
                                                {
                                                    Juegos.Juego juegoDLC = global::BaseDatos.Juegos.Buscar.UnJuego(dlc);

                                                    if (juegoDLC?.PrecioMinimosHistoricos != null)
                                                    {
                                                        decimal precioMinimoDLCComparar = 10000;

                                                        foreach (var historico in juegoDLC.PrecioMinimosHistoricos)
                                                        {
                                                            if (historico.DRM == juego.DRM)
                                                            {
                                                                if (historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoDLCComparar)
                                                                {
                                                                    precioMinimoDLCComparar = historico.PrecioCambiado;
                                                                }
                                                                else if (historico.Precio > 0 && historico.Precio < precioMinimoDLCComparar)
                                                                {
                                                                    precioMinimoDLCComparar = historico.Precio;
                                                                }
                                                            }
                                                        }

                                                        if (precioMinimoDLCComparar < 10000)
                                                        {
                                                            precioMinimoDecimal = precioMinimoDecimal + precioMinimoDLCComparar;
                                                        }
                                                    }
                                                }
                                            }

                                            if (precioMinimoDecimal > 0)
                                            {
                                                precioMinimo = Herramientas.Precios.Euro(precioMinimoDecimal);
                                            }
                                        }
                                    }

                                    if (string.IsNullOrEmpty(precioMinimo) == true)
                                    {
                                        texto = texto + "[*]" + nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ")";
                                    }
                                    else
                                    {
                                        texto = texto + "[*]" + nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ") (" + precioMinimo + ")";
                                    }    

                                    if (juego.DLCs?.Count > 0)
                                    {
                                        if (juego.DLCs.Count == 1)
                                        {
                                            texto = texto + " +" + juego.DLCs?.Count.ToString() + " DLC";
                                        }
                                        else if (juego.DLCs.Count > 1)
                                        {
                                            texto = texto + " +" + juego.DLCs?.Count.ToString() + " DLCs";
                                        }
                                    }

									bool añadirNuevo = false;

									if (juego.Juego.Bundles?.Count == 1)
									{
										añadirNuevo = true;
									}

									if (juego.Juego.Suscripciones?.Count > 0)
									{
										foreach (var suscripcion in juego.Juego.Suscripciones)
										{
											if (suscripcion.DRM == juego.DRM)
											{
												añadirNuevo = false;
												break;
											}
										}
									}

									if (añadirNuevo == true)
                                    {
                                        texto = texto + " • [b]" + Herramientas.Idiomas.BuscarTexto(idioma, "String24", "Bundle") + "[/b]";
                                    }

                                    texto = texto + Environment.NewLine;
                                }
                            }

                            texto = texto + "[/list]" + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }

                if (elotrolado == true)
                {
                    texto = texto + ElOtroLado(bundle);
				}
			}

			return texto;
		}

        public static string ElOtroLado(Bundles2.Bundle bundle)
        {
            string texto = null;

			if (bundle.Pick == false)
            {
                double precioBundle = double.Parse(bundle.Tiers.LastOrDefault().Precio);

                List<JuegoValoracion> juegosValoracion = new List<JuegoValoracion>();

                //Carga inicial de los juegos
                foreach (var juego in bundle.Juegos)
                {
                    JuegoValoracion juegoValoracion = new JuegoValoracion();
                    juegoValoracion.Nombre = juego.Nombre;
                    juegoValoracion.DRM = juego.DRM;

                    if (juego.Juego == null)
                    {
						juego.Juego = global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId); // Cargo datos de la web en caso de ser tan subnormal de no haberlos cargados llegados a este punto
					}
                    
                    if (juego.Juego?.PrecioMinimosHistoricos != null)
                    {
                        foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
                        {
                            if (historico.DRM == juego.DRM)
                            {
                                if (historico.PrecioCambiado > 0)
                                {
                                    juegoValoracion.PrecioMinimoHistorico = (double)historico.PrecioCambiado;
                                }
                                else
                                {
                                    juegoValoracion.PrecioMinimoHistorico = (double)historico.Precio;
                                }

                                break;
							}
                        }
                    }

                    if (juego.Juego?.Bundles != null)
                    {
						juegoValoracion.NumeroBundles = juego.Juego.Bundles.Count - 1; // Excluyo el bundle actual

                        if (juego.Juego.Suscripciones?.Count > 0)
                        {
                            foreach (var suscripcion in juego.Juego.Suscripciones)
                            {
                                if (suscripcion.DRM == JuegoDRM.Steam && suscripcion.Tipo == Suscripciones2.SuscripcionTipo.HumbleChoice)
                                {
                                    juegoValoracion.NumeroBundles = juegoValoracion.NumeroBundles + 1; // Sumo Humble Choice como si fuera un bundle extra
                                    break;
                                }
                            }
                        }
					}

                    if (juego.Juego?.Analisis != null)
                    {
                        if (string.IsNullOrEmpty(juego.Juego.Analisis?.Cantidad) == false)
                        {
							juegoValoracion.NumeroReseñas = int.Parse(juego.Juego.Analisis.Cantidad.Replace(",", null));
						}
                    }

                    if (juego.Juego?.Caracteristicas != null)
                    {
                        juegoValoracion.FechaSalida = juego.Juego.Caracteristicas.FechaLanzamientoSteam;
					}

                    juegoValoracion.Tier = juego.Tier.Posicion;

					juegosValoracion.Add(juegoValoracion);
				}

                if (juegosValoracion.Count > 0)
                {
                    int cantidadReseñasMinima = juegosValoracion.Min(j => j.NumeroReseñas);
                    int cantidadReseñasMaxima = juegosValoracion.Max(j => j.NumeroReseñas);

					// Primer cálculo para estimar el valor de los juegos (mirar abajo)
					foreach (var juego in juegosValoracion)
					{
						juego.Valoracion = CalcularValoracionJuego(juego, cantidadReseñasMinima, cantidadReseñasMaxima);
					}

					// Segundo cálculo para encajar la suma total de valoraciones y que no supere el precio mínimo histórico en cada juego
					double sumaValoraciones = juegosValoracion.Sum(j => j.Valoracion);
					foreach (var juego in juegosValoracion)
					{
						juego.Valoracion = juego.Valoracion / sumaValoraciones * precioBundle;

						if (juego.Valoracion > juego.PrecioMinimoHistorico)
                        {
							juego.Valoracion = juego.PrecioMinimoHistorico;
						}
					}

					// Tercer cálculo donde voy a aplicar un bucle hasta que se ajusten las valoraciones al precio del bundle (limitado a 10000 pasadas), si la cago es aquí
					bool sigueSobrando = true;
                    int i = 0;
					while (sigueSobrando == true && i < 10000)
					{
						sigueSobrando = false;
                        i += 1;

						double sumaFinal = juegosValoracion.Sum(j => j.Valoracion);
						double sobrante = precioBundle - sumaFinal;

						if (sobrante > 0.00001)
						{
							List<JuegoValoracion> juegosDondeAplicarRestante = juegosValoracion.Where(j => j.Valoracion < j.PrecioMinimoHistorico).ToList();

                            if (juegosDondeAplicarRestante.Count == 0)
                            {
                                break;
                            }

							sigueSobrando = true;

							double cantidadARepartir = juegosDondeAplicarRestante.Sum(j => CalcularValoracionJuego(j, cantidadReseñasMinima, cantidadReseñasMaxima));

							foreach (JuegoValoracion juego in juegosDondeAplicarRestante)
							{
								double extra = CalcularValoracionJuego(juego, cantidadReseñasMinima, cantidadReseñasMaxima) / cantidadARepartir * sobrante;
								juego.Valoracion = Math.Min(juego.Valoracion + extra, juego.PrecioMinimoHistorico);
							}
						}
					}

                    // Generar el BBCode para el foro

                    texto = texto + "BBCode para el hilo de compras conjuntas:[spoiler][code]";
                    texto = texto + "[url=" + bundle.Enlace + "]" + bundle.Nombre + "[/url] • " + Herramientas.Precios.Euro(precioBundle) + Environment.NewLine + Environment.NewLine;
                    texto = texto + "[list]";

					foreach (var juego in juegosValoracion.OrderBy(j => j.Nombre))
                    {
                        texto = texto + "[*]" + juego.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + "): " + Herramientas.Precios.Euro((decimal)juego.Valoracion) + " - Libre" + Environment.NewLine;
                    }

					texto = texto + "[/list]";
					texto = texto + "[/code][/spoiler]";
				}
			}
			else
			{
				texto = texto + "BBCode para el hilo de compras conjuntas:[spoiler][code]";
				texto = texto + "[url=" + bundle.Enlace + "]" + bundle.Nombre + "[/url]" + Environment.NewLine + Environment.NewLine;

				foreach (var tier in bundle.Tiers)
				{
					if (tier.CantidadJuegos == 1)
					{
						texto = texto + "[b]" + tier.CantidadJuegos.ToString() + " juego • " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio)) + "[/b]" + Environment.NewLine;
					}
					else if (tier.CantidadJuegos > 1)
					{
						texto = texto + "[b]" + tier.CantidadJuegos.ToString() + " juegos • " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio)) + "[/b] / " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio) / tier.CantidadJuegos) + " (cada unidad)" + Environment.NewLine;
					}
				}

				texto = texto + Environment.NewLine + "[list]";

				foreach (var juego in bundle.Juegos.OrderBy(j => j.Nombre))
				{
					texto = texto + "[*]" + juego.Nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ") - Libre" + Environment.NewLine;
				}

				texto = texto + "[/list]";
				texto = texto + "[/code][/spoiler]";
			}

			return texto;
		}

        public static double CalcularValoracionJuego(JuegoValoracion juego, int minimoReseñas, int maximoReseñas)
        {
			// Valor que asigno en función de si el juego ha salido hace pocos meses

			double valorFecha = 1;

            if (juego.FechaSalida != null)
            {
                double meses = (DateTime.Now - juego.FechaSalida.Value).TotalDays / 30;

                double meses2 = Math.Min(meses, 120); // Máximo de 10 años (por si acaso)

                valorFecha = 2 - (meses2 / 120);
            }

			// Valor que asigno en función del tier del bundle en el que está el juego

			double valorTier = 1;
            valorTier = valorTier + (juego.Tier * 0.5);

            // Valor que asigno a las reseñas que oscila entre el juego con menor cantidad de reseñas y el mayor

            double valorReseñas = 1;
            valorReseñas = (juego.NumeroReseñas - minimoReseñas) / (maximoReseñas - minimoReseñas);
            valorReseñas = valorReseñas * 2;

			// Junto todo quitando peso en función de los bundles en los que ha estado

			return (valorReseñas + valorFecha + valorTier) / (double)(juego.NumeroBundles + 1);
		}
	}

    public class JuegoValoracion
    {
        public string Nombre { get; set; }
        public JuegoDRM DRM { get; set; }
        public double PrecioMinimoHistorico { get; set; }
        public int NumeroBundles { get; set; }
        public int NumeroReseñas { get; set; }
        public double Valoracion { get; set; }
        public DateTime? FechaSalida { get; set; }
        public int Tier { get; set; }
	}
}
