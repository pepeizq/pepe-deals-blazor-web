#nullable disable

namespace APIs.IndiePass
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion indiepass = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.IndiePass,
				Nombre = "Indie Pass",
				ImagenLogo = "/imagenes/suscripciones/indiepass_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/indiepass_icono.webp",
				ColorDestacado = "#e9452e",
				Enlace = "https://www.indiepass.com/",
				DRMDefecto = Juegos.JuegoDRM.IndiePass,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 5.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionlunapremium",
				SoloStreaming = true,
				AdminAñadir = false,
				UsuarioPuedeAbrir = true
			};

			return indiepass;
		}
	}
}
