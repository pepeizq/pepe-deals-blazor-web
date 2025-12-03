#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            if (conexion == null)
            {
				conexion = Herramientas.BaseDatos.Conectar();
			}
            else
            {
                if (conexion.State != System.Data.ConnectionState.Open)
                {
					conexion = Herramientas.BaseDatos.Conectar();
				}
            }

            string id = null;

			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email=@Email OR PatreonMail=@PatreonMail";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				comando.Parameters.AddWithValue("@Email", correoBuscar);
				comando.Parameters.AddWithValue("@PatreonMail", correoBuscar);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(0)) == false)
							{
                                id = lector.GetString(0);	
							}
						}
					}
				}
			}

            if (string.IsNullOrEmpty(id) == false)
            {
				string sqlActualizar = "UPDATE AspNetUsers " +
					"SET PatreonLastCheck=@PatreonLastCheck, PatreonContribution=@PatreonContribution WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@Id", id);
					comando.Parameters.AddWithValue("@PatreonLastCheck", fechaActualizar);
					comando.Parameters.AddWithValue("@PatreonContribution", contribucion);

					try
					{
						comando.ExecuteNonQuery();
					}
					catch
					{

					}
				}
			}
		}

		public static bool PatreonCorreo(string usuarioId, string correoNuevo, SqlConnection conexion = null)
		{
			if (conexion == null)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.BaseDatos.Conectar();
				}
			}

			string id = null;

			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email=@Email OR PatreonMail=@PatreonMail";

			using (SqlCommand comando = new SqlCommand(busqueda, conexion))
			{
				comando.Parameters.AddWithValue("@Email", correoNuevo);
				comando.Parameters.AddWithValue("@PatreonMail", correoNuevo);

				using (SqlDataReader lector = comando.ExecuteReader())
				{
					while (lector.Read())
					{
						if (lector.IsDBNull(0) == false)
						{
							if (string.IsNullOrEmpty(lector.GetString(0)) == false)
							{
								id = lector.GetString(0);
							}
						}
					}
				}
			}

			if (string.IsNullOrEmpty(id) == true)
			{
				string sqlActualizar = "UPDATE AspNetUsers " +
					"SET PatreonMail=@PatreonMail WHERE Id=@Id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
				{
					comando.Parameters.AddWithValue("@Id", usuarioId);
					comando.Parameters.AddWithValue("@PatreonMail", correoNuevo);

					try
					{
						comando.ExecuteNonQuery();
						return false;
					}
					catch
					{

					}
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
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Bool", ex, null, false);
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
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos String", ex, null, false);
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
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Int", ex, null, false);
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
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Decimal", ex, null, false);
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
