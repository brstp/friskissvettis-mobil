using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WURFL.Commons;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class BlackBerryMatcher : MatcherBase
	{
		private static readonly IDictionary<string, string> BlackBerries = BlackBerryMatcher.GenericBlackBerries();
		private static readonly Regex BlackBerryOsVersionRegex = new Regex("Black[Bb]erry[^/\\s]+/(\\d.\\d)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		public BlackBerryMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.Contains("BlackBerry") || userAgent.Contains("Blackberry");
		}
		private static IDictionary<string, string> GenericBlackBerries()
		{
			return new Dictionary<string, string>
			{

				{
					"2.",
					"blackberry_generic_ver2"
				},

				{
					"3.2",
					"blackberry_generic_ver3_sub2"
				},

				{
					"3.3",
					"blackberry_generic_ver3_sub30"
				},

				{
					"3.5",
					"blackberry_generic_ver3_sub50"
				},

				{
					"3.6",
					"blackberry_generic_ver3_sub60"
				},

				{
					"3.7",
					"blackberry_generic_ver3_sub70"
				},

				{
					"4.1",
					"blackberry_generic_ver4_sub10"
				},

				{
					"4.2",
					"blackberry_generic_ver4_sub20"
				},

				{
					"4.3",
					"blackberry_generic_ver4_sub30"
				},

				{
					"4.5",
					"blackberry_generic_ver4_sub50"
				},

				{
					"4.6",
					"blackberry_generic_ver4_sub60"
				},

				{
					"4.7",
					"blackberry_generic_ver4_sub70"
				},

				{
					"4.",
					"blackberry_generic_ver4"
				},

				{
					"5.",
					"blackberry_generic_ver5"
				},

				{
					"6.",
					"blackberry_generic_ver6"
				}
			};
		}
		private static string BlackBerryOsVersion(string userAgent)
		{
			Match match = BlackBerryMatcher.BlackBerryOsVersionRegex.Match(userAgent);
			if (!match.Success)
			{
				return null;
			}
			return match.Groups[1].Value;
		}
		protected override string ApplyRecoveryMatch(WURFLRequest normalizedRequest)
		{
			string userAgent = normalizedRequest.UserAgent;
			string text = BlackBerryMatcher.BlackBerryOsVersion(userAgent);
			if (text == null)
			{
				return Constants.Generic;
			}
			string text2 = Collections.Find<string>(BlackBerryMatcher.BlackBerries.Keys, Predicates.PrefixOf(text));
			if (text2 != null)
			{
				return BlackBerryMatcher.BlackBerries[text2];
			}
			return Constants.Generic;
		}
	}
}
