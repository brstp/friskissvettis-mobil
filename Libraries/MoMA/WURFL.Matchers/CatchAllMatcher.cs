using System;
using System.Collections.Generic;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class CatchAllMatcher : MatcherBase
	{
		private const int MozillaTolerance = 5;
		public CatchAllMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest normalziedRequest)
		{
			return true;
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			if (!userAgent.StartsWith("Mozilla"))
			{
				return base.LookForMatchingUserAgent(userAgent);
			}
			if (userAgent.StartsWith("Mozilla/4"))
			{
				return StringMatchers.LevenshteinDistance(CatchAllMatcher.GetMozillaData(base.UserAgents, "Mozilla/4"), userAgent, 5);
			}
			if (userAgent.StartsWith("Mozilla/5"))
			{
				return StringMatchers.LevenshteinDistance(CatchAllMatcher.GetMozillaData(base.UserAgents, "Mozilla/5"), userAgent, 5);
			}
			return StringMatchers.LevenshteinDistance(CatchAllMatcher.GetMozillaData(base.UserAgents, "Mozilla"), userAgent, 5);
		}
		private static ICollection<string> GetMozillaData(ICollection<string> userAgentsSet, string starting)
		{
			return Collections.Select<string>(userAgentsSet, Predicates.StartsWith(starting));
		}
	}
}
