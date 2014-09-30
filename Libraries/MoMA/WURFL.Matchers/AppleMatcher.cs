using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class AppleMatcher : MatcherBase
	{
		private const string Iphone = "iPhone";
		private const string Ipad = "iPad";
		private const string Ipod = "iPod";
		private static readonly string[] AppleKeyWords = new string[]
		{
			"iPhone",
			"iPod",
			"iPad"
		};
		public AppleMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return Collections.Exist<string>(AppleMatcher.AppleKeyWords, Predicates.ContainedIn(request.UserAgent));
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int tolerance = userAgent.StartsWith("Apple") ? AppleMatcher.ThirdSpaceOrLength(userAgent) : StringUtils.FirstSemiColon(userAgent);
			if (this.IsDebugEnabled)
			{
				this.Logger.Debug("Applying RIS(First Semicolon/Third Space ) UA: " + userAgent);
			}
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, tolerance);
		}
		private static int ThirdSpaceOrLength(string userAgent)
		{
			return StringUtils.OrdinalIndexOfOrLength(userAgent, " ", 3);
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			if (userAgent.Contains("iPad"))
			{
				return "apple_ipad_ver1";
			}
			if (userAgent.Contains("iPod"))
			{
				return "apple_ipod_touch_ver1";
			}
			return "apple_iphone_ver1";
		}
	}
}
