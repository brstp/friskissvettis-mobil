using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Specific
{
	public class SafariNormalizer : IUserAgentNormalizer
	{
		public static readonly SafariNormalizer Instance = new SafariNormalizer();
		private static readonly Regex SafariRegex = new Regex("(Mozilla\\/5\\.0.*)(?:;\\s*U;.*)(Safari\\/\\d{0,3})", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			Match match = SafariNormalizer.SafariRegex.Match(userAgent);
			if (match.Success)
			{
				return match.Groups[1].Value + " " + match.Groups[2].Value;
			}
			return userAgent;
		}
	}
}
