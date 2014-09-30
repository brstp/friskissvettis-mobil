using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Generic
{
	public class YesWAPRemover : IUserAgentNormalizer
	{
		private static readonly Regex YesWAPRegex = new Regex("\\s*Mozilla/4\\.0 \\(YesWAP mobile phone proxy\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			return YesWAPRemover.YesWAPRegex.Replace(userAgent, "");
		}
	}
}
