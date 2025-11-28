#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;
using Streaming2;
using Suscripciones2;
using Tiendas2;

namespace BaseDatos.Admin
{
	public static class Buscar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static int Dato(string id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				return conexion.QueryFirstOrDefault<int>("SELECT contenido FROM adminDatos WHERE id=@id OPTION (MAXDOP 8)", new { id });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Buscar Dato", ex);
			}

			return 0;
		}

		public static bool TiendasPosibleUsar(TimeSpan tiempo, string tiendaId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				DateTime? fecha = conexion.QueryFirstOrDefault<DateTime?>("SELECT fecha FROM adminTiendas WHERE id=@id", new { id = tiendaId });

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

		public static List<AdminTarea> TiendasEnUso(TimeSpan tiempo, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			List<AdminTarea> tiendas = conexion.Query<AdminTarea>("SELECT id, fecha FROM adminTiendas ORDER BY fecha DESC").ToList();

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

		public static int TiendasValorAdicional(string id, string valor, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			var fila = conexion.QueryFirstOrDefault("SELECT * FROM adminTiendas WHERE id=@id", new { id });

			if (fila == null)
			{
				return 0;
			}

			var diccionario = (IDictionary<string, object>)fila;

			if (diccionario.ContainsKey(valor) && diccionario[valor] != null)
			{
				return Convert.ToInt32(diccionario[valor]);
			}

			return 0;
		}

		public static List<AdminTarea> TareasTiendas(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			return conexion.Query<AdminTarea>(sql).ToList();
		}

		public static List<AdminTarea> TareasSuscripciones(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			return conexion.Query<AdminTarea>(sql).ToList();
		}

		public static List<AdminTarea> TareasStreaming(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			return conexion.Query<AdminTarea>(sql).ToList();
		}

		public static List<AdminTarea> TareasUltimos60Segundos(List<AdminTarea> tareas, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			List<AdminTarea> nuevas = conexion.Query<AdminTarea>(sql).ToList();

			var diccionarioNuevas = nuevas.ToDictionary(t => t.Id);

			List<AdminTarea> fusionadas = tareas.Select(t => diccionarioNuevas.TryGetValue(t.Id, out var actualizada) ? actualizada : t).ToList();

			return fusionadas;
		}

		public static bool TareaPosibleUsar(string id, TimeSpan tiempo, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			DateTime? fecha = conexion.QueryFirstOrDefault<DateTime?>("SELECT fecha FROM adminTareas WHERE id=@id", new { id });

			if (fecha == null)
			{
				return true;
			}

			return (DateTime.Now - fecha.Value) >= tiempo;
		}

		public static bool TiendasLibre(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			int enUso = conexion.QueryFirst<int>("SELECT COUNT(*) FROM adminTiendas WHERE fecha > DATEADD(SECOND, -60, GETDATE())");

			return enUso == 0;
		}

		public static int CantidadErrores(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			return conexion.QueryFirst<int>("SELECT COUNT(*) FROM errores OPTION (MAXDOP 8)");
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
