using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class MitsubishiMatcher : MatcherBase
	{
		public MitsubishiMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("Mitsu");
		}
	}
}
