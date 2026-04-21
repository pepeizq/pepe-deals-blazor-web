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
					MERGE INTO usuariosActualizar AS target
					USING (SELECT @idUsuario AS idUsuario, @metodo AS metodo) AS fuente
					ON target.idUsuario = fuente.idUsuario
					WHEN NOT MATCHED THEN
						INSERT (idUsuario, metodo)
						VALUES (fuente.idUsuario, fuente.metodo);
					";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(añadir, new
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
