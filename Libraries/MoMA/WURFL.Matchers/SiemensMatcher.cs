using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SiemensMatcher : MatcherBase
	{
		public SiemensMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("SIE-");
		}
	}
}
