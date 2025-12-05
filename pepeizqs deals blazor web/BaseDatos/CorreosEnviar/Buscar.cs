#nullable disable

using Dapper;

namespace BaseDatos.CorreosEnviar
{
	public static class Buscar
	{
		public static List<CorreoPendienteEnviar> PendientesEnviar()
		{
			try
			{
				string busqueda = "SELECT * FROM correosEnviar";

				return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.Query<CorreoPendienteEnviar>(busqueda, transaction: sentencia).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Correos Enviar Buscar", ex);
			}

			return new List<CorreoPendienteEnviar>();
		}
	}

	public class CorreoPendienteEnviar
	{
		public int Id { get; set; }
		public string Html { get; set; }
		public string Titulo { get; set; }
		public string CorreoDesde { get; set; }
		public string CorreoHacia { get; set; }
		public CorreoPendienteTipo Tipo { get; set; }
		public string Json { get; set; }
		public DateTime Fecha { get; set; }
	}

	public enum CorreoPendienteTipo
	{
		Desconocido,
		Minimo,
		Minimos,
		Noticia,
		ContraseñaReseteada,
		ContraseñaOlvidada,
		ContraseñaCambio,
		CorreoCambio,
		CorreoConfirmacion,
		DeseadoBundle,
		DeseadosBundle
	}
}
