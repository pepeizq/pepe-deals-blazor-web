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
				string resultado = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<string>(busqueda, new { nombre });
				});

                if (string.IsNullOrEmpty(resultado) == true)
                {
					string insertar = "INSERT INTO juegosIDs " +
						   "(nombre, ids, nombreCodigo) VALUES " +
						   "(@nombre, @ids, @nombreCodigo) ";

					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(insertar, new 
						{ 
							nombre, 
							ids,
							nombreCodigo = Herramientas.Buscador.LimpiarNombre(nombre, true)
						}, transaction: sentencia);
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
