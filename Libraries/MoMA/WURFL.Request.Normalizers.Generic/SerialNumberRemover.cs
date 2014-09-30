using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Generic
{
	public class SerialNumberRemover : IUserAgentNormalizer
	{
		private static readonly Regex SerialNumberRegex = new Regex("/SN[\\d|X]+\\s", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			return SerialNumberRemover.SerialNumberRegex.Replace(userAgent, " ");
		}
	}
}
