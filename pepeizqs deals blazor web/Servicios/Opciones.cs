#nullable disable

namespace Servicios
{

	public class Opciones
	{
		private bool mostrarOpciones = false;

		public event Action OnMostrarOpcionesChanged;

		public bool MostrarOpciones
		{
			get => mostrarOpciones;
			set
			{
				if (mostrarOpciones != value)
				{
					mostrarOpciones = value;
					NotifyStateChanged();
				}
			}
		}

		public void AlternarOpciones()
		{
			MostrarOpciones = !MostrarOpciones;
		}

		private void NotifyStateChanged() => OnMostrarOpcionesChanged?.Invoke();
	}
}
