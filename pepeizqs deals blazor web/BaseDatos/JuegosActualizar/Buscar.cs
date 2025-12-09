#nullable disable

using Dapper;

namespace BaseDatos.JuegosActualizar
{

	public static class Buscar
	{
		public static async Task<bool> Existe(int idJuego, int idPlataforma, string metodo)
		{
			string sql = "SELECT COUNT(*) FROM fichasActualizar " +
				   "WHERE idJuego=@idJuego AND idPlataforma=@idPlataforma AND metodo=@metodo";

			try
			{
				int valor = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteScalarAsync<int>(sql, new
					{
						idJuego,
						idPlataforma,
						metodo
					}, transaction: sentencia);
				});

				return valor > 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("JuegosActualizar Buscar Existe", ex);	
			}

			return false;
		}

		public static async Task<List<JuegoActualizar>> Todos()
		{
			try
			{
				return await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryAsync<JuegoActualizar>("SELECT * FROM fichasActualizar", transaction: sentencia).ContinueWith(t => t.Result.ToList());
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("JuegosActualizar Buscar Todos", ex);
			}

			return new List<JuegoActualizar>();
		}
	}

	public class JuegoActualizar
	{
		public int IdJuego { get; set; }
		public int IdPlataforma { get; set; }
		public string Metodo { get; set; }
	}
}
