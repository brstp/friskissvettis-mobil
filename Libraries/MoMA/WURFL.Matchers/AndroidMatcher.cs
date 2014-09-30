using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WURFL.Commons;
using WURFL.Matchers.StringMatcher;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class AndroidMatcher : MatcherBase
	{
		private const string GenericAndroid = "generic_android";
		private static readonly Regex AndroidOsVersionRegex = new Regex("Android[\\s/](\\d).(\\d)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly IDictionary<string, string> Andoroids = AndroidMatcher.GenericAndroids();
		public AndroidMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
			AndroidMatcher.GenericAndroids();
		}
		public override bool CanMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			return userAgent.StartsWith("Mozilla") && userAgent.Contains("Android");
		}
		protected override string LookForMatchingUserAgent(string userAgent)
		{
			int tolerance = AndroidMatcher.FirsSpaceAfterAndroidString(userAgent);
			return StringMatchers.LongestCommonPrefix(base.UserAgents, userAgent, tolerance);
		}
		private static int FirsSpaceAfterAndroidString(string userAgent)
		{
			return StringUtils.IndexOfOrLength(userAgent, " ", StringUtils.IndexOfOrLength(userAgent, "Android"));
		}
		private static string AndroidOsVersion(string userAgent)
		{
			Match match = AndroidMatcher.AndroidOsVersionRegex.Match(userAgent);
			if (!match.Success)
			{
				return string.Empty;
			}
			return match.Groups[1].Value + "_" + match.Groups[2];
		}
		private static IDictionary<string, string> GenericAndroids()
		{
			return new Dictionary<string, string>
			{

				{
					"",
					"generic_android"
				},

				{
					"1_5",
					"generic_android_ver1_5"
				},

				{
					"1_6",
					"generic_android_ver1_6"
				},

				{
					"2_0",
					"generic_android_ver2"
				},

				{
					"2_1",
					"generic_android_ver2_1"
				},

				{
					"2_2",
					"generic_android_ver2_2"
				}
			};
		}
		protected override string ApplyRecoveryMatch(WURFLRequest request)
		{
			string userAgent = request.UserAgent;
			if (AndroidMatcher.IsFroyo(userAgent))
			{
				return "generic_android_ver2_2";
			}
			string key = AndroidMatcher.AndroidOsVersion(userAgent);
			if (!AndroidMatcher.Andoroids.ContainsKey(key))
			{
				return "generic_android";
			}
			return AndroidMatcher.Andoroids[key];
		}
		private static bool IsFroyo(string userAgent)
		{
			return userAgent.Contains("Froyo");
		}
	}
}
