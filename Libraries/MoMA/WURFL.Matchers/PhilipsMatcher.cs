using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class PhilipsMatcher : MatcherBase
	{
		public PhilipsMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("Philips") || request.UserAgent.StartsWith("PHILIPS");
		}
	}
}
