#nullable disable

using Herramientas;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APIs.Boosteroid
{
	public static class Streaming
	{
		public static Streaming2.Streaming Generar()
		{
			Streaming2.Streaming boosteroid = new Streaming2.Streaming
			{
				Id = Streaming2.StreamingTipo.Boosteroid,
				Nombre = "Boosteroid",
				ImagenLogo = "/imagenes/streaming/boosteroid_logo.webp",
				ImagenIcono = "/imagenes/streaming/boosteroid_icono.webp"
			};

			return boosteroid;
		}

		public static async Task Buscar(SqlConnection conexion)
		{
			BaseDatos.Admin.Actualizar.Tiendas("boosteroid", DateTime.Now, 0);

			int cantidad = 0;

			int i = 1;
			while (i < 100)
			{
				string html = await Decompiladores.Estandar("https://cloud.boosteroid.com/api/v1/public/applications?orderBy=popularity&collection=1&page=" + i.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					BoosteroidDatos datos = JsonSerializer.Deserialize<BoosteroidDatos>(html);

					if (datos != null)
					{
						if (datos.Juegos == null)
						{
							break;
						}
						else
						{
							if (datos.Juegos.Count == 0)
							{
								break;
							}
							else
							{
								foreach (var juego in datos.Juegos)
								{
									List<string> drms = new List<string>();

									if (juego.DRMs != null)
									{
										if (string.IsNullOrEmpty(juego.DRMs.Steam) == false)
										{
											drms.Add("Steam");
										}

										if (string.IsNullOrEmpty(juego.DRMs.EpicGames) == false)
										{
											drms.Add("Epic Games");
										}

										if (string.IsNullOrEmpty(juego.DRMs.BattleNet) == false)
										{
											drms.Add("Battle.net");
										}
									}

									if (drms.Count > 0)
									{
										DateTime fecha = DateTime.Now;
										fecha = fecha + TimeSpan.FromDays(1);

										bool encontrado = false;

										string sqlBuscar = "SELECT * FROM streamingboosteroid WHERE id=@id";

										if (conexion == null)
										{
											conexion = Herramientas.BaseDatos.Conectar();
										}
										else
										{
											if (conexion.State != System.Data.ConnectionState.Open)
											{
												conexion = Herramientas.BaseDatos.Conectar();
											}
										}

										using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion))
										{
											comando.Parameters.AddWithValue("@id", juego.Id);

											using (SqlDataReader lector = comando.ExecuteReader())
											{
												encontrado = lector.Read();

												cantidad += 1;
												BaseDatos.Admin.Actualizar.Tiendas("boosteroid", DateTime.Now, cantidad);
											}
										}

										if (encontrado == true)
										{
											string sqlActualizar = "UPDATE streamingboosteroid " +
																"SET fecha=@fecha WHERE id=@id";

											if (conexion == null)
											{
												conexion = Herramientas.BaseDatos.Conectar();
											}
											else
											{
												if (conexion.State != System.Data.ConnectionState.Open)
												{
													conexion = Herramientas.BaseDatos.Conectar();
												}
											}

											using (SqlCommand comandoActualizar = new SqlCommand(sqlActualizar, conexion))
											{
												comandoActualizar.Parameters.AddWithValue("@id", juego.Id);
												comandoActualizar.Parameters.AddWithValue("@fecha", fecha);

												try
												{
													comandoActualizar.ExecuteNonQuery();
												}
												catch
												{

												}
											}
										}
										else 
										{
											string sqlInsertar = "INSERT INTO streamingboosteroid " +
															"(id, nombre, drms, fecha) VALUES " +
															"(@id, @nombre, @drms, @fecha) ";

											if (conexion == null)
											{
												conexion = Herramientas.BaseDatos.Conectar();
											}
											else
											{
												if (conexion.State != System.Data.ConnectionState.Open)
												{
													conexion = Herramientas.BaseDatos.Conectar();
												}
											}

											using (SqlCommand comandoInsertar = new SqlCommand(sqlInsertar, conexion))
											{
												comandoInsertar.Parameters.AddWithValue("@id", juego.Id);
												comandoInsertar.Parameters.AddWithValue("@nombre", juego.Nombre);
												comandoInsertar.Parameters.AddWithValue("@drms", JsonSerializer.Serialize(drms));
												comandoInsertar.Parameters.AddWithValue("@fecha", fecha);

												try
												{
													comandoInsertar.ExecuteNonQuery();
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje("Insertar Boosteroid " + juego.Nombre, ex);
												}
											}
										}
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

	public class BoosteroidDatos
	{
		[JsonPropertyName("data")]
		public List<BoosteroidDatosJuego> Juegos { get; set; }
	}

	public class BoosteroidDatosJuego
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("icon")]
		public string Imagen { get; set; }

		[JsonPropertyName("stores")]
		public BoosteroidDatosJuegoTiendas DRMs { get; set; }
	}

	public class BoosteroidDatosJuegoTiendas
	{
		[JsonPropertyName("steam")]
		public string Steam { get; set; }

		[JsonPropertyName("epic-games-store")]
		public string EpicGames { get; set; }

		[JsonPropertyName("battlenet")]
		public string BattleNet { get; set; }
	}
}
