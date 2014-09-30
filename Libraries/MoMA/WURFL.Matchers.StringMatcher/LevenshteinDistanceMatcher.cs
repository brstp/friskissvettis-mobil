using System;
using System.Collections.Generic;
namespace WURFL.Matchers.StringMatcher
{
	public class LevenshteinDistanceMatcher : IStringMatcher
	{
		public static readonly LevenshteinDistanceMatcher Instance = new LevenshteinDistanceMatcher();
		public string Match(ICollection<string> candidates, string needle, int tolerance)
		{
			string result = null;
			int num = tolerance;
			int num2 = needle.Length;
			IEnumerator<string> enumerator = candidates.GetEnumerator();
			while (enumerator.MoveNext() && num2 > 0)
			{
				string current = enumerator.Current;
				if (Math.Abs(current.Length - needle.Length) <= tolerance)
				{
					num2 = this.GetLevenshteinDistance(current, needle, tolerance);
					if (num2 < num || num2 == 0)
					{
						num = num2;
						result = current;
					}
				}
			}
			return result;
		}
		public int GetLevenshteinDistance(string s, string t, int tolerance)
		{
			if (s == null || t == null)
			{
				throw new ArgumentException("Strings must not be null");
			}
			if (tolerance == 0)
			{
				if (!s.Equals(t))
				{
					return 2147483647;
				}
				return 0;
			}
			else
			{
				int length = s.Length;
				int length2 = t.Length;
				if (length == 0)
				{
					return length2;
				}
				if (length2 == 0)
				{
					return length;
				}
				int[] array = new int[length + 1];
				int[] array2 = new int[length + 1];
				for (int i = 0; i <= length; i++)
				{
					array[i] = i;
				}
				for (int j = 1; j <= length2; j++)
				{
					char c = t[j - 1];
					array2[0] = j;
					for (int i = 1; i <= length; i++)
					{
						int num = (s[i - 1] == c) ? 0 : 1;
						array2[i] = Math.Min(Math.Min(array2[i - 1] + 1, array[i] + 1), array[i - 1] + num);
						if (i == j && array2[i] > tolerance + 3)
						{
							return 2147483647;
						}
					}
					int[] array3 = array;
					array = array2;
					array2 = array3;
				}
				return array[length];
			}
		}
	}
}
