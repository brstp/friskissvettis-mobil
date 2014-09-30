using System;
using System.Text.RegularExpressions;
namespace WURFL.Request.Normalizers.Generic
{
	public class TracFoneSerialNumberRemover : IUserAgentNormalizer
	{
		private static readonly Regex TracFoneSerialNumberRegex = new Regex("\\[TF[\\d|X]+\\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public string Normalize(string userAgent)
		{
			return TracFoneSerialNumberRemover.TracFoneSerialNumberRegex.Replace(userAgent, "");
		}
	}
}
