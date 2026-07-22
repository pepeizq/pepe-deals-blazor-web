#nullable disable

namespace Herramientas
{
	public static class Bots
	{
		public static List<string> botsUserAgents = [
			"AhrefsBot",
			"Amazonbot",
			"Applebot",
			"Applebot-Extended",
			"archive.org_bot",
			"ArchiveBot",
			"Baiduspider",
			"Barkrowler",
			"Bingbot",
			"BingPreview",
			"Bytespider",
			"bot",
			"CCBot",
			"Chrome-Lighthouse",
			"claudebot",
			"CloudFlare",
			"DataForSeoBot",
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
			"Go-http-client",
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
			"meta-externalagent",
			"MJ12bot",
			"MojeekBot",
			"nbot",
			"OpenWebSearchBot",
			"Owler",
			"PerplexityBot",
			"Perplexity-User",
			"PetalBot",
			"Qwantify",
			"redditbot",
			"Scrapy",
			"SERankingBacklinksBot",
			"SeznamBot",
			"SemrushBot",
			"sift",
			"Slurp",
			"Sogou web spider",
			"Sogou+web+spider",
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
