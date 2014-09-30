using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class QtekMatcher : MatcherBase
	{
		public QtekMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("Qtek");
		}
	}
}
