#nullable disable

using Dapper;

namespace BaseDatos.Admin
{
	public static class Actualizar
	{
		public static async Task Tiendas(string tienda, DateTime fecha, int cantidad)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE adminTiendas SET fecha=@fecha, mensaje=@mensaje WHERE id=@id", new { id = tienda, fecha = fecha, mensaje = cantidad }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas", ex);
			}
		}

		public static async Task TiendasValorAdicional(string tienda, string valor, int cantidad)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync($"UPDATE adminTiendas SET {valor}=@cantidad WHERE id=@id", new { id = tienda, cantidad }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas Valor Adicional", ex);
			}
		}

		public static async void TareaUso(string id, DateTime fecha)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE adminTareas SET fecha=@fecha WHERE id=@id", new { id = id, fecha = fecha }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tareas", ex);
			}
		}

		public static async Task Dato(string id, int contenido)
		{
			try
			{
				await Herramientas.BaseDatos.EjecutarConConexionAsync(async sentencia =>
				{
					await sentencia.Connection.ExecuteAsync("UPDATE adminDatos SET contenido=@contenido WHERE id=@id", new { id = id, contenido = contenido }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Datos", ex);
			}
		}
	}
}
