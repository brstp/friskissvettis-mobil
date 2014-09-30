using System;
using System.Collections.Generic;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class BotMatcher : MatcherBase
	{
		private const int BotsLdTolerance = 4;
		private static readonly string[] CurrentBotKeywords = new string[]
		{
			"bot",
			"crawler",
			"spider",
			"novarra",
			"transcoder",
			"yahoo! searchmonkey",
			"yahoo! slurp",
			"feedfetcher-google",
			"toolbar",
			"mowser"
		};
		private readonly List<string> botKeywords = new List<string>();
		public BotMatcher() : this(NoOpNormalizer.Instance, new List<string>())
		{
		}
		public BotMatcher(ICollection<string> botKeywords) : this(NoOpNormalizer.Instance, botKeywords)
		{
		}
		public BotMatcher(IUserAgentNormalizer userAgentNormalizer) : this(userAgentNormalizer, new List<string>())
		{
		}
		public BotMatcher(IUserAgentNormalizer userAgentNormalizer, ICollection<string> keywords) : base(userAgentNormalizer)
		{
			this.botKeywords = new List<string>(keywords);
			this.botKeywords.AddRange(BotMatcher.CurrentBotKeywords);
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return Collections.Exist<string>(this.botKeywords, Predicates.ContainedInCaseInsensitive(request.UserAgent));
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			return StringMatchers.LevenshteinDistance(base.UserAgents, userAgent, 4);
		}
		protected override string ApplyRecoveryMatch(WURFLRequest normalizedRequest)
		{
			return "generic_web_crawler";
		}
	}
}
