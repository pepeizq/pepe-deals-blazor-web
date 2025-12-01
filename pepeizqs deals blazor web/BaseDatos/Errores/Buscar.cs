#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Errores
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

		public static List<Error> Todos(SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT * FROM errores ORDER BY fecha DESC";

			return conexion.Query<Error>(busqueda).ToList();
        }
    }

    public class Error
    {
        public string Seccion;
        public string Mensaje;
        public string Stacktrace;
        public DateTime Fecha;
        public string Enlace;
        public string SentenciaSQL;
    }
}
