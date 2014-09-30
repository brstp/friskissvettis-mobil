using System;
using WURFL.Matchers.Utils;
using WURFL.Request;
namespace WURFL.Matchers
{
	public abstract class DesktopBrowserMatcher : MatcherBase
	{
		public DesktopBrowserMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return !MatchersUtil.IsMobileBrowser(request.UserAgent) && this.CanMatchDesktopBrowser(request);
		}
		protected abstract bool CanMatchDesktopBrowser(WURFLRequest request);
	}
}
