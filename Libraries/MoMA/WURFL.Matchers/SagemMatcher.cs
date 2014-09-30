using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SagemMatcher : MatcherBase
	{
		public SagemMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Sagem") || userAgent.StartsWith("SAGEM");
		}
	}
}
