using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Specific
{
	public class FirefoxNormalizer : IUserAgentNormalizer
	{
		private static readonly Regex FirefoxMajorAndMinorRegex = new Regex("Firefox/\\d.\\d", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public static readonly FirefoxNormalizer Instance = new FirefoxNormalizer();
		public string Normalize(string userAgent)
		{
			Match match = FirefoxNormalizer.FirefoxMajorAndMinorRegex.Match(userAgent);
			if (match.Success)
			{
				return match.Groups[0].Value;
			}
			return userAgent;
		}
	}
}
