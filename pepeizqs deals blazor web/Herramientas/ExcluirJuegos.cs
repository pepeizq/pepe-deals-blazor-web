namespace Herramientas
{
	public static class ExcluirJuegos
	{
		public static async Task Añadir(string idCuenta, int idJuego)
		{
			string juegosExcluidosTexto = await global::BaseDatos.Usuarios.Buscar.JuegosExcluidos(idCuenta);

			List<string> juegosExcluidos = new List<string>();

			if (string.IsNullOrEmpty(juegosExcluidosTexto) == false)
			{
				juegosExcluidos = juegosExcluidosTexto.Split(',').ToList();
			}
	
			if (juegosExcluidos.Count == 0 || juegosExcluidos.Contains(idJuego.ToString()) == false)
			{
				juegosExcluidos.Add(idJuego.ToString());
				string nuevoJuegosExcluidosTexto = string.Join(",", juegosExcluidos);
				await global::BaseDatos.Usuarios.Actualizar.Opcion("GamesExcluded", nuevoJuegosExcluidosTexto, idCuenta);
			}
		}

		public static async Task Quitar(string idCuenta, int idJuego)
		{
			string juegosExcluidosTexto = await global::BaseDatos.Usuarios.Buscar.JuegosExcluidos(idCuenta);

			if (string.IsNullOrEmpty(juegosExcluidosTexto) == false)
			{
				List<string> juegosExcluidos = juegosExcluidosTexto.Split(',').ToList();

				if (juegosExcluidos.Contains(idJuego.ToString()))
				{
					juegosExcluidos.Remove(idJuego.ToString());
					string nuevoJuegosExcluidosTexto = string.Join(",", juegosExcluidos);
					await global::BaseDatos.Usuarios.Actualizar.Opcion("GamesExcluded", nuevoJuegosExcluidosTexto, idCuenta);
				}
			}
		}
	}
}
