using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class KDDIMatcher : MatcherBase
	{
		public KDDIMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("KDDI");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int num = KDDIMatcher.Tolerance(userAgent);
			if (this.IsDebugEnabled)
			{
				this.Logger.Debug(string.Concat(new object[]
				{
					"Applying RIS with tolerance[",
					num,
					"] UA: ",
					userAgent
				}));
			}
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, num);
		}
		private static int Tolerance(string userAgent)
		{
			if (userAgent.StartsWith("KDDI/"))
			{
				return StringUtils.SecondSlash(userAgent);
			}
			if (userAgent.StartsWith("KDDI"))
			{
				return StringUtils.FirstSlash(userAgent);
			}
			return StringUtils.IndexOfOrLength(userAgent, ")");
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			if (request.UserAgent.IndexOf("Opera") != -1)
			{
				return "opera";
			}
			return "opwv_v62_generic";
		}
	}
}
