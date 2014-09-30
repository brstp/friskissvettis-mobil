using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class NokiaMatcher : MatcherBase
	{
		public NokiaMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("Nokia");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int tolerance = NokiaMatcher.IndexOfAnyOrLengthAfterNokia(userAgent);
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, tolerance);
		}
		private static int IndexOfAnyOrLengthAfterNokia(string userAgent)
		{
			int num = userAgent.IndexOfAny(new char[]
			{
				'/',
				' '
			}, userAgent.IndexOf("Nokia"));
			if (num <= -1)
			{
				return userAgent.Length;
			}
			return num;
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			if (userAgent.Contains("Series60"))
			{
				return "nokia_generic_series60";
			}
			if (userAgent.Contains("Series80"))
			{
				return "nokia_generic_series80";
			}
			return Constants.Generic;
		}
	}
}
