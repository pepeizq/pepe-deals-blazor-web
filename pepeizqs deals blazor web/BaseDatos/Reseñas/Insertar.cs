#nullable disable

using Dapper;

namespace BaseDatos.Reseñas
{
	public static class Insertar
	{
		public static void Ejecutar(int id, int positivos, int negativos, string idioma, string contenido)
		{
			try
			{
				bool existe = Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
				{
					return sentencia.Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM juegosAnalisis WHERE id=@id", new { id }, transaction: sentencia) > 0;
				});

				string fechaCol = "fecha" + idioma;
				string positivosCol = "positivos" + idioma;
				string negativosCol = "negativos" + idioma;
				string contenidoCol = "contenido" + idioma;

				if (existe == false)
				{
					string sqlInsertar = $@"INSERT INTO juegosAnalisis (id, {positivosCol}, {negativosCol}, {fechaCol}, {contenidoCol}) VALUES (@id, @positivos, @negativos, @fecha, @contenido)";

					Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
					{
						sentencia.Connection.Execute(sqlInsertar, new { id, positivos, negativos, fecha = DateTime.Now, contenido }, transaction: sentencia);
					});
				}
				else
				{
					string sqlActualizar = $@"UPDATE juegosAnalisis SET {positivosCol}=@positivos, {negativosCol}=@negativos, {fechaCol}=@fecha, {contenidoCol}=@contenido WHERE id=@id";

					Herramientas.BaseDatos.EjecutarConConexion(sentencia =>
					{
						sentencia.Connection.Execute(sqlActualizar, new { id, positivos, negativos, fecha = DateTime.Now, contenido }, transaction: sentencia);
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Insertar", ex);
			}
		}
	}
}
