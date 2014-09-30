using System;
namespace WURFL.Request
{
	public class RequestFactory
	{
		public static WURFLRequest FromUserAgent(string userAgent)
		{
			return new WURFLRequest(userAgent);
		}
	}
}
