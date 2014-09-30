using System;
using WURFL.Commons;
namespace WURFL.Matchers.Utils
{
	public static class MatchersUtil
	{
		private static readonly string[] CurrentMobileBrowserKeywords = new string[]
		{
			"cldc",
			"symbian",
			"midp",
			"j2me",
			"mobile",
			"wireless",
			"palm",
			"phone",
			"pocket pc",
			"pocketpc",
			"netfront",
			"bolt",
			"iris",
			"brew",
			"openwave",
			"windows ce",
			"wap2",
			"android",
			"opera mini",
			"opera mobi",
			"maemo",
			"fennec",
			"blazer",
			"160x160",
			"tablet",
			"webos",
			"sony",
			"nintendo"
		};
		public static bool IsMobileBrowser(string userAgent)
		{
			return Collections.Exist<string>(MatchersUtil.CurrentMobileBrowserKeywords, Predicates.ContainedInCaseInsensitive(userAgent));
		}
	}
}
