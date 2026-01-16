#nullable disable

using Dapper;
using Juegos;

namespace BaseDatos.RedesSociales
{
    public static class Buscar
    {
        public static async Task<List<Juego>> OfertasDelDia(int drm)
        {
            string busqueda = @"SELECT TOP 100 j.idMaestra, j.nombre, j.precioMinimosHistoricos, CONVERT(datetime2, JSON_VALUE(j.precioMinimosHistoricos, '$[0].FechaDetectado')) AS Fecha, JSON_VALUE(j.precioMinimosHistoricos, '$[0].DRM') AS DRM, j.analisis, CONVERT(datetime2, JSON_VALUE(j.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM seccionMinimos j
                WHERE JSON_VALUE(j.precioMinimosHistoricos, '$[0].DRM') = @drm
                AND CAST(CONVERT(datetime2, JSON_VALUE(j.precioMinimosHistoricos, '$[0].FechaDetectado')) AS date) = CAST(GETDATE() AS date)
                ";

			if (drm == 0)
			{
				busqueda = busqueda + @" AND CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',','')) > 100";
			}

			busqueda = busqueda + @" ORDER BY CASE WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',','')) END DESC"
;
			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda, new { drm })).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Ofertas Dia " + drm.ToString(), ex);
				}
			}

			return null;
        }
    }
}