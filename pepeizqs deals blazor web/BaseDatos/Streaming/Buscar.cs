#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BaseDatos.Streaming
{
    public static class Buscar
    {
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static List<JuegoDRM> DRMs(string tabla, int idJuego, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT drms FROM streaming" + tabla + " WHERE idJuego = " + idJuego.ToString() + " AND fecha > DATEADD(DAY, -7, CAST(GETDATE() as date))";

			var resultados = conexion.Query<string>(busqueda, new { idJuego });

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

        public static bool AmazonLuna(int idJuego, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT nombre FROM streamingamazonluna WHERE idJuego = " + idJuego.ToString();

			var existe = conexion.ExecuteScalar<string>(busqueda);

			return existe != null;
		}
    }
}
