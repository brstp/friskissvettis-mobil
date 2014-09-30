using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class OperaMatcher : MatcherBase
	{
		private const int OperaLdTolerance = 1;
		private readonly IDictionary<string, string> OperaVersions = new Dictionary<string, string>();
		private static readonly Regex OperaVersion = new Regex(".*Opera[\\s/](\\d+).*", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public OperaMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
			this.CreateCatchAllIds();
		}
		private void CreateCatchAllIds()
		{
			this.OperaVersions.Add("", "opera");
			this.OperaVersions.Add("7", "opera_7");
			this.OperaVersions.Add("8", "opera_8");
			this.OperaVersions.Add("9", "opera_9");
			this.OperaVersions.Add("10", "opera_10");
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return !request.IsMobileRequest && request.UserAgent.Contains("Opera");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			return StringMatchers.LevenshteinDistance(base.UserAgents, userAgent, 1);
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			string operaVersionNumber = this.OperaVersionNumber(request.UserAgent);
			string text = this.OperaId(operaVersionNumber);
			if (!base.DeviceExist(text))
			{
				return Constants.Generic;
			}
			return text;
		}
		private string OperaId(string operaVersionNumber)
		{
			if (this.OperaVersions.ContainsKey(operaVersionNumber))
			{
				return this.OperaVersions[operaVersionNumber];
			}
			return "opera";
		}
		private string OperaVersionNumber(string userAgent)
		{
			Match match = OperaMatcher.OperaVersion.Match(userAgent);
			if (!match.Success)
			{
				return string.Empty;
			}
			return match.Groups[1].Value;
		}
	}
}
