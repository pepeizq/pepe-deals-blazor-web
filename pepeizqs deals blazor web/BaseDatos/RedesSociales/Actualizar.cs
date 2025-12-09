#nullable disable

using Microsoft.Data.SqlClient;

namespace BaseDatos.RedesSociales
{

	public static class Actualizar
	{
		public static void Tienda(string enlace, string nuevaTienda, SqlConnection conexion = null)
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

            string sqlActualizar = "UPDATE redesSocialesPosteador SET tienda=@nuevaTienda WHERE enlace=@enlace";

            using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
            {
                comando.Parameters.AddWithValue("@nuevaTienda", nuevaTienda);
                comando.Parameters.AddWithValue("@enlace", enlace);

                try
                {
                    comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Errores.Insertar.Mensaje("Actualizar Redes Sociales Posteador", ex, false);
                }
            }
        }
	}
}
