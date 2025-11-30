#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Portada
{
	public static class Limpiar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Total(SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

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

			conexion.Execute(limpiar);
		}
	}
}
