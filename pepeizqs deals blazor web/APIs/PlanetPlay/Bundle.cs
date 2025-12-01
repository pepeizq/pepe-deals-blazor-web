namespace APIs.PlanetPlay
{

	public static class Bundle
	{
		public static Bundles2.Bundle Generar()
		{
			Bundles2.Bundle bundle = new Bundles2.Bundle()
			{
				BundleTipo = Bundles2.BundleTipo.PlanetPlay,
				Tienda = "PlanetPlay",
				ImagenTienda = "/imagenes/tiendas/planetplay_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/planetplay_icono.webp",
				EnlaceBase = "planetplay.com",
				Pick = false
			};

			DateTime fechaEmpieza = DateTime.Now;
			fechaEmpieza = new DateTime(fechaEmpieza.Year, fechaEmpieza.Month, fechaEmpieza.Day, fechaEmpieza.Hour, 0, 0);

			bundle.FechaEmpieza = fechaEmpieza;

			DateTime fechaTermina = DateTime.Now;
			fechaTermina = fechaTermina.AddDays(7);
			fechaTermina = new DateTime(fechaTermina.Year, fechaTermina.Month, fechaTermina.Day, fechaTermina.Hour, 0, 0);

			bundle.FechaTermina = fechaTermina;

			return bundle;
		}
	}
}
