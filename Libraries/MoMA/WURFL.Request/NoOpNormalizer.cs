using System;
namespace WURFL.Request
{
	public class NoOpNormalizer : IUserAgentNormalizer
	{
		public static readonly IUserAgentNormalizer Instance = new NoOpNormalizer();
		public string Normalize(string userAgent)
		{
			return userAgent;
		}
	}
}
