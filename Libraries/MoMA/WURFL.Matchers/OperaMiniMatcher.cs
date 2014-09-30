using System;
using System.Collections.Generic;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	internal class OperaMiniMatcher : MatcherBase
	{
		private readonly IDictionary<string, string> OperaMini = new Dictionary<string, string>();
		public OperaMiniMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
			this.CreateCatchAllIds();
		}
		private void CreateCatchAllIds()
		{
			this.OperaMini.Add("Opera Mini/1", "browser_opera_mini_release1");
			this.OperaMini.Add("Opera Mini/2", "browser_opera_mini_release2");
			this.OperaMini.Add("Opera Mini/3", "browser_opera_mini_release3");
			this.OperaMini.Add("Opera Mini/4", "browser_opera_mini_release4");
			this.OperaMini.Add("Opera Mini/5", "browser_opera_mini_release5");
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return request.UserAgent.Contains("Opera Mini");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, ToleranceCalculators.First("/").After("Opera Mini").Tolerance(userAgent));
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			string text = Collections.Find<string>(this.OperaMini.Keys, Predicates.ContainedIn(request.UserAgent));
			if (text == null)
			{
				return Constants.Generic;
			}
			return this.OperaMini[text];
		}
	}
}
