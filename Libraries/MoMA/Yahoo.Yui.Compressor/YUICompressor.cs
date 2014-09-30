using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
namespace Yahoo.Yui.Compressor
{
	public static class YUICompressor
	{
		public static string Compress(string css, int columnWidth)
		{
			return YUICompressor.Compress(css, columnWidth, true);
		}
		public static string Compress(string css, int columnWidth, bool removeComments)
		{
			if (string.IsNullOrEmpty(css))
			{
				throw new ArgumentNullException("css");
			}
			int length = css.Length;
			int num = 0;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			if (removeComments)
			{
				while ((num = css.IndexOf("/*", num, StringComparison.OrdinalIgnoreCase)) >= 0)
				{
					int num2 = css.IndexOf("*/", num + 2, StringComparison.OrdinalIgnoreCase);
					if (num2 < 0)
					{
						num2 = length;
					}
					string value = css.Substring(num + 2, num2 - (num + 2));
					arrayList.Add(value);
					string text = css.Replace(num + 2, num2, "___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + (arrayList.Count - 1) + "___");
					num += 2;
					css = text;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			Regex regex = new Regex("(\"([^\\\\\"]|\\\\.|\\\\)*\")|('([^\\\\']|\\\\.|\\\\)*')");
			Match match = regex.Match(css);
			int index = 0;
			int count;
			while (match.Success)
			{
				string value2 = match.Groups[0].Value;
				if (!string.IsNullOrEmpty(value2))
				{
					string text2 = match.Value;
					char c = text2[0];
					text2 = text2.Substring(1, text2.Length - 2);
					if (text2.IndexOf("___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_") >= 0)
					{
						count = arrayList.Count;
						for (int i = 0; i < count; i++)
						{
							text2 = text2.Replace("___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + i + "___", arrayList[i].ToString());
						}
					}
					text2 = text2.RegexReplace("(?i)progid:DXImageTransform.Microsoft.Alpha\\(Opacity=", "alpha(opacity=");
					arrayList2.Add(text2);
					string replacement = string.Concat(new object[]
					{
						c,
						"___YUICSSMIN_PRESERVED_TOKEN_",
						arrayList2.Count - 1,
						"___",
						c
					});
					index = match.AppendReplacement(stringBuilder, css, replacement, index);
					match = match.NextMatch();
				}
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();
			count = arrayList.Count;
			for (int j = 0; j < count; j++)
			{
				string text3 = arrayList[j].ToString();
				string text4 = "___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + j + "___";
				if (text3.StartsWith("!"))
				{
					arrayList2.Add(text3);
					css = css.Replace(text4, "___YUICSSMIN_PRESERVED_TOKEN_" + (arrayList2.Count - 1) + "___");
				}
				else
				{
					if (text3.EndsWith("\\"))
					{
						arrayList2.Add("\\");
						css = css.Replace(text4, "___YUICSSMIN_PRESERVED_TOKEN_" + (arrayList2.Count - 1) + "___");
						j++;
						arrayList2.Add(string.Empty);
						css = css.Replace("___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + j + "___", "___YUICSSMIN_PRESERVED_TOKEN_" + (arrayList2.Count - 1) + "___");
					}
					else
					{
						if (text3.Length == 0)
						{
							num = css.IndexOf(text4);
							if (num > 2 && css[num - 3] == '>')
							{
								arrayList2.Add(string.Empty);
								css = css.Replace(text4, "___YUICSSMIN_PRESERVED_TOKEN_" + (arrayList2.Count - 1) + "___");
							}
						}
						css = css.Replace("/*" + text4 + "*/", string.Empty);
					}
				}
			}
			css = css.RegexReplace("\\s+", " ");
			stringBuilder = new StringBuilder();
			regex = new Regex("(^|\\})(([^\\{:])+:)+([^\\{]*\\{)");
			match = regex.Match(css);
			index = 0;
			while (match.Success)
			{
				string text5 = match.Value;
				text5 = text5.Replace(":", "___YUICSSMIN_PSEUDOCLASSCOLON___");
				text5 = text5.Replace("\\\\", "\\\\\\\\");
				text5 = text5.Replace("\\$", "\\\\\\$");
				index = match.AppendReplacement(stringBuilder, css, text5, index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();
			css = css.RegexReplace("\\s+([!{};:>+\\(\\)\\],])", "$1");
			css = css.RegexReplace("___YUICSSMIN_PSEUDOCLASSCOLON___", ":");
			css = css.RegexReplace(":first\\-(line|letter)(\\{|,)", ":first-$1 $2");
			css = css.RegexReplace("\\*/ ", "*/");
			css = css.RegexReplace("^(.*)(@charset \"[^\"]*\";)", "$2$1");
			css = css.RegexReplace("^(\\s*@charset [^;]+;\\s*)+", "$1");
			css = css.RegexReplace("\\band\\(", "and (");
			css = css.RegexReplace("([!{}:;>+\\(\\[,])\\s+", "$1");
			css = css.RegexReplace(";+}", "}");
			css = css.RegexReplace("([\\s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)", "$1$2");
			css = css.RegexReplace(":0 0 0 0(;|})", ":0$1");
			css = css.RegexReplace(":0 0 0(;|})", ":0$1");
			css = css.RegexReplace(":0 0(;|})", ":0$1");
			stringBuilder = new StringBuilder();
			regex = new Regex("(?i)(background-position|transform-origin|webkit-transform-origin|moz-transform-origin|o-transform-origin|ms-transform-origin):0(;|})");
			match = regex.Match(css);
			index = 0;
			while (match.Success)
			{
				index = match.AppendReplacement(stringBuilder, css, match.Groups[1].Value.ToLowerInvariant() + ":0 0" + match.Groups[2], index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();
			css = css.RegexReplace("(:|\\s)0+\\.(\\d+)", "$1.$2");
			stringBuilder = new StringBuilder();
			regex = new Regex("rgb\\s*\\(\\s*([0-9,\\s]+)\\s*\\)");
			match = regex.Match(css);
			index = 0;
			while (match.Success)
			{
				string[] array = match.Groups[1].Value.Split(new char[]
				{
					','
				});
				StringBuilder stringBuilder2 = new StringBuilder("#");
				string[] array2 = array;
				for (int k = 0; k < array2.Length; k++)
				{
					string s = array2[k];
					int num3;
					if (!int.TryParse(s, out num3))
					{
						num3 = 0;
					}
					if (num3 < 16)
					{
						stringBuilder2.Append("0");
					}
					stringBuilder2.Append(num3.ToHexString().ToLowerInvariant());
				}
				index = match.AppendReplacement(stringBuilder, css, stringBuilder2.ToString(), index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();
			stringBuilder = new StringBuilder();
			regex = new Regex("([^\"'=\\s])(\\s*)#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])");
			match = regex.Match(css);
			index = 0;
			while (match.Success)
			{
				if (match.Groups[3].Value.EqualsIgnoreCase(match.Groups[4].Value) && match.Groups[5].Value.EqualsIgnoreCase(match.Groups[6].Value) && match.Groups[7].Value.EqualsIgnoreCase(match.Groups[8].Value))
				{
					string replacement2 = string.Concat(new string[]
					{
						match.Groups[1].Value,
						match.Groups[2].Value,
						"#",
						match.Groups[3].Value,
						match.Groups[5].Value,
						match.Groups[7].Value
					});
					index = match.AppendReplacement(stringBuilder, css, replacement2, index);
				}
				else
				{
					index = match.AppendReplacement(stringBuilder, css, match.Value, index);
				}
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();
			stringBuilder = new StringBuilder();
			regex = new Regex("(?i)(border|border-top|border-right|border-bottom|border-right|outline|background):none(;|})");
			match = regex.Match(css);
			index = 0;
			while (match.Success)
			{
				string replacement3 = match.Groups[1].Value.ToLowerInvariant() + ":0" + match.Groups[2].Value;
				index = match.AppendReplacement(stringBuilder, css, replacement3, index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();
			css = css.RegexReplace("(?i)progid:DXImageTransform.Microsoft.Alpha\\(Opacity=", "alpha(opacity=");
			css = css.RegexReplace("[^\\}\\{/;]+\\{\\}", string.Empty);
			if (columnWidth >= 0)
			{
				int l = 0;
				int num4 = 0;
				stringBuilder = new StringBuilder(css);
				while (l < stringBuilder.Length)
				{
					char c2 = stringBuilder[l++];
					if (c2 == '}' && l - num4 > columnWidth)
					{
						stringBuilder.Insert(l, '\n');
						num4 = l;
					}
				}
				css = stringBuilder.ToString();
			}
			css = css.RegexReplace(";;+", ";");
			count = arrayList2.Count;
			for (int m = 0; m < count; m++)
			{
				css = css.Replace("___YUICSSMIN_PRESERVED_TOKEN_" + m + "___", arrayList2[m].ToString());
			}
			css = css.Trim();
			return css;
		}
	}
}
