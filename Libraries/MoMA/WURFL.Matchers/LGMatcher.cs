using System;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class LGMatcher : MatcherBase
	{
		public LGMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("lg") || StringUtils.ContainsAnyOf(userAgent, new string[]
			{
				"LG-",
				"LGE"
			});
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int num = StringUtils.IndexOfOrLength(userAgent, "/", userAgent.IndexOf("LG"));
			if (this.IsDebugEnabled)
			{
				this.Logger.DebugFormat("Applying RIS with tolerance [{0}] UA: {1}", num, userAgent);
			}
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, num);
		}
	}
}
