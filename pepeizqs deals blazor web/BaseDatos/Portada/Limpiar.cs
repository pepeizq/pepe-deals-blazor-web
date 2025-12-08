#nullable disable

using Dapper;

namespace BaseDatos.Portada
{
	public static class Limpiar
	{
		public static async Task Total()
		{
			string limpiar = @"DELETE sm
FROM seccionMinimos sm
CROSS APPLY OPENJSON(sm.PrecioMinimosHistoricos)
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
    );";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteAsync(limpiar, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Limpiar", ex);
			}
		}
	}
}
