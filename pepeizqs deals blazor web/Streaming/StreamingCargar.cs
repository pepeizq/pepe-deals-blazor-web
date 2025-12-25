#nullable disable

namespace Streaming2
{
    public enum StreamingTipo
    {
        GeforceNOW,
        AmazonLuna,
		Boosteroid,
		Desconocido
	}

    public class StreamingCargar
    {
        public static List<Streaming> GenerarListado()
        {
            List<Streaming> streaming = new List<Streaming>
            {
                APIs.GeforceNOW.Streaming.Generar(),
                APIs.GOG.Streaming.Generar(),
                APIs.Boosteroid.Streaming.Generar()
            };

            return streaming;
        }

		public static Streaming DevolverStreaming(string streamingTexto)
		{
			foreach (var streaming in GenerarListado())
			{
				if (streaming.Id.ToString()?.ToLower() == streamingTexto?.ToLower())
				{
					return streaming;
				}
			}

			return null;
		}

		public static Streaming DevolverStreaming(StreamingTipo streamingTipo)
		{
			foreach (var streaming in GenerarListado())
			{
				if (streaming.Id == streamingTipo)
				{
					return streaming;
				}
			}

			return null;
		}
	}
}
