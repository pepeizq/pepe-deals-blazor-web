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
    }
}
