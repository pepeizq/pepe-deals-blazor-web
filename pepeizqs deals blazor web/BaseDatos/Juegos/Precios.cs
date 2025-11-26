#nullable disable

using BaseDatos.Usuarios;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BaseDatos.Juegos
{
	public static class Precios
	{
		public static void Actualizar(int id, int idSteam, List<JuegoPrecio> ofertasActuales, List<JuegoPrecio> ofertasHistoricas, List<JuegoHistorico> historicos, JuegoPrecio nuevaOferta, SqlConnection conexion, 
			string slugGOG = null, string idGOG = null, string slugEpic = null, JuegoAnalisis reseñas = null)
		{
			bool cambioPrecio = true;
			bool ultimaModificacion = false;
			bool añadir = true;

            #region Limpiar Duplicados Historicos

            if (ofertasHistoricas?.Count > 0)
            {
                int duplicadosDRM = 0;

                foreach (var historico in ofertasHistoricas)
                {
                    if (historico.DRM == nuevaOferta.DRM)
                    {
                        duplicadosDRM += 1;
                    }
                }

                if (duplicadosDRM > 1)
                {
                    decimal precioMinimoValor = 100000;
                    JuegoPrecio precioMinimoFinal = null;

                    if (ofertasHistoricas?.Count > 0)
                    {
                        foreach (var historico in ofertasHistoricas)
                        {
                            if (historico.DRM == nuevaOferta.DRM)
                            {
                                if (historico.Moneda != Herramientas.JuegoMoneda.Euro && historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoValor)
                                {
                                    precioMinimoValor = historico.PrecioCambiado;
                                    precioMinimoFinal = historico;
                                }
                                else if (historico.Moneda != Herramientas.JuegoMoneda.Euro && historico.PrecioCambiado == 0 && historico.Precio < precioMinimoValor)
                                {
                                    precioMinimoValor = historico.Precio;
                                    precioMinimoFinal = historico;
                                }
                                else if (historico.Moneda == Herramientas.JuegoMoneda.Euro && historico.Precio > 0 && historico.Precio < precioMinimoValor)
                                {
                                    precioMinimoValor = historico.Precio;
                                    precioMinimoFinal = historico;
                                }
                            }
                        }

                        ofertasHistoricas.RemoveAll(x => x.DRM == nuevaOferta.DRM);

                        ofertasHistoricas.Add(precioMinimoFinal);
                    }
                }
            }

            #endregion

            #region Aplicar Codigo Descuento

            if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado == 0)
            {
                nuevaOferta.PrecioCambiado = Herramientas.Divisas.Cambio(nuevaOferta.Precio, nuevaOferta.Moneda);

                if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
                {
                    decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
                    nuevaOferta.PrecioCambiado = nuevaOferta.PrecioCambiado - (nuevaOferta.PrecioCambiado * descuento);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
                {
                    decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
                    nuevaOferta.Precio = nuevaOferta.Precio - (nuevaOferta.Precio * descuento);
                }
            }

            #endregion

            if (ofertasActuales?.Count > 0)
            {
                foreach (JuegoPrecio precio in ofertasActuales)
                {
                    if (nuevaOferta.Enlace == precio.Enlace &&
                        nuevaOferta.DRM == precio.DRM &&
                        nuevaOferta.Tienda == precio.Tienda)
                    {
                        bool cambiarFechaDetectado = false;

                        if (nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && precio.Moneda == Herramientas.JuegoMoneda.Euro && (nuevaOferta.Precio < precio.Precio || nuevaOferta.Precio > precio.Precio))
                        {
                            cambiarFechaDetectado = true;
                        }
                        else if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && precio.Moneda == Herramientas.JuegoMoneda.Euro && (nuevaOferta.PrecioCambiado < precio.Precio || nuevaOferta.PrecioCambiado > precio.Precio))
                        {
                            cambiarFechaDetectado = true;
                        }
                        else if (nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && precio.Moneda != Herramientas.JuegoMoneda.Euro && (nuevaOferta.Precio < precio.PrecioCambiado || nuevaOferta.Precio > precio.PrecioCambiado))
                        {
                            cambiarFechaDetectado = true;
                        }
                        else if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && precio.Moneda != Herramientas.JuegoMoneda.Euro && (nuevaOferta.PrecioCambiado < precio.PrecioCambiado || nuevaOferta.PrecioCambiado > precio.PrecioCambiado))
                        {
                            cambiarFechaDetectado = true;
                        }

                        DateTime tempFecha = precio.FechaDetectado;
                        tempFecha = tempFecha.AddDays(21);

                        if (tempFecha < nuevaOferta.FechaDetectado)
                        {
                            cambiarFechaDetectado = true;
                        }

                        if (cambiarFechaDetectado == true)
                        {
                            precio.FechaDetectado = nuevaOferta.FechaDetectado;
                        }

                        if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro)
                        {
                            precio.PrecioCambiado = nuevaOferta.PrecioCambiado;
                        }
                        else
                        {
                            precio.PrecioCambiado = 0;
                        }

                        precio.Precio = nuevaOferta.Precio;
                        precio.Descuento = nuevaOferta.Descuento;
                        precio.FechaActualizacion = nuevaOferta.FechaActualizacion;
                        precio.FechaTermina = nuevaOferta.FechaTermina;
                        precio.CodigoDescuento = nuevaOferta.CodigoDescuento;
                        precio.CodigoTexto = nuevaOferta.CodigoTexto;
                        precio.Nombre = nuevaOferta.Nombre;
                        precio.Imagen = nuevaOferta.Imagen;
                        precio.Moneda = nuevaOferta.Moneda;
                        precio.BundleSteam = nuevaOferta.BundleSteam;

                        añadir = false;
                        break;
                    }
                }
            }
            else
            {
                ofertasActuales = new List<JuegoPrecio>();
            }

            if (añadir == true)
			{
				ofertasActuales.Add(nuevaOferta);
			}

            if (ofertasHistoricas?.Count > 0)
            {
                bool drmEncontrado = false;

                foreach (JuegoPrecio minimo in ofertasHistoricas)
                {
                    if (nuevaOferta.DRM == minimo.DRM)
                    {
                        drmEncontrado = true;

                        if ((minimo.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio < minimo.Precio) ||
                            (minimo.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado < minimo.PrecioCambiado) ||
                            (minimo.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado < minimo.Precio) ||
                            (minimo.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio < minimo.PrecioCambiado))
                        {
                            historicos = ComprobarHistoricos(historicos, nuevaOferta);

                            bool notificar = false;

                            if ((minimo.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio + 0.1m < minimo.Precio) ||
                                (minimo.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.PrecioCambiado) ||
                                (minimo.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.Precio) ||
                                (minimo.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio + 0.1m < minimo.PrecioCambiado))
                            {
                                notificar = true;

                                if (nuevaOferta.DRM == JuegoDRM.Steam && reseñas != null)
                                {
                                    bool validoRedesSociales = false;

                                    string tempCantidadReseñas = reseñas?.Cantidad;
                                    tempCantidadReseñas = tempCantidadReseñas?.Replace(",", "").Replace(".", "").Replace(" ", "").Trim();

                                    int cantidadReseñas = int.Parse(tempCantidadReseñas);

                                    string tempValoracionReseñas = reseñas?.Porcentaje;

                                    if (string.IsNullOrEmpty(tempValoracionReseñas) == false)
                                    {
										tempValoracionReseñas = tempValoracionReseñas?.Replace("%", "").Trim();

										if (string.IsNullOrEmpty(tempValoracionReseñas) == false)
                                        {
											int valoracionReseñas = 0;

                                            try
                                            {
                                                valoracionReseñas = int.Parse(tempValoracionReseñas);
											}
                                            catch
                                            {
                                                BaseDatos.Errores.Insertar.Mensaje("Error al convertir la valoración de reseñas a número entero", new Exception($"Juego ID: {id}, Valoración Reseñas: {tempValoracionReseñas}"));
												valoracionReseñas = 0;
											}

											if (nuevaOferta.Tienda.ToLower() == "steam" && cantidadReseñas > 10000 && valoracionReseñas > 60)
											{
												validoRedesSociales = true;
											}
											else if (nuevaOferta.Tienda.ToLower() != "steam" && cantidadReseñas > 5000 && valoracionReseñas > 50)
											{
												validoRedesSociales = true;
											}

											if (nuevaOferta.Tienda.ToLower() == "2game")
											{
												validoRedesSociales = false;
											}

											if (validoRedesSociales == true)
											{
												BaseDatos.RedesSociales.Insertar.Ejecutar(id, nuevaOferta);
											}
										}
									}
                                }
                            }

                            ultimaModificacion = true;

                            if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro)
                            {
                                minimo.PrecioCambiado = nuevaOferta.PrecioCambiado;
                            }
                            else
                            {
                                minimo.PrecioCambiado = 0;
                            }

                            minimo.Precio = nuevaOferta.Precio;
                            minimo.Moneda = nuevaOferta.Moneda;
                            minimo.Descuento = nuevaOferta.Descuento;
                            minimo.FechaDetectado = nuevaOferta.FechaDetectado;
                            minimo.FechaActualizacion = nuevaOferta.FechaActualizacion;
                            minimo.FechaTermina = nuevaOferta.FechaTermina;
                            minimo.CodigoDescuento = nuevaOferta.CodigoDescuento;
                            minimo.CodigoTexto = nuevaOferta.CodigoTexto;
                            minimo.Nombre = nuevaOferta.Nombre;
                            minimo.Imagen = nuevaOferta.Imagen;
                            minimo.Enlace = nuevaOferta.Enlace;
                            minimo.Tienda = nuevaOferta.Tienda;
                            minimo.BundleSteam = nuevaOferta.BundleSteam;

                            //------------------------------------------

                            if (notificar == true)
                            {
                                List<string> usuariosInteresados = BaseDatos.Usuarios.Buscar.ListaUsuariosTienenDeseado(id, nuevaOferta.DRM);

								if (usuariosInteresados?.Count > 0)
								{
									foreach (var usuarioInteresado in usuariosInteresados)
									{
										if (Usuarios.Buscar.UsuarioTieneJuego(usuarioInteresado, id, nuevaOferta.DRM) == false)
										{
											DeseadosDatos datosDeseados = null;

											string datosDeseadosTexto = BaseDatos.Usuarios.Buscar.Opcion(usuarioInteresado, "WishlistData");

											if (string.IsNullOrEmpty(datosDeseadosTexto) == true)
											{
												datosDeseados = new DeseadosDatos();
											}
											else
											{
												try
												{
													datosDeseados = JsonSerializer.Deserialize<DeseadosDatos>(datosDeseadosTexto);
												}
												catch
												{
													datosDeseados = new DeseadosDatos();
												}

												datosDeseados.Cantidad = datosDeseados.Cantidad + 1;
												datosDeseados.UltimoJuego = DateTime.Now;
											}

											BaseDatos.Usuarios.Actualizar.Opcion("WishlistData", JsonSerializer.Serialize(datosDeseados), usuarioInteresado);

											string correo = Usuarios.Buscar.UsuarioQuiereCorreos(usuarioInteresado, "NotificationLows");

											if (string.IsNullOrEmpty(correo) == false)
											{
												try
												{
													Herramientas.Correos.DeseadoMinimo.Nuevo(usuarioInteresado, id, minimo, correo);
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo", ex);
												}
											}

											//bool enviarPush = Usuarios.Buscar.UsuarioQuiereNotificacionesPushMinimos(usuarioInteresado);

											//if (enviarPush == true)
											//{
											//	try
											//	{
											//		Herramientas.NotificacionesPush.EnviarMinimo(usuarioInteresado, id, minimo, nuevaOferta.DRM);
											//	}
											//	catch (Exception ex)
											//	{
											//		BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo", ex);
											//	}
											//}
										}
									}
								}
							}
                        }
                        else
                        {
                            if ((minimo.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.Precio, 2)) ||
                                (minimo.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.PrecioCambiado, 2)) ||
                                (minimo.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.Precio, 2)) ||
                                (minimo.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.Moneda == Herramientas.JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.PrecioCambiado, 2)))
                            {
                                int cantidadHistoricos = historicos?.Count ?? 0;
                                historicos = ComprobarHistoricos(historicos, nuevaOferta);

                                if (historicos.Count > cantidadHistoricos)
                                {
                                    cambioPrecio = true;
                                }
                                else
                                {
                                    cambioPrecio = false;
                                }

                                ultimaModificacion = true;

                                minimo.Imagen = nuevaOferta.Imagen;
                                minimo.Enlace = nuevaOferta.Enlace;
                                minimo.Tienda = nuevaOferta.Tienda;
                                minimo.Moneda = nuevaOferta.Moneda;
                                minimo.Descuento = nuevaOferta.Descuento;

                                minimo.FechaActualizacion = nuevaOferta.FechaActualizacion;

                                DateTime tempFecha = nuevaOferta.FechaDetectado;
                                tempFecha = tempFecha.AddDays(30);

                                bool cambiarFechaDetectado = false;

                                if (tempFecha < minimo.FechaDetectado)
                                {
                                    cambiarFechaDetectado = true;
                                }

                                if (cambiarFechaDetectado == true)
                                {
                                    minimo.FechaDetectado = nuevaOferta.FechaDetectado;
                                }
                            }
                        }
                    }
                }

                if (drmEncontrado == false)
                {
                    ofertasHistoricas.Add(nuevaOferta);
                }
            }
            else
            {
                ofertasHistoricas = new List<JuegoPrecio>();
                ofertasHistoricas.Add(nuevaOferta);
            }

            DateTime? ahora = null;

			if (ultimaModificacion == true)
			{
				ahora = DateTime.Now;
			}

			Juegos.Actualizar.Comprobacion(cambioPrecio, id, ofertasActuales, ofertasHistoricas, historicos, conexion, slugGOG, idGOG, slugEpic, ahora, reseñas);
		}

		private static List<JuegoHistorico> ComprobarHistoricos(List<JuegoHistorico> historicos, JuegoPrecio nuevaOferta)
		{
			if (historicos == null)
			{
				historicos = new List<JuegoHistorico>();
			}

			if (historicos.Count == 0)
			{
				JuegoHistorico nuevoHistorico = new JuegoHistorico();
				nuevoHistorico.DRM = nuevaOferta.DRM;
				nuevoHistorico.Tienda = nuevaOferta.Tienda;
				nuevoHistorico.Fecha = nuevaOferta.FechaDetectado;

				if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro)
				{
                    nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
                }
				else
				{
					nuevoHistorico.Precio = nuevaOferta.Precio;
                }

				historicos.Add(nuevoHistorico);
			}
			else if (historicos.Count > 0)
			{
				JuegoHistorico nuevoHistorico = new JuegoHistorico();
				nuevoHistorico.DRM = nuevaOferta.DRM;
				nuevoHistorico.Tienda = nuevaOferta.Tienda;
				nuevoHistorico.Fecha = nuevaOferta.FechaDetectado;

                if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro)
                {
					if (nuevaOferta.PrecioCambiado > 0)
					{
                        nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
                    }
					else
					{
                        nuevoHistorico.Precio = nuevaOferta.Precio;
                    }
                }
                else
                {
                    nuevoHistorico.Precio = nuevaOferta.Precio;
                }

                bool mismoDRM = false;
				decimal precioMasBajo2 = 1000000;
				DateTime fechaMasBajo2 = new DateTime();

				foreach (var historico in historicos)
				{
					if (historico.DRM == nuevoHistorico.DRM)
					{
						mismoDRM = true;

						if (precioMasBajo2 >= historico.Precio)
						{
							precioMasBajo2 = historico.Precio;
							fechaMasBajo2 = historico.Fecha;
                        }
                    }
                }

				if (mismoDRM == true)
				{
					if (precioMasBajo2 > nuevoHistorico.Precio)
					{
						historicos.Add(nuevoHistorico);
					}
					
					if (decimal.Round(precioMasBajo2, 2) == decimal.Round(nuevoHistorico.Precio, 2))
					{
						DateTime historico2 = fechaMasBajo2;
						historico2 = historico2.AddDays(22);

                        if (historico2 < nuevoHistorico.Fecha)
						{
							historicos.Add(nuevoHistorico);
						}
					}
				}
                else
				{
					historicos.Add(nuevoHistorico);
				}
			}

            return historicos;
		}
	}
}
