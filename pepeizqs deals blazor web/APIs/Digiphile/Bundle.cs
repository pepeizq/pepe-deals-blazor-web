namespace APIs.Digiphile
{
	public static class Bundle
	{
		public static Bundles2.Bundle Generar()
		{
			Bundles2.Bundle bundle = new Bundles2.Bundle()
			{
				BundleTipo = Bundles2.BundleTipo.Digiphile,
				Tienda = "Digiphile",
				ImagenTienda = "/imagenes/bundles/digiphile_300x80.webp",
				ImagenIcono = "/imagenes/bundles/digiphile_icono.ico",
				EnlaceBase = "digiphile.co",
				Pick = false
			};

			DateTime fechaEmpieza = DateTime.Now;
			fechaEmpieza = new DateTime(fechaEmpieza.Year, fechaEmpieza.Month, fechaEmpieza.Day, fechaEmpieza.Hour, 0, 0);

			bundle.FechaEmpieza = fechaEmpieza;

			DateTime fechaTermina = DateTime.Now;
			fechaTermina = fechaTermina.AddDays(35);
			fechaTermina = new DateTime(fechaTermina.Year, fechaTermina.Month, fechaTermina.Day, fechaTermina.Hour, 0, 0);

			bundle.FechaTermina = fechaTermina;

			return bundle;
		}
	}
}