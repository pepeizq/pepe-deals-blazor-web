using Herramientas;
using Microsoft.VisualBasic;
using System.Net;

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

        public static async Task<Bundles2.Bundle> ExtraerDatos(Bundles2.Bundle bundle)
        {
            if (bundle.Enlace != "https://www.greenmangamingbundles.com/")
            {
				string html = await Decompiladores.Estandar(bundle.Enlace);

                if (string.IsNullOrEmpty(html) == false)
                {
                    if (html.Contains("<title>") == true)
                    {
                        int int1 = html.IndexOf("<title>") + 7;
                        string temp1 = html.Remove(0, int1);

                        string titulo = temp1.Substring(0, temp1.IndexOf("</title>")).Trim();
                        titulo = titulo.Replace("| Green Man Gaming Bundles", null);
                        titulo = titulo.Trim();

						bundle.Nombre = WebUtility.HtmlDecode(titulo);
					}

                    if (html.Contains(Strings.ChrW(34) + "og:image" + Strings.ChrW(34)) == true)
                    {
                        int int1 = html.IndexOf(Strings.ChrW(34) + "og:image" + Strings.ChrW(34)) + 12;
                        string temp1 = html.Remove(0, int1);

                        string imagen = temp1.Substring(0, temp1.IndexOf(Strings.ChrW(34))).Trim();

                        bundle.Imagen = imagen;
					}
				}
			}

			return bundle;
		}
	}
}
