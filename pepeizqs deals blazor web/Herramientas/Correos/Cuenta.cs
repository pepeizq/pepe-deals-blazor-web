#nullable disable

namespace Herramientas.Correos
{

	public static class Cuenta
	{
		public static async Task ContraseñaReseteada(string idioma, string correoHacia)
		{
			if (string.IsNullOrEmpty(idioma) == true)
			{
				idioma = "en";
			}

			string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #293751; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div>
										{{texto1}}.
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

			html = html.Replace("{{texto1}}", Herramientas.Idiomas.BuscarTexto(idioma, "Reset1", "Mails"));
			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			string titulo = Herramientas.Idiomas.BuscarTexto(idioma, "Reset1", "Mails");

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.ContraseñaReseteada);
		}

		public static async Task ContraseñaOlvidada(string idioma, string codigo, string correoHacia)
		{
			if (string.IsNullOrEmpty(idioma) == true)
			{
				idioma = "en";
			}

			string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #293751; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div>
										{{texto1}} <a href=""{{codigo}}"" target=""_blank"">{{texto2}}</a>.
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

			html = html.Replace("{{texto1}}", Herramientas.Idiomas.BuscarTexto(idioma, "Reset3", "Mails"));
			html = html.Replace("{{texto2}}", Herramientas.Idiomas.BuscarTexto(idioma, "Reset4", "Mails"));
			html = html.Replace("{{codigo}}", codigo);
			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			string titulo = Herramientas.Idiomas.BuscarTexto(idioma, "Reset2", "Mails");

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.ContraseñaOlvidada);
		}

		public static async Task CambioContraseña(string idioma, string correoHacia)
		{
			if (string.IsNullOrEmpty(idioma) == true)
			{
				idioma = "en";
			}

			string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #293751; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div>
										{{texto1}}
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

			html = html.Replace("{{texto1}}", Herramientas.Idiomas.BuscarTexto(idioma, "Change2", "Mails"));
			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			string titulo = Herramientas.Idiomas.BuscarTexto(idioma, "Change1", "Mails");

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.ContraseñaCambio);
		}

		public static async Task CambioCorreo(string idioma, string codigo, string correoHacia)
		{
			if (string.IsNullOrEmpty(idioma) == true)
			{
				idioma = "en";
			}

			string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #293751; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div>
										{{texto1}} <a href=""{{codigo}}"" target=""_blank"">{{texto2}}</a>.
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

			html = html.Replace("{{texto1}}", Herramientas.Idiomas.BuscarTexto(idioma, "Mail2", "Mails"));
			html = html.Replace("{{texto2}}", Herramientas.Idiomas.BuscarTexto(idioma, "Mail3", "Mails"));
			html = html.Replace("{{codigo}}", codigo);
			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			string titulo = Herramientas.Idiomas.BuscarTexto(idioma, "Mail1", "Mails");

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.CorreoCambio);
		}

		public static async Task ConfirmacionCorreo(string idioma, string codigo, string correoHacia)
		{
			if (string.IsNullOrEmpty(idioma) == true)
			{
				idioma = "en";
			}

			string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #293751; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div>
										{{texto1}} <a href=""{{codigo}}"" target=""_blank"">{{texto2}}</a>.
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

			html = html.Replace("{{texto1}}", Herramientas.Idiomas.BuscarTexto(idioma, "Confirm2", "Mails"));
			html = html.Replace("{{texto2}}", Herramientas.Idiomas.BuscarTexto(idioma, "Confirm3", "Mails"));
			html = html.Replace("{{codigo}}", codigo);
			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			string titulo = Herramientas.Idiomas.BuscarTexto(idioma, "Confirm1", "Mails");

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.CorreoConfirmacion);
		}
	}
}
