#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Streaming
{
    public static class Buscar
    {
		public static async Task<List<JuegoDRM>> DRMs(string tabla, int idJuego)
        {
			string busqueda = "SELECT drms FROM streaming" + tabla + " WHERE idJuego = " + idJuego.ToString() + " AND fecha > DATEADD(DAY, -7, CAST(GETDATE() as date))";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<string>(busqueda, new { idJuego })).ToList();
				});

				List<JuegoDRM> listaDRMs = new List<JuegoDRM>();

				foreach (var drmsTexto in resultados)
				{
					if (!string.IsNullOrEmpty(drmsTexto))
					{
						var drms = JsonSerializer.Deserialize<List<string>>(drmsTexto);

						foreach (var drm in drms)
						{
							listaDRMs.Add(JuegoDRM2.Traducir(drm));
						}
					}
				}

				return listaDRMs;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Streaming DRMs", ex);
			}

			return null;
        }

        public static async Task<bool> AmazonLuna(int idJuego)
        {
			if (idJuego > 0)
			{
				string busqueda = "SELECT 1 FROM streamingamazonluna WHERE idJuego = " + idJuego.ToString();

				try
				{
					var existe = await Herramientas.BaseDatos.Select(async conexion =>
					{
						var resultado = await conexion.ExecuteScalarAsync<int?>(busqueda);

						return resultado.HasValue;
					});

					return existe;
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Amazon Luna", ex);
				}
			}

			return false;
		}

		public static async Task<List<JuegoStreaming>> BuscarJuegos(string tabla, int orden)
		{
			if (string.IsNullOrEmpty(tabla) == false)
			{
				string busqueda = $@"SELECT
  j.id,
  j.nombre,
  j.imagenes,
  j.idSteam,
  j.idGOG,
  s.drms
FROM {tabla} s
INNER JOIN juegos j ON s.idJuego = j.id
WHERE s.idJuego <> 0
GROUP BY j.id, j.nombre, j.imagenes, j.idSteam, j.idGOG, j.analisis, s.drms";

				if (orden == 0)
				{
					busqueda += $@"
ORDER BY CASE
WHEN j.analisis = 'null' OR j.analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',',''))
END DESC";
				}

				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoStreaming>(busqueda)).ToList();
				});
			}

			return null;
		}
    }
}
