#nullable disable

using Dapper;
using Juegos;
using Suscripciones2;

namespace BaseDatos.Suscripciones
{
    public static class Buscar
    {
		public static async Task<List<JuegoSuscripcion>> Actuales(SuscripcionTipo tipo = SuscripcionTipo.Desconocido)
        {
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

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Actuales", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> Año(string año)
		{
			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE YEAR(fechaEmpieza) = @Año
      AND GETDATE() > fechaTermina
) AS sub
ORDER BY sub.Nombre DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda, new { año })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Año", ex);
			}

			return null;
		}

		public static async Task<JuegoSuscripcion> Id(int id)
		{
			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE id = @Id
) AS sub";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<JuegoSuscripcion>(busqueda, new { Id = id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Id", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> JuegoId(int id)
		{
			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE juegoId = @JuegoId
) AS sub";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda, new { JuegoId = id })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones JuegoId", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> UltimasAñadidas()
		{
			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE fechaEmpieza >= DATEADD(day, -7, GETDATE())
) AS sub
ORDER BY fechaEmpieza DESC"; 
			
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Ultimas Añadidas", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> Ultimos(string cantidad)
		{
			string busqueda = @"
SELECT TOP {cantidad} sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
) AS sub
ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Ultimos", ex);
			}

			return null;
		}
	}
}
