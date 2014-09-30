using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Specific
{
	public class ChromeNormalizer : IUserAgentNormalizer
	{
		public static readonly ChromeNormalizer Instance = new ChromeNormalizer();
		private static readonly Regex ChromeWithMajorVersionRegex = new Regex("Chrome/\\d", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			if (ChromeNormalizer.ContainsChromeWithMajorVersion(userAgent))
			{
				return userAgent.Substring(userAgent.IndexOf("Chrome"), 8);
			}
			return userAgent;
		}
		private static bool ContainsChromeWithMajorVersion(string userAgent)
		{
			return ChromeNormalizer.ChromeWithMajorVersionRegex.IsMatch(userAgent);
		}
	}
}
