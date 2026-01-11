#nullable disable

using Juegos;
using Tiendas2;

namespace Herramientas.RedesSociales
{
	public static class Reddit
	{
		public static async Task<bool> Postear(IConfiguration configuracion, Noticias.Noticia noticia, string dominio)
		{
			string cuenta = configuracion.GetValue<string>("Reddit:Cuenta");
			string contraseña = configuracion.GetValue<string>("Reddit:Contraseña");
			string clientId = configuracion.GetValue<string>("Reddit:ClientID");
			string clientSecret = configuracion.GetValue<string>("Reddit:ClientSecret");

            var webAgent = new Reddit2.BotWebAgent(cuenta, contraseña, clientId, clientSecret, "https://" + dominio + "/");          
            var reddit = new RedditSharp.Reddit(webAgent, false);
            
			RedditSharp.Things.Subreddit subreddit = await reddit.GetSubredditAsync("gamedealsue");

			if (subreddit != null)
			{
                string titulo = string.Empty;
                string texto = string.Empty;

                if (noticia.NoticiaTipo == Noticias.NoticiaTipo.Bundles)
                {
                    titulo = "[Bundle] " + noticia.TituloEn;

                    var bundle = await global::BaseDatos.Bundles.Buscar.UnBundle(noticia.BundleId);
					texto = await Herramientas.Bundles.Reddit.Generar(bundle);
                }

                if (noticia.NoticiaTipo == Noticias.NoticiaTipo.Gratis)
                {
                    titulo = "[Free] " + noticia.TituloEn;

                    List<string> ids = Herramientas.Listados.Generar(noticia.GratisIds);
                    List<Juegos.JuegoGratis> juegosGratis = new List<Juegos.JuegoGratis>();

                    if (ids?.Count > 0)
                    {
                        foreach (var id in ids)
                        {
                            var gratis = await global::BaseDatos.Gratis.Buscar.UnGratis(id);

                            if (gratis != null)
                            {
                                juegosGratis.Add(gratis);
                            }
                        }
                    }

                    texto = await Gratis(juegosGratis);
                }

                if (noticia.NoticiaTipo == Noticias.NoticiaTipo.Suscripciones)
                {
                    titulo = "[Subscriptions] " + noticia.TituloEn;

                    List<string> ids = Herramientas.Listados.Generar(noticia.SuscripcionesIds);
                    List<Juegos.JuegoSuscripcion> juegosSuscripciones = new List<Juegos.JuegoSuscripcion>();

                    if (ids?.Count > 0)
                    {
                        foreach (var id in ids)
                        {
                            var suscripcion = await global::BaseDatos.Suscripciones.Buscar.Id(int.Parse(id));

                            if (suscripcion != null)
                            {
                                juegosSuscripciones.Add(suscripcion);
                            }
                        }
                    }

                    texto = await Suscripciones(juegosSuscripciones);
                }
               
                if (string.IsNullOrEmpty(texto) == false)
                {
                    try
                    {
                        RedditSharp.Things.Post post = await subreddit.SubmitTextPostAsync(titulo, texto);
                        
                        if (post != null)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex) 
                    {
                        global::BaseDatos.Errores.Insertar.Mensaje("Reddit Postear Noticia", ex);
                    }
                }
			}

			return false;
		}

        public static async Task<string> Gratis(List<JuegoGratis> gratis)
        {
            string texto = null;

            if (gratis?.Count > 0)
            {
                foreach (var gratis2 in gratis)
                {
                    texto = texto + "[" + gratis2.Enlace + "](" + gratis2.Enlace + ")" + Environment.NewLine + Environment.NewLine;

                    if (gratis2.FechaTermina >= DateTime.Now)
                    {
                        texto = texto + "You can claim it for free until " + gratis2.FechaTermina.ToString("m") + ", also other data:" + Environment.NewLine + Environment.NewLine;

                        Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(gratis2.JuegoId);

                        if (juego != null)
                        {
                            if (juego.Analisis != null)
                            {
                                texto = texto + "* It has an " + juego.Analisis.Porcentaje + "% rating on Steam with " + juego.Analisis.Cantidad + " reviews." + Environment.NewLine;
                            }

                            List<int> bundlesActivos = new List<int>();
                            List<int> bundlesViejunos = new List<int>();

                            if (juego.Bundles?.Count > 0)
                            {
                                foreach (var bundle in juego.Bundles)
                                {
                                    if (bundle.FechaEmpieza <= DateTime.Now && bundle.FechaTermina >= DateTime.Now)
                                    {
                                        bundlesActivos.Add(bundle.BundleId);
                                    }
                                    else
                                    {
                                        bundlesViejunos.Add(bundle.BundleId);
                                    }
                                }
                            }

                            if (bundlesActivos.Count > 0)
                            {
                                foreach (var bundle in bundlesActivos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* It's in the bundle: [" + bundle2.Nombre + " • " + bundle2.Tienda + "](" + bundle2.Enlace + ")" + Environment.NewLine;
                                    }
                                }
                            }

							List<string> bundlesViejunosTabla = new List<string>();

							if (bundlesViejunos.Count > 0)
                            {
                                foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
										bundlesViejunosTabla.Add($"| {bundle2.Nombre} | {bundle2.Tienda} | {Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en")} |");
									}
                                }
                            }

                            if (bundlesViejunosTabla.Count > 0)
                            {
								texto += Environment.NewLine + "**Old Bundles:**" + Environment.NewLine;
								texto += "| Name | Store | Time |" + Environment.NewLine + "|---|---|---|" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, bundlesViejunosTabla) + Environment.NewLine;
							}
                        }



                        if (juego.Suscripciones?.Count > 0)
                        {
							List<string> suscripcionesViejunasTabla = new List<string>();

							foreach (var suscripcion in juego.Suscripciones)
                            {
                                if (suscripcion.FechaEmpieza <= DateTime.Now && suscripcion.FechaTermina >= DateTime.Now)
                                {
                                    texto = texto + "* It's in the subscription: [" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + "](" + suscripcion.Enlace + ")" + Environment.NewLine;
                                }
                                else
                                {
									suscripcionesViejunasTabla.Add($"| {Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre} | {suscripcion.Enlace} | {Calculadora.DiferenciaTiempo(suscripcion.FechaEmpieza, "en")} |");
								}
                            }

							if (suscripcionesViejunasTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Subscriptions:**" + Environment.NewLine;
								texto += "| Name | Link | Time |" + Environment.NewLine + "|---|---|---|" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, suscripcionesViejunasTabla) + Environment.NewLine;
							}
						}

						texto = texto + Environment.NewLine + Environment.NewLine;
					}
                }
            }

            return texto;
        }

        public static async Task<string> Suscripciones(List<JuegoSuscripcion> suscripciones)
        {
            string texto = null;

            if (suscripciones?.Count > 0)
            {
                foreach (var suscripcion2 in suscripciones)
                {
                    texto = texto + "[" + suscripcion2.Nombre + "](" + suscripcion2.Enlace + ") - " + JuegoDRM2.DevolverDRM(suscripcion2.DRM) + Environment.NewLine + Environment.NewLine;

                    if (suscripcion2.FechaTermina >= DateTime.Now)
                    {
                        Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(suscripcion2.JuegoId);

                        if (juego != null)
                        {
                            if (juego.Analisis?.Porcentaje.Length > 1)
                            {
                                string reseñasTemp = juego.Analisis?.Cantidad;
                                reseñasTemp = reseñasTemp.Replace(",", null);
                                int reseñasTemp2 = int.Parse(reseñasTemp);

                                texto = texto + "> It has an " + juego.Analisis.Porcentaje + "% rating on Steam with " + reseñasTemp2.ToString("N0").Replace(",", ".") + " reviews." + Environment.NewLine;
                            }

                            List<int> bundlesActivos = new List<int>();
                            List<int> bundlesViejunos = new List<int>();

                            if (juego.Bundles?.Count > 0)
                            {
                                foreach (var bundle in juego.Bundles)
                                {
                                    if (bundle.FechaEmpieza <= DateTime.Now && bundle.FechaTermina >= DateTime.Now)
                                    {
                                        bundlesActivos.Add(bundle.BundleId);
                                    }
                                    else
                                    {
                                        bundlesViejunos.Add(bundle.BundleId);
                                    }
                                }
                            }

                            if (bundlesActivos.Count > 0)
                            {
                                foreach (var bundle in bundlesActivos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* It's in the bundle: [" + bundle2.Nombre + " • " + bundle2.Tienda + "](" + bundle2.Enlace + ")" + Environment.NewLine;
                                    }
                                }
                            }

                            if (bundlesViejunos.Count > 0)
                            {
								List<string> bundlesViejunosTabla = new List<string>();

								foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
										bundlesViejunosTabla.Add($"{bundle2.Nombre}|{bundle2.Tienda}|{Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en")}");
									}
                                }

								if (bundlesViejunosTabla.Count > 0)
								{
									texto += Environment.NewLine + "**Old Bundles:**" + Environment.NewLine + Environment.NewLine;
									texto += "Name|Store|Time" + Environment.NewLine + "---|---|---" + Environment.NewLine;
									texto += string.Join(Environment.NewLine, bundlesViejunosTabla) + Environment.NewLine;
								}
							}
                        }

                        if (juego.Gratis?.Count > 0)
                        {
							List<string> gratisViejunosTabla = new List<string>();

							foreach (var gratis in juego.Gratis)
                            {
                                if (gratis.FechaEmpieza <= DateTime.Now && gratis.FechaTermina >= DateTime.Now)
                                {
                                    texto = texto + "* It's free in: [" + Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre + "](" + gratis.Enlace + ")" + Environment.NewLine;
                                }
                                else
                                {
									gratisViejunosTabla.Add($"{Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre}|{Calculadora.DiferenciaTiempo(gratis.FechaEmpieza, "en")}");
								}
                            }

							if (gratisViejunosTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Free:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Time" + Environment.NewLine + "---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, gratisViejunosTabla) + Environment.NewLine;
							}
						}

						if (juego.Suscripciones?.Count > 0)
						{
							List<string> suscripcionesViejunasTabla = new List<string>();

							foreach (var suscripcion in juego.Suscripciones)
							{
								if (suscripcion.FechaEmpieza <= DateTime.Now && suscripcion.FechaTermina >= DateTime.Now)
								{
									texto = texto + "* It's in the subscription: [" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + "](" + suscripcion.Enlace + ")" + Environment.NewLine;
								}
								else if (suscripcion.FechaEmpieza != DateTime.MinValue)
								{
									suscripcionesViejunasTabla.Add($"{Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre}|{Calculadora.DiferenciaTiempo(suscripcion.FechaEmpieza, "en")}");
								}
							}

							if (suscripcionesViejunasTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Subscriptions:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Time" + Environment.NewLine + "---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, suscripcionesViejunasTabla) + Environment.NewLine;
							}
						}
					}

                    texto = texto + Environment.NewLine + Environment.NewLine;
                }
            }

            return texto;
        }

        public static async Task PostearOfertasDia(IConfiguration configuracion, List<Juego> juegos)
        {
            string cuenta = configuracion.GetValue<string>("Reddit:Cuenta");
            string contraseña = configuracion.GetValue<string>("Reddit:Contraseña");
            string clientId = configuracion.GetValue<string>("Reddit:ClientID");
            string clientSecret = configuracion.GetValue<string>("Reddit:ClientSecret");

            var webAgent = new Reddit2.BotWebAgent(cuenta, contraseña, clientId, clientSecret, "https://pepeizqdeals.com/");
            var reddit = new RedditSharp.Reddit(webAgent, false);

            RedditSharp.Things.Subreddit subreddit = await reddit.GetSubredditAsync("gamedealsue");

            string titulo = "Today's Deals [" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + "]";
            string texto = null;

            if (juegos?.Count > 0)
            {
				Dictionary<string, List<Juego>> juegosPorTienda = new Dictionary<string, List<Juego>>();
				List<Tienda> tiendas = Tiendas2.TiendasCargar.GenerarListado();

                foreach (var juego in juegos)
                {
					var tiendaJuego = tiendas.FirstOrDefault(t => t.Id == juego.PrecioMinimosHistoricos[0].Tienda);
					string nombreTienda = tiendaJuego.Nombre;

					if (juegosPorTienda.ContainsKey(nombreTienda) == false)
                    {
						juegosPorTienda[nombreTienda] = new List<Juego>();
					}

					juegosPorTienda[nombreTienda].Add(juego);
				}

				foreach (var juegosTienda in juegosPorTienda)
                {
					string tienda = juegosTienda.Key;
					List<Juego> listaJuegos = juegosTienda.Value;

                    if (listaJuegos?.Count > 0)
                    {
                        listaJuegos = listaJuegos.OrderBy(t => t.Nombre).ToList();

                        string codigo = string.Empty;

                        if (string.IsNullOrEmpty(listaJuegos[0]?.PrecioMinimosHistoricos[0]?.CodigoTexto) == false)
                        {
                            codigo = Environment.NewLine + ">Code: **" + listaJuegos[0]?.PrecioMinimosHistoricos[0]?.CodigoTexto + "**";
						}

                        texto = texto + tienda + codigo + Environment.NewLine;

						foreach (var juego in listaJuegos)
                        {
							string precio = Herramientas.Precios.Euro(juego.PrecioMinimosHistoricos[0].Precio);

							if (juego.PrecioMinimosHistoricos[0].PrecioCambiado > 0)
							{
								precio = Herramientas.Precios.Euro(juego.PrecioMinimosHistoricos[0].PrecioCambiado);

							}

							texto = texto + "* [" + juego.Nombre + " • " + juego.PrecioMinimosHistoricos[0].Descuento.ToString() + "% • " + precio + "](" + juego.PrecioMinimosHistoricos[0].Enlace + ")" + Environment.NewLine;
						}

						texto = texto + Environment.NewLine + "---" + Environment.NewLine + Environment.NewLine;
					}
				}
            }

			if (string.IsNullOrEmpty(texto) == false)
			{
				try
				{
					await subreddit.SubmitTextPostAsync(titulo, texto);
				}
				catch (Exception ex)
				{
					global::BaseDatos.Errores.Insertar.Mensaje("Reddit Ofertas Dia", ex);
				}
			}
		}
    }
}


