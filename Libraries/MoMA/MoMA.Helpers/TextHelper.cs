using System;
using System.Collections.Generic;
using System.Text;
namespace MoMA.Helpers
{
	public class TextHelper
	{
		public static T FirstMatchOf<T>(string searchString, Dictionary<T, string> searchItems, T defaultValue)
		{
			T result = defaultValue;
			int num = 2147483647;
			foreach (KeyValuePair<T, string> current in searchItems)
			{
				int num2 = searchString.IndexOf(current.Value);
				if (num2 >= 0 && num2 < num)
				{
					result = current.Key;
					num = num2;
				}
			}
			return result;
		}
		public static string Convert(string inputString, Encoding FromEncoding, Encoding ToEncoding)
		{
			Encoding encoding = Encoding.GetEncoding(500);
			Encoding aSCII = Encoding.ASCII;
			byte[] bytes = aSCII.GetBytes(inputString);
			byte[] bytes2 = Encoding.Convert(aSCII, encoding, bytes);
			return aSCII.GetString(bytes2);
		}
		public static string ReplaceTemplateValues(string template, Dictionary<string, string> values)
		{
			return TextHelper.ReplaceTemplateValues(template, values, '{', '}');
		}
		public static string ReplaceTemplateValues(string template, Dictionary<string, string> values, char start, char end)
		{
			foreach (KeyValuePair<string, string> current in values)
			{
				template = template.Replace(start + current.Key + end, current.Value);
			}
			return template;
		}
	}
}
