#nullable disable

using Juegos;

namespace APIs.EA
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion eaPlay = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.EAPlay,
				Nombre = "EA Play",
				ImagenLogo = "/imagenes/suscripciones/eaplay_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/eaplay_icono.webp",
				ImagenFondo = "/imagenes/suscripciones/eaplay_fondo.webp",
				ColorDestacado = "rgba(13, 16, 66, 0.3)",
				Enlace = "https://www.ea.com/ea-play",
				DRMDefecto = JuegoDRM.EA,
				AdminInteractuar = false,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				SuscripcionesRelacionadas = new List<Suscripciones2.SuscripcionTipo>() { Suscripciones2.SuscripcionTipo.EAPlayPro },
				Precio = 5.99,
				AdminPendientes = true,
				TablaPendientes = "tiendaea",
				UsuarioPuedeAbrir = true
			};

			return eaPlay;
		}

		public static Suscripciones2.Suscripcion GenerarPro()
		{
			Suscripciones2.Suscripcion eaPlayPro = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.EAPlayPro,
				Nombre = "EA Play Pro",
				ImagenLogo = "/imagenes/suscripciones/eaplaypro_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/eaplay_icono.webp",
				ImagenFondo = "/imagenes/suscripciones/eaplay_fondo.webp",
				ColorDestacado = "rgba(13, 16, 66, 0.3)",
				Enlace = "https://www.ea.com/ea-play",
				DRMDefecto = JuegoDRM.EA,
				AdminInteractuar = false,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
                Precio = 16.99,
                AdminPendientes = true,
                TablaPendientes = "tiendaea",
				UsuarioPuedeAbrir = true
			};

			return eaPlayPro;
		}
	}
}
