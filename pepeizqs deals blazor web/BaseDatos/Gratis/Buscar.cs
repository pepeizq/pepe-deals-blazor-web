#nullable disable

using Dapper;
using Gratis2;
using Juegos;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Gratis
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

		public static List<JuegoGratis> Actuales(GratisTipo tipo = GratisTipo.Desconocido, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT sub.*
FROM(
	SELECT *, gratis AS Tipo

	FROM gratis

	WHERE GETDATE() BETWEEN fechaEmpieza AND fechaTermina
) AS sub";

			if (tipo != GratisTipo.Desconocido)
			{
				busqueda = busqueda + " AND sub.gratis=" + (int)tipo ;
			}

			busqueda = busqueda + " ORDER BY DATEPART(MONTH,sub.fechaTermina), DATEPART(DAY,sub.fechaTermina)";

			return conexion.Query<JuegoGratis>(busqueda).ToList();
		}

        public static List<JuegoGratis> Año(string año, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE YEAR(fechaEmpieza) = @año
      AND GETDATE() > fechaTermina
) AS sub
ORDER BY sub.nombre DESC;
";

			return conexion.Query<JuegoGratis>(busqueda, new { año }).ToList();
		}

		public static JuegoGratis UnJuego(string juegoId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT TOP 1 sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE juegoId = @juegoId
) AS sub
ORDER BY sub.ID DESC;";

			return conexion.QueryFirstOrDefault<JuegoGratis>(busqueda, new { juegoId });
		}

		public static JuegoGratis UnGratis(string id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE id = @id
) AS sub;";

			return conexion.QueryFirstOrDefault<JuegoGratis>(busqueda, new { id });
		}

		public static List<JuegoGratis> Ultimos(int cantidad, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = @"SELECT sub.*
FROM (
    SELECT TOP (@cantidad) *, gratis AS Tipo
    FROM gratis
    ORDER BY id DESC
) AS sub;
";

			return conexion.Query<JuegoGratis>(busqueda, new { cantidad }).ToList();
		}
    }
}
