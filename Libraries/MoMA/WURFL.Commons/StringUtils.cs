using System;
namespace WURFL.Commons
{
	public static class StringUtils
	{
		public static int FirstSlash(string target)
		{
			return StringUtils.IndexOfOrLength(target, "/");
		}
		public static int SecondSlash(string target)
		{
			return StringUtils.OrdinalIndexOfOrLength(target, "/", 2);
		}
		public static int FirstSemiColon(string target)
		{
			return StringUtils.OrdinalIndexOfOrLength(target, ";", 1);
		}
		public static int FirstSpace(string target)
		{
			return StringUtils.IndexOfOrLength(target, " ");
		}
		public static int IndexOfOrLength(string target, string needle)
		{
			return StringUtils.IndexOfOrLength(target, needle, 0);
		}
		public static int IndexOfOrLength(string target, string needle, int startIndex)
		{
			return StringUtils.OrdinalIndexOfOrLength(target, needle, 1, startIndex);
		}
		public static int OrdinalIndexOfOrLength(string target, string needle, int ordinal)
		{
			return StringUtils.OrdinalIndexOfOrLength(target, needle, ordinal, 0);
		}
		public static int OrdinalIndexOfOrLength(string target, string needle, int ordinal, int startIndex)
		{
			if (startIndex >= target.Length)
			{
				return target.Length;
			}
			int num = 0;
			int num2 = startIndex;
			do
			{
				num2 = target.IndexOf(needle, num2 + 1);
				if (num2 < 0)
				{
					break;
				}
				num++;
			}
			while (num < ordinal);
			if (num2 == -1)
			{
				return target.Length;
			}
			return num2;
		}
		public static bool StartsWithAnyOf(string haystack, params string[] needles)
		{
			for (int i = 0; i < needles.Length; i++)
			{
				string value = needles[i];
				if (haystack.StartsWith(value))
				{
					return true;
				}
			}
			return false;
		}
		public static bool ContainsAnyOf(string haystack, params string[] needles)
		{
			for (int i = 0; i < needles.Length; i++)
			{
				string value = needles[i];
				if (haystack.Contains(value))
				{
					return true;
				}
			}
			return false;
		}
		public static bool ContainsAllOf(string haystack, params string[] needles)
		{
			for (int i = 0; i < needles.Length; i++)
			{
				string value = needles[i];
				if (!haystack.Contains(value))
				{
					return false;
				}
			}
			return true;
		}
	}
}
