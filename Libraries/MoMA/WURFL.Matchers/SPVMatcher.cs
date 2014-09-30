using System;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SPVMatcher : MatcherBase
	{
		public SPVMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("SPV");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int tolerance = ToleranceCalculators.First(";").After("SPV").Tolerance(userAgent);
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, tolerance);
		}
	}
}
