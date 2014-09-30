using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class MotorolaMatcher : MatcherBase
	{
		public MotorolaMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Mot-") || userAgent.Contains("MOT-") || userAgent.Contains("Motorola");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			if (userAgent.StartsWith("Mot-") || userAgent.StartsWith("MOT-") || userAgent.StartsWith("Motorola"))
			{
				return base.LookForMatchingUserAgent(userAgent);
			}
			return StringMatchers.LevenshteinDistance(base.UserAgents, userAgent, 5);
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			if (userAgent.Contains("MIB/2.2") || userAgent.Contains("MIB/BER2.2"))
			{
				return "mot_mib22_generic";
			}
			return Constants.Generic;
		}
	}
}
