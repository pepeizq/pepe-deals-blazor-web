#nullable disable

using Juegos;

namespace Herramientas.Bundles
{
	public static class ValorEstimado
	{
		public static async Task<List<JuegoValoracion>> Generar(Bundles2.Bundle bundle)
		{
			if (bundle.Pick == false)
			{
				double precioBundle = double.Parse(bundle.Tiers.LastOrDefault().Precio);

				List<JuegoValoracion> juegosValoracion = new List<JuegoValoracion>();

				//Carga inicial de los juegos
				foreach (var juego in bundle.Juegos)
				{
					JuegoValoracion juegoValoracion = new JuegoValoracion();
					juegoValoracion.Nombre = juego.Nombre;
					juegoValoracion.DRM = juego.DRM;

					if (juego.Juego == null)
					{
						juego.Juego = await global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId); // Cargo datos de la web en caso de ser tan subnormal de no haberlos cargados llegados a este punto
					}

					if (juego.Juego?.PrecioMinimosHistoricos != null)
					{
						foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
						{
							if (historico.DRM == juego.DRM)
							{
								if (historico.PrecioCambiado > 0)
								{
									juegoValoracion.PrecioMinimoHistorico = (double)historico.PrecioCambiado;
								}
								else
								{
									juegoValoracion.PrecioMinimoHistorico = (double)historico.Precio;
								}

								break;
							}
						}
					}

					if (juego.Juego?.Bundles != null)
					{
						juegoValoracion.NumeroBundles = juego.Juego.Bundles.Count - 1; // Excluyo el bundle actual

						if (juego.Juego.Suscripciones?.Count > 0)
						{
							foreach (var suscripcion in juego.Juego.Suscripciones)
							{
								if (suscripcion.DRM == JuegoDRM.Steam && suscripcion.Tipo == Suscripciones2.SuscripcionTipo.HumbleChoice)
								{
									juegoValoracion.NumeroBundles = juegoValoracion.NumeroBundles + 1; // Sumo Humble Choice como si fuera un bundle extra
									break;
								}
							}
						}
					}

					if (juego.Juego?.Analisis != null)
					{
						if (string.IsNullOrEmpty(juego.Juego.Analisis?.Cantidad) == false)
						{
							juegoValoracion.NumeroReseñas = int.Parse(juego.Juego.Analisis.Cantidad.Replace(",", null));
						}
					}

					if (juego.Juego?.Caracteristicas != null)
					{
						juegoValoracion.FechaSalida = juego.Juego.Caracteristicas.FechaLanzamientoSteam;
					}

					juegoValoracion.Tier = juego.Tier.Posicion;

					juegosValoracion.Add(juegoValoracion);
				}

				if (juegosValoracion.Count > 0)
				{
					int cantidadReseñasMinima = juegosValoracion.Min(j => j.NumeroReseñas);
					int cantidadReseñasMaxima = juegosValoracion.Max(j => j.NumeroReseñas);

					// Primer cálculo para estimar el valor de los juegos (mirar abajo)
					foreach (var juego in juegosValoracion)
					{
						juego.Valoracion = CalcularValoracionJuego(juego, cantidadReseñasMinima, cantidadReseñasMaxima);
					}

					// Segundo cálculo para encajar la suma total de valoraciones y que no supere el precio mínimo histórico en cada juego
					double sumaValoraciones = juegosValoracion.Sum(j => j.Valoracion);
					foreach (var juego in juegosValoracion)
					{
						juego.Valoracion = juego.Valoracion / sumaValoraciones * precioBundle;

						if (juego.Valoracion > juego.PrecioMinimoHistorico)
						{
							juego.Valoracion = juego.PrecioMinimoHistorico;
						}
					}

					// Tercer cálculo donde voy a aplicar un bucle hasta que se ajusten las valoraciones al precio del bundle (limitado a 10000 pasadas), si la cago es aquí
					bool sigueSobrando = true;
					int i = 0;
					while (sigueSobrando == true && i < 10000)
					{
						sigueSobrando = false;
						i += 1;

						double sumaFinal = juegosValoracion.Sum(j => j.Valoracion);
						double sobrante = precioBundle - sumaFinal;

						if (sobrante > 0.00001)
						{
							List<JuegoValoracion> juegosDondeAplicarRestante = juegosValoracion.Where(j => j.Valoracion < j.PrecioMinimoHistorico).ToList();

							if (juegosDondeAplicarRestante.Count == 0)
							{
								break;
							}

							sigueSobrando = true;

							double cantidadARepartir = juegosDondeAplicarRestante.Sum(j => CalcularValoracionJuego(j, cantidadReseñasMinima, cantidadReseñasMaxima));

							foreach (JuegoValoracion juego in juegosDondeAplicarRestante)
							{
								double extra = CalcularValoracionJuego(juego, cantidadReseñasMinima, cantidadReseñasMaxima) / cantidadARepartir * sobrante;
								juego.Valoracion = Math.Min(juego.Valoracion + extra, juego.PrecioMinimoHistorico);
							}
						}
					}
				}

				return juegosValoracion;
			}

			return null;
		}

		public static double CalcularValoracionJuego(JuegoValoracion juego, int minimoReseñas, int maximoReseñas)
		{
			// Valor que asigno en función de si el juego ha salido hace pocos meses

			double valorFecha = 1;

			if (juego.FechaSalida != null)
			{
				double meses = (DateTime.Now - juego.FechaSalida.Value).TotalDays / 30;

				double meses2 = Math.Min(meses, 120); // Máximo de 10 años (por si acaso)

				valorFecha = 2 - (meses2 / 120);
			}

			// Valor que asigno en función del tier del bundle en el que está el juego

			double valorTier = 1;
			valorTier = valorTier + (juego.Tier * 0.5);

			// Valor que asigno a las reseñas que oscila entre el juego con menor cantidad de reseñas y el mayor

			double valorReseñas = 1;
			valorReseñas = (juego.NumeroReseñas - minimoReseñas) / (maximoReseñas - minimoReseñas);
			valorReseñas = valorReseñas * 2;

			// Junto todo quitando peso en función de los bundles en los que ha estado

			return (valorReseñas + valorFecha + valorTier) / (double)(juego.NumeroBundles + 1);
		}
	}

	public class JuegoValoracion
	{
		public string Nombre { get; set; }
		public JuegoDRM DRM { get; set; }
		public double PrecioMinimoHistorico { get; set; }
		public int NumeroBundles { get; set; }
		public int NumeroReseñas { get; set; }
		public double Valoracion { get; set; }
		public DateTime? FechaSalida { get; set; }
		public int Tier { get; set; }
	}
}
