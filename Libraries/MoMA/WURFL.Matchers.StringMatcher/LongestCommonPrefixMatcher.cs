using System;
using System.Collections.Generic;
namespace WURFL.Matchers.StringMatcher
{
	public class LongestCommonPrefixMatcher : IStringMatcher
	{
		public static readonly LongestCommonPrefixMatcher Instance = new LongestCommonPrefixMatcher();
		public string Match(ICollection<string> candidates, string needle, int tolerance)
		{
			string result = null;
			int length = needle.Length;
			IList<string> list = new List<string>(candidates);
			int num = -1;
			int num2 = -1;
			int num3 = 0;
			int num4 = list.Count - 1;
			while (num3 <= num4 && num2 < length)
			{
				int num5 = (num3 + num4) / 2;
				string text = list[num5];
				int num6 = this.LongestCommonPrefixLength(needle, text);
				if (num6 > num2)
				{
					num = num5;
					num2 = num6;
				}
				int num7 = string.Compare(text, needle, StringComparison.Ordinal);
				if (num7 < 0)
				{
					num3 = num5 + 1;
				}
				else
				{
					if (num7 <= 0)
					{
						break;
					}
					num4 = num5 - 1;
				}
			}
			if (num2 >= tolerance)
			{
				int num8 = num2;
				while (num > 0 && num8 == num2)
				{
					string stringTwo = list[num - 1];
					num8 = this.LongestCommonPrefixLength(needle, stringTwo);
					if (num8 == num2)
					{
						num--;
					}
				}
				result = list[num];
			}
			return result;
		}
		public int LongestCommonPrefixLength(string stringOne, string stringTwo)
		{
			int num = Math.Min(stringOne.Length, stringTwo.Length);
			int num2 = 0;
			while (num2 < num && stringOne[num2] == stringTwo[num2])
			{
				num2++;
			}
			return num2;
		}
	}
}
