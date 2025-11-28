#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Reseñas
{
	public static class Insertar
	{
		private static SqlConnection CogerConexion(SqlConnection conexion)
		{
			if (conexion == null || conexion.State != System.Data.ConnectionState.Open)
			{
				conexion = Herramientas.BaseDatos.Conectar();
			}

			return conexion;
		}

		public static void Ejecutar(int id, int positivos, int negativos, string idioma, string contenido, SqlConnection conexion = null)
		{
			conexion = CogerConexion(conexion);

			bool existe = conexion.ExecuteScalar<int>("SELECT COUNT(1) FROM juegosAnalisis WHERE id=@id", new { id }) > 0; 
				
			string fechaCol = "fecha" + idioma; 
			string positivosCol = "positivos" + idioma; 
			string negativosCol = "negativos" + idioma; 
			string contenidoCol = "contenido" + idioma; 
			
			if (existe == false) 
			{ 
				string sqlInsertar = $@"INSERT INTO juegosAnalisis (id, {positivosCol}, {negativosCol}, {fechaCol}, {contenidoCol}) VALUES (@id, @positivos, @negativos, @fecha, @contenido)"; 
				
				conexion.Execute(sqlInsertar, new { id, positivos, negativos, fecha = DateTime.Now, contenido }); 
			} 
			else 
			{ 
				string sqlActualizar = $@"UPDATE juegosAnalisis SET {positivosCol}=@positivos, {negativosCol}=@negativos, {fechaCol}=@fecha, {contenidoCol}=@contenido WHERE id=@id"; 
				
				conexion.Execute(sqlActualizar, new { id, positivos, negativos, fecha = DateTime.Now, contenido }); 
			}
		}
	}
}
