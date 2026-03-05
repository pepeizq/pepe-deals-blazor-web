namespace APIs.Digiphile
{
	public static class Gratis
	{
		public static Gratis2.Gratis Generar()
		{
			Gratis2.Gratis digiphile = new Gratis2.Gratis
			{
				Tipo = Gratis2.GratisTipo.Digiphile,
				Nombre = "Digiphile",
				ImagenLogo = "/imagenes/bundles/digiphile_300x80.webp",
				ImagenIcono = "/imagenes/bundles/digiphile_icono.ico",
				DRMDefecto = Juegos.JuegoDRM.Steam,
				DRMEnseñar = false
			};

			DateTime fechaEpic = DateTime.Now;
			fechaEpic = fechaEpic.AddDays(3);
			fechaEpic = new DateTime(fechaEpic.Year, fechaEpic.Month, fechaEpic.Day, fechaEpic.Hour, 0, 0);

			digiphile.FechaSugerencia = fechaEpic;

			return digiphile;
		}
	}
}