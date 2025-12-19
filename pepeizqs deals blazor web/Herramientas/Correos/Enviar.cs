#nullable disable

namespace Herramientas.Correos
{
	public static class Enviar
	{
		public static async Task<bool> Ejecutar(string html, string titulo, string correoDesde, string correoHacia)
		{
			if (string.IsNullOrEmpty(html) == false &&
				string.IsNullOrEmpty(titulo) == false &&
				string.IsNullOrEmpty(correoDesde) == false &&
				string.IsNullOrEmpty(correoHacia) == false)
			{
				WebApplicationBuilder builder = WebApplication.CreateBuilder();

				string host = builder.Configuration.GetValue<string>("WebmasterDeals:CorreoBot");
				string contraseña = builder.Configuration.GetValue<string>("WebmasterDeals:CorreoBotContraseña");

				System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage();
				mensaje.From = new System.Net.Mail.MailAddress(correoDesde, "pepe's deals");
				mensaje.To.Add(correoHacia);
				mensaje.Subject = titulo;
				mensaje.Body = html;
				mensaje.IsBodyHtml = true;

				System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();
				cliente.Host = host;

				if (correoDesde.ToLower().Contains("gmail.com") == true)
				{
					cliente.Port = 587;
					cliente.Credentials = new System.Net.NetworkCredential(correoDesde, contraseña);
					cliente.EnableSsl = true;

					try
					{
						cliente.Send(mensaje);
						return true;
					}
					catch (Exception ex)
					{
						DateTime nuevaFecha = DateTime.Now;
						nuevaFecha = nuevaFecha.AddMinutes(10);

						await global::BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", nuevaFecha);

						global::BaseDatos.Errores.Insertar.Mensaje("Correo Enviar", ex, false);
						return false;
					}
				}
				else
				{
					cliente.Port = 8889;
					cliente.Credentials = new System.Net.NetworkCredential(correoDesde, contraseña);
					cliente.EnableSsl = false;

					try
					{
						cliente.Send(mensaje);
						return true;
					}
					catch (Exception ex)
					{
						DateTime nuevaFecha = DateTime.Now;
						nuevaFecha = nuevaFecha.AddMinutes(10);

						await global::BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", nuevaFecha);

						global::BaseDatos.Errores.Insertar.Mensaje("Correo Enviar " + correoDesde + " - " + correoHacia, ex, false);

						return false;
					}
				}
			}

			return false;
		}
	}
}
