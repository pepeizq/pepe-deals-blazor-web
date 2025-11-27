#nullable disable

using Microsoft.Data.SqlClient;
using Tiendas2;

namespace BaseDatos.Admin
{
	public static class Buscar
	{
		public static int Dato(string id, SqlConnection conexion = null)
		{
			int dato = 0;

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

			string seleccionarDato = "SELECT contenido FROM adminDatos WHERE id=@id OPTION (MAXDOP 8)";

			using (SqlCommand comando = new SqlCommand(seleccionarDato, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						if (lector.IsDBNull(0) == false)
						{
							dato = lector.GetInt32(0);
						}
					}
				}
			}

			return dato;
		}

		public static bool TiendasPosibleUsar(TimeSpan tiempo, string tiendaId, SqlConnection conexion = null)
		{
			bool esPosible = false;

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

			string seleccionarTarea = "SELECT fecha FROM adminTiendas WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(seleccionarTarea, conexion))
			{
				comando.Parameters.AddWithValue("@id", tiendaId);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						DateTime ultimaComprobacion = lector.GetDateTime(0);

						if ((DateTime.Now - ultimaComprobacion) > tiempo)
						{
							esPosible = true;
						}
					}
				}
			}

			return esPosible;
		}

		public static AdminTarea TiendasEnUso(TimeSpan tiempo, SqlConnection conexion = null)
		{
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

			List<AdminTarea> tiendas = new List<AdminTarea>();

			string seleccionarTarea = "SELECT id, fecha FROM adminTiendas ORDER BY fecha DESC";

			using (SqlCommand comando = new SqlCommand(seleccionarTarea, conexion))
			{
				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						AdminTarea tienda = new AdminTarea
						{
							Id = lector.GetString(0),
							Fecha = lector.GetDateTime(1)
						};

						bool añadir = true;

						foreach (var tienda2 in TiendasCargar.GenerarListado())
						{
							if (tienda2.Id == tienda.Id)
							{
								if (tienda2.AdminInteractuar == false)
								{
									añadir = false;
								}

								break;
							}
						}

						if (añadir == true)
						{
							tiendas.Add(tienda);
						}
					}
				}
			}

			if (tiendas.Count > 0)
			{
				foreach (var tienda in tiendas)
				{
					DateTime ultimaComprobacion = tienda.Fecha;

					if ((DateTime.Now - ultimaComprobacion) < tiempo)
					{
						return tienda;
					}
				}
			}

			return null;
		}

		public static int TiendasValorAdicional(string id, string valor, SqlConnection conexion = null)
		{
			int valorAdicional = 0;

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

			string sqlBusqueda = "SELECT * FROM adminTiendas WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlBusqueda, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						if (lector.IsDBNull(lector.GetOrdinal(valor)) == false)
						{
							valorAdicional = lector.GetInt32(lector.GetOrdinal(valor));
						}
					}
				}
			}

			return valorAdicional;
		}

		public static AdminTarea Tarea(string id, SqlConnection conexion = null)
		{
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

			string seleccionarTarea = "SELECT * FROM adminTiendas WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(seleccionarTarea, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(0)) == false)
							{
								AdminTarea tarea = new AdminTarea
								{
									Id = lector.GetString(0),
									Fecha = lector.GetDateTime(1),
									Cantidad = lector.GetInt32(2),
									Valor1 = lector.GetInt32(3),
									Valor2 = lector.GetInt32(4)
								};

								return tarea;
							}
						}
					}
				}
			}

			return null;
		}

		public static bool TareaPosibleUsar(string id, TimeSpan tiempo, SqlConnection conexion = null)
		{
			bool esPosible = true;

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

			string seleccionarTarea = "SELECT fecha FROM adminTareas WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(seleccionarTarea, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						DateTime ultimaComprobacion = lector.GetDateTime(0);

						if ((DateTime.Now - ultimaComprobacion) < tiempo)
						{
							esPosible = false;
						}
					}
				}
			}

			return esPosible;
		}

		public static bool TiendasLibre(SqlConnection conexion = null)
		{
			int enUso = 0;

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

			string seleccionarTarea = "SELECT COUNT(*) FROM adminTiendas WHERE fecha > DATEADD(SECOND, -60, GETDATE())";

			using (SqlCommand comando = new SqlCommand(seleccionarTarea, conexion))
			{
				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						if (lector.IsDBNull(0) == false)
						{
							enUso = enUso + lector.GetInt32(0);
						}
					}
				}
			}

			if (enUso == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static int CantidadErrores(SqlConnection conexion = null)
		{
			int cantidadErrores = 0;

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

			string seleccionarTarea = "SELECT COUNT(*) FROM errores  OPTION (MAXDOP 8)";

			using (SqlCommand comando = new SqlCommand(seleccionarTarea, conexion))
			{
				using (SqlDataReader lector = comando.ExecuteReader())
				{
					if (lector.Read() == true)
					{
						if (lector.IsDBNull(0) == false)
						{
							cantidadErrores = lector.GetInt32(0);
						}
					}
				}
			}

			return cantidadErrores;
		}
	}

	public class AdminTarea
	{
		public string Id;
		public DateTime Fecha;
		public int Cantidad;
		public int Valor1;
		public int Valor2;
	}
}
