using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Generic
{
	public class LocaleRemover : IUserAgentNormalizer
	{
		private static readonly Regex LocaleRegex = new Regex("; ([a-z][a-z]?(-[aA-zZ][aA-zZ]))", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			return LocaleRemover.LocaleRegex.Replace(userAgent, "");
		}
	}
}
