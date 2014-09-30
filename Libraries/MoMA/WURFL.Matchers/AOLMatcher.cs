using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class AOLMatcher : DesktopBrowserMatcher
	{
		public AOLMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		protected override bool CanMatchDesktopBrowser(WURFLRequest request)
		{
			return request.UserAgent.Contains("AOL");
		}
	}
}
