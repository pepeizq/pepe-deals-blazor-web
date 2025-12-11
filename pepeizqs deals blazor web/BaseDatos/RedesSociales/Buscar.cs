#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;

namespace BaseDatos.RedesSociales
{
    public static class Buscar
    {
        public static async Task<List<Juego>> OfertasDelDia()
        {
            string busqueda = @"SELECT TOP 100 j.idMaestra, j.nombre, j.precioMinimosHistoricos, CONVERT(datetime2, JSON_VALUE(j.precioMinimosHistoricos, '$[0].FechaDetectado')) AS Fecha, JSON_VALUE(j.precioMinimosHistoricos, '$[0].DRM') AS DRM, j.analisis, CONVERT(datetime2, JSON_VALUE(j.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM seccionMinimos j
                WHERE CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',','')) > 1000 AND JSON_VALUE(j.precioMinimosHistoricos, '$[0].DRM') = '0'
                AND CAST(CONVERT(datetime2, JSON_VALUE(j.precioMinimosHistoricos, '$[0].FechaDetectado')) AS date) = CAST(GETDATE() AS date)
                ORDER BY CASE WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',','')) END DESC";

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						return await sentencia.Connection.QueryAsync<Juego>(busqueda, transaction: sentencia).ContinueWith(t => t.Result.ToList());
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Ofertas Dia", ex);
				}
			}

			return null;
        }
    }
}