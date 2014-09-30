using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Yahoo.Yui.Compressor
{
	public static class Extensions
	{
		public static int AppendReplacement(this Capture capture, StringBuilder value, string input, string replacement, int index)
		{
			string value2 = input.Substring(index, capture.Index - index);
			value.Append(value2);
			value.Append(replacement);
			return capture.Index + capture.Length;
		}
		public static void AppendTail(this StringBuilder value, string input, int index)
		{
			value.Append(input.Substring(index));
		}
		public static string RegexReplace(this string input, string pattern, string replacement)
		{
			return Regex.Replace(input, pattern, replacement);
		}
		public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options)
		{
			return Regex.Replace(input, pattern, replacement, options);
		}
		public static string Fill(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}
		public static string RemoveRange(this string input, int startIndex, int endIndex)
		{
			return input.Remove(startIndex, endIndex - startIndex);
		}
		public static bool EqualsIgnoreCase(this string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
		}
		public static string ToHexString(this int value)
		{
			return value.ToString("X");
		}
		public static string ToPluralString(this int value)
		{
			if (value != 1)
			{
				return "s";
			}
			return string.Empty;
		}
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
		{
			return value == null || value.Count<T>() <= 0;
		}
		public static IList<T> ToListIfNotNullOrEmpty<T>(this IList<T> value)
		{
			if (!value.IsNullOrEmpty<T>())
			{
				return value;
			}
			return null;
		}
		public static string Replace(this string value, int startIndex, int endIndex, string newContent)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			string str = value.Substring(0, startIndex);
			string str2 = value.Substring(endIndex);
			return str + newContent + str2;
		}
	}
}
