using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SonyEricssonMatcher : MatcherBase
	{
		public SonyEricssonMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("SonyEricsson");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int num = userAgent.StartsWith("SonyEricsson") ? StringUtils.FirstSlash(userAgent) : StringUtils.SecondSlash(userAgent);
			if (this.IsDebugEnabled)
			{
				this.Logger.DebugFormat("Applying RIS({0}) UA: {1}", num, userAgent);
			}
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, num);
		}
	}
}
