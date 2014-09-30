using System;
namespace WURFL.Request
{
	public interface IUserAgentNormalizer
	{
		string Normalize(string userAgent);
	}
}
