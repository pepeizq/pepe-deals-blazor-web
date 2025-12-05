#nullable disable

using Dapper;
using Streaming2;
using Suscripciones2;
using Tiendas2;

namespace BaseDatos.Admin
{
	public static class Buscar
	{
		public static int Dato(string id)
		{
			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<int>("SELECT contenido FROM adminDatos WHERE id=@id OPTION (MAXDOP 8)", new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Buscar Dato", ex);
			}

			return 0;
		}

		public static bool TiendasPosibleUsar(TimeSpan tiempo, string tiendaId)
		{
			try
			{
				DateTime? fecha = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<DateTime?>("SELECT fecha FROM adminTiendas WHERE id=@id", new { id = tiendaId }, transaction: sentencia);
				});

				if (fecha == null)
				{
					return false;
				}

				return (DateTime.Now - fecha.Value) > tiempo;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Buscar Dato", ex);
			}

			return false;
		}

		public static List<AdminTarea> TiendasEnUso(TimeSpan tiempo)
		{
			try
			{
				List<AdminTarea> tiendas = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<AdminTarea>("SELECT id, fecha FROM adminTiendas ORDER BY fecha DESC", transaction: sentencia).ToList();
				});

				tiendas = tiendas.Where(t =>
				{
					foreach (Tienda tienda2 in TiendasCargar.GenerarListado())
					{
						if (tienda2.Id == t.Id && tienda2.AdminInteractuar == false)
						{
							return false;
						}
					}

					foreach (Suscripcion suscripcion2 in SuscripcionesCargar.GenerarListado())
					{
						if (suscripcion2.Id.ToString() == t.Id && suscripcion2.AdminInteractuar == true)
						{
							return false;
						}
					}

					foreach (Streaming2.Streaming streaming2 in StreamingCargar.GenerarListado())
					{
						if (streaming2.Id.ToString() == t.Id)
						{
							return false;
						}
					}

					return true;
				}).ToList();

				List<AdminTarea> tiendasEnUso = new List<AdminTarea>();

				foreach (var tienda in tiendas)
				{
					if ((DateTime.Now - tienda.Fecha) < tiempo)
					{
						tiendasEnUso.Add(tienda);
					}
				}

				return tiendasEnUso;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas En Uso", ex);
			}

			return new List<AdminTarea>();
		}

		public static int TiendasValorAdicional(string id, string valor)
		{
			try
			{
				var fila = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault("SELECT * FROM adminTiendas WHERE id=@id", new { id }, transaction: sentencia);
				});

				if (fila == null)
				{
					return 0;
				}

				var diccionario = (IDictionary<string, object>)fila;

				if (diccionario.ContainsKey(valor) && diccionario[valor] != null)
				{
					return Convert.ToInt32(diccionario[valor]);
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas Valor Adicional", ex);
			}

			return 0;
		}

		public static List<AdminTarea> TareasTiendas()
		{
			try
			{
				string sql = null;

				foreach (Tienda tienda in TiendasCargar.GenerarListado())
				{
					if (tienda.AdminEnseñar == true)
					{
						if (string.IsNullOrEmpty(sql) == true)
						{
							sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{tienda.Id}'";

						}
						else
						{
							sql = sql + Environment.NewLine + $"UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{tienda.Id}'";
						}
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
					{
						return sentencia.Connection.Query<AdminTarea>(sql, transaction: sentencia).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Tiendas", ex);
			}

			return new List<AdminTarea>();
		}

		public static List<AdminTarea> TareasSuscripciones()
		{
			try
			{
				string sql = null;

				foreach (Suscripcion suscripcion in SuscripcionesCargar.GenerarListado())
				{
					if (suscripcion.AdminInteractuar == true)
					{
						if (string.IsNullOrEmpty(sql) == true)
						{
							sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{suscripcion.Id}'";

						}
						else
						{
							sql = sql + Environment.NewLine + $"UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{suscripcion.Id}'";
						}
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
					{
						return sentencia.Connection.Query<AdminTarea>(sql, transaction: sentencia).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Tiendas", ex);
			}

			return new List<AdminTarea>();
		}

		public static List<AdminTarea> TareasStreaming()
		{
			try
			{
				string sql = null;

				foreach (var streaming in StreamingCargar.GenerarListado())
				{
					if (string.IsNullOrEmpty(sql) == true)
					{
						sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{streaming.Id}'";

					}
					else
					{
						sql = sql + Environment.NewLine + $"UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{streaming.Id}'";
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
					{
						return sentencia.Connection.Query<AdminTarea>(sql, transaction: sentencia).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Tiendas", ex);
			}

			return new List<AdminTarea>();
		}

		public static List<AdminTarea> TareasUltimos60Segundos(List<AdminTarea> tareas)
		{
			try
			{
				var ids = tareas.Where(t => t.Fecha.AddSeconds(60) >= DateTime.Now).Select(t => t.Id).Distinct().ToList();

				if (ids.Count == 0)
				{
					return tareas;
				}

				string sql = string.Empty;

				if (ids?.Count > 0)
				{
					sql = "SELECT id, fecha, mensaje AS Cantidad, valorAdicional AS Valor1, valorAdicional2 AS Valor2 FROM adminTiendas WHERE id IN (";

					int i = 0;
					while (i < ids.Count)
					{
						if (i == 0)
						{
							sql = sql + "'" + ids[i] + "'";
						}
						else
						{
							sql = sql + ", '" + ids[i] + "'";
						}

						i += 1;
					}

					sql = sql + ")";
				}

				List<AdminTarea> nuevas = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<AdminTarea>(sql, transaction: sentencia).ToList();
				});

				var diccionarioNuevas = nuevas.ToDictionary(t => t.Id);

				List<AdminTarea> fusionadas = tareas.Select(t => diccionarioNuevas.TryGetValue(t.Id, out var actualizada) ? actualizada : t).ToList();

				return fusionadas;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Ultimos 60 Segundos", ex);
			}

			return new List<AdminTarea>();
		}

		public static bool TareaPosibleUsar(string id, TimeSpan tiempo)
		{
			try
			{
				DateTime? fecha = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<DateTime?>("SELECT fecha FROM adminTareas WHERE id=@id", new { id }, transaction: sentencia);
				}); 

				if (fecha == null)
				{
					return true;
				}

				return (DateTime.Now - fecha.Value) >= tiempo;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tarea Posible Usar", ex);
			}

			return true;
		}

		public static bool TiendasLibre()
		{
			try
			{
				int enUso = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM adminTiendas WHERE fecha > DATEADD(SECOND, -60, GETDATE())", transaction: sentencia);
				}); 

				return enUso == 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas Libre", ex);
			}

			return true;
		}

		public static int CantidadErrores()
		{
			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM errores OPTION (MAXDOP 8)", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Cantidad Errores", ex);
			}

			return 0;
		}
	}

	public class AdminTarea
	{
		public string Id { get; set; }
		public DateTime Fecha { get; set; }
		public int Cantidad { get; set; }
		public int Valor1 { get; set; }
		public int Valor2 { get; set; }
	}
}
