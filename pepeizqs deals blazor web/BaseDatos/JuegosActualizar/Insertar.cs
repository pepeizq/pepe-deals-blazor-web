#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.JuegosActualizar
{
	public static class Insertar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Ejecutar(int idJuego, int idPlataforma, string metodo, SqlConnection conexion = null)
		{
			if (idJuego > 0 && idPlataforma > 0 && string.IsNullOrEmpty(metodo) == false)
			{
				conexion = CogerConexion(conexion);

				if (Buscar.Existe(idJuego, idPlataforma, metodo) == false)
				{
					string sqlAñadir = "INSERT INTO fichasActualizar " +
						 "(idJuego, idPlataforma, metodo) VALUES " +
						 "(@idJuego, @idPlataforma, @metodo) ";

					try
					{
						conexion.Execute(sqlAñadir, new
						{
							IdJuego = idJuego,
							IdPlataforma = idPlataforma,
							Metodo = metodo
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Insertar Fichas Actualizar", ex);
					}
				}
			}
		}
	}
}
