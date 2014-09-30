using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class HTCMatcher : MatcherBase
	{
		public HTCMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("HTC") || userAgent.Contains("XV6875.1");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int tolerance = StringUtils.FirstSlash(userAgent);
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, tolerance);
		}
	}
}
