using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace Herramientas
{
	public static class EnlaceAdaptador
	{
		public static string Nombre(string nombre)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return string.Empty;
			}

			string s2 = Regex.Replace(nombre, @"(?<![a-zA-Z0-9])[^a-zA-Z0-9]|[^a-zA-Z0-9](?![a-zA-Z0-9])", "_");
			s2 = s2.Replace("'", string.Empty);
			s2 = s2.Replace("’", string.Empty);
			s2 = s2.Replace(".", string.Empty);
            s2 = s2.Replace("+", string.Empty);
			s2 = s2.Replace("/", string.Empty);
            s2 = s2.Replace("™", string.Empty);
			s2 = s2.Replace("-", string.Empty);
			s2 = s2.Replace("~", string.Empty);
			s2 = s2.Replace(":", string.Empty);
			s2 = s2.Replace("%E2", string.Empty);
			s2 = s2.Replace("%80", string.Empty);
			s2 = s2.Replace("%99", string.Empty);
			s2 = s2.Replace("&", "_");
			s2 = s2.Replace(Strings.ChrW(160).ToString(), "_");
			s2 = s2.Replace(" ", "_");
			s2 = s2.Replace("*", "_");
			s2 = s2.Replace("】", "-");
			s2 = s2.Replace("ü", "u");

			int i = 0;
			while (i < 10)
			{
				s2 = s2.Replace("__", "_");
				i += 1;
			}

			if (s2.LastIndexOf("_") == s2.Length - 1)
			{
				s2 = s2.Remove(s2.Length - 1, 1);
			}

			if (s2.IndexOf("_") == 0)
			{
				s2 = s2.Remove(0, 1);
			}

			if (s2.Length == 0)
			{
				s2 = "_";
			}

			return s2;
		}
	}
}
