#nullable disable

using Dapper;

namespace BaseDatos.Admin
{
	public static class Actualizar
	{
		public static void Tiendas(string tienda, DateTime fecha, int cantidad)
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute("UPDATE adminTiendas SET fecha=@fecha, mensaje=@mensaje WHERE id=@id", new { id = tienda, fecha = fecha, mensaje = cantidad }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas", ex);
			}
		}

		public static void TiendasValorAdicional(string tienda, string valor, int cantidad)
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute($"UPDATE adminTiendas SET {valor}=@cantidad WHERE id=@id", new { id = tienda, cantidad }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas Valor Adicional", ex);
			}
		}

		public static void TareaUso(string id, DateTime fecha)
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute("UPDATE adminTareas SET fecha=@fecha WHERE id=@id", new { id = id, fecha = fecha }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tareas", ex);
			}
		}

		public static void Dato(string id, int contenido)
		{
			try
			{
				Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					sentencia.Connection.Execute("UPDATE adminDatos SET contenido=@contenido WHERE id=@id", new { id = id, contenido = contenido }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Datos", ex);
			}
		}
	}
}
