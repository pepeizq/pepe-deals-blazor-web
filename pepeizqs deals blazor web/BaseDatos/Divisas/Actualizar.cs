#nullable disable

using Dapper;
using Herramientas;

namespace BaseDatos.Divisas
{
	public static class Actualizar
	{
		public static async void Ejecutar(Divisa divisa)
		{
			string sqlActualizar = "UPDATE divisas " +
                    "SET id=@id, cantidad=@cantidad, fecha=@fecha WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(sqlActualizar, new
					{
						id = divisa.Id,
						cantidad = divisa.Cantidad,
						fecha = divisa.Fecha
					}, transaction: sentencia);
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Divisa Actualizar " + divisa.Id, ex);
			}
		}
	}
}
