#nullable disable

namespace Herramientas.Bundles
{
	public static class Reddit
	{
		public static async Task<string> Generar(Bundles2.Bundle bundle)
		{
			string texto = null;

			if (bundle != null)
			{
				texto = "[" + bundle.Enlace + "](" + bundle.Enlace + ")" + Environment.NewLine + Environment.NewLine;

				if (bundle.Tiers != null)
				{
					bundle.Tiers.Sort(delegate (Bundles2.BundleTier t1, Bundles2.BundleTier t2)
					{
						return t1.Posicion.CompareTo(t2.Posicion);
					});

					decimal totalMinimos = 0;

					foreach (var tier in bundle.Tiers.ToList())
					{
						List<Bundles2.BundleJuego> juegosTier = new List<Bundles2.BundleJuego>();

						if (bundle.Juegos != null)
						{
							if (bundle.Juegos.Count > 0)
							{
								foreach (var juego in bundle.Juegos)
								{
									if (juego.Tier != null)
									{
										if (juego.Tier.Posicion == tier.Posicion)
										{
											juegosTier.Add(juego);
										}
									}
								}
							}
						}

						if (juegosTier.Count > 0)
						{
							juegosTier = juegosTier.OrderBy(x => x.Nombre).ToList();
						}

						foreach (var juego in juegosTier)
						{
							if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
							{
								if (string.IsNullOrEmpty(juego.Juego?.Maestro) == false)
								{
									if (juego.Juego?.Maestro != "no")
									{
										foreach (var juego2 in juegosTier)
										{
											if (juego2.JuegoId == juego.Juego.Maestro)
											{
												if (juego2.DLCs == null)
												{
													juego2.DLCs = new List<string>();
												}

												bool añadir = true;

												if (juego2.DLCs.Count > 0)
												{
													foreach (var dlc in juego2.DLCs)
													{
														if (dlc == juego.JuegoId)
														{
															añadir = false;
															break;
														}
													}
												}

												if (añadir == true)
												{
													juego2.DLCs.Add(juego.JuegoId);
												}
											}
										}
									}
								}
							}
						}

						if (bundle.Pick == true)
						{
							if (tier.Posicion == 1)
							{
								foreach (var tier2 in bundle.Tiers)
								{
									if (tier2.CantidadJuegos == 1)
									{
										texto = texto + "**" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto("en", "String21", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "**" + Environment.NewLine + Environment.NewLine;
									}
									else if (tier2.CantidadJuegos > 1)
									{
										texto = texto + "**" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto("en", "String8", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "** / " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio) / tier2.CantidadJuegos) + " (" + Herramientas.Idiomas.BuscarTexto("en", "String20", "Bundle") + ")" + Environment.NewLine + Environment.NewLine;
									}
								}

								texto = texto + Environment.NewLine;
							}
						}
						else
						{
							texto = texto + "**Tier " + tier.Posicion.ToString() + ": " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio)) + "**  " + Environment.NewLine;

							foreach (var juego in juegosTier)
							{
								if (juego.Juego == null)
								{
									juego.Juego = await global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
								}

								if (juego.Juego?.PrecioMinimosHistoricos != null)
								{
									foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
									{
										if (historico.DRM == juego.DRM)
										{
											if (historico.PrecioCambiado > 0)
											{
												totalMinimos = totalMinimos + historico.PrecioCambiado;
											}
											else
											{
												totalMinimos = totalMinimos + historico.Precio;
											}

											break;
										}
									}
								}
							}

							texto = texto + Herramientas.Idiomas.BuscarTexto("en", "String14", "Bundle") + ": " + Herramientas.Precios.Euro(totalMinimos) + Environment.NewLine + Environment.NewLine;
						}

						if (juegosTier.Count > 0)
						{
							foreach (var juego in juegosTier)
							{
								if (juego.Juego == null)
								{
									juego.Juego = await global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
								}

								if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
								{
									if (string.IsNullOrEmpty(juego.Juego?.Maestro) == false)
									{
										if (juego.Juego?.Maestro != "no")
										{
											foreach (var juego2 in juegosTier)
											{
												if (juego2.JuegoId == juego.Juego?.Maestro)
												{
													if (juego2.DLCs == null)
													{
														juego2.DLCs = new List<string>();
													}

													bool añadir = true;

													if (juego2.DLCs.Count > 0)
													{
														foreach (var dlc in juego2.DLCs)
														{
															if (dlc == juego.JuegoId)
															{
																añadir = false;
																break;
															}
														}
													}

													if (añadir == true)
													{
														juego2.DLCs.Add(juego.JuegoId);
													}
												}
											}
										}
									}
								}
							}

							texto = texto + "Game | DRM | Historical Price | Reviews | DLCs" + Environment.NewLine;
							texto = texto + "---- | ---- | ------------- | ----- | -----" + Environment.NewLine;

							foreach (var juego in juegosTier)
							{
								bool mostrar = true;

								if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
								{
									if (string.IsNullOrEmpty(juego.Juego.Maestro) == false)
									{
										if (juego.Juego.Maestro != "no")
										{
											foreach (var juego2 in juegosTier)
											{
												if (juego2.JuegoId == juego.Juego.Maestro)
												{
													mostrar = false;
													break;
												}
											}
										}
									}
								}

								if (mostrar == true)
								{
									string nombre = juego.Nombre;
									string precioMinimo = "--";
									string dlcs = "--";
									string reseñas = "--";

									if (juego.Juego != null)
									{
										if (juego.DRM == Juegos.JuegoDRM.Steam && juego.Juego.IdSteam > 0)
										{
											nombre = "[" + nombre + "](https://store.steampowered.com/app/" + juego.Juego.IdSteam + ")";
										}
										else if (juego.DRM == Juegos.JuegoDRM.GOG && string.IsNullOrEmpty(juego.Juego.SlugGOG) == false)
										{
											nombre = "[url=https://www.gog.com/game/" + juego.Juego.SlugGOG + "]" + nombre + "[/url]";
										}
										else if (juego.DRM == Juegos.JuegoDRM.Epic && string.IsNullOrEmpty(juego.Juego.SlugEpic) == false)
										{
											nombre = "[url=https://www.epicgames.com/store/p/" + juego.Juego.SlugEpic + "]" + nombre + "[/url]";
										}

										if (juego.Juego.PrecioMinimosHistoricos != null)
										{
											decimal precioMinimoDecimal = 0;
											decimal precioMinimoComparar = 10000;

											foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
											{
												if (historico.DRM == juego.DRM)
												{
													if (historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoComparar)
													{
														precioMinimoComparar = historico.PrecioCambiado;
													}
													else if (historico.Precio > 0 && historico.Precio < precioMinimoComparar)
													{
														precioMinimoComparar = historico.Precio;
													}
												}
											}

											if (precioMinimoComparar < 10000)
											{
												precioMinimoDecimal = precioMinimoComparar;
											}

											if (juego.DLCs?.Count > 0)
											{
												foreach (var dlc in juego.DLCs)
												{
													Juegos.Juego juegoDLC = await global::BaseDatos.Juegos.Buscar.UnJuego(dlc);

													if (juegoDLC?.PrecioMinimosHistoricos != null)
													{
														decimal precioMinimoDLCComparar = 10000;

														foreach (var historico in juegoDLC.PrecioMinimosHistoricos)
														{
															if (historico.DRM == juego.DRM)
															{
																if (historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoDLCComparar)
																{
																	precioMinimoDLCComparar = historico.PrecioCambiado;
																}
																else if (historico.Precio > 0 && historico.Precio < precioMinimoDLCComparar)
																{
																	precioMinimoDLCComparar = historico.Precio;
																}
															}
														}

														if (precioMinimoDLCComparar < 10000)
														{
															precioMinimoDecimal = precioMinimoDecimal + precioMinimoDLCComparar;
														}
													}
												}
											}

											if (precioMinimoDecimal > 0)
											{
												precioMinimo = Herramientas.Precios.Euro(precioMinimoDecimal);
											}

											if (juego.DLCs?.Count > 0)
											{
												if (juego.DLCs.Count == 1)
												{
													dlcs = "1 DLC";
												}
												else if (juego.DLCs.Count > 1)
												{
													dlcs = juego.DLCs?.Count.ToString() + " DLCs";
												}
											}

											if (juego.Juego.Analisis != null)
											{
												reseñas = juego.Juego.Analisis.Porcentaje + "%";
											}
										}
									}

									texto = texto + nombre + " | " + Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + " | " + precioMinimo + " | " + reseñas + " | " + dlcs + Environment.NewLine;
								}
							}

							texto = texto + Environment.NewLine + Environment.NewLine;
						}
					}
				}
			}

			if (string.IsNullOrEmpty(texto) == false)
			{
				texto = texto.Trim();
			}

			return texto;
		}
	}
}
