#nullable disable

using Dapper;
using Tiendas2;

namespace BaseDatos.Portada
{
	public static class Limpiar
	{
		public static async Task Total(TiendaRegion region)
		{
			string tabla = "seccionMinimos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				tabla = "seccionMinimosUS";
			}

			string precioMinimosHistoricos = "sm.precioMinimosHistoricos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "sm.precioMinimosHistoricosUS";
			}

			string limpiar = $@"WHILE 1 = 1
BEGIN
    DELETE TOP (500) sm
    FROM {tabla} sm
    CROSS APPLY OPENJSON({precioMinimosHistoricos})
    WITH (
        FechaActualizacion DATETIME2 '$.FechaActualizacion',
        Tienda NVARCHAR(50) '$.Tienda'
    ) AS pmh
    WHERE
        NOT (
            (pmh.Tienda IN ('steam', 'steambundles') AND pmh.FechaActualizacion >= DATEADD(hour, -24, GETDATE())) OR
            (pmh.Tienda IN ('humblestore', 'humblechoice') AND pmh.FechaActualizacion >= DATEADD(hour, -25, GETDATE())) OR
            (pmh.Tienda = 'epicgamesstore' AND pmh.FechaActualizacion >= DATEADD(hour, -48, GETDATE())) OR
            (pmh.FechaActualizacion >= DATEADD(hour, -12, GETDATE()))
        );

    IF @@ROWCOUNT = 0 BREAK;
END";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(limpiar, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Limpiar", ex, false);
			}
		}
	}
}
