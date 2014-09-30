using System;
using System.Collections.Generic;
namespace WURFL.Matchers.StringMatcher
{
	public static class StringMatchers
	{
		public static string LevenshteinDistance(ICollection<string> candidates, string needle, int tolerance)
		{
			return LevenshteinDistanceMatcher.Instance.Match(candidates, needle, tolerance);
		}
		internal static string LongestCommonPrefix(ICollection<string> candidates, string needle, int tolerance)
		{
			return LongestCommonPrefixMatcher.Instance.Match(candidates, needle, tolerance);
		}
	}
}
