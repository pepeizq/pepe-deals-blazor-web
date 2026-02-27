#nullable disable

using BaseDatos.Divisas;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Herramientas
{
	public enum JuegoMoneda
	{
		Euro,
		Dolar,
		Libra
	}

	public static class Divisas
	{
		public static async Task ActualizarDatos2()
		{
			string htmlEuro = await Decompiladores.Estandar("https://api.frankfurter.app/latest?from=EUR&symbols=USD,GBP");

			if (string.IsNullOrEmpty(htmlEuro) == false)
			{
				FrankfurterRespuesta respuesta = JsonSerializer.Deserialize<FrankfurterRespuesta>(htmlEuro);

				if (respuesta != null)
				{
					foreach (var rate in respuesta.Rates)
					{
						string id = string.Empty;

						if (rate.Key == "USD")
						{
							id = "EUR-USD";
						}
						else if (rate.Key == "GBP")
						{
							id = "EUR-GBP";
						}

						Divisa divisa = new Divisa
						{
							Id = id,
							Cantidad = rate.Value,
							Fecha = DateTime.Now
						};

						await Actualizar.Ejecutar(divisa);
					}
				}
			}

			string htmlDolar = await Decompiladores.Estandar("https://api.frankfurter.app/latest?from=USD&symbols=EUR,GBP");

			if (string.IsNullOrEmpty(htmlDolar) == false)
			{
				FrankfurterRespuesta respuesta = JsonSerializer.Deserialize<FrankfurterRespuesta>(htmlDolar);

				if (respuesta != null)
				{
					foreach (var rate in respuesta.Rates)
					{
						string id = string.Empty;

						if (rate.Key == "EUR")
						{
							id = "USD-EUR";
						}
						else if (rate.Key == "GBP")
						{
							id = "USD-GBP";
						}

						Divisa divisa = new Divisa
						{
							Id = id,
							Cantidad = rate.Value,
							Fecha = DateTime.Now
						};

						await Actualizar.Ejecutar(divisa);
					}
				}
			}
		}

		public static async Task ActualizarDatos()
		{
			string html = await Decompiladores.Estandar("http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml");

			if (string.IsNullOrEmpty(html) == false)
			{
				using (TextReader lector = new StringReader(html))
				{
					XmlDocument documento = new XmlDocument();
					documento.Load(lector);

					foreach (XmlNode nodo in documento.DocumentElement.ChildNodes[2].ChildNodes[0].ChildNodes)
					{
						if (nodo.Attributes["rate"].Value != null)
						{
							if (nodo.Attributes["currency"].Value == "USD")
							{
								Divisa dolar = new Divisa
								{
									Id = "USD",
									Cantidad = Convert.ToDecimal(nodo.Attributes["rate"].Value),
									Fecha = DateTime.Now
								};

								if (Buscar.Una(dolar.Id) == null)
								{
									await Insertar.Ejecutar(dolar);
								}
								else
								{
									await Actualizar.Ejecutar(dolar);
								}
							}
							else if (nodo.Attributes["currency"].Value == "GBP")
							{
								Divisa libra = new Divisa
								{
									Id = "GBP",
									Cantidad = Convert.ToDecimal(nodo.Attributes["rate"].Value),
									Fecha = DateTime.Now
								};

								if (Buscar.Una(libra.Id) == null)
								{
									await Insertar.Ejecutar(libra);
								}
								else
								{
									await Actualizar.Ejecutar(libra);
								}
							}
						}
					}
				}
			}
		}

		public static decimal CambioEuro(decimal cantidad, JuegoMoneda moneda)
		{
			string buscar = string.Empty;

            if (moneda == JuegoMoneda.Dolar)
            {
				buscar = "EUR-USD";
            }
			else if (moneda == JuegoMoneda.Libra)
			{
				buscar = "EUR-GBP";
			}

			if (buscar != string.Empty)
			{
				Divisa divisa = Buscar.Una(buscar);

				decimal temp = cantidad / divisa.Cantidad;

				temp = Math.Round(temp, 2);

				return temp;
			}

			return cantidad;
		}

		public static decimal CambioDolar(decimal cantidad, JuegoMoneda moneda)
		{
			string buscar = string.Empty;

			if (moneda == JuegoMoneda.Euro)
			{
				buscar = "USD-EUR";
			}
			else if (moneda == JuegoMoneda.Libra)
			{
				buscar = "USD-GBP";
			}

			if (buscar != string.Empty)
			{
				Divisa divisa = Buscar.Una(buscar);

				decimal temp = cantidad / divisa.Cantidad;

				temp = Math.Round(temp, 2);

				return temp;
			}

			return cantidad;
		}

		public static string DevolverSimbolo(decimal cantidad, JuegoMoneda moneda)
		{
			string precioTexto = string.Empty;

			if (moneda == JuegoMoneda.Dolar)
			{
				precioTexto = "$" + cantidad.ToString();
			}
			else if (moneda == JuegoMoneda.Libra)
			{
				precioTexto = "£" + cantidad.ToString();
			}

			if (precioTexto.Contains(".") == true)
			{
				int int1 = precioTexto.IndexOf(".");

				if (int1 == precioTexto.Length - 2)
				{
					precioTexto = precioTexto + "0";
				}
			}

			return precioTexto;
		}
	}

	public class Divisa
	{
		public string Id { get; set; }
		public decimal Cantidad { get; set; }
		public DateTime Fecha { get; set; }
	}

	public class FrankfurterRespuesta
	{
		[JsonPropertyName("amount")]
		public decimal Cantidad { get; set; }

		[JsonPropertyName("base")]
		public string Base { get; set; }

		[JsonPropertyName("date")]
		public string Fecha { get; set; }

		[JsonPropertyName("rates")]
		public Dictionary<string, decimal> Rates { get; set; }
	}
}
