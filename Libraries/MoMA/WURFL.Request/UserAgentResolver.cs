using System;
using System.Collections.Generic;
using System.Web;
namespace WURFL.Request
{
	public class UserAgentResolver : IUserAgentResolver
	{
		private delegate string ExtractUserAgent(HttpRequest httpRequest);
		private const string UaParameter = "UA";
		private readonly List<UserAgentResolver.ExtractUserAgent> extractors = new List<UserAgentResolver.ExtractUserAgent>();
		public UserAgentResolver()
		{
			this.extractors.Add(new UserAgentResolver.ExtractUserAgent(UserAgentResolver.FromQueryString));
			this.extractors.Add(new UserAgentResolver.ExtractUserAgent(UserAgentResolver.FromXSkyFireVersionHeader));
			this.extractors.Add(new UserAgentResolver.ExtractUserAgent(UserAgentResolver.FromXDeviceUserAgentHeader));
			this.extractors.Add(new UserAgentResolver.ExtractUserAgent(UserAgentResolver.FromUserAgent));
		}
		public string Resolve(HttpRequest httpRequest)
		{
			string text = null;
			foreach (UserAgentResolver.ExtractUserAgent current in this.extractors)
			{
				text = current(httpRequest);
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
			}
			return text;
		}
		private static string FromQueryString(HttpRequest httpRequest)
		{
			return httpRequest.QueryString["UA"];
		}
		private static string FromUserAgent(HttpRequest httpRequest)
		{
			return httpRequest.UserAgent;
		}
		private static string FromXSkyFireVersionHeader(HttpRequest httpRequest)
		{
			string value = httpRequest.Headers["X-Skyfire-Version"];
			if (!string.IsNullOrEmpty(value))
			{
				return "Generic_Skyfire_Browser";
			}
			return null;
		}
		private static string FromXDeviceUserAgentHeader(HttpRequest httpRequest)
		{
			return httpRequest.Headers["x-device-user-agent"];
		}
	}
}
