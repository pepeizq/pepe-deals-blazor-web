#nullable disable

using Bundles2;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Bundles
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

		public static List<Bundle> Actuales(int ordenamiento = 0, BundleTipo tipo = BundleTipo.Desconocido, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM bundles WHERE (GETDATE() BETWEEN fechaEmpieza AND fechaTermina)";

			if (tipo != BundleTipo.Desconocido)
			{
				busqueda = busqueda + " AND (bundleTipo=" + (int)tipo + ")";
			}

			if (ordenamiento == 0)
			{
				busqueda = busqueda + " ORDER BY DATEPART(YEAR,fechaTermina), DATEPART(MONTH,fechaTermina), DATEPART(DAY,fechaTermina)";
			}
			else if (ordenamiento == 1)
			{
				busqueda = busqueda + " ORDER BY DATEPART(YEAR,fechaEmpieza) DESC, DATEPART(MONTH,fechaEmpieza) DESC, DATEPART(DAY,fechaEmpieza) DESC";
			}
			else if (ordenamiento == 2)
			{
				busqueda = busqueda + " ORDER BY nombre";
			}
			else if (ordenamiento == 3)
			{
				busqueda = busqueda + " ORDER BY nombre DESC";
			}

			return conexion.Query<Bundle>(busqueda).ToList();
		}

		public static List<Bundle> Año(string año, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM bundles WHERE YEAR(fechaEmpieza) = " + año + " AND GETDATE() > fechaTermina ORDER BY nombre DESC";

			return conexion.Query<Bundle>(busqueda).ToList();
		}

		public static Bundle UnBundle(int bundleId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM bundles WHERE id=@id";

			return conexion.QueryFirstOrDefault<Bundle>(busqueda, new { id = bundleId });
		}

        public static List<Bundle> Ultimos(int cantidad, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM bundles ORDER BY id DESC";

			return conexion.Query<Bundle>(busqueda).ToList();
        }

		public static List<Bundle> Aleatorios(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT TOP 50 id, nombre FROM bundles ORDER BY NEWID()";

			return conexion.Query<Bundle>(busqueda).ToList();
		}
	}
}
