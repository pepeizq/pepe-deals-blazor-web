#nullable disable

using Dapper;
using Herramientas;

namespace BaseDatos.Divisas
{
	public static class Buscar
	{
		public static Divisa Ejecutar(string id)
		{
			if (string.IsNullOrEmpty(id) == false) 
			{
                string sqlBuscar = "SELECT * FROM divisas WHERE id=@id";

				try
				{
					return Herramientas.BaseDatos.Select(conexion =>
					{
						return conexion.QueryFirstOrDefault<Divisa>(sqlBuscar, new { id });
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
