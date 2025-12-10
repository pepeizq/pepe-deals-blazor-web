#nullable disable

using Dapper;

namespace BaseDatos.UsuariosActualizar
{
	public static class Insertar
	{
		public static async Task Ejecutar(string idUsuario, string metodo)
		{
			if (string.IsNullOrEmpty(idUsuario) == false && string.IsNullOrEmpty(metodo) == false)
			{
				string añadir = @"
					IF NOT EXISTS (
						SELECT 1 FROM usuariosActualizar 
						WHERE idUsuario = @idUsuario
					)
					BEGIN
						INSERT INTO usuariosActualizar (idUsuario, metodo)
						VALUES (@idUsuario, @metodo)
					END
					";

				try
				{
					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						await sentencia.Connection.ExecuteAsync(añadir, new
						{
							idUsuario,
							metodo
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuarios Actualizar Insertar", ex);
				}
			}
		}
	}
}
