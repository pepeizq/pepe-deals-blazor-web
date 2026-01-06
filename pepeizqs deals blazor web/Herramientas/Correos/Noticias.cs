#nullable disable

using Noticias;

namespace Herramientas.Correos
{

	public static class Noticias
	{
		public static async Task<bool> Enviar(Noticia noticia, string correoHacia, string idioma, string dominio)
		{
			try
			{
				string titulo = noticia.TituloEn;
				string enlace = "https://" + dominio + "/link/news/" + noticia.Id.ToString() + "/";
				string contenido = noticia.ContenidoEn;

				if (string.IsNullOrEmpty(idioma) == false)
				{
					if (Herramientas.Idiomas.ComprobarIdiomaUso("es", idioma) == true)
					{
						titulo = noticia.TituloEs;
						contenido = noticia.ContenidoEs;
					}
				}

				string html = string.Empty;

				html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #002033; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5; line-height: 22px;"">
									<div style=""font-size: 18px; margin-bottom: 20px; line-height: 25px;"">
										{{titulo}}
									</div>

									<hr/>

									<div style=""margin-top: 20px; display: flex; flex-direction: column; gap: 40px; color: #f5f5f5; background-color: #0d1621; padding: 20px; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
										<a href=""{{enlace}}"" style=""color: #f5f5f5; user-select: none; width: 100%; text-align: left; font-size: 16px; text-decoration: none;"" target=""_blank"">
											<div>
												<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 18px;"">
													<img src=""{{imagen}}"" style=""max-width: 100%; max-height: 300px; margin-top: 10px;"" />
												</div>

												<div style=""margin-top: 30px"">
													{{contenido}}
												</div>
											</div>
										</a>
									</div>

									<div style=""margin-top: 20px;"">
										<a href=""{{enlace}}"" style=""color: #95c0fe; background-color: #0d1621; display: inline-block; user-select: none; width: 100%; text-align: center; border: 0px; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
											<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 17px; margin: 15px;"">
												{{texto1}}
											</div>
										</a>
									</div>

									<div style=""margin-top: 40px;"">
										<div>
											&copy; {{año}} • <a href=""https://pepeizqapps.com/"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">pepeizq's apps</a> • <a href=""https://pepe.deals/"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">pepe's deals</a>
										</div>
										<div style=""margin-top: 20px; font-size: 14px;"">
											{{mensaje}} <a href=""https://pepe.deals/contact"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">/contact/</a>
										</div>
									</div>
								</div>
							</body>
							</html>";

				html = html.Replace("{{enlace}}", enlace);
				html = html.Replace("{{titulo}}", titulo);
				html = html.Replace("{{imagen}}", noticia.Imagen);
				html = html.Replace("{{contenido}}", contenido);
				html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
				html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));
				html = html.Replace("{{texto1}}", Herramientas.Idiomas.BuscarTexto(idioma, "News1", "Mails"));

				if (html.Contains("<ul>") == true)
				{
					html = html.Replace("<ul>", @"<ul style=""line-height: 22px;"">");
				}

				await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.Noticia);

				return true;
			}
			catch (Exception ex)
			{
				global::BaseDatos.Errores.Insertar.Mensaje("Correos - Enviar Noticia", ex);
			}

			return false;
		}
	}
}
