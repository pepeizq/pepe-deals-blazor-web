#nullable disable

using Juegos;

namespace Suscripciones2
{
	public class Suscripcion
	{
		public SuscripcionTipo Id;
		public string Nombre;
		public string ImagenLogo;
		public string ImagenIcono;
		public string ImagenFondo;
		public string ColorDestacado;
		public string Enlace;
		public DateTime FechaSugerencia;
		public JuegoDRM DRMDefecto;
		public string ImagenNoticia;
		public bool AdminInteractuar;
		public bool UsuarioEnlacesEspecificos;
		public bool ParaSiempre; //true si pagas los juegos son para siempre
		public List<SuscripcionTipo> SuscripcionesRelacionadas;
		public double Precio;
		public bool AdminPendientes;
		public string TablaPendientes;
		public bool SoloStreaming;
		public bool AdminAñadir;
		public bool UsuarioPuedeAbrir;
		public int SteamPaquete;
	}

    public class SuscripcionComponente
    {
        public Suscripcion Tipo;
        public List<JuegoSuscripcion> Juegos;
    }
}
