#nullable disable

namespace Herramientas
{
	public static class Listados
	{
		public static List<string> Generar(string datos)
		{
			try
			{
				if (datos != null)
				{
					var datosPartidos = datos.Split(',');
					return new List<string>(datosPartidos);
				}
			}
			catch 
			{
				Environment.Exit(1);
			}
		
			return null;
		}
	}
}
