
using ApexCharts;
using Azure.Core;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Herramientas.Ficheros
{

	public static class Imgur
	{
		private const string ClientId = "68a076ce5dadb1f";
		private const string ClientSecret = "1b3e2e92bde541f145b456fa2da5e0623d71c744";
		private const string UploadUrl = "https://api.imgur.com/3/image";

		public static async Task<string> SubirImagen(string imageUrl)
		{
			using HttpClient client = new HttpClient();

			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("client_id", ClientId),
				new KeyValuePair<string, string>("client_secret", ClientSecret)
			});

			var response = await client.PostAsync("https://api.imgur.com/oauth2/token", content);
			string responseBody = await response.Content.ReadAsStringAsync();
			global::BaseDatos.Errores.Insertar.Mensaje("test", responseBody);
			using JsonDocument doc = JsonDocument.Parse(responseBody);
			var token = doc.RootElement.GetProperty("access_token").GetString();

			using HttpClient client2 = new HttpClient();
			client2.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

			var content2 = new MultipartFormDataContent();
			content2.Add(new StringContent(imageUrl), "image");
			content2.Add(new StringContent("URL"), "type");

			var response2 = await client2.PostAsync(UploadUrl, content2);
			string responseBody2 = await response2.Content.ReadAsStringAsync();

			if (!response2.IsSuccessStatusCode)
				throw new Exception($"Error: {response2.StatusCode} - {responseBody2}");

			using JsonDocument doc2 = JsonDocument.Parse(responseBody2);
			return doc2.RootElement.GetProperty("data").GetProperty("link").GetString();
		}
	}
}
