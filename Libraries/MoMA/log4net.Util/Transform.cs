using System;
using System.Text.RegularExpressions;
using System.Xml;
namespace log4net.Util
{
	public sealed class Transform
	{
		private const string CDATA_END = "]]>";
		private const string CDATA_UNESCAPABLE_TOKEN = "]]";
		private static Regex INVALIDCHARS = new Regex("[^\\x09\\x0A\\x0D\\x20-\\xFF\\u00FF-\\u07FF\\uE000-\\uFFFD]", RegexOptions.Compiled);
		private Transform()
		{
		}
		public static void WriteEscapedXmlString(XmlWriter writer, string textData, string invalidCharReplacement)
		{
			string text = Transform.MaskXmlInvalidCharacters(textData, invalidCharReplacement);
			int num = 12 * (1 + Transform.CountSubstrings(text, "]]>"));
			int num2 = 3 * (Transform.CountSubstrings(text, "<") + Transform.CountSubstrings(text, ">")) + 4 * Transform.CountSubstrings(text, "&");
			if (num2 <= num)
			{
				writer.WriteString(text);
			}
			else
			{
				int i = text.IndexOf("]]>");
				if (i < 0)
				{
					writer.WriteCData(text);
				}
				else
				{
					int num3 = 0;
					while (i > -1)
					{
						writer.WriteCData(text.Substring(num3, i - num3));
						if (i == text.Length - 3)
						{
							num3 = text.Length;
							writer.WriteString("]]>");
							break;
						}
						writer.WriteString("]]");
						num3 = i + 2;
						i = text.IndexOf("]]>", num3);
					}
					if (num3 < text.Length)
					{
						writer.WriteCData(text.Substring(num3));
					}
				}
			}
		}
		public static string MaskXmlInvalidCharacters(string textData, string mask)
		{
			return Transform.INVALIDCHARS.Replace(textData, mask);
		}
		private static int CountSubstrings(string text, string substring)
		{
			int num = 0;
			int i = 0;
			int length = text.Length;
			int length2 = substring.Length;
			int result;
			if (length == 0)
			{
				result = 0;
			}
			else
			{
				if (length2 == 0)
				{
					result = 0;
				}
				else
				{
					while (i < length)
					{
						int num2 = text.IndexOf(substring, i);
						if (num2 == -1)
						{
							break;
						}
						num++;
						i = num2 + length2;
					}
					result = num;
				}
			}
			return result;
		}
	}
}
