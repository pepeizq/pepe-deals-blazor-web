#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Admin
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

		public static void Tiendas(string tienda, DateTime fecha, int cantidad, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE adminTiendas SET fecha=@fecha, mensaje=@mensaje WHERE id=@id", new { id = tienda, fecha = fecha, mensaje = cantidad });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas", ex);
			}
		}

		public static void TiendasValorAdicional(string tienda, string valor, int cantidad, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute($"UPDATE adminTiendas SET {valor}=@cantidad WHERE id=@id", new { id = tienda, cantidad });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas Valor Adicional", ex);
			}
		}

		public static void TareaUso(string id, DateTime fecha, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE adminTareas SET fecha=@fecha WHERE id=@id", new { id = id, fecha = fecha });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tareas", ex);
			}
		}

		public static void Dato(string id, int contenido, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			try
			{
				conexion.Execute("UPDATE adminDatos SET contenido=@contenido WHERE id=@id", new { id = id, contenido = contenido });
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Datos", ex);
			}
		}
	}
}
