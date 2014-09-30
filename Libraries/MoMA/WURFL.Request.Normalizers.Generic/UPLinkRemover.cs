using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Generic
{
	public class UPLinkRemover : IUserAgentNormalizer
	{
		private static readonly Regex UPLinkRegex = new Regex("\\s*UP.Link.+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			return UPLinkRemover.UPLinkRegex.Replace(userAgent, "");
		}
	}
}
