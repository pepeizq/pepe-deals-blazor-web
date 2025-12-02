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
				ImagenLogo = "/imagenes/suscripciones/eaplay.webp",
				ImagenIcono = "/imagenes/tiendas/ea_icono.webp",
				Enlace = "https://www.ea.com/ea-play",
				DRMDefecto = JuegoDRM.EA,
				AdminInteractuar = false,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 5.99,
				AdminPendientes = true,
				TablaPendientes = "tiendaea"
			};

			return eaPlay;
		}

		public static Suscripciones2.Suscripcion GenerarPro()
		{
			Suscripciones2.Suscripcion eaPlayPro = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.EAPlayPro,
				Nombre = "EA Play Pro",
				ImagenLogo = "/imagenes/suscripciones/eaplaypro.webp",
				ImagenIcono = "/imagenes/tiendas/ea_icono.webp",
				Enlace = "https://www.ea.com/ea-play",
				DRMDefecto = JuegoDRM.EA,
				AdminInteractuar = false,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				IncluyeSuscripcion = Suscripciones2.SuscripcionTipo.EAPlay,
                Precio = 16.99,
                AdminPendientes = true,
                TablaPendientes = "tiendaea"
            };

			return eaPlayPro;
		}
	}
}
