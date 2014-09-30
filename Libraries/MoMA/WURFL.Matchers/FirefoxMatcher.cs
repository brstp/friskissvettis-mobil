using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class FirefoxMatcher : DesktopBrowserMatcher
	{
		public FirefoxMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		protected override bool CanMatchDesktopBrowser(WURFLRequest request)
		{
			return request.UserAgent.Contains("Firefox");
		}
	}
}
