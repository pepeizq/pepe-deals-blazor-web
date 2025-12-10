#nullable disable

using Dapper;

namespace BaseDatos.UsuariosActualizar
{
	public static class Limpiar
	{
		public static async Task Una(UsuarioActualizar usuario)
		{
			if (usuario == null)
			{
				return;
			}

			string eliminar = "DELETE FROM usuariosActualizar WHERE idUsuario=@idUsuario AND metodo=@metodo";

			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync(eliminar, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuarios Actualizar Limpiar", ex);
			}
		}
	}
}