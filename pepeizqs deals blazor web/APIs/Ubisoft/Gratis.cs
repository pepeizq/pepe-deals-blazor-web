namespace APIs.Ubisoft
{
    public static class Gratis
    {
        public static Gratis2.Gratis Generar()
        {
            Gratis2.Gratis ubi = new Gratis2.Gratis
            {
                Tipo = Gratis2.GratisTipo.Ubisoft,
                Nombre = "Ubisoft",
                ImagenLogo = "/imagenes/tiendas/ubisoft_300x80.webp",
                ImagenIcono = "/imagenes/tiendas/ubisoft_icono.webp",
                DRMDefecto = Juegos.JuegoDRM.Ubisoft,
                DRMEnseñar = false
            };

            DateTime fechaUbi = DateTime.Now;
            fechaUbi = fechaUbi.AddDays(7);

            ubi.FechaSugerencia = fechaUbi;

            return ubi;
        }

		public static string Referido(string enlace)
		{
			enlace = enlace.Replace(":", "%3A");
			enlace = enlace.Replace("/", "%2F");
			enlace = enlace.Replace("/", "%2F");
			enlace = enlace.Replace("?", "%3F");
			enlace = enlace.Replace("=", "%3D");

			return "https://ubisoft.pxf.io/c/1382810/1186371/12050?u=" + enlace;
		}
	}
}
