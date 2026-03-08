namespace Noticias
{

	public static class Steam
	{
		public static Noticia GenerarNoticia(Noticia noticia)
		{
			noticia.SteamEn = new NoticiaSteam();
			noticia.SteamEs = new NoticiaSteam();

			if (noticia.NoticiaTipo == NoticiaTipo.Bundles)
			{
				noticia.SteamEn.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("en", "Steam1", "NewsTemplates"), Bundles2.BundlesCargar.DevolverBundle(noticia.BundleTipo));
				noticia.SteamEs.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("es", "Steam1", "NewsTemplates"), Bundles2.BundlesCargar.DevolverBundle(noticia.BundleTipo));
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Gratis)
			{
				noticia.SteamEn.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("en", "Steam2", "NewsTemplates"), Gratis2.GratisCargar.DevolverGratis(noticia.GratisTipo));
				noticia.SteamEs.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("es", "Steam2", "NewsTemplates"), Gratis2.GratisCargar.DevolverGratis(noticia.GratisTipo));
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Suscripciones)
			{
				noticia.SteamEn.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("en", "Steam3", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(noticia.SuscripcionTipo));
				noticia.SteamEs.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("es", "Steam3", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(noticia.SuscripcionTipo));
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Web)
			{
				noticia.SteamEn.Titulo = Herramientas.Idiomas.BuscarTexto("en", "Steam4", "NewsTemplates");
				noticia.SteamEs.Titulo = Herramientas.Idiomas.BuscarTexto("es", "Steam4", "NewsTemplates");
			}
			else
			{
				noticia.SteamEn.Titulo = Herramientas.Idiomas.BuscarTexto("en", "Steam5", "NewsTemplates");
				noticia.SteamEs.Titulo = Herramientas.Idiomas.BuscarTexto("es", "Steam5", "NewsTemplates");
			}

			if (noticia.SteamEn.Titulo.Length > 80)
			{
				noticia.SteamEn.Titulo = noticia.SteamEn.Titulo.Remove(80, noticia.SteamEn.Titulo.Length - 80);
			}

			if (noticia.SteamEs.Titulo.Length > 80)
			{
				noticia.SteamEs.Titulo = noticia.SteamEs.Titulo.Remove(80, noticia.SteamEs.Titulo.Length - 80);
			}

			string subtituloEn = noticia.TituloEn;

			if (subtituloEn.Length > 120)
			{

			}

			return noticia;
		}
	}
}
