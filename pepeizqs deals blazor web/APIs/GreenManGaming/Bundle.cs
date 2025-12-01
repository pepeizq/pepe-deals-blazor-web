namespace APIs.GreenManGaming
{
	public static class Bundle
	{
        public static Bundles2.Bundle Generar()
        {
            Bundles2.Bundle bundle = new Bundles2.Bundle()
            {
                BundleTipo = Bundles2.BundleTipo.GreenManGaming,
                Tienda = "Green Man Gaming",
                ImagenTienda = "/imagenes/bundles/gmg_300x80.webp",
                ImagenIcono = "/imagenes/bundles/gmg_icono.webp",
                EnlaceBase = "greenmangamingbundles.com",
                Pick = false,
                Twitter = "GreenManGaming"
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

        public static string Referido(string enlace)
        {
            string sku = enlace;
            sku = sku.Remove(0, sku.IndexOf("/bundles/") + 9);

            if (sku.Contains("/") == true)
            {
                sku = sku.Remove(sku.IndexOf("/"), sku.Length - sku.IndexOf("/"));
            }

            enlace = enlace.Replace(":", "%3A");
            enlace = enlace.Replace("/", "%2F");
            enlace = enlace.Replace("/", "%2F");
            enlace = enlace.Replace("?", "%3F");
            enlace = enlace.Replace("=", "%3D");

            return "https://greenmangaming.sjv.io/c/1382810/1219987/15105?prodsku=" + sku + "&u=" + enlace;
        }
    }
}
