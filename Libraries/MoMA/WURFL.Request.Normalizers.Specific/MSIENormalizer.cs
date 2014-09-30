using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Specific
{
	public class MSIENormalizer : IUserAgentNormalizer
	{
		private static readonly Regex MSIEMajorAndMinorRegex = new Regex("MSIE\\s\\d\\.\\d", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public static readonly MSIENormalizer Instance = new MSIENormalizer();
		public string Normalize(string userAgent)
		{
			Match match = MSIENormalizer.MSIEMajorAndMinorRegex.Match(userAgent);
			if (match.Success)
			{
				return match.Groups[0].Value;
			}
			return userAgent;
		}
	}
}
