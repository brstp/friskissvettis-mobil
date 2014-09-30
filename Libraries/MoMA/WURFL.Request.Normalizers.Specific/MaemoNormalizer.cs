using System;
namespace WURFL.Request.Normalizers.Specific
{
	public class MaemoNormalizer : IUserAgentNormalizer
	{
		public static readonly MaemoNormalizer Instance = new MaemoNormalizer();
		public string Normalize(string userAgent)
		{
			if (userAgent.Contains("Maemo"))
			{
				return userAgent.Substring(userAgent.IndexOf("Maemo"));
			}
			return userAgent;
		}
	}
}
