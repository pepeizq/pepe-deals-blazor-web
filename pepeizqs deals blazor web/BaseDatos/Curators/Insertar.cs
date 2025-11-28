#nullable disable

using APIs.Steam;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BaseDatos.Curators
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

		public static void Ejecutar(SteamCuratorAPI api, SqlConnection conexion = null)
		{
			if (api != null)
			{
				if (string.IsNullOrEmpty(api.Nombre) == false || string.IsNullOrEmpty(api.Slug) == false)
				{
					conexion = CogerConexion(conexion);

					string nombre = string.IsNullOrEmpty(api.Nombre) ? api.Slug : api.Nombre; 
					string slug = string.IsNullOrEmpty(api.Slug) ? Herramientas.EnlaceAdaptador.Nombre(api.Nombre) : api.Slug; 
					
					try { 
						conexion.Execute(@"INSERT INTO curators (idSteam, nombre, imagen, descripcion, slug, steamIds, web, fecha) VALUES (@idSteam, @nombre, @imagen, @descripcion, @slug, @steamIds, @web, @fecha)", new { 
							idSteam = api.Id, 
							nombre, 
							imagen = api.Imagen, 
							descripcion = api.Descripcion, 
							slug, 
							steamIds = JsonSerializer.Serialize(api.SteamIds), 
							web = JsonSerializer.Serialize(api.Web), 
							fecha = DateTime.Now 
						}); 
					} 
					catch (Exception ex) 
					{ 
						BaseDatos.Errores.Insertar.Mensaje("Insertar Curator", ex); 
					}
				}
			}
		}
	}
}
