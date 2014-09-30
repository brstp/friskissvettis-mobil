using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class PanasonicMatcher : MatcherBase
	{
		public PanasonicMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("Panasonic");
		}
	}
}
