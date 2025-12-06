#nullable disable

using Dapper;

namespace BaseDatos.Pendientes
{
	public static class Insertar
	{
        public static async void AñadirId(string nombre, string ids)
        {
            string busqueda = "SELECT ids FROM juegosIDs WHERE nombre=@nombre";

            try
            {
				string resultado = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.ExecuteScalarAsync<string>(busqueda, new { nombre }, transaction: sentencia);
				});

                if (string.IsNullOrEmpty(resultado) == true)
                {
					string insertar = "INSERT INTO juegosIDs " +
						   "(nombre, ids) VALUES " +
						   "(@nombre, @ids) ";

					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						await sentencia.Connection.ExecuteAsync(insertar, new { nombre, ids }, transaction: sentencia);
					});
				}
			}
            catch (Exception ex)
            {
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Insertar", ex);
			}
        }
    }
}
