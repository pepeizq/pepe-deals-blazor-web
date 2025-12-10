#nullable disable

using Bundles2;
using System.Text.Json;

namespace Herramientas.Correos
{
	public class CorreoDeseadoBundleJson
	{
		public string BundleNombre { get; set; }
		public string BundleImagen { get; set; }
		public string BundleEnlace { get; set; }
		public string TiendaNombre { get; set; }
		public string NombreJuego { get; set; }
		public string ImagenJuego { get; set; }
		public string Idioma { get; set; }
	}

	public static class DeseadoBundle
	{
		public static async Task Nuevo(string usuarioId, Bundle bundle, BundleJuego juego, string correoHacia)
		{
			string idioma = await global::BaseDatos.Usuarios.Buscar.IdiomaSobreescribir(usuarioId);

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
								<div style=""min-width: 0; word-wrap: break-word; background-color: #222b44; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div style=""display: flex; align-items: center; gap: 20px; color: #f5f5f5; background-color: #293751; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
										<img src=""{{imagenJuego}}"" style=""max-width: 200px;""/>
				        
										<div style=""padding: 10px; line-height: 25px;"">
											{{mensajeAviso}}
										</div>
									</div>

									<div style=""margin-top: 40px;"">
										<a href=""{{enlace}}"" style=""color: #f5f5f5; background-color: #293751; display: inline-block; user-select: none; width: 100%; text-align: left; font-size: 16px; border: 0px; text-decoration: none; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
											<img src=""{{imagenBundle}}"" style=""max-width: 100%; max-height: 100%;"" />
										</a>
									</div>

									<div style=""margin-top: 20px;"">
										<a href=""{{enlace}}"" style=""color: #95c0fe; background-color: #293751; display: inline-block; user-select: none; width: 100%; text-align: center; border: 0px; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
											<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 17px; margin: 15px;"">
												{{mensajeAbrir}}
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

			string enlace = Herramientas.EnlaceAcortador.Generar(bundle.Enlace, bundle.BundleTipo, false, false);
			html = html.Replace("{{enlace}}", enlace);
			html = html.Replace("{{imagenJuego}}", juego.Imagen);

			string mensajeAviso = null;
			if (bundle.Nombre.ToLower().Contains("bundle") == true)
			{
				mensajeAviso = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String1", "Mails"), juego.Nombre, bundle.Tienda, bundle.Nombre);
			}
			else
			{
				mensajeAviso = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String2", "Mails"), juego.Nombre, bundle.Tienda, bundle.Nombre);
			}

			html = html.Replace("{{mensajeAviso}}", mensajeAviso);
			html = html.Replace("{{imagenBundle}}", bundle.ImagenNoticia);
			html = html.Replace("{{mensajeAbrir}}", Herramientas.Idiomas.BuscarTexto(idioma, "String3", "Mails"));

			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			CorreoDeseadoBundleJson json = new CorreoDeseadoBundleJson();
			json.BundleNombre = bundle.Nombre;
			json.BundleEnlace = enlace;
			json.BundleImagen = bundle.ImagenNoticia;
			json.TiendaNombre = bundle.Tienda;
			json.NombreJuego = juego.Nombre;
			json.ImagenJuego = juego.Imagen;
			json.Idioma = idioma;

			global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, mensajeAviso, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadoBundle, JsonSerializer.Serialize(json));
		}

		public static void Nuevos(List<CorreoDeseadoBundleJson> jsons, string correoHacia, DateTime horaOriginal)
		{
			if (jsons?.Count > 0)
			{
				string idioma = jsons[0].Idioma;

				if (string.IsNullOrEmpty(idioma) == true)
				{
					idioma = "en";
				}

				string titulo = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String4", "Mails"), jsons.Count, jsons[0].BundleNombre);

				string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #222b44; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div style=""font-size: 18px; line-height: 25px;"">
										{{descripcion}}
									</div>";

				foreach (var json in jsons)
				{
					string htmlJson = @"<div style=""margin-top: 20px; display: flex; align-content: center; align-items: center; justify-content: center; gap: 20px; color: #f5f5f5; background-color: #293751; padding: 20px; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
											<img src=""{{imagenJuego}}"" style=""max-width: 100%; max-height: 200px;"" />

											<div>{{nombreJuego}}</div>
										</div>";

					htmlJson = htmlJson.Replace("{{imagenJuego}}", json.ImagenJuego);
					htmlJson = htmlJson.Replace("{{nombreJuego}}", json.NombreJuego);

					html = html + htmlJson;
				}

				html = html + @"<div style=""margin-top: 30px;"">
									<a href=""{{enlaceBundle}}"" style=""color: #f5f5f5; background-color: #293751; display: inline-block; user-select: none; width: 100%; padding: 6px; text-align: left; font-size: 16px; border: 0px; text-decoration: none; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
										<img src=""{{imagenBundle}}"" style=""max-width: 100%; max-height: 100%;"" />
									</a>
								</div>

								<div style=""margin-top: 20px;"">
									<a href=""{{enlaceBundle}}"" style=""color: #95c0fe; background-color: #293751; display: inline-block; user-select: none; width: 100%; text-align: center; border: 0px; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
										<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 17px; margin: 15px;"">
											{{mensajeAbrir}}
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


				string descripcion = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String4", "Mails"), jsons[0].TiendaNombre, jsons.Count);

				html = html.Replace("{{descripcion}}", descripcion);
				html = html.Replace("{{enlaceBundle}}", jsons[0].BundleEnlace);
				html = html.Replace("{{imagenBundle}}", jsons[0].BundleImagen);
				html = html.Replace("{{mensajeAbrir}}", Herramientas.Idiomas.BuscarTexto(idioma, "String3", "Mails"));

				html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
				html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

				global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, horaOriginal, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadosBundle, JsonSerializer.Serialize(jsons));
			}
		}
	}
}
