using Juegos;

namespace APIs.Stadia
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion stadia = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.StadiaPro,
				Nombre = "Stadia Pro",
				ImagenLogo = "/imagenes/suscripciones/stadiapro_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/stadiapro_icono.webp",
				ColorDestacado = "rgba(28, 23, 43, 0.3)",
				DRMDefecto = JuegoDRM.Stadia,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 4.99,
				AdminPendientes = false,
				SoloStreaming = true,
				AdminAñadir = false,
				UsuarioPuedeAbrir = false,
				FechaSugerencia = new DateTime(2023, 1, 18, 0, 0, 0)
			};

			return stadia;
		}
	}
}
