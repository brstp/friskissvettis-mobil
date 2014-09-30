using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class GrundingMatcher : MatcherBase
	{
		public GrundingMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Grunding") || userAgent.StartsWith("GRUNDING");
		}
	}
}
