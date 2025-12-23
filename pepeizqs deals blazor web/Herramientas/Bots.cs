#nullable disable

namespace Herramientas
{
	public static class Bots
	{
		public static List<string> botsUserAgents = [
			"AhrefsBot",
			"Applebot",
			"Applebot-Extended",
			"archive.org_bot",
			"ArchiveBot",
			"Baiduspider",
			"Barkrowler",
			"Bingbot",
			"BingPreview",
			"bot",
			"CCBot",
			"Chrome-Lighthouse",
            "CloudFlare",
            "Discordbot",
			"DotBot",
			"DuckAssistBot",
			"DuckDuckBot",
			"Ecosia",
			"Exabot",
			"facebook",
			"feedburner",
			"fetcher",
			"Feedfetcher-Google",
			"Feedly",
			"FlipboardBot",
			"Googlebot",
			"Googlebot-Mobile",
			"Google-Extended",
			"Google-Safety",
			"GoogleAssociationService",
			"Google StoreBot",
			"Google Web Preview",
			"ia_archiver",
			"Lighthouse",
			"link",
			"MJ12bot",
			"MojeekBot",
			"nbot",
			"OpenWebSearchBot",
			"Owler",
			"PerplexityBot",
			"Perplexity-User",
			"Qwantify",
			"SeznamBot",
			"sift",
			"Slurp",
			"spider",
			"TelegramBot",
			"Twitterbot",
			"Valve Client",
			"Valve Steam",
			"YandexBot",
			"Yeti",
			"zoominfobot"
		];

		public static bool UserAgentEsBot(string userAgent)
		{
			if (string.IsNullOrEmpty(userAgent) == false)
			{
				foreach (var bot in botsUserAgents)
				{
					if (userAgent.ToLower().Contains(bot.ToLower()) == true)
					{
						return true;
					}
				}
			}
			else
			{
				return true;
			}

			return false;
		}
	}
}
