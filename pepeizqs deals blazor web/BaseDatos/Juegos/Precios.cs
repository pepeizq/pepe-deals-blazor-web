#nullable disable

using BaseDatos.Usuarios;
using Dapper;
using Herramientas;
using Juegos;
using System.Text;
using System.Text.Json;
using Tiendas2;

namespace BaseDatos.Juegos
{
	public static class Precios
	{
		public static async Task<(string sql, DynamicParameters parametros)> Comprobacion(int id, int idSteam, List<JuegoPrecio> ofertasActuales, List<JuegoPrecio> ofertasHistoricas, List<JuegoHistorico> historicos, JuegoPrecio nuevaOferta,  
			string slugGOG = null, string idGOG = null, string slugEpic = null, JuegoAnalisis reseñas = null, int indice = 0)
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

			decimal RedondearHaciaAbajo(decimal valor)
			{
				return Math.Floor(valor * 100) / 100;
			}

			if (nuevaOferta.Moneda != Herramientas.JuegoMoneda.Euro && nuevaOferta.PrecioCambiado == 0)
            {
                nuevaOferta.PrecioCambiado = Herramientas.Divisas.CambioEuro(nuevaOferta.Precio, nuevaOferta.Moneda);

                if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
                {
                    decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
                    nuevaOferta.PrecioCambiado = nuevaOferta.PrecioCambiado - (nuevaOferta.PrecioCambiado * descuento);
                    nuevaOferta.PrecioCambiado = RedondearHaciaAbajo(nuevaOferta.PrecioCambiado);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
                {
                    decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
                    nuevaOferta.Precio = nuevaOferta.Precio - (nuevaOferta.Precio * descuento);
					nuevaOferta.Precio = RedondearHaciaAbajo(nuevaOferta.Precio);
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

                        if (minimo.Moneda != JuegoMoneda.Euro && minimo.Precio > 0 && minimo.PrecioCambiado == 0)
                        {
                            minimo.PrecioCambiado = minimo.Precio;
                        }

						if ((minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio < minimo.Precio) ||
                            (minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado < minimo.PrecioCambiado) ||
                            (minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado < minimo.Precio) ||
                            (minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio < minimo.PrecioCambiado))
                        {
							historicos = ComprobarHistoricos(TiendaRegion.Europa, historicos, nuevaOferta);

                            bool notificar = false;

                            if ((minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio + 0.1m < minimo.Precio) ||
                                (minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.PrecioCambiado) ||
                                (minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.Precio) ||
                                (minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio + 0.1m < minimo.PrecioCambiado))
                            {
                                notificar = true;
                            }

                            ultimaModificacion = true;

                            if (nuevaOferta.Moneda != JuegoMoneda.Euro)
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
								List<string> usuariosInteresados = await BaseDatos.Usuarios.Buscar.ListaUsuariosTienenDeseado(id, nuevaOferta.DRM);

								if (usuariosInteresados?.Count > 0)
								{
									foreach (var usuarioInteresado in usuariosInteresados)
									{
										if (await Usuarios.Buscar.UsuarioTieneJuego(usuarioInteresado, id, nuevaOferta.DRM) == false)
										{
											DeseadosDatos datosDeseados = null;

											string datosDeseadosTexto = await BaseDatos.Usuarios.Buscar.OpcionStringRegion(TiendaRegion.Europa, usuarioInteresado, "WishlistData");

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

											await BaseDatos.Usuarios.Actualizar.Opcion("WishlistData", JsonSerializer.Serialize(datosDeseados), usuarioInteresado);

											string correo = await Usuarios.Buscar.UsuarioQuiereCorreos(TiendaRegion.Europa, usuarioInteresado, "NotificationLows");

											if (string.IsNullOrEmpty(correo) == false)
											{
												try
												{
													await Herramientas.Correos.DeseadoMinimo.Nuevo(usuarioInteresado, id, minimo, correo);
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo", ex);
												}
											}

											bool enviarPush = await BaseDatos.Usuarios.Buscar.OpcionBoolRegion(TiendaRegion.Europa, usuarioInteresado, "NotificationPushLows");

											if (enviarPush == true)
											{
												try
												{
													decimal precioNotificar = minimo.Precio;

													if (minimo.PrecioCambiado > 0)
													{
														precioNotificar = minimo.PrecioCambiado;
													}

													var notificaciones = ServiciosGlobales.ServiceProvider.GetRequiredService<NotificacionesPush>();
													await notificaciones.EnviarNotificacion(usuarioInteresado, minimo.Nombre + " - " + Herramientas.Precios.Euro(precioNotificar), minimo.Enlace);
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo", ex);
												}
											}
										}
									}
								}
							}
                        }
                        else
                        {
                            if ((minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.Precio, 2)) ||
                                (minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.PrecioCambiado, 2)) ||
                                (minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.Precio, 2)) ||
                                (minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.PrecioCambiado, 2)))
                            {
                                int cantidadHistoricos = historicos?.Count ?? 0;
                                historicos = ComprobarHistoricos(TiendaRegion.Europa, historicos, nuevaOferta);

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

			var sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");
			sql.Append($"precioActualesTiendas=@precioActualesTiendas{indice}, ");
			sql.Append($"precioMinimosHistoricos=@precioMinimosHistoricos{indice} ");

			if (cambioPrecio) sql.Append($", historicos=@historicos{indice} ");
			if (!string.IsNullOrEmpty(slugGOG)) sql.Append($", idGog=@idGog{indice}, slugGOG=@slugGOG{indice} ");
			if (!string.IsNullOrEmpty(slugEpic)) sql.Append($", slugEpic=@slugEpic{indice} ");
			if (ahora != null) sql.Append($", ultimaModificacion=@ultimaModificacion{indice} ");

			sql.Append($"WHERE id=@id{indice};");

			var parametros = new DynamicParameters();
			parametros.Add($"@id{indice}", id);
			parametros.Add($"@precioActualesTiendas{indice}", JsonSerializer.Serialize(ofertasActuales));
			parametros.Add($"@precioMinimosHistoricos{indice}", JsonSerializer.Serialize(ofertasHistoricas));
			if (cambioPrecio) parametros.Add($"@historicos{indice}", JsonSerializer.Serialize(historicos));
			if (!string.IsNullOrEmpty(slugGOG)) { parametros.Add($"@idGog{indice}", idGOG); parametros.Add($"@slugGOG{indice}", slugGOG); }
			if (!string.IsNullOrEmpty(slugEpic)) parametros.Add($"@slugEpic{indice}", slugEpic);
			if (ahora != null) parametros.Add($"@ultimaModificacion{indice}", ahora);

			return (sql.ToString(), parametros);
		}

		public static async Task<(string sql, DynamicParameters parametros)> ComprobacionUS(int id, int idSteam, List<JuegoPrecio> ofertasActualesUS, List<JuegoPrecio> ofertasHistoricasUS, List<JuegoHistorico> historicosUS, JuegoPrecio nuevaOferta,
			string slugGOG = null, string idGOG = null, string slugEpic = null, JuegoAnalisis reseñas = null, int indice = 0)
		{
			bool cambioPrecio = true;
			bool ultimaModificacion = false;
			bool añadir = true;

			#region Aplicar Codigo Descuento

			decimal RedondearHaciaAbajo(decimal valor)
			{
				return Math.Floor(valor * 100) / 100;
			}

			if (nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado == 0)
			{
				nuevaOferta.PrecioCambiado = Herramientas.Divisas.CambioDolar(nuevaOferta.Precio, nuevaOferta.Moneda);

				if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
				{
					decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
					nuevaOferta.PrecioCambiado = nuevaOferta.PrecioCambiado - (nuevaOferta.PrecioCambiado * descuento);
					nuevaOferta.PrecioCambiado = RedondearHaciaAbajo(nuevaOferta.PrecioCambiado);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
				{
					decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
					nuevaOferta.Precio = nuevaOferta.Precio - (nuevaOferta.Precio * descuento);
					nuevaOferta.Precio = RedondearHaciaAbajo(nuevaOferta.Precio);
				}
			}

			#endregion

			if (ofertasActualesUS?.Count > 0)
			{
				foreach (JuegoPrecio precio in ofertasActualesUS)
				{
					if (nuevaOferta.Enlace == precio.Enlace &&
						nuevaOferta.DRM == precio.DRM &&
						nuevaOferta.Tienda == precio.Tienda)
					{
						bool cambiarFechaDetectado = false;

						if (nuevaOferta.Moneda == JuegoMoneda.Dolar && precio.Moneda == JuegoMoneda.Dolar && (nuevaOferta.Precio < precio.Precio || nuevaOferta.Precio > precio.Precio))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda != JuegoMoneda.Dolar && precio.Moneda == JuegoMoneda.Dolar && (nuevaOferta.PrecioCambiado < precio.Precio || nuevaOferta.PrecioCambiado > precio.Precio))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda == JuegoMoneda.Dolar && precio.Moneda != JuegoMoneda.Dolar && (nuevaOferta.Precio < precio.PrecioCambiado || nuevaOferta.Precio > precio.PrecioCambiado))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda != JuegoMoneda.Dolar && precio.Moneda != JuegoMoneda.Dolar && (nuevaOferta.PrecioCambiado < precio.PrecioCambiado || nuevaOferta.PrecioCambiado > precio.PrecioCambiado))
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
				ofertasActualesUS = new List<JuegoPrecio>();
			}

			if (añadir == true)
			{
				ofertasActualesUS.Add(nuevaOferta);
			}

			if (ofertasHistoricasUS?.Count > 0)
			{
				bool drmEncontrado = false;

				foreach (JuegoPrecio minimo in ofertasHistoricasUS)
				{
					if (nuevaOferta.DRM == minimo.DRM)
					{
						drmEncontrado = true;

						if ((minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio < minimo.Precio) ||
							(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado < minimo.PrecioCambiado) ||
							(minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado < minimo.Precio) ||
							(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio < minimo.PrecioCambiado))
						{
							historicosUS = ComprobarHistoricos(TiendaRegion.EstadosUnidos, historicosUS, nuevaOferta);

							bool notificar = false;

							if ((minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio + 0.1m < minimo.Precio) ||
								(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.PrecioCambiado) ||
								(minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.Precio) ||
								(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio + 0.1m < minimo.PrecioCambiado))
							{
								notificar = true;
							}

							ultimaModificacion = true;

							if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
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
								List<string> usuariosInteresados = await BaseDatos.Usuarios.Buscar.ListaUsuariosTienenDeseado(id, nuevaOferta.DRM);

								if (usuariosInteresados?.Count > 0)
								{
									foreach (var usuarioInteresado in usuariosInteresados)
									{
										if (await Usuarios.Buscar.UsuarioTieneJuego(usuarioInteresado, id, nuevaOferta.DRM) == false)
										{
											DeseadosDatos datosDeseados = null;

											string datosDeseadosTexto = await BaseDatos.Usuarios.Buscar.OpcionStringRegion(TiendaRegion.EstadosUnidos, usuarioInteresado, "WishlistData");

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

											await BaseDatos.Usuarios.Actualizar.Opcion("WishlistData", JsonSerializer.Serialize(datosDeseados), usuarioInteresado);

											string correo = await Usuarios.Buscar.UsuarioQuiereCorreos(TiendaRegion.EstadosUnidos, usuarioInteresado, "NotificationLows");

											if (string.IsNullOrEmpty(correo) == false)
											{
												try
												{
													await Herramientas.Correos.DeseadoMinimo.Nuevo(usuarioInteresado, id, minimo, correo);
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo", ex);
												}
											}

											bool enviarPush = await BaseDatos.Usuarios.Buscar.OpcionBoolRegion(TiendaRegion.EstadosUnidos, usuarioInteresado, "NotificationPushLows");

											if (enviarPush == true)
											{
												try
												{
													decimal precioNotificar = minimo.Precio;

													if (minimo.PrecioCambiado > 0)
													{
														precioNotificar = minimo.PrecioCambiado;
													}

													var notificaciones = ServiciosGlobales.ServiceProvider.GetRequiredService<NotificacionesPush>();
													await notificaciones.EnviarNotificacion(usuarioInteresado, minimo.Nombre + " - " + Herramientas.Precios.Dolar(precioNotificar), minimo.Enlace);
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo", ex);
												}
											}
										}
									}
								}
							}
						}
						else
						{
							if ((minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.Precio, 2)) ||
							   (minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.PrecioCambiado, 2)) ||
							   (minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.Precio, 2)) ||
							   (minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.PrecioCambiado, 2)))
							{
								int cantidadHistoricos = historicosUS?.Count ?? 0;
								historicosUS = ComprobarHistoricos(TiendaRegion.EstadosUnidos, historicosUS, nuevaOferta);

								if (historicosUS.Count > cantidadHistoricos)
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
					ofertasHistoricasUS.Add(nuevaOferta);
				}
			}
			else
			{
				ofertasHistoricasUS = new List<JuegoPrecio>();
				ofertasHistoricasUS.Add(nuevaOferta);
			}

			DateTime? ahora = null;

			if (ultimaModificacion == true)
			{
				ahora = DateTime.Now;
			}

			var sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");
			sql.Append($"precioActualesTiendasUS=@precioActualesTiendasUS{indice}, ");
			sql.Append($"precioMinimosHistoricosUS=@precioMinimosHistoricosUS{indice} ");

			if (cambioPrecio) sql.Append($", historicosUS=@historicosUS{indice} ");
			if (!string.IsNullOrEmpty(slugGOG)) sql.Append($", idGog=@idGog{indice}, slugGOG=@slugGOG{indice} ");
			if (!string.IsNullOrEmpty(slugEpic)) sql.Append($", slugEpic=@slugEpic{indice} ");
			if (ahora != null) sql.Append($", ultimaModificacion=@ultimaModificacion{indice} ");

			sql.Append($"WHERE id=@id{indice};");

			var parametros = new DynamicParameters();
			parametros.Add($"@id{indice}", id);
			parametros.Add($"@precioActualesTiendasUS{indice}", JsonSerializer.Serialize(ofertasActualesUS));
			parametros.Add($"@precioMinimosHistoricosUS{indice}", JsonSerializer.Serialize(ofertasHistoricasUS));
			if (cambioPrecio) parametros.Add($"@historicosUS{indice}", JsonSerializer.Serialize(historicosUS));
			if (!string.IsNullOrEmpty(slugGOG)) { parametros.Add($"@idGog{indice}", idGOG); parametros.Add($"@slugGOG{indice}", slugGOG); }
			if (!string.IsNullOrEmpty(slugEpic)) parametros.Add($"@slugEpic{indice}", slugEpic);
			if (ahora != null) parametros.Add($"@ultimaModificacion{indice}", ahora);

			return (sql.ToString(), parametros);
		}

		private static List<JuegoHistorico> ComprobarHistoricos(TiendaRegion region, List<JuegoHistorico> historicos, JuegoPrecio nuevaOferta)
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

                if (region == TiendaRegion.Europa)
                {
					if (nuevaOferta.Moneda != JuegoMoneda.Euro)
					{
						nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
					}
					else
					{
						nuevoHistorico.Precio = nuevaOferta.Precio;
					}
				}
                else if (region == TiendaRegion.EstadosUnidos)		
                {
					if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
					{
						nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
					}
					else
					{
						nuevoHistorico.Precio = nuevaOferta.Precio;
					}
				}

				historicos.Add(nuevoHistorico);
			}
			else if (historicos.Count > 0)
			{
				JuegoHistorico nuevoHistorico = new JuegoHistorico();
				nuevoHistorico.DRM = nuevaOferta.DRM;
				nuevoHistorico.Tienda = nuevaOferta.Tienda;
				nuevoHistorico.Fecha = nuevaOferta.FechaDetectado;

				if (region == TiendaRegion.Europa)
                {
					if (nuevaOferta.Moneda != JuegoMoneda.Euro)
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
				}
                else if (region == TiendaRegion.EstadosUnidos)
                {
					if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
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

public static class ServiciosGlobales
{
	public static IServiceProvider ServiceProvider { get; set; }
}
