#nullable disable

using Dapper;
using Herramientas;

namespace BaseDatos.Divisas
{
	public static class Buscar
	{
		public static async Task<Divisa> Ejecutar(string id)
		{
			if (string.IsNullOrEmpty(id) == false) 
			{
                string sqlBuscar = "SELECT * FROM divisas WHERE id=@id";

				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<Divisa>(sqlBuscar, new { id });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Divisa Buscar", ex, false);
				}
			}

			return null;
		}
	}
}
