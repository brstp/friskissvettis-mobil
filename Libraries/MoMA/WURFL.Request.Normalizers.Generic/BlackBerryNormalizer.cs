using System;
namespace WURFL.Request.Normalizers.Generic
{
	public class BlackBerryNormalizer : IUserAgentNormalizer
	{
		public string Normalize(string userAgent)
		{
			string result = userAgent;
			int num = userAgent.IndexOf("BlackBerry");
			if (num > 0 && !userAgent.Contains("AppleWebKit"))
			{
				result = userAgent.Substring(num);
			}
			return result;
		}
	}
}
