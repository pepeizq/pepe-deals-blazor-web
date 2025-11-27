#nullable disable

using Microsoft.Data.SqlClient;

namespace BaseDatos.Portada
{
	public static class Limpiar
	{
		public static void Total(SqlConnection conexion = null)
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

			using (SqlCommand comando = new SqlCommand(limpiar, conexion))
			{
				comando.ExecuteNonQuery();
			}
		}
	}
}
