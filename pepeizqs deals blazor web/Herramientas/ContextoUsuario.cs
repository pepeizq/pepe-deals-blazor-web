#nullable disable

using Tiendas2;

namespace Herramientas
{

	public static class ContextoUsuario
	{
		public static ContextoUsuarioDatos Leer(IHttpContextAccessor contexto)
		{
			ContextoUsuarioDatos datos = new ContextoUsuarioDatos();

            datos.Idioma = contexto?.HttpContext?.Request?.Headers["Accept-Language"].ToString().Split(";")?.FirstOrDefault()?.Split(",").FirstOrDefault() ?? "en";
            datos.UsuarioId = contexto?.HttpContext?.User?.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            datos.UserAgent = contexto?.HttpContext?.Request?.Headers?.UserAgent.ToString();
			datos.Dominio = contexto?.HttpContext?.Request?.Host.Value;
			datos.Region = contexto?.HttpContext?.Request?.Cookies.TryGetValue("user_currency", out string valorRegion) == true && int.TryParse(valorRegion, out int valorRegionInt) && Enum.IsDefined(typeof(TiendaRegion), valorRegionInt) ? (TiendaRegion)valorRegionInt : TiendaRegion.Europa;

			return datos;
        }
	}

    public class ContextoUsuarioDatos
	{
		public string Idioma {  get; set; }
        public string UsuarioId { get; set; }
        public string UserAgent { get; set; }
		public string Dominio { get; set; }
		public TiendaRegion Region { get; set; } = TiendaRegion.Europa;
	}
}
