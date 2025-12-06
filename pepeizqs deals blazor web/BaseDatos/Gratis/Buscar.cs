#nullable disable

using Dapper;
using Gratis2;
using Juegos;

namespace BaseDatos.Gratis
{
	public static class Buscar
	{
		public static List<JuegoGratis> Actuales(GratisTipo tipo = GratisTipo.Desconocido)
		{
			string busqueda = @"SELECT sub.*
FROM(
	SELECT *, gratis AS Tipo

	FROM gratis

	WHERE GETDATE() BETWEEN fechaEmpieza AND fechaTermina
) AS sub";

			if (tipo != GratisTipo.Desconocido)
			{
				busqueda = busqueda + " WHERE sub.gratis=" + (int)tipo ;
			}

			busqueda = busqueda + " ORDER BY DATEPART(MONTH,sub.fechaTermina), DATEPART(DAY,sub.fechaTermina)";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<JuegoGratis>(busqueda, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Actuales", ex);
			}

			return new List<JuegoGratis>();
		}

        public static List<JuegoGratis> Año(string año)
        {
			string busqueda = @"SELECT sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE YEAR(fechaEmpieza) = @año
      AND GETDATE() > fechaTermina
) AS sub
ORDER BY sub.nombre DESC;
";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<JuegoGratis>(busqueda, new { año }, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Año", ex);
			}

			return new List<JuegoGratis>();
		}

		public static JuegoGratis UnJuego(string juegoId)
		{
			string busqueda = @"SELECT TOP 1 sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE juegoId = @juegoId
) AS sub
ORDER BY sub.ID DESC;";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<JuegoGratis>(busqueda, new { juegoId }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Uno", ex);
			}

			return new JuegoGratis();
		}

		public static JuegoGratis UnGratis(string id)
		{
			string busqueda = @"SELECT sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE id = @id
) AS sub;";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.QueryFirstOrDefault<JuegoGratis>(busqueda, new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Uno", ex);
			}

			return new JuegoGratis();
		}

		public static List<JuegoGratis> Ultimos(int cantidad)
        {
			string busqueda = @"SELECT sub.*
FROM (
    SELECT TOP (@cantidad) *, gratis AS Tipo
    FROM gratis
    ORDER BY id DESC
) AS sub;
";

			try
			{
				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<JuegoGratis>(busqueda, new { cantidad }, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Ultimos", ex);
			}

			return new List<JuegoGratis>();
		}
    }
}
