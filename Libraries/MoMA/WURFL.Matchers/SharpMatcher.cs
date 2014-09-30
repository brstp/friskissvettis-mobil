using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SharpMatcher : MatcherBase
	{
		public SharpMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Sharp") || userAgent.StartsWith("SHARP");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int tolerance = StringUtils.FirstSlash(userAgent);
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, tolerance);
		}
	}
}
