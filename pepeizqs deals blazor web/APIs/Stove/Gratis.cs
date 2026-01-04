namespace APIs.Stove
{
	public static class Gratis
	{
		public static Gratis2.Gratis Generar()
		{
			Gratis2.Gratis stove = new Gratis2.Gratis
			{
				Tipo = Gratis2.GratisTipo.Stove,
				Nombre = "Stove",
				ImagenLogo = "/imagenes/tiendas/stove_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/stove_icono.ico",
				DRMDefecto = Juegos.JuegoDRM.Stove,
				DRMEnseñar = false
			};

			DateTime fechaStove = DateTime.Now;
			fechaStove = fechaStove.AddDays(7);

			stove.FechaSugerencia = fechaStove;

			return stove;
		}
	}
}
