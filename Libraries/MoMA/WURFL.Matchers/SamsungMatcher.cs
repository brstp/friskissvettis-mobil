using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class SamsungMatcher : MatcherBase
	{
		public SamsungMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return StringUtils.ContainsAnyOf(userAgent, new string[]
			{
				"Samsung/SGH",
				"Samsung"
			}) || StringUtils.StartsWithAnyOf(userAgent, new string[]
			{
				"SEC-",
				"SAMSUNG",
				"SPH",
				"SGH",
				"SCH"
			});
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int num = SamsungMatcher.Tolerance(userAgent);
			if (this.IsDebugEnabled)
			{
				this.Logger.DebugFormat("Applying Ris with tolrance ({0}) UA: {1}", num, userAgent);
			}
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, num);
		}
		private static int Tolerance(string userAgent)
		{
			if (userAgent.StartsWith("SEC-") || userAgent.StartsWith("SAMSUNG-") || userAgent.StartsWith("SCH"))
			{
				return StringUtils.FirstSlash(userAgent);
			}
			if (userAgent.StartsWith("Samsung") || userAgent.StartsWith("SPH") || userAgent.StartsWith("SGH"))
			{
				return StringUtils.FirstSpace(userAgent);
			}
			if (userAgent.StartsWith("SAMSUNG/"))
			{
				return StringUtils.SecondSlash(userAgent);
			}
			if (userAgent.Contains("Samsung/SGH"))
			{
				return SamsungMatcher.SecondSlashAfterSamsungOrLength(userAgent);
			}
			return userAgent.Length;
		}
		private static int SecondSlashAfterSamsungOrLength(string userAgent)
		{
			return StringUtils.OrdinalIndexOfOrLength(userAgent, "/", 2, userAgent.IndexOf("Samsung"));
		}
	}
}
