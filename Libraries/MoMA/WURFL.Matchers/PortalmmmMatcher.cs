using System;
using WURFL.Commons;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class PortalmmmMatcher : MatcherBase
	{
		public PortalmmmMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("portalmmm");
		}
		protected override string ApplyConclusiveMatch(WURFLRequest request)
		{
			if (!base.UserAgents.Contains(request.UserAgent))
			{
				return Constants.Generic;
			}
			return request.UserAgent;
		}
	}
}
