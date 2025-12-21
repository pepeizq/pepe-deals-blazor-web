#nullable disable

using Dapper;
using Juegos;
using Microsoft.VisualBasic;
using static pepeizqs_deals_blazor_web.Componentes.Cuenta.Cuenta.Juegos;
using static pepeizqs_deals_blazor_web.Componentes.Secciones.Minimos;

namespace BaseDatos.Juegos
{
	public static class Buscar
	{
		public static async Task<Juego> UnJuego(int id)
		{
			return await UnJuego(id.ToString());
		}

		public static async Task<Juego> UnJuego(string id = null, string idSteam = null, string idGog = null, string idEpic = null)
		{
			string sqlBuscar = string.Empty;

			if (id == "descartado")
			{
				return null;
			}

			if (string.IsNullOrEmpty(id) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<Juego>("SELECT * FROM juegos WHERE id=@id", new { id });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Uno Web", ex);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(idSteam) == false)
				{
					try
					{
						return await Herramientas.BaseDatos.Select(async conexion =>
						{
							return await conexion.QueryFirstOrDefaultAsync<Juego>("SELECT * FROM juegos WHERE idSteam=@idSteam", new { idSteam });
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Juego Uno Steam", ex);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(idGog) == false)
					{
						try
						{
							return await Herramientas.BaseDatos.Select(async conexion =>
							{
								return await conexion.QueryFirstOrDefaultAsync<Juego>("SELECT * FROM juegos WHERE slugGog=@slugGog", new { slugGog = idGog });
							});
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Juego Uno GOG", ex);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(idEpic) == false)
						{
							try
							{
								return await Herramientas.BaseDatos.Select(async conexion =>
								{
									return await conexion.QueryFirstOrDefaultAsync<Juego>("SELECT * FROM juegos WHERE slugEpic=@slugEpic", new { slugEpic = idEpic });
								});
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje("Juego Uno Epic", ex);
							}
						}
					}
				}
			}

			return null;
		}

		public static async Task<Juego> UnJuegoReducido(int id)
		{
			string busqueda = @"SELECT
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.media, j.freeToPlay,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaTermina < GETDATE()
        FOR JSON PATH
    ) AS GratisPasados,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaTermina < GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesPasados
FROM juegos j
WHERE id=@id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Juego>(busqueda, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Uno Reducido", ex);
			}

			return null;
		}

		public static async Task<Juego> UnJuegoComparador(int id)
		{
			string busqueda = @"SELECT
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
    j.exeEpic, j.exeUbisoft, j.freeToPlay,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaTermina < GETDATE()
        FOR JSON PATH
    ) AS GratisPasados,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaTermina < GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesPasados
FROM juegos j
WHERE id=@id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Juego>(busqueda, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Uno Comparador", ex);
			}

			return null;
		}

		public static async Task<List<Juego>> MultiplesJuegos(List<string> ids)
        {
            string sqlBuscar = string.Empty;

            if (ids != null)
            {
                if (ids.Count > 0)
                {
                    sqlBuscar = "SELECT * FROM juegos WHERE id IN (";

                    int i = 0;
                    while (i < ids.Count)
                    {
                        if (i == 0)
                        {
                            sqlBuscar = sqlBuscar + "'" + ids[i] + "'";
                        }
                        else
                        {
                            sqlBuscar = sqlBuscar + ", '" + ids[i] + "'";
                        }

                        i += 1;
                    }

                    sqlBuscar = sqlBuscar + ")";
                    sqlBuscar = sqlBuscar + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
                }
            }

			if (string.IsNullOrEmpty(sqlBuscar) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(sqlBuscar)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Multiples", ex);
				}
			}

			return new List<Juego>();
        }

        public static async Task<List<Juego>> MultiplesJuegos(List<JuegoDeseado> ids)
        {
            List<Juego> juegos = new List<Juego>();
            string sqlBuscar = string.Empty;

            if (ids != null)
            {
                if (ids.Count > 0)
                {
                    sqlBuscar = "SELECT * FROM juegos WHERE id IN (";

                    int i = 0;
                    while (i < ids.Count)
                    {
                        if (i == 0)
                        {
                            sqlBuscar = sqlBuscar + "'" + ids[i].IdBaseDatos + "'";
                        }
                        else
                        {
                            sqlBuscar = sqlBuscar + ", '" + ids[i].IdBaseDatos + "'";
                        }

                        i += 1;
                    }

                    sqlBuscar = sqlBuscar + ")";
                    sqlBuscar = sqlBuscar + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
                }
            }

            if (string.IsNullOrEmpty(sqlBuscar) == false)
            {
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(sqlBuscar)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Multiples", ex);
				}
			}

            return juegos;
        }

		public static async Task<List<Juego>> MultiplesJuegosSteam2(List<int> ids)
		{
			string sqlBuscar = string.Empty;

			if (ids?.Count > 0)
			{
				sqlBuscar = @"SELECT 
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas, j.media,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
    j.exeEpic, j.exeUbisoft, j.freeToPlay,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales
FROM juegos j
WHERE idSteam IN (";

				int i = 0;
				while (i < ids.Count)
				{
					if (i == 0)
					{
						sqlBuscar = sqlBuscar + "'" + ids[i] + "'";
					}
					else
					{
						sqlBuscar = sqlBuscar + ", '" + ids[i] + "'";
					}

					i += 1;
				}

				sqlBuscar = sqlBuscar + ")";
				sqlBuscar = sqlBuscar + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
			}

			if (string.IsNullOrEmpty(sqlBuscar) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(sqlBuscar)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Steam", ex);
				}
			}

			return new List<Juego>();
		}

		public static async Task<List<int>> MultiplesJuegosSteamOrdenado(List<int> ids)
		{
			string sqlBuscar = string.Empty;

			if (ids?.Count > 0)
			{
				sqlBuscar = "SELECT idSteam FROM juegos WHERE idSteam IN (";

				int i = 0;
				while (i < ids.Count)
				{
					if (i == 0)
					{
						sqlBuscar = sqlBuscar + "'" + ids[i] + "'";
					}
					else
					{
						sqlBuscar = sqlBuscar + ", '" + ids[i] + "'";
					}

					i += 1;
				}

				sqlBuscar = sqlBuscar + ")";
				sqlBuscar = sqlBuscar + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
			}

			if (string.IsNullOrEmpty(sqlBuscar) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<int>(sqlBuscar)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Steam Ordenado", ex);
				}
			}

			return new List<int>();
		}

		public static async Task<List<Juego>> MultiplesJuegosGOG(List<string> ids)
		{
			string sqlBuscar = string.Empty;

			if (ids != null)
			{
				if (ids.Count > 0)
				{
					sqlBuscar = "SELECT * FROM juegos WHERE idGOG IN (";

					int i = 0;
					while (i < ids.Count)
					{
						if (i == 0)
						{
							sqlBuscar = sqlBuscar + "'" + ids[i] + "'";
						}
						else
						{
							sqlBuscar = sqlBuscar + ", '" + ids[i] + "'";
						}

						i += 1;
					}

					sqlBuscar = sqlBuscar + ")";
				}
			}

			if (string.IsNullOrEmpty(sqlBuscar) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(sqlBuscar)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Multiples GOG", ex);
				}
			}

			return new List<Juego>();
		}

		public static async Task<List<JuegoUsuario>> MultiplesJuegosUsuario(List<JuegoUsuario> juegos, JuegoDRM drm, List<string> ids)
		{
			bool cogerNumero = false;
			string campo = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				cogerNumero = true;
				campo = "idSteam";
			}
			else if (drm == JuegoDRM.GOG)
			{
				cogerNumero = true;
				campo = "idGOG";
			}
			else if (drm == JuegoDRM.Amazon)
			{
				campo = "idAmazon";
			}
			else if (drm == JuegoDRM.Epic)
			{
				campo = "exeEpic";
			}
			else if (drm == JuegoDRM.Ubisoft)
			{
				campo = "exeUbisoft";
			}
			else if (drm == JuegoDRM.EA)
			{
				campo = "exeEA";
			}

			if (string.IsNullOrEmpty(campo) == false)
			{
				if (ids != null)
				{
					string sqlBuscar = string.Empty;

					if (ids.Count > 0)
					{
						sqlBuscar = "SELECT id, nombre, JSON_VALUE(imagenes, '$.Capsule_231x87'), " + campo + " FROM juegos WHERE " + campo + " IN (";

						int i = 0;
						while (i < ids.Count)
						{
							if (i == 0)
							{
								sqlBuscar = sqlBuscar + "'" + ids[i] + "'";
							}
							else
							{
								sqlBuscar = sqlBuscar + ", '" + ids[i] + "'";
							}

							i += 1;
						}

						sqlBuscar = sqlBuscar + ")";
					}

					if (string.IsNullOrEmpty(sqlBuscar) == false)
					{
						try
						{
							var resultados = await Herramientas.BaseDatos.Select(async conexion =>
							{
								return (await conexion.QueryAsync<(int id, string nombre, string imagen, object drmValor)>(sqlBuscar)).ToList();
							});

							foreach (var fila in resultados)
							{
								var existente = juegos?.FirstOrDefault(j => j.Id == fila.id);

								string drmId = null;

								if (fila.drmValor != null && fila.drmValor is not DBNull)
								{
									drmId = cogerNumero ? fila.drmValor.ToString() : (string)fila.drmValor;
								}

								if (existente != null)
								{
									existente.DRMs.Add(new JuegoUsuarioDRM
									{
										DRM = drm,
										Id = drmId
									});

									continue;
								}

								var nuevo = new JuegoUsuario
								{
									Id = fila.id,
									Nombre = fila.nombre,
									Imagen = fila.imagen,
									DRMs = new List<JuegoUsuarioDRM>
									{
										new JuegoUsuarioDRM
										{
											DRM = drm,
											Id = drmId
										}
									}
								};

								juegos.Add(nuevo);
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Usuario", ex);
						}
					}
				}
			}

			return juegos;
		}

		public static async Task<List<Juego>> Nombre2(string nombre, int cantidadResultados = 10, bool reducido = false)
		{
			string busqueda = @"SELECT TOP (@cantidad) 
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
    j.exeEpic, j.exeUbisoft, j.freeToPlay,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales
FROM juegos j
WHERE 1=1";

			if (reducido == true)
			{
				busqueda = @"SELECT TOP (@cantidad) 
								j.id, j.nombre, j.imagenes, j.tipo, j.nombreCodigo
							FROM juegos j
							WHERE 1=1";
			}

			string[] palabras = nombre.Split(" ");

			foreach (var palabra in palabras)
			{
				if (string.IsNullOrEmpty(palabra) == false)
				{
					string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);

					busqueda = busqueda + $" AND j.nombreCodigo LIKE '%{palabraLimpia}%'";
				}
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				busqueda = busqueda + @" ORDER BY CASE 
WHEN j.analisis = 'null' OR j.analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',',''))
END DESC";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda, new { cantidad = cantidadResultados })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Nombre", ex);
			}

			return new List<Juego>();
		}

		public static async Task<List<Juego>> NombreComparador(string nombre, int cantidadResultados = 10)
		{
			string busqueda = @"SELECT TOP (@cantidad) 
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
    j.exeEpic, j.exeUbisoft, j.freeToPlay,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaTermina < GETDATE()
        FOR JSON PATH
    ) AS GratisPasados,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaTermina < GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesPasados
FROM juegos j
WHERE 1=1";

			string[] palabras = nombre.Split(" ");

			foreach (var palabra in palabras)
			{
				if (string.IsNullOrEmpty(palabra) == false)
				{
					string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);

					busqueda = busqueda + $" AND nombreCodigo LIKE '%{palabraLimpia}%'";
				}
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				busqueda = busqueda + @" ORDER BY CASE 
WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
END DESC";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda, new { cantidad = cantidadResultados })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Comparador", ex);
			}

			return new List<Juego>();
		}

		public static async Task<List<Juego>> Nombre(string nombre, int cantidad = 30, bool todo = true, int tipo = -1, bool logeado = false, bool prioridad = true)
		{
			if (string.IsNullOrEmpty(nombre) == false)
			{
				string busqueda = string.Empty;
				string busquedaTodo = "*";

				if (todo == false)
				{
					busquedaTodo = "id, nombre, imagenes, precioMinimosHistoricos, precioActualesTiendas, bundles, gratis, suscripciones, tipo, analisis, idSteam, idGog, idAmazon, exeEpic, exeUbisoft, freeToPlay";
				}

				if (nombre.Contains(" ") == true)
				{
					if (nombre.Contains("  ") == true)
					{
						nombre = nombre.Replace("  ", " ");
					}

					string[] palabras = nombre.Split(" ");

					int i = 0;
					foreach (var palabra in palabras)
					{
						if (string.IsNullOrEmpty(palabra) == false)
						{
							string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);

							if (palabraLimpia.Length > 0)
							{
								if (i == 0)
								{
									busqueda = "SELECT TOP " + cantidad + " " + busquedaTodo + " FROM juegos WHERE CHARINDEX('" + palabraLimpia + "', nombreCodigo) > 0 ";
								}
								else
								{
									bool buscar = true;

									if (palabra.ToLower() == "and")
									{
										buscar = false;
									}
									else if (palabra.ToLower() == "dlc")
									{
										buscar = false;
									}
									if (palabra.ToLower() == "expansion")
									{
										buscar = false;
									}

									if (buscar == true)
									{
										busqueda = busqueda + " AND CHARINDEX('" + palabraLimpia + "', nombreCodigo) > 0 ";
									}
								}

								i += 1;
							}
						}
					}
				}
				else
				{
					busqueda = "SELECT TOP " + cantidad + " " + busquedaTodo + " FROM juegos WHERE nombreCodigo LIKE '%" + Herramientas.Buscador.LimpiarNombre(nombre) + "%'";
				}

				if (tipo > -1)
				{
					busqueda = busqueda + " AND tipo = " + tipo.ToString();
				}

				if (logeado == false)
				{
					busqueda = busqueda + " AND (mayorEdad='false' OR mayorEdad IS NULL)";
				}

				if (string.IsNullOrEmpty(busqueda) == false)
				{
					busqueda = busqueda + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
				}

				if (prioridad == true)
				{
					busqueda = busqueda + " OPTION (MAXDOP 8);";
				}

				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Nombre", ex);
				}
			}

			return null;
		}

		public static async Task<List<Juego>> Minimos(int posicion = 0, int ordenar = 0, List<MostrarJuegoTienda> tiendas = null, List<MostrarJuegoDRM> drms = null, List<MostrarJuegoCategoria> categorias = null, int? minimoDescuento = null, decimal? maximoPrecio = null, List<MostrarJuegoSteamDeck> deck = null, int lanzamiento = 0, int inteligenciaArtificial = 0, int? minimoReseñas = 0)
		{
			string busqueda = @"SELECT j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas, j.Media,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.freeToPlay, j.idMaestra,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.idMaestra
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.idMaestra
          AND g.fechaTermina < GETDATE()
        FOR JSON PATH
    ) AS GratisPasados,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.idMaestra
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.idMaestra
          AND s.FechaTermina < GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesPasados
FROM seccionMinimos j";

			string dondeTiendas = string.Empty;

			#region Where

			if (tiendas?.Count > 0)
			{
				foreach (var tienda in tiendas)
				{
					if (tienda.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeTiendas) == false)
						{
							dondeTiendas = dondeTiendas + " OR ";
						}

						dondeTiendas = dondeTiendas + "JSON_VALUE(precioMinimosHistoricos, '$[0].Tienda') = '" + tienda.TiendaId + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeTiendas) == false)
			{
				dondeTiendas = " (" + dondeTiendas + ")";
			}

			string dondeDRMs = string.Empty;

			if (drms?.Count > 0)
			{
				foreach (var drm in drms)
				{
					if (drm.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeDRMs) == false)
						{
							dondeDRMs = dondeDRMs + " OR ";
						}

						dondeDRMs = dondeDRMs + "JSON_VALUE(precioMinimosHistoricos, '$[0].DRM') = '" + ((int)drm.DRMId).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeDRMs) == false)
			{
				dondeDRMs = " (" + dondeDRMs + ")";
			}

			string dondeCategorias = string.Empty;

			if (categorias?.Count > 0)
			{
				foreach (var categoria in categorias)
				{
					if (categoria.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeCategorias) == false)
						{
							dondeCategorias = dondeCategorias + " OR ";
						}

						dondeCategorias = dondeCategorias + "tipo = '" + ((int)categoria.Categoria).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeCategorias) == false)
			{
				dondeCategorias = " (" + dondeCategorias + ")";
			}

			string dondeMinimoDescuento = string.Empty;

			if (minimoDescuento == null)
			{
				minimoDescuento = 1;
			}

			if (minimoDescuento > 0)
			{
				dondeMinimoDescuento = "JSON_VALUE(precioMinimosHistoricos, '$[0].Descuento') >= " + minimoDescuento.ToString();
			}

			if (string.IsNullOrEmpty(dondeMinimoDescuento) == false)
			{
				dondeMinimoDescuento = " (" + dondeMinimoDescuento + ")";
			}

			string dondeMaximoPrecio = string.Empty;

			if (maximoPrecio == null)
			{
				maximoPrecio = 90;
			}

			if (maximoPrecio > 0)
			{
				dondeMaximoPrecio = "CONVERT(decimal, JSON_VALUE(precioMinimosHistoricos, '$[0].Precio')) <= " + maximoPrecio.ToString();
			}

			if (string.IsNullOrEmpty(dondeMaximoPrecio) == false)
			{
				dondeMaximoPrecio = " (" + dondeMaximoPrecio + ")";
			}

			string dondeDeck = string.Empty;

			if (deck != null)
			{
				if (deck.Count > 0)
				{
					foreach (var d in deck)
					{
						if (d.Estado == true)
						{
							if (string.IsNullOrEmpty(dondeDeck) == false)
							{
								dondeDeck = dondeDeck + " OR ";
							}

							dondeDeck = dondeDeck + "deck = '" + ((int)d.Tipo).ToString() + "'";
						}
					}
				}
			}

			if (string.IsNullOrEmpty(dondeDeck) == false)
			{
				dondeDeck = " (" + dondeDeck + ")";
			}

			if (string.IsNullOrEmpty(dondeTiendas) == false && string.IsNullOrEmpty(dondeDRMs) == false && string.IsNullOrEmpty(dondeCategorias) == false && string.IsNullOrEmpty(dondeMinimoDescuento) == false && string.IsNullOrEmpty(dondeMaximoPrecio) == false && string.IsNullOrEmpty(dondeDeck) == false)
			{
				busqueda = busqueda + " WHERE " + dondeTiendas + " AND " + dondeDRMs + " AND " + dondeCategorias + " AND " + dondeMinimoDescuento + " AND " + dondeMaximoPrecio + " AND " + dondeDeck;
			}

			if (lanzamiento == 1)
			{
				busqueda = busqueda + " AND (JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam') > DATEADD(MONTH, -6, CAST(GETDATE() as date)) OR JSON_VALUE(caracteristicas, '$.FechaLanzamientoOriginal') > DATEADD(MONTH, -6, CAST(GETDATE() as date))) ";
			}

			if (lanzamiento == 2)
			{
				busqueda = busqueda + " AND (JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam') > DATEADD(MONTH, -12, CAST(GETDATE() as date)) OR JSON_VALUE(caracteristicas, '$.FechaLanzamientoOriginal') > DATEADD(MONTH, -12, CAST(GETDATE() as date))) ";
			}

			if (lanzamiento == 3)
			{
				busqueda = busqueda + " AND (JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam') > DATEADD(MONTH, -24, CAST(GETDATE() as date)) OR JSON_VALUE(caracteristicas, '$.FechaLanzamientoOriginal') > DATEADD(MONTH, -24, CAST(GETDATE() as date))) ";
			}

			if (inteligenciaArtificial == 1)
			{
				busqueda = busqueda + " AND (inteligenciaArtificial = 'true')";
			}

			if (inteligenciaArtificial == 2)
			{
				busqueda = busqueda + " AND (inteligenciaArtificial = 'false' OR inteligenciaArtificial IS NULL)";
			}

			if (minimoReseñas != null)
			{
				if (minimoReseñas > 0)
				{
					busqueda = busqueda + " AND analisis IS NOT NULL and CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',','')) > " + minimoReseñas.ToString();
				}
			}

			#endregion

			#region Order

			if (ordenar == 0)
			{
				busqueda = busqueda + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
			}

			if (ordenar == 1)
			{
				busqueda = busqueda + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, JSON_VALUE(analisis, '$.Porcentaje'))\r\n END DESC";
			}

			if (ordenar == 2)
			{
				busqueda = busqueda + " ORDER BY nombre";
			}

			if (ordenar == 3)
			{
				busqueda = busqueda + " ORDER BY nombre DESC";
			}

			if (ordenar == 4)
			{
				busqueda = busqueda + " ORDER BY CASE WHEN precioMinimosHistoricos = 'null' OR precioMinimosHistoricos IS NULL THEN 1000000 ELSE CAST(JSON_VALUE(precioMinimosHistoricos, '$[0].Precio') AS decimal(18,2)) END";
			}

			if (ordenar == 5)
			{
				busqueda = busqueda + " ORDER BY CASE WHEN precioMinimosHistoricos = 'null' OR precioMinimosHistoricos IS NULL THEN 0 ELSE CAST(JSON_VALUE(precioMinimosHistoricos, '$[0].Descuento') AS bigint) END DESC";
			}

			if (ordenar == 6)
			{
				busqueda = busqueda + " ORDER BY CASE WHEN precioMinimosHistoricos = 'null' OR precioMinimosHistoricos IS NULL THEN DATEADD(YEAR, -20, CAST(GETDATE() as date)) ELSE CAST(JSON_VALUE(precioMinimosHistoricos, '$[0].FechaDetectado') AS date) END DESC";
			}

			if (ordenar == 7)
			{
				busqueda = busqueda + " ORDER BY CASE WHEN caracteristicas = 'null' OR caracteristicas IS NULL THEN DATEADD(YEAR, -20, CAST(GETDATE() as date)) ELSE CAST(JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam') AS date) END DESC";
			}

			#endregion

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				busqueda = busqueda + @$" OFFSET {posicion} ROWS
										FETCH NEXT 100 ROWS ONLY";

				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Minimos", ex);
				}
			}

			return null;
		}

		public static async Task<List<Juego>> Ultimos(string tabla, int cantidad)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>("SELECT TOP (" + cantidad + ") * FROM " + tabla + " ORDER BY id DESC")).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Ultimos", ex);
			}

			return new List<Juego>();
		}

		public static async Task<List<Juego>> DLCs(string idMaestro = null, JuegoTipo tipo = JuegoTipo.DLC)
		{
			string busqueda = null;

			if (string.IsNullOrEmpty(idMaestro) == false)
			{
				if (tipo == JuegoTipo.DLC)
				{
					busqueda = "SELECT * FROM juegos WHERE maestro='" + idMaestro + "' AND tipo='1' ORDER BY nombre DESC";
				}
				else if (tipo == JuegoTipo.Music)
				{
					busqueda = "SELECT * FROM juegos WHERE maestro='" + idMaestro + "' AND tipo='3' ORDER BY nombre DESC";
				}
			}
			else
			{
				busqueda = "SELECT * FROM juegos WHERE (maestro IS NULL AND tipo='1') OR (maestro='no' AND tipo='1') OR (maestro IS NULL AND tipo='3') OR (maestro='no' AND tipo='3') ORDER BY nombre DESC";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego DLCs", ex);
			}

			return new List<Juego>();
		}

		public static async Task<int> DLCsCantidad()
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QuerySingleAsync<int>("SELECT COUNT(*) FROM juegos WHERE (maestro IS NULL OR maestro = 'no') AND tipo IN ('1','3')");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego DLCs Cantidad", ex);
			}

			return 0;
		}

		public static async Task<List<Juego>> Filtro(List<string> ids, int posicion = 0)
		{
			List<string> etiquetas = new List<string>();
			List<string> categorias = new List<string>();
			List<string> generos = new List<string>();
			List<string> decks = new List<string>();
			List<string> sistemas = new List<string>();
			List<string> tipos = new List<string>();

			if (ids?.Count > 0)
			{
				foreach (var id in ids)
				{
					if (id.Contains("t") == true)
					{
						etiquetas.Add(id);
					}

					if (id.Contains("c") == true || id.Contains("a") == true)
					{
						categorias.Add(id);
					}

					if (id.Contains("g") == true)
					{
						generos.Add(id);
					}

					if (id.Contains("d") == true)
					{
						decks.Add(id);
					}

					if (id.Contains("s") == true)
					{
						sistemas.Add(id);
					}

					if (id.Contains("i") == true)
					{
						tipos.Add(id);
					}
				}
			}

			string etiquetasTexto = string.Empty;

			if (etiquetas?.Count > 0)
			{
				int i = 0;

				foreach (var etiqueta in etiquetas)
				{
					string etiqueta2 = etiqueta;
					etiqueta2 = etiqueta2.Replace("t", null);

					if (etiquetasTexto.Contains(etiqueta2) == false)
					{
						if (i == 0)
						{
							etiquetasTexto = "j.etiquetas LIKE '%" + Strings.ChrW(34) + etiqueta2 + Strings.ChrW(34) + "%'";
						}
						else
						{
							etiquetasTexto = etiquetasTexto + " AND j.etiquetas LIKE '%" + Strings.ChrW(34) + etiqueta2 + Strings.ChrW(34) + "%'";
						}

						i += 1;
					}
				}

				if (string.IsNullOrEmpty(etiquetasTexto) == false)
				{
					etiquetasTexto = " AND ISJSON(j.etiquetas) > 0 AND (" + etiquetasTexto + ")";
				}
			}

			string categoriasTexto = string.Empty;

			if (categorias.Count > 0)
			{
				int i = 0;

				foreach (var categoria in categorias)
				{
					string categoria2 = categoria;
					categoria2 = categoria2.Replace("c", null);
					categoria2 = categoria2.Replace("a", null);

					if (categoriasTexto.Contains(categoria2) == false)
					{
						if (i == 0)
						{
							categoriasTexto = "j.categorias LIKE '%" + Strings.ChrW(34) + categoria2 + Strings.ChrW(34) + "%'";
						}
						else
						{
							categoriasTexto = categoriasTexto + " AND j.categorias LIKE '%" + Strings.ChrW(34) + categoria2 + Strings.ChrW(34) + "%'";
						}

						i += 1;
					}
				}

				if (string.IsNullOrEmpty(categoriasTexto) == false)
				{
					categoriasTexto = " AND ISJSON(j.categorias) > 0 AND (" + categoriasTexto + ")";
				}
			}

			string generosTexto = string.Empty;

			if (generos.Count > 0)
			{
				int i = 0;

				foreach (var genero in generos)
				{
					string genero2 = genero;
					genero2 = genero2.Replace("g", null);

					if (generosTexto.Contains(genero2) == false)
					{
						if (i == 0)
						{
							generosTexto = "j.generos LIKE '%" + Strings.ChrW(34) + genero2 + Strings.ChrW(34) + "%'";
						}
						else
						{
							generosTexto = generosTexto + " AND j.generos LIKE '%" + Strings.ChrW(34) + genero2 + Strings.ChrW(34) + "%'";
						}

						i += 1;
					}
				}

				if (string.IsNullOrEmpty(generosTexto) == false)
				{
					generosTexto = " AND ISJSON(j.generos) > 0 AND (" + generosTexto + ")";
				}
			}

			string deckTexto = string.Empty;

			if (decks.Count > 0)
			{
				int i = 0;

				foreach (var deck in decks)
				{
					string deck2 = deck;
					deck2 = deck2.Replace("d", null);

					if (deckTexto.Contains(deck2) == false)
					{
						if (i == 0)
						{
							deckTexto = "j.deck = " + deck2;
						}
						else
						{
							deckTexto = deckTexto + " AND j.deck = " + deck2;
						}

						i += 1;
					}
				}

				if (string.IsNullOrEmpty(deckTexto) == false)
				{
					deckTexto = " AND (" + deckTexto + ")";
				}
			}

			string sistemasTexto = string.Empty;

			if (sistemas.Count > 0)
			{
				foreach (var sistema in sistemas)
				{
					string sistema2 = sistema;
					sistema2 = sistema2.Replace("s", null);

					if (string.IsNullOrEmpty(sistemasTexto) == false)
					{
						sistemasTexto = sistemasTexto + " AND ";
					}

					if (sistema2 == "1")
					{
						sistemasTexto = sistemasTexto + "j.caracteristicas LIKE '%" + Strings.ChrW(34) + "Windows" + Strings.ChrW(34) + ":true%'";
					}

					if (sistema2 == "2")
					{
						sistemasTexto = sistemasTexto + "j.caracteristicas LIKE '%" + Strings.ChrW(34) + "Mac" + Strings.ChrW(34) + ":true%'";
					}

					if (sistema2 == "3")
					{
						sistemasTexto = sistemasTexto + "j.caracteristicas LIKE '%" + Strings.ChrW(34) + "Linux" + Strings.ChrW(34) + ":true%'";
					}
				}

				if (string.IsNullOrEmpty(sistemasTexto) == false)
				{
					sistemasTexto = " AND (" + sistemasTexto + ")";
				}
			}

			string tiposTexto = string.Empty;

			if (tipos.Count > 0)
			{
				int i = 0;

				foreach (var tipo in tipos)
				{
					string tipo2 = tipo;
					tipo2 = tipo2.Replace("i", null);

					if (tiposTexto.Contains(tipo2) == false)
					{
						if (i == 0)
						{
							tiposTexto = "j.tipo = " + tipo2;
						}
						else
						{
							tiposTexto = tiposTexto + " AND j.tipo = " + tipo2;
						}

						i += 1;
					}
				}

				if (string.IsNullOrEmpty(tiposTexto) == false)
				{
					tiposTexto = " AND (" + tiposTexto + ")";
				}
			}

			string busqueda = @"SELECT j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.bundles, j.tipo, j.analisis, j.idSteam, j.idGog, j.media, j.freeToPlay,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaEmpieza <= GETDATE()
          AND g.fechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS GratisActuales,
	(
        SELECT g.gratis
        FROM gratis g
        WHERE g.juegoId = j.id
          AND g.fechaTermina < GETDATE()
        FOR JSON PATH
    ) AS GratisPasados,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaEmpieza <= GETDATE()
          AND s.FechaTermina >= GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesActuales,
    (
        SELECT s.suscripcion
        FROM suscripciones s
        WHERE s.juegoId = j.id
          AND s.FechaTermina < GETDATE()
        FOR JSON PATH
    ) AS SuscripcionesPasados, CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',','')) AS Cantidad FROM juegos j " + Environment.NewLine +
				"WHERE ISJSON(analisis) > 0 " + etiquetasTexto + " " + categoriasTexto + " " + generosTexto + " " + deckTexto + " " + sistemasTexto + " " + tiposTexto +
				" ORDER BY Cantidad DESC";

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				busqueda = busqueda + @$" OFFSET {posicion} ROWS
										FETCH NEXT 50 ROWS ONLY";

				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Filtro", ex);
				}
			}

			return new List<Juego>();
		}

		public static async Task<List<Juego>> Duplicados()
		{
			string busqueda = @"SELECT * FROM juegos
 WHERE idSteam > 0 AND idSteam IN
    (SELECT idSteam FROM juegos GROUP BY idSteam HAVING COUNT(*) > 1)
    ORDER BY idSteam ";

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Duplicados", ex);
				}
			}

			return new List<Juego>();
		}

		public static List<int> BundleSteam(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return new List<int>();
			}

			string sql = @"
				SELECT j.idSteam
				FROM juegos j
				WHERE j.id IN (
					SELECT value 
					FROM STRING_SPLIT(
						(SELECT idjuegos FROM tiendasteambundles WHERE enlace = @enlaceSteam),
						','
					)
				)";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<int>(sql, new { enlaceSteam = id }, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Bundle Steam", ex);
			}

			return new List<int>();
		}

        public static async Task<List<Juego>> Aleatorios(bool fechaAPISteam = false)
        {
			string sqlBase = @"SELECT TOP 300 id, nombre FROM juegos ORDER BY NEWID()";

			string sqlFecha = @"
				SELECT TOP 300 
					id, 
					nombre, 
					idSteam, 
					fechaSteamAPIComprobacion,
					JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam') AS FechaLanzamientoSteam
				FROM juegos
				WHERE idSteam > 0
				ORDER BY NEWID()";

			if (fechaAPISteam == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(sqlBase)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Aleatorios 1", ex);
				}
			}

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(sqlFecha)).ToList();
				});

				List<Juego> juegos = new List<Juego>();

				foreach (var resultado in resultados)
				{
					Juego juego = new Juego
					{
						Id = resultado.id,
						Nombre = resultado.nombre,
						IdSteam = resultado.idSteam,
						Caracteristicas = null
					};

					if (resultado.fechaSteamAPIComprobacion != null)
					{
						juego.FechaSteamAPIComprobacion = DateTime.Parse(resultado.fechaSteamAPIComprobacion);
					}

					if (resultado.FechaLanzamientoSteam != null)
					{
						juego.Caracteristicas = new JuegoCaracteristicas
						{
							FechaLanzamientoSteam = DateTime.Parse(resultado.FechaLanzamientoSteam)
						};
					}

					juegos.Add(juego);
				}

				return juegos;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Aleatorios 2", ex);
			}

			return new List<Juego>();
		}
    }
}
