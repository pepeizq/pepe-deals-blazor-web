#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Plataformas
{
	public static class Buscar
	{
		public static async Task InsertarPlataforma(string id, string nombre, string campoJuego, string tablaTemporal, string tablaDescartes)
		{
			try
			{
				string sqlExisteJuego = $"SELECT 1 FROM juegos WHERE {campoJuego} = @id";

				bool yaPuesto = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteScalarAsync<int?>(sqlExisteJuego, new { id }, transaction: sentencia) != null;
				});

				if (yaPuesto == true)
				{
					return;
				}

				string sqlExisteTemporal = $"SELECT 1 FROM {tablaTemporal} WHERE id = @id";

				bool yaTemporal = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteScalarAsync<int?>(sqlExisteTemporal, new { id }, transaction: sentencia) != null;
				});

				if (yaTemporal == true)
				{
					return;
				}

				string sqlExisteDescartado = $"SELECT 1 FROM {tablaDescartes} WHERE id = @id";

				bool yaDescartado = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteScalarAsync<int?>(sqlExisteDescartado, new { id }, transaction: sentencia) != null;
				});

				if (yaDescartado == true)
				{
					return;
				}

				string sqlInsertar = null;

				if (string.IsNullOrEmpty(nombre) == true)
				{
					sqlInsertar = $"INSERT INTO {tablaTemporal} (id) VALUES (@id)";
				}
				else
				{
					sqlInsertar = $"INSERT INTO {tablaTemporal} (id, nombre) VALUES (@id, @nombre)";
				}

				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteAsync(sqlInsertar, new { id, nombre }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Insertar Plataforma", ex);
			}
		}

		public static async Task Amazon(string id, string nombre = null)
		{
			await InsertarPlataforma(
				id,
				nombre,
				campoJuego: "idAmazon",
				tablaTemporal: "temporalamazonjuegos",
				tablaDescartes: "amazonDescartes"
			);
		}

		public static async Task Epic(string id, string nombre)
		{
			await InsertarPlataforma(
				id,
				nombre,
				campoJuego: "exeEpic",
				tablaTemporal: "temporalepicjuegos",
				tablaDescartes: "epicDescartes"
			);
		}

		public static async Task Ubisoft(string id, string nombre)
		{
			await InsertarPlataforma(
				id,
				nombre,
				campoJuego: "exeUbisoft",
				tablaTemporal: "temporalubisoftjuegos",
				tablaDescartes: "ubisoftDescartes"
			);
		}

		public static async Task EA(string id, string nombre)
		{
			await InsertarPlataforma(
				id,
				nombre,
				campoJuego: "exeEA",
				tablaTemporal: "temporaleajuegos",
				tablaDescartes: "eaDescartes"
			);
		}
	}
}
