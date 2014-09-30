using System;
using System.Collections.Generic;
using WURFL.Commons;
using WURFL.Request;
namespace WURFL.Matchers
{
	public class LGUPLUSMatcher : MatcherBase
	{
		public LGUPLUSMatcher(IUserAgentNormalizer userAgentNormalizer) : base(userAgentNormalizer)
		{
		}
		public override bool CanMatch(WURFLRequest request)
		{
			return StringUtils.ContainsAnyOf(request.UserAgent, new string[]
			{
				"LGUPLUS",
				"lgtelecom"
			});
		}
		protected override string ApplyConclusiveMatch(WURFLRequest request)
		{
			return Constants.Generic;
		}
		private static IEnumerable<KeyValuePair<string, string[]>> Lgupluses()
		{
			return new Dictionary<string, string[]>
			{

				{
					"generic_lguplus_rexos_facebook_browser",
					new string[]
					{
						"Windows NT 5",
						"POLARIS"
					}
				},

				{
					"generic_lguplus_rexos_webviewer_browser",
					new string[]
					{
						"Windows NT 5"
					}
				},

				{
					"generic_lguplus_winmo_facebook_browser",
					new string[]
					{
						"Windows CE",
						"POLARIS"
					}
				},

				{
					"generic_lguplus_android_webkit_browser",
					new string[]
					{
						"Android",
						"AppleWebKit"
					}
				}
			};
		}
		protected override string ApplyRecoveryMatch(WURFLRequest normalizedRequest)
		{
			foreach (KeyValuePair<string, string[]> current in LGUPLUSMatcher.Lgupluses())
			{
				if (StringUtils.ContainsAllOf(normalizedRequest.UserAgent, current.Value))
				{
					return current.Key;
				}
			}
			return "generic_lguplus";
		}
	}
}
