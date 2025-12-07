#nullable disable

using Dapper;

namespace BaseDatos.Errores
{
    public static class Buscar
    {
		public static async Task<List<Error>> Todos()
        {
			string busqueda = "SELECT * FROM errores ORDER BY fecha DESC";

			return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
			{
				return await sentencia.Connection.QueryAsync<Error>(busqueda, transaction: sentencia).ContinueWith(t => t.Result.ToList());
			});
        }
    }

    public class Error
    {
        public string Seccion { get; set; }
        public string Mensaje { get; set; }
		public string Stacktrace { get; set; }
		public DateTime Fecha { get; set; }
		public string Enlace { get; set; }
		public string SentenciaSQL { get; set; }
	}
}
