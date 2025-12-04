#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Pendientes
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

		public static void Tienda(string idTienda, string enlace, string idJuegos, SqlConnection conexion = null)
		{
            if (string.IsNullOrEmpty(idJuegos) == false)
            {
                if (idJuegos != "0")
                {
					conexion = CogerConexion(conexion);

                    string sqlActualizar = "UPDATE tienda" + idTienda + " " +
                    "SET idJuegos=@idJuegos WHERE enlace=@enlace";

					try
					{
						conexion.Execute(sqlActualizar, new { idJuegos, enlace });
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Actualizar Pendientes Tienda", ex);
					}
				}
            }
		}

        public static void Suscripcion(string tablaInsertar, string tablaBorrar, string enlace, string nombreJuego, string imagen, List<string> idJuegos, SqlConnection conexion = null)
        {
			conexion = CogerConexion(conexion);

			foreach (var idJuego in idJuegos)
			{
				string descartado = "no";
				var juego = BaseDatos.Juegos.Buscar.UnJuego(idJuego);

				if (idJuego != "0")
				{
					if (juego != null)
					{
						if (nombreJuego == "vacio")
						{
							nombreJuego = juego.Nombre;
						}
							
						if (imagen == "vacio")
						{
							imagen = juego.Imagenes.Header_460x215;
						}
					}
				}
				else
				{
					descartado = "si";
				}

				string sqlInsertar = $@"
					INSERT INTO {tablaInsertar}
					(enlace, nombre, imagen, idJuegos, descartado)
					VALUES
					(@enlace, @nombre, @imagen, @idJuegos, @descartado)
				";

				try
				{
					conexion.Execute(sqlInsertar, new
					{
						enlace,
						nombre = nombreJuego,
						imagen,
						idJuegos = idJuego,
						descartado
					});
				}
				catch
				{
					
				}

				string sqlBorrar = $@"
					DELETE FROM {tablaBorrar}
					WHERE enlace = @enlace
				";

				try
				{
					conexion.Execute(sqlBorrar, new { enlace });
				}
				catch
				{
					
				}
			}
		}

        public static void Streaming(string idStreaming, string nombreCodigo, int idJuego, SqlConnection conexion)
        {
            if (idJuego > 0)
            {
                string sqlActualizar = "UPDATE streaming" + idStreaming + " " +
                   "SET idJuego=@idJuego WHERE nombreCodigo=@nombreCodigo";

                using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
                {
                    comando.Parameters.AddWithValue("@idJuego", idJuego);
                    comando.Parameters.AddWithValue("@nombreCodigo", nombreCodigo);

                    try
                    {
                        comando.ExecuteNonQuery();
                    }
                    catch
                    {

                    }
                }

				string sqlActualizar2 = "UPDATE streaming" + idStreaming + " " +
				   "SET idJuego=@idJuego WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(sqlActualizar2, conexion))
				{
					comando.Parameters.AddWithValue("@idJuego", idJuego);
					comando.Parameters.AddWithValue("@id", nombreCodigo);

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

        public static void PlataformaAmazon(string idAmazon, int idJuego, SqlConnection conexion)
        {
            if (idJuego > 0)
            {
				bool yaPuesto = false;

				string busqueda = "SELECT idAmazon FROM juegos WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@id", idJuego);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								if (string.IsNullOrEmpty(lector.GetString(0)) == false)
								{
									yaPuesto = true;
								}
							}
						}
					}
				}

				if (yaPuesto == false)
				{
					string sqlActualizar = "UPDATE juegos " +
						"SET idAmazon=@idAmazon WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idJuego);
						comando.Parameters.AddWithValue("@idAmazon", idAmazon);

						try
						{
							comando.ExecuteNonQuery();
							yaPuesto = true;
						}
						catch
						{

						}
					}
				}

				if (yaPuesto == true)
				{
					string sqlBorrar = "DELETE FROM temporalAmazonJuegos WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlBorrar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idAmazon);

						comando.ExecuteNonQuery();
					}
				}
			}
		}

		public static void PlataformaEpic(string idEpic, int idJuego, SqlConnection conexion)
		{
			if (idJuego > 0)
			{
				bool yaPuesto = false;

				string busqueda = "SELECT exeEpic FROM juegos WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@id", idJuego);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								if (string.IsNullOrEmpty(lector.GetString(0)) == false)
								{
									yaPuesto = true;
								}
							}
						}
					}
				}

				if (yaPuesto == false)
				{
					string sqlActualizar = "UPDATE juegos " +
						"SET exeEpic=@exeEpic WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idJuego);
						comando.Parameters.AddWithValue("@exeEpic", idEpic);

						try
						{
							comando.ExecuteNonQuery();
							yaPuesto = true;
						}
						catch
						{

						}
					}
				}

				if (yaPuesto == true)
				{
					string sqlBorrar = "DELETE FROM temporalEpicJuegos WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlBorrar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idEpic);

						comando.ExecuteNonQuery();
					}
				}
			}
		}

		public static void PlataformaUbisoft(string idUbisoft, int idJuego, SqlConnection conexion)
		{
			if (idJuego > 0)
			{
				bool yaPuesto = false;

				string busqueda = "SELECT exeUbisoft FROM juegos WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@id", idJuego);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								if (string.IsNullOrEmpty(lector.GetString(0)) == false)
								{
									yaPuesto = true;
								}
							}
						}
					}
				}

				if (yaPuesto == false)
				{
					string sqlActualizar = "UPDATE juegos " +
						"SET exeUbisoft=@exeUbisoft WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idJuego);
						comando.Parameters.AddWithValue("@exeEA", idUbisoft);

						try
						{
							comando.ExecuteNonQuery();
							yaPuesto = true;
						}
						catch
						{

						}
					}
				}

				if (yaPuesto == true)
				{
					string sqlBorrar = "DELETE FROM temporalUbisoftJuegos WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlBorrar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idUbisoft);

						comando.ExecuteNonQuery();
					}
				}
			}
		}

		public static void PlataformaEA(string idEa, int idJuego, SqlConnection conexion)
		{
			if (idJuego > 0)
			{
				bool yaPuesto = false;

				string busqueda = "SELECT exeEA FROM juegos WHERE id=@id";

				using (SqlCommand comando = new SqlCommand(busqueda, conexion))
				{
					comando.Parameters.AddWithValue("@id", idJuego);

					using (SqlDataReader lector = comando.ExecuteReader())
					{
						if (lector.Read() == true)
						{
							if (lector.IsDBNull(0) == false)
							{
								if (string.IsNullOrEmpty(lector.GetString(0)) == false)
								{
									yaPuesto = true;
								}
							}
						}
					}
				}

				if (yaPuesto == false)
				{
					string sqlActualizar = "UPDATE juegos " +
						"SET exeEA=@exeEA WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idJuego);
						comando.Parameters.AddWithValue("@exeEA", idEa);

						try
						{
							comando.ExecuteNonQuery();
							yaPuesto = true;
						}
						catch
						{

						}
					}
				}
				
				if (yaPuesto == true)
				{
					string sqlBorrar = "DELETE FROM temporalEaJuegos WHERE id=@id";

					using (SqlCommand comando = new SqlCommand(sqlBorrar, conexion))
					{
						comando.Parameters.AddWithValue("@id", idEa);

						comando.ExecuteNonQuery();
					}
				}
			}
		}

		public static void DescartarTienda(string idTienda, string enlace, SqlConnection conexion)
		{
			string sqlActualizar = "UPDATE tienda" + idTienda + " " +
					"SET descartado=@descartado WHERE enlace=@enlace";

			using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
			{
				comando.Parameters.AddWithValue("@descartado", "si");
				comando.Parameters.AddWithValue("@enlace", enlace);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

		public static void DescartarSuscripcion(string idSuscripcion, string enlace, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			string sqlInsertar = @$"
				INSERT INTO suscripcion{idSuscripcion}
				(enlace, idJuegos, descartado) 
				VALUES 
				(@Enlace, @IdJuegos, @Descartado)
			";

			try
			{
				conexion.Execute(sqlInsertar, new
				{
					Enlace = enlace,
					IdJuegos = "0",
					Descartado = "si"
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Error Descarte 1 Suscripcion " + idSuscripcion, ex);
			}

			try
			{
				string sqlBorrar = $"DELETE FROM temporal{idSuscripcion} WHERE enlace = @enlace";

				conexion.Execute(sqlBorrar, new
				{
					enlace
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Error Descarte 2 Suscripcion " + idSuscripcion, ex);
			}
		}

        public static void DescartarStreaming(string idStreaming, string nombreCodigo, SqlConnection conexion)
        {
            string sqlActualizar = "UPDATE streaming" + idStreaming + " " +
                    "SET descartado=@descartado WHERE nombreCodigo=@nombreCodigo";

            using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion))
            {
                comando.Parameters.AddWithValue("@descartado", 1);
                comando.Parameters.AddWithValue("@nombreCodigo", nombreCodigo);

                try
                {
                    comando.ExecuteNonQuery();
                }
                catch
                {

                }
            }

			string sqlActualizar2 = "UPDATE streaming" + idStreaming + " " +
					"SET descartado=@descartado WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlActualizar2, conexion))
			{
				comando.Parameters.AddWithValue("@descartado", 1);
				comando.Parameters.AddWithValue("@id", nombreCodigo);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch
				{

				}
			}
		}

        public static void DescartarPlataforma(string idPlataforma, string idJuego, SqlConnection conexion)
        {
			string sqlBorrar = "DELETE FROM temporal" + idPlataforma + "Juegos WHERE id=@id";

			using (SqlCommand comando = new SqlCommand(sqlBorrar, conexion))
			{
				comando.Parameters.AddWithValue("@id", idJuego);

				comando.ExecuteNonQuery();
			}

			string sqlInsertar = "INSERT INTO " + idPlataforma + "Descartes " +
							"(id) VALUES " +
							"(@id) ";

			using (SqlCommand comando = new SqlCommand(sqlInsertar, conexion))
			{
				comando.Parameters.AddWithValue("@id", idJuego);

				try
				{
					comando.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Descarte Plataforma", ex);
				}
			}
		}
    }
}
