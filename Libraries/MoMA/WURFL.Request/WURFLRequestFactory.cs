using System;
using System.Web;
namespace WURFL.Request
{
	public class WURFLRequestFactory : IWURFLRequestFactory
	{
		private readonly IUserAgentResolver userAgentResolver;
		public WURFLRequestFactory(IUserAgentResolver userAgentResolver)
		{
			this.userAgentResolver = userAgentResolver;
		}
		public WURFLRequest CreateRequest(string userAgent)
		{
			return new WURFLRequest(userAgent);
		}
		public WURFLRequest CreateRequest(HttpRequest httpRequest)
		{
			string userAgent = this.userAgentResolver.Resolve(httpRequest);
			return new WURFLRequest(userAgent);
		}
	}
}
