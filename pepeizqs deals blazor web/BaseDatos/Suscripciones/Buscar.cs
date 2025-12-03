#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using Suscripciones2;

namespace BaseDatos.Suscripciones
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

		public static List<JuegoSuscripcion> Actuales(SuscripcionTipo tipo = SuscripcionTipo.Desconocido, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE GETDATE() BETWEEN fechaEmpieza AND fechaTermina
) AS sub";

			if (tipo != SuscripcionTipo.Desconocido)
			{
				busqueda += " WHERE sub.suscripcion = " + (int)tipo;
			}

			busqueda += " ORDER BY DATEPART(MONTH, sub.fechaTermina), DATEPART(DAY, sub.fechaTermina)";

			return conexion.Query<JuegoSuscripcion>(busqueda).ToList();
        }

		public static List<JuegoSuscripcion> Año(string año, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE YEAR(fechaEmpieza) = @Año
      AND GETDATE() > fechaTermina
) AS sub
ORDER BY sub.Nombre DESC";

			return conexion.Query<JuegoSuscripcion>(busqueda, new { año }).ToList();
		}

		public static JuegoSuscripcion UnJuego(string enlace, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE enlace = @Enlace
) AS sub";

			return conexion.QueryFirstOrDefault<JuegoSuscripcion>(busqueda, new { Enlace = enlace });
		}

		public static JuegoSuscripcion Id(int id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE id = @Id
) AS sub";

			return conexion.QueryFirstOrDefault<JuegoSuscripcion>(busqueda, new { Id = id });
		}

		public static List<JuegoSuscripcion> JuegoId(int id, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE juegoId = @JuegoId
) AS sub";

			return conexion.Query<JuegoSuscripcion>(busqueda, new { JuegoId = id }).ToList();
		}

		public static List<JuegoSuscripcion> UltimasAñadidas(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE fechaEmpieza >= DATEADD(day, -7, GETDATE())
) AS sub
ORDER BY fechaEmpieza DESC"; 
			
			return conexion.Query<JuegoSuscripcion>(busqueda).ToList();
		}

		public static List<JuegoSuscripcion> Ultimos(string cantidad, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = @"
SELECT TOP {cantidad} sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
) AS sub
ORDER BY id DESC";

			return conexion.Query<JuegoSuscripcion>(busqueda).ToList();
		}
	}
}
