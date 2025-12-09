#nullable disable

using Dapper;
using Herramientas;
using System.Text.Json;

namespace APIs.GOG
{
	public static class Streaming
	{
		public static Streaming2.Streaming Generar()
		{
			Streaming2.Streaming amazonluna = new Streaming2.Streaming
			{
				Id = Streaming2.StreamingTipo.AmazonLuna,
				Nombre = "Amazon Luna (GOG)",
				ImagenLogo = "/imagenes/streaming/amazonluna_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp"
			};

			return amazonluna;
		}

		public static async Task Buscar()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString(), DateTime.Now, 0);

			int cantidad = 0;

			int i = 1;
			int limite = 20;
			while (i < limite + 1)
			{
				string enlace = "https://catalog.gog.com/v1/filtered-catalog?limit=48&order=desc:discount&productType=in:game,pack,dlc,extras&page=" + i.ToString() + "&pageId=2f70726f6d6f2f706c61792d6f6e2d6c756e61&sectionId=7efd2a6a-0831-43af-bc1f-616509912d24&countryCode=ES&locale=en-US&currencyCode=EUR";
                string html = await Decompiladores.Estandar(enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					GOGOfertas datos = null;

					try
					{
						datos = JsonSerializer.Deserialize<GOGOfertas>(html);
					}
					catch
					{
						BaseDatos.Errores.Insertar.Mensaje("Amazon Luna GOG", html, enlace);
                    }

                    if (datos?.Juegos?.Count > 0)
					{
						limite = datos.Paginas;

						foreach (var juego in datos.Juegos)
						{
							if (juego.Tipo == "game" || juego.Tipo == "pack")
							{
								DateTime fecha = DateTime.Now;
								fecha = fecha + TimeSpan.FromDays(1);

								bool? encontrado = false;

								string sqlBuscar = "SELECT 1 FROM streamingamazonluna WHERE nombreCodigo=@nombreCodigo";

								try
								{
									encontrado = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
									{
										var resultado = await sentencia.Connection.QueryFirstOrDefaultAsync<bool?>(
											sqlBuscar,
											new { nombreCodigo = juego.Id },
											transaction: sentencia
										);

										return resultado ?? false;  
									});
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Amazon Luna 1", ex);
								}

								if (encontrado == true)
								{
									cantidad += 1;
									await BaseDatos.Admin.Actualizar.Tiendas(Generar().Id.ToString(), DateTime.Now, cantidad);

									string sqlActualizar = "UPDATE streamingamazonluna " +
															"SET fecha=@fecha WHERE nombreCodigo=@nombreCodigo";

									try
									{
										await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
										{
											return await sentencia.Connection.ExecuteAsync(sqlActualizar, new { nombreCodigo = juego.Id, fecha }, transaction: sentencia);
										});
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Amazon Luna 2", ex);
									}
								}
								else
								{
									string sqlInsertar = "INSERT INTO streamingamazonluna " +
														"(nombreCodigo, nombre, drms, fecha) VALUES " +
														"(@nombreCodigo, @nombre, @drms, @fecha) ";

									try
									{
										await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
										{
											return await sentencia.Connection.ExecuteAsync(sqlInsertar, new
											{
												nombreCodigo = juego.Id,
												nombre = juego.Nombre,
												drms = "gog",
												fecha
											}, transaction: sentencia);
										});
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Amazon Luna 3", ex);
									}
								}
							}
						}						
					}
				}

				i += 1;
			}
		}
	}
}
