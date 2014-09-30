using System;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class NecMatcher : MatcherBase
	{
		private const int NecLdTolerance = 2;
		public NecMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("NEC") || request.UserAgent.StartsWith("KGT");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			if (userAgent.StartsWith("NEC"))
			{
				return base.LookForMatchingUserAgent(userAgent);
			}
			if (this.IsDebugEnabled)
			{
				this.Logger.DebugFormat("Applying LD({0}) On UA: {1} ", 2, userAgent);
			}
			return StringMatchers.LevenshteinDistance(base.UserAgents, userAgent, 2);
		}
	}
}
