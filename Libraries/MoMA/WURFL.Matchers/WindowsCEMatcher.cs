using System;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class WindowsCEMatcher : MatcherBase
	{
		private const int WindowsCETolerance = 3;
		public WindowsCEMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.Contains("Mozilla/") && (userAgent.Contains("WindowsCE") || userAgent.Contains("Windows CE"));
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			if (this.IsDebugEnabled)
			{
				this.Logger.DebugFormat("Applying LD({0}) UA: {1}", 3, userAgent);
			}
			return StringMatchers.LevenshteinDistance(base.UserAgents, userAgent, 3);
		}
		protected override string ApplyRecoveryMatch(WURFLRequest normalizedRequest)
		{
			return "generic_ms_mobile_browser_ver1";
		}
	}
}
