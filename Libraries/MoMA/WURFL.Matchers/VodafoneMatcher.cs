using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class VodafoneMatcher : MatcherBase
	{
		public VodafoneMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("Vodafone");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int num = StringUtils.OrdinalIndexOfOrLength(userAgent, "/", 3);
			if (this.IsDebugEnabled)
			{
				this.Logger.DebugFormat("Applying RIS({0}) UA: {1}", num, userAgent);
			}
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, num);
		}
	}
}
