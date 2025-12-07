#nullable disable

using BaseDatos.Cupones;
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
					return Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
					{
						return sentencia.Connection.QueryFirstOrDefault<Divisa>(sqlBuscar, new { id }, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Cupones Activos", ex, null, false);
				}
			}

			return null;
		}
	}
}
