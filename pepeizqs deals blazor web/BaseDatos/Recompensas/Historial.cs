#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Recompensas
{
    public class RecompensaHistorial
    {
        public string UsuarioId { get; set; }
        public int Coins { get; set; }
        public string Razon { get; set; }
        public DateTime Fecha { get; set; }
    }

    public static class Historial
    {
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Insertar(string usuarioId, int coins, string razon, DateTime fecha, SqlConnection conexion = null)
        {
            conexion = CogerConexion(conexion);

            try
            {
				string sqlInsertar = "INSERT INTO recompensasHistorial " +
				   "(usuarioId, coins, razon, fecha) VALUES " +
				   "(@usuarioId, @coins, @razon, @fecha) ";

				conexion.Execute(sqlInsertar, new
				{
					usuarioId,
					coins,
					razon,
					fecha
				});
			}
            catch (Exception ex)
            {
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Insertar", ex);
			}
        }

        public static List<RecompensaHistorial> Leer(string usuarioId = null, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			try
			{
				string busqueda = "SELECT TOP 30 * FROM recompensasHistorial";
				
				if (string.IsNullOrEmpty(usuarioId) == false)
				{
					busqueda = busqueda + " WHERE usuarioId = @usuarioId";
				}

				busqueda = busqueda + " ORDER BY fecha DESC";

				return conexion.Query<RecompensaHistorial>(busqueda, new { usuarioId }).ToList();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Leer", ex, conexion);
			}
			
            return new List<RecompensaHistorial>();
        }
    }
}
