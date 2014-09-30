using System;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class DoCoMoMatcher : MatcherBase
	{
		public DoCoMoMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.StartsWith("DoCoMo");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			return null;
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			if (request.UserAgent.StartsWith("DoCoMo/2"))
			{
				return "docomo_generic_jap_ver2";
			}
			return "docomo_generic_jap_ver1";
		}
	}
}
