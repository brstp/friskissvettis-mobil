using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SafariMatcher : DesktopBrowserMatcher
	{
		public SafariMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		protected override bool CanMatchDesktopBrowser(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Mozilla") && userAgent.Contains("Safari");
		}
	}
}
