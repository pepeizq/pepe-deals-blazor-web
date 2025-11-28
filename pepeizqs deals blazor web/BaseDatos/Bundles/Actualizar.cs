#nullable disable

using Microsoft.Data.SqlClient;

namespace BaseDatos.Bundles
{
	public static class Actualizar
	{
		public static void Nombre(string id, string nombre, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET nombre=@nombre " +
					"WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);
				comando.Parameters.AddWithValue("@nombre", nombre);

				comando.ExecuteNonQuery();
				try
				{
					
				}
				catch
				{

				}
			}
		}

		public static void FechaEmpieza(string id, string fechaEmpieza, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET fechaEmpieza=@fechaEmpieza " +
					"WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);
				comando.Parameters.AddWithValue("@fechaEmpieza", fechaEmpieza);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void FechaTermina(string id, string fechaTermina, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET fechaTermina=@fechaTermina " +
					"WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);
				comando.Parameters.AddWithValue("@fechaTermina", fechaTermina);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

        public static void ImagenBundle(string id, string imagen, SqlConnection conexion)
        {
            string sqlActualizar = "UPDATE bundles " +
                    "SET imagen=@imagen " +
                    "WHERE id=@id";

            using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
            {
                comando.Parameters.AddWithValue("@id", id);
                comando.Parameters.AddWithValue("@imagen", imagen);

                comando.ExecuteNonQuery();
                try
                {

                }
                catch
                {

                }
			}
		}

        public static void ImagenNoticia(string id, string imagen, SqlConnection conexion)
        {
            string sqlActualizar = "UPDATE bundles " +
                    "SET imagenNoticia=@imagenNoticia " +
                    "WHERE id=@id";

            using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
            {
                comando.Parameters.AddWithValue("@id", id);
                comando.Parameters.AddWithValue("@imagenNoticia", imagen);

                comando.ExecuteNonQuery();
                try
                {

                }
                catch
                {

                }
            }
        }

        public static void Juegos(string id, string juegos, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET juegos=@juegos " +
					"WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);
				comando.Parameters.AddWithValue("@juegos", juegos);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}

		public static void Tiers(string id, string tiers, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET tiers=@tiers " +
					"WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@id", id);
				comando.Parameters.AddWithValue("@tiers", tiers);

				comando.ExecuteNonQuery();
				try
				{

				}
				catch
				{

				}
			}
		}
	}
}
