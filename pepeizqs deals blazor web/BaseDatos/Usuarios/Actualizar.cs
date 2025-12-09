#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Usuarios
{
    public static class Actualizar
    {
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void PatreonComprobacion(string correoBuscar, DateTime fechaActualizar, int contribucion, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR PatreonMail = @PatreonMail";
			string id = string.Empty;

			try
			{
				id = conexion.QueryFirstOrDefault<string>(busqueda, new { 
					Email = correoBuscar, 
					PatreonMail = correoBuscar 
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Comprobacion 1", ex);
			}

			if (string.IsNullOrEmpty(id) == false)
			{
				string sqlActualizar = @"UPDATE AspNetUsers 
                                 SET PatreonLastCheck = @PatreonLastCheck, 
                                     PatreonContribution = @PatreonContribution 
                                 WHERE Id = @Id";

				try
				{
					conexion.Execute(sqlActualizar, new { 
						Id = id, 
						PatreonLastCheck = fechaActualizar, 
						PatreonContribution = contribucion 
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Comprobacion 2", ex);
				}
			}
		}

		public static bool PatreonCorreo(string usuarioId, string correoNuevo, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR PatreonMail = @PatreonMail";
			string id = string.Empty;

			try
			{
				id = conexion.QueryFirstOrDefault<string>(busqueda, new { 
					Email = correoNuevo, 
					PatreonMail = correoNuevo 
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Correo 1", ex);
			}

			if (string.IsNullOrEmpty(id))
			{
				string sqlActualizar = "UPDATE AspNetUsers SET PatreonMail = @PatreonMail WHERE Id = @Id";

				try
				{
					conexion.Execute(sqlActualizar, new { 
						Id = usuarioId, 
						PatreonMail = correoNuevo 
					});
					
					return false; 
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Correo 2", ex);
				}
			}

			return true;
		}

		public static void Opcion(string variable, bool valor, string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'");
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Bool", ex, false);
			}
		}

		public static void Opcion(string variable, string valor, string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'");
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos String", ex, false);
			}
		}

		public static void Opcion(string variable, int valor, string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'");
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Int", ex, false);
			}
		}

		public static void Opcion(string variable, decimal valor, string usuarioId, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id=" + usuarioId);
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Decimal", ex, false);
			}
		}
	}

    public class Clave
    {
        public string Nombre;
        public string JuegoId;
        public string Codigo;
    }

	public class DeseadosDatos
	{
		public int Cantidad { get; set; }
		public DateTime? UltimaVisita { get; set; }
		public DateTime? UltimoJuego { get; set; }
	}
}
