#nullable disable

using Juegos;

namespace Herramientas.RedesSociales
{
	public static class Reddit
	{
		public static async Task<bool> Postear(Noticias.Noticia noticia, string dominio)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder();
			string cuenta = builder.Configuration.GetValue<string>("Reddit:Cuenta");
			string contraseña = builder.Configuration.GetValue<string>("Reddit:Contraseña");
			string clientId = builder.Configuration.GetValue<string>("Reddit:ClientID");
			string clientSecret = builder.Configuration.GetValue<string>("Reddit:ClientSecret");

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
                    texto = Bundle(global::BaseDatos.Bundles.Buscar.UnBundle(noticia.BundleId));
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
                            var gratis = global::BaseDatos.Gratis.Buscar.UnGratis(id);

                            if (gratis != null)
                            {
                                juegosGratis.Add(gratis);
                            }
                        }
                    }

                    texto = Gratis(juegosGratis);
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
                            var suscripcion = global::BaseDatos.Suscripciones.Buscar.Id(int.Parse(id));

                            if (suscripcion != null)
                            {
                                juegosSuscripciones.Add(suscripcion);
                            }
                        }
                    }

                    texto = Suscripciones(juegosSuscripciones);
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

        public static string Bundle(Bundles2.Bundle bundle)
        {
            string texto = null;

            if (bundle != null)
            {
                texto = "[" + bundle.Enlace + "](" + bundle.Enlace + ")" + Environment.NewLine + Environment.NewLine;

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

                        if (bundle.Juegos != null)
                        {
                            if (bundle.Juegos.Count > 0)
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
                                        texto = texto + "**" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto("en", "String21", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "**" + Environment.NewLine;
                                    }
                                    else if (tier2.CantidadJuegos > 1)
                                    {
                                        texto = texto + "**" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto("en", "String8", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "** / " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio) / tier2.CantidadJuegos) + " (" + Herramientas.Idiomas.BuscarTexto("en", "String20", "Bundle") + ")" + Environment.NewLine;
                                    }
                                }

                                texto = texto + Environment.NewLine;
                            }
                        }
                        else
                        {
                            texto = texto + "**Tier " + tier.Posicion.ToString() + ": " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio)) + "**  " + Environment.NewLine;

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

                            texto = texto + Herramientas.Idiomas.BuscarTexto("en", "String14", "Bundle") + ": " + Herramientas.Precios.Euro(totalMinimos) + Environment.NewLine + Environment.NewLine;
                        }

                        if (juegosTier.Count > 0)
                        {
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
                                            nombre = "[" + nombre + "](https://store.steampowered.com/app/" + juego.Juego.IdSteam + ")";
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
                                        texto = texto + "* " + nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ")";
                                    }
                                    else
                                    {
                                        texto = texto + "* " + nombre + " (" + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + ") (" + precioMinimo + ")";
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

                                    if (juego.Juego.Bundles.Count == 1)
                                    {
                                        texto = texto + " • **" + Herramientas.Idiomas.BuscarTexto("en", "String24", "Bundle") + "**";
                                    }

                                    texto = texto + Environment.NewLine;
                                }
                            }

                            texto = texto + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }
            }

            return texto;
        }

        public static string Gratis(List<JuegoGratis> gratis)
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

                        Juego juego = global::BaseDatos.Juegos.Buscar.UnJuego(gratis2.JuegoId);

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
                                    Bundles2.Bundle bundle2 = global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* It's in the bundle: [" + bundle2.NombreBundle + " • " + bundle2.NombreTienda + "](" + bundle2.Enlace + ")" + Environment.NewLine;
                                    }
                                }
                            }

                            if (bundlesViejunos.Count > 0)
                            {
                                foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* It was in the bundle: " + bundle2.NombreBundle + " • " + bundle2.NombreTienda + " (" + Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en") + ")" + Environment.NewLine;
                                    }
                                }
                            }
                        }

                        if (juego.Suscripciones?.Count > 0)
                        {
                            foreach (var suscripcion in juego.Suscripciones)
                            {
                                if (suscripcion.FechaEmpieza <= DateTime.Now && suscripcion.FechaTermina >= DateTime.Now)
                                {
                                    texto = texto + "* It's in the subscription: [" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + "](" + suscripcion.Enlace + ")" + Environment.NewLine;
                                }
                                else
                                {
                                    texto = texto + "* It was in the subscription: " + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + " (" + Calculadora.DiferenciaTiempo(suscripcion.FechaEmpieza, "en") + ")" + Environment.NewLine;
                                }
                            }
                        }

						texto = texto + Environment.NewLine + Environment.NewLine;
					}
                }
            }

            return texto;
        }

        public static string Suscripciones(List<JuegoSuscripcion> suscripciones)
        {
            string texto = null;

            if (suscripciones?.Count > 0)
            {
                foreach (var suscripcion2 in suscripciones)
                {
                    texto = texto + "[" + suscripcion2.Nombre + "](" + suscripcion2.Enlace + ")" + Environment.NewLine + Environment.NewLine;

                    if (suscripcion2.FechaTermina >= DateTime.Now)
                    {
                        Juego juego = global::BaseDatos.Juegos.Buscar.UnJuego(suscripcion2.JuegoId);

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
                                    Bundles2.Bundle bundle2 = global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* It's in the bundle: [" + bundle2.NombreBundle + " • " + bundle2.NombreTienda + "](" + bundle2.Enlace + ")" + Environment.NewLine;
                                    }
                                }
                            }

                            if (bundlesViejunos.Count > 0)
                            {
                                foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* It was in the bundle: " + bundle2.NombreBundle + " • " + bundle2.NombreTienda + " (" + Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en") + ")" + Environment.NewLine;
                                    }
                                }
                            }
                        }

                        if (juego.Gratis?.Count > 0)
                        {
                            foreach (var gratis in juego.Gratis)
                            {
                                if (gratis.FechaEmpieza <= DateTime.Now && gratis.FechaTermina >= DateTime.Now)
                                {
                                    texto = texto + "* It's free in: [" + Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre + "](" + gratis.Enlace + ")" + Environment.NewLine;
                                }
                                else
                                {
                                    texto = texto + "* It was free in: " + Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre + " (" + Calculadora.DiferenciaTiempo(gratis.FechaEmpieza, "en") + ")" + Environment.NewLine;
                                }
                            }
                        }
                    }

                    texto = texto + Environment.NewLine + Environment.NewLine;
                }
            }

            return texto;
        }

        public static async Task Postear(string enlace, int id, string tienda)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            string cuenta = builder.Configuration.GetValue<string>("Reddit:Cuenta");
            string contraseña = builder.Configuration.GetValue<string>("Reddit:Contraseña");
            string clientId = builder.Configuration.GetValue<string>("Reddit:ClientID");
            string clientSecret = builder.Configuration.GetValue<string>("Reddit:ClientSecret");

            var webAgent = new Reddit2.BotWebAgent(cuenta, contraseña, clientId, clientSecret, "https://pepeizqdeals.com/");
            var reddit = new RedditSharp.Reddit(webAgent, false);

            RedditSharp.Things.Subreddit subreddit = await reddit.GetSubredditAsync("gamedealsue");

            string nombreTienda = string.Empty;
            foreach (var tienda2 in Tiendas2.TiendasCargar.GenerarListado())
            {
                if (tienda2.Id == tienda)
                {
                    nombreTienda = tienda2.Nombre;
                }
            }

            Juego juego2 = global::BaseDatos.Juegos.Buscar.UnJuego(id);

            if (juego2 != null)
            {
                bool valido = true;

                if (string.IsNullOrEmpty(juego2.MayorEdad) == false)
                {
                    if (juego2.MayorEdad.ToLower() == "true")
                    {
                        valido = false;
                    }
                }

                if (valido == true)
                {
                    decimal precio = 0;
                    string descuento = "0";
                    string codigoTexto = string.Empty;

                    foreach (var precio2 in juego2.PrecioMinimosHistoricos)
                    {
                        if (precio2.Tienda == tienda && precio2.Enlace == enlace)
                        {
                            if (precio2.PrecioCambiado > 0)
                            {
                                precio = precio2.PrecioCambiado;
                            }
                            else
                            {
                                precio = precio2.Precio;
                            }

                            descuento = precio2.Descuento.ToString();
                            codigoTexto = precio2.CodigoTexto;

                            break;
                        }
                    }

                    if (precio > 0 && descuento != "0")
                    {
                        string titulo = "[" + nombreTienda + "] " + juego2.Nombre + " (" + Herramientas.Precios.Euro(precio) + " / " + descuento + "% off)";

                        string texto = "[" + enlace + "](" + enlace + ")  " + Environment.NewLine;

                        if (string.IsNullOrEmpty(codigoTexto) == false)
                        {
                            texto = texto + "Use code " + codigoTexto + " to obtain the price indicated in the title.  " + Environment.NewLine;
                        }

                        if (juego2.Bundles?.Count > 0)
                        {
                            List<int> bundlesActivos = new List<int>();
                            List<int> bundlesViejunos = new List<int>();

                            foreach (var bundle in juego2.Bundles)
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

                            if (bundlesActivos.Count > 0)
                            {
                                texto = texto + Environment.NewLine + juego2.Nombre + " is part of the following bundles:" + Environment.NewLine;

                                foreach (var bundle in bundlesActivos)
                                {
                                    Bundles2.Bundle bundle2 = global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* [" + bundle2.NombreBundle + " • " + bundle2.NombreTienda + "](" + bundle2.Enlace + ")" + Environment.NewLine;
                                    }
                                }
                            }

                            if (bundlesViejunos.Count > 0)
                            {
                                texto = texto + Environment.NewLine + juego2.Nombre + " was part of the following bundles:" + Environment.NewLine;

                                foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + "* " + bundle2.NombreBundle + " • " + bundle2.NombreTienda + " (" + Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en") + ")" + Environment.NewLine;
                                    }
                                }
                            }
                        }

                        if (juego2.Gratis?.Count > 0)
                        {
                            foreach (var gratis in juego2.Gratis)
                            {
                                if (gratis.FechaEmpieza <= DateTime.Now && gratis.FechaTermina >= DateTime.Now)
                                {
                                    if (texto.Contains(juego2.Nombre + " is currently free on:") == false)
                                    {
                                        texto = texto + Environment.NewLine + juego2.Nombre + " is currently free on:" + Environment.NewLine;
                                    }

                                    texto = texto + "* [" + Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre + "](" + gratis.Enlace + ")" + Environment.NewLine;
                                }
                                else
                                {
                                    if (texto.Contains(juego2.Nombre + " was free on:") == false)
                                    {
                                        texto = texto + Environment.NewLine + juego2.Nombre + " was free on:" + Environment.NewLine;
                                    }

                                    texto = texto + "* " + Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre + " (" + Calculadora.DiferenciaTiempo(gratis.FechaEmpieza, "en") + ")" + Environment.NewLine;
                                }
                            }
                        }

                        if (juego2.Suscripciones?.Count > 0)
                        {
                            foreach (var suscripcion in juego2.Suscripciones)
                            {
                                if (suscripcion.FechaEmpieza <= DateTime.Now && suscripcion.FechaTermina >= DateTime.Now)
                                {
                                    if (texto.Contains(juego2.Nombre + " is currently available on subscription services:") == false)
                                    {
                                        texto = texto + Environment.NewLine + juego2.Nombre + " is currently available on subscription services:" + Environment.NewLine;
                                    }

                                    texto = texto + "* [" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + "](" + suscripcion.Enlace + ")" + Environment.NewLine;
                                }
                                else
                                {
                                    if (texto.Contains(juego2.Nombre + " was available on subscription services:") == false)
                                    {
                                        texto = texto + Environment.NewLine + juego2.Nombre + " was available on subscription services:" + Environment.NewLine;
                                    }

                                    texto = texto + "* " + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + " (" + Calculadora.DiferenciaTiempo(suscripcion.FechaEmpieza, "en") + ")" + Environment.NewLine;
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
                                global::BaseDatos.Errores.Insertar.Mensaje("Reddit Postear Noticia", ex);
                            }
                        }
                    }
                }
            }
        }
    }
}


