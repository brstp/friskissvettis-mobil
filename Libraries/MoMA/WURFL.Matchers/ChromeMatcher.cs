using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class ChromeMatcher : DesktopBrowserMatcher
	{
		public ChromeMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		protected override bool CanMatchDesktopBrowser(WURFLRequest request)
		{
			return request.UserAgent.Contains("Chrome");
		}
	}
}
