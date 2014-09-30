using System;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class PantechMatcher : MatcherBase
	{
		private const int PantechLdTollerance = 4;
		public PantechMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Pantech") || userAgent.StartsWith("PT-") || userAgent.StartsWith("PANTECH") || userAgent.StartsWith("PG-");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			if (userAgent.StartsWith("Pantech"))
			{
				if (this.IsDebugEnabled)
				{
					this.Logger.DebugFormat("Applying LD({0}) UA: {1}", 4, userAgent);
				}
				return StringMatchers.LevenshteinDistance(base.UserAgents, userAgent, 4);
			}
			return base.LookForMatchingUserAgent(userAgent);
		}
	}
}
