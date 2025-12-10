#nullable disable

using Dapper;

namespace BaseDatos.Usuarios
{
    public static class Actualizar
    {
		public static async Task PatreonComprobacion(string correoBuscar, DateTime fechaActualizar, int contribucion)
        {
			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR PatreonMail = @PatreonMail";
			string id = string.Empty;

			try
			{
				id = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync<string>(busqueda, new
					{
						Email = correoBuscar,
						PatreonMail = correoBuscar
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Comprobacion 1", ex, false);
			}

			if (string.IsNullOrEmpty(id) == false)
			{
				string actualizar = @"UPDATE AspNetUsers 
                                 SET PatreonLastCheck = @PatreonLastCheck, 
                                     PatreonContribution = @PatreonContribution 
                                 WHERE Id = @Id";

				try
				{
					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						await sentencia.Connection.ExecuteAsync(actualizar, new
						{
							Id = id,
							PatreonLastCheck = fechaActualizar,
							PatreonContribution = contribucion
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Comprobacion 2", ex, false);
				}
			}
		}

		public static async Task<bool> PatreonCorreo(string usuarioId, string correoNuevo)
		{
			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR PatreonMail = @PatreonMail";
			string id = string.Empty;

			try
			{
				id = await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					return await sentencia.Connection.QueryFirstOrDefaultAsync<string>(busqueda, new
					{
						Email = correoNuevo,
						PatreonMail = correoNuevo
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Correo 1", ex, false);
			}

			if (string.IsNullOrEmpty(id))
			{
				string actualizar = "UPDATE AspNetUsers SET PatreonMail = @PatreonMail WHERE Id = @Id";

				try
				{
					await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
					{
						await sentencia.Connection.ExecuteAsync(actualizar, new
						{
							Id = usuarioId,
							PatreonMail = correoNuevo
						}, transaction: sentencia);
					});

					return false; 
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Correo 2", ex, false);
				}
			}

			return true;
		}

		public static async Task Opcion(string variable, bool valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Bool", ex, false);
			}
		}

		public static async Task Opcion(string variable, string valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos String", ex, false);
			}
		}

		public static async Task Opcion(string variable, int valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Int", ex, false);
			}
		}

		public static async Task Opcion(string variable, decimal valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Decimal", ex, false);
			}
		}
	}

    public class Clave
    {
        public string Nombre { get; set; }
		public string JuegoId { get; set; }
		public string Codigo { get; set; }
	}

	public class DeseadosDatos
	{
		public int Cantidad { get; set; }
		public DateTime? UltimaVisita { get; set; }
		public DateTime? UltimoJuego { get; set; }
	}
}
