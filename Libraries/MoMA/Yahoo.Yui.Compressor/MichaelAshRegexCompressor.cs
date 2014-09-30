using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
namespace Yahoo.Yui.Compressor
{
	public class MichaelAshRegexCompressor
	{
		private static Hashtable _shortColorNames;
		private static Hashtable _shortHexColors;
		private static readonly object SyncObject = new object();
		public static string Compress(string css, int columnWidth)
		{
			return MichaelAshRegexCompressor.Compress(css, columnWidth, true);
		}
		public static string Compress(string css, int columnWidth, bool removeComments)
		{
			MichaelAshRegexCompressor.CreateHashTable();
			MatchEvaluator evaluator = new MatchEvaluator(MichaelAshRegexCompressor.RGBMatchHandler);
			MatchEvaluator evaluator2 = new MatchEvaluator(MichaelAshRegexCompressor.ShortColorNameMatchHandler);
			MatchEvaluator evaluator3 = new MatchEvaluator(MichaelAshRegexCompressor.ShortColorHexMatchHandler);
			if (removeComments)
			{
				css = MichaelAshRegexCompressor.RemoveCommentBlocks(css);
			}
			css = Regex.Replace(css, "\\s+", " ");
			css = Regex.Replace(css, "\\x22\\x5C\\x22}\\x5C\\\\x22\\x22", "___PSEUDOCLASSBMH___");
			css = Regex.Replace(css, "(?#no preceding space needed)\\s+((?:[!{};>+()\\],])|(?<={[^{}]*):(?=[^}]*}))", "$1");
			css = Regex.Replace(css, "([!{}:;>+([,])\\s+", "$1");
			css = Regex.Replace(css, "([^;}])}", "$1;}");
			css = Regex.Replace(css, "(\\d+)\\.0+(p(?:[xct])|(?:[cem])m|%|in|ex)\\b", "$1$2");
			css = Regex.Replace(css, "([\\s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)\\b", "$1$2");
			css = Regex.Replace(css, "(?<=font-weight:)normal\\b", "400");
			css = Regex.Replace(css, "(?<=font-weight:)bold\\b", "700");
			css = MichaelAshRegexCompressor.ShortHandAllProperties(css);
			css = Regex.Replace(css, ":\\s*(inherit|auto|0|(?:(?:\\d*\\.?\\d+(?:p(?:[xct])|(?:[cem])m|%|in|ex))))(\\s+\\1){1,3};", ":$1;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, ":\\s*((inherit|auto|0|(?:(?:\\d*\\.?\\d+(?:p(?:[xct])|(?:[cem])m|%|in|ex))))\\s+(inherit|auto|0|(?:(?:\\d?\\.?\\d(?:p(?:[xct])|(?:[cem])m|%|in|ex)))))\\s+\\2\\s+\\3;", ":$1;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, ":\\s*((?:(?:inherit|auto|0|(?:(?:\\d*\\.?\\d+(?:p(?:[xct])|(?:[cem])m|%|in|ex))))\\s+)?(inherit|auto|0|(?:(?:\\d?\\.?\\d(?:p(?:[xct])|(?:[cem])m|%|in|ex))))\\s+(?:0|(?:(?:\\d?\\.?\\d(?:p(?:[xct])|(?:[cem])m|%|in|ex)))))\\s+\\2;", ":$1;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "background-position:0;", "background-position:0 0;");
			css = Regex.Replace(css, "(:|\\s)0+\\.(\\d+)", "$1.$2");
			css = Regex.Replace(css, "(outline|border)-style\\s*:\\s*(none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset)(?:\\s+\\2){1,3};", "$1-style:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(outline|border)-style\\s*:\\s*((none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset)\\s+(none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset ))(?:\\s+\\3)(?:\\s+\\4);", "$1-style:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(outline|border)-style\\s*:\\s*((?:(?:none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset)\\s+)?(none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset )\\s+(?:none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset ))(?:\\s+\\3);", "$1-style:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(outline|border)-style\\s*:\\s*((none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset)\\s+(?:none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset ))(?:\\s+\\3);", "$1-style:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(outline|border)-color\\s*:\\s*((?:\\#(?:[0-9A-F]{3}){1,2})|\\S+)(?:\\s+\\2){1,3};", "$1-color:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(outline|border)-color\\s*:\\s*(((?:\\#(?:[0-9A-F]{3}){1,2})|\\S+)\\s+((?:\\#(?:[0-9A-F]{3}){1,2})|\\S+))(?:\\s+\\3)(?:\\s+\\4);", "$1-color:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(outline|border)-color\\s*:\\s*((?:(?:(?:\\#(?:[0-9A-F]{3}){1,2})|\\S+)\\s+)?((?:\\#(?:[0-9A-F]{3}){1,2})|\\S+)\\s+(?:(?:\\#(?:[0-9A-F]{3}){1,2})|\\S+))(?:\\s+\\3);", "$1-color:$2;", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "rgb\\s*\\x28((?:25[0-5])|(?:2[0-4]\\d)|(?:[01]?\\d?\\d))\\s*,\\s*((?:25[0-5])|(?:2[0-4]\\d)|(?:[01]?\\d?\\d))\\s*,\\s*((?:25[0-5])|(?:2[0-4]\\d)|(?:[01]?\\d?\\d))\\s*\\x29", evaluator);
			css = Regex.Replace(css, "(?<![\\x22\\x27=]\\s*)\\#(?:([0-9A-F])\\1)(?:([0-9A-F])\\2)(?:([0-9A-F])\\3)", "#$1$2$3", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(?<=color\\s*:\\s*.*)\\#(?<hex>f00)\\b", "red", RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(?<=color\\s*:\\s*.*)\\#(?<hex>[0-9a-f]{6})", evaluator2, RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "(?<=color\\s*:\\s*)\\b(Black|Fuchsia|LightSlateGr[ae]y|Magenta|White|Yellow)\\b", evaluator3, RegexOptions.IgnoreCase);
			css = Regex.Replace(css, "[^}]+{;}", "");
			css = Regex.Replace(css, ";(})", "$1");
			if (columnWidth > 0)
			{
				css = MichaelAshRegexCompressor.BreakLines(css, columnWidth);
			}
			return css;
		}
		private static string RemoveCommentBlocks(string input)
		{
			int i = 0;
			bool flag = false;
			for (i = input.IndexOf("/*", i); i >= 0; i = input.IndexOf("/*", i))
			{
				int num = input.IndexOf("*/", i + 2);
				if (num >= i + 2)
				{
					if (input[num - 1] == '\\')
					{
						i = num + 2;
						flag = true;
					}
					else
					{
						if (flag)
						{
							i = num + 2;
							flag = false;
						}
						else
						{
							input = input.Remove(i, num + 2 - i);
						}
					}
				}
			}
			return input;
		}
		private static string RGBMatchHandler(Match m)
		{
			StringBuilder stringBuilder = new StringBuilder("#");
			for (int i = 1; i <= 3; i++)
			{
				stringBuilder.Append(int.Parse(m.Groups[i].Value).ToString("x2"));
			}
			return stringBuilder.ToString();
		}
		private static string BreakLines(string css, int columnWidth)
		{
			int i = 0;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(css);
			while (i < stringBuilder.Length)
			{
				char c = stringBuilder[i++];
				if (c == '}' && i - num > columnWidth)
				{
					stringBuilder.Insert(i, Environment.NewLine);
					num = i;
				}
			}
			return stringBuilder.ToString();
		}
		private static string ReplaceNonEmpty(string inputText, string replacementText)
		{
			if (replacementText.Trim() != string.Empty)
			{
				inputText = string.Format(" {0}", replacementText);
			}
			return inputText;
		}
		private static string ShortColorNameMatchHandler(Match m)
		{
			string result = m.Value;
			if (MichaelAshRegexCompressor._shortColorNames.ContainsKey(m.Groups["hex"].Value))
			{
				result = MichaelAshRegexCompressor._shortColorNames[m.Groups["hex"].Value].ToString();
			}
			return result;
		}
		private static string ShortColorHexMatchHandler(Match m)
		{
			return MichaelAshRegexCompressor._shortHexColors[m.Value.ToLower()].ToString();
		}
		private static void CreateHashTable()
		{
			if (MichaelAshRegexCompressor._shortColorNames == null)
			{
				object syncObject;
				Monitor.Enter(syncObject = MichaelAshRegexCompressor.SyncObject);
				try
				{
					if (MichaelAshRegexCompressor._shortColorNames == null)
					{
						MichaelAshRegexCompressor._shortColorNames = new Hashtable
						{

							{
								"F0FFFF".ToLower(),
								"Azure".ToLower()
							},

							{
								"F5F5DC".ToLower(),
								"Beige".ToLower()
							},

							{
								"FFE4C4".ToLower(),
								"Bisque".ToLower()
							},

							{
								"A52A2A".ToLower(),
								"Brown".ToLower()
							},

							{
								"FF7F50".ToLower(),
								"Coral".ToLower()
							},

							{
								"FFD700".ToLower(),
								"Gold".ToLower()
							},

							{
								"808080".ToLower(),
								"Grey".ToLower()
							},

							{
								"008000".ToLower(),
								"Green".ToLower()
							},

							{
								"4B0082".ToLower(),
								"Indigo".ToLower()
							},

							{
								"FFFFF0".ToLower(),
								"Ivory".ToLower()
							},

							{
								"F0E68C".ToLower(),
								"Khaki".ToLower()
							},

							{
								"FAF0E6".ToLower(),
								"Linen".ToLower()
							},

							{
								"800000".ToLower(),
								"Maroon".ToLower()
							},

							{
								"000080".ToLower(),
								"Navy".ToLower()
							},

							{
								"808000".ToLower(),
								"Olive".ToLower()
							},

							{
								"FFA500".ToLower(),
								"Orange".ToLower()
							},

							{
								"DA70D6".ToLower(),
								"Orchid".ToLower()
							},

							{
								"CD853F".ToLower(),
								"Peru".ToLower()
							},

							{
								"FFC0CB".ToLower(),
								"Pink".ToLower()
							},

							{
								"DDA0DD".ToLower(),
								"Plum".ToLower()
							},

							{
								"800080".ToLower(),
								"Purple".ToLower()
							},

							{
								"FA8072".ToLower(),
								"Salmon".ToLower()
							},

							{
								"A0522D".ToLower(),
								"Sienna".ToLower()
							},

							{
								"C0C0C0".ToLower(),
								"Silver".ToLower()
							},

							{
								"FFFAFA".ToLower(),
								"Snow".ToLower()
							},

							{
								"D2B48C".ToLower(),
								"Tan".ToLower()
							},

							{
								"008080".ToLower(),
								"Teal".ToLower()
							},

							{
								"FF6347".ToLower(),
								"Tomato".ToLower()
							},

							{
								"EE82EE".ToLower(),
								"Violet".ToLower()
							},

							{
								"F5DEB3".ToLower(),
								"Wheat".ToLower()
							}
						};
					}
				}
				finally
				{
					Monitor.Exit(syncObject);
				}
			}
			if (MichaelAshRegexCompressor._shortHexColors == null)
			{
				object syncObject2;
				Monitor.Enter(syncObject2 = MichaelAshRegexCompressor.SyncObject);
				try
				{
					if (MichaelAshRegexCompressor._shortHexColors == null)
					{
						MichaelAshRegexCompressor._shortHexColors = new Hashtable
						{

							{
								"black",
								"#000"
							},

							{
								"fuchsia",
								"#f0f"
							},

							{
								"lightSlategray",
								"#789"
							},

							{
								"lightSlategrey",
								"#789"
							},

							{
								"magenta",
								"#f0f"
							},

							{
								"white",
								"#fff"
							},

							{
								"yellow",
								"#ff0"
							}
						};
					}
				}
				finally
				{
					Monitor.Exit(syncObject2);
				}
			}
		}
		private static string ShortHandAllProperties(string css)
		{
			Regex regex = new Regex("{[^{}]*}");
			Regex re = new Regex("(?<fullProperty>(?:(?<property>padding)-(?<position>top|right|bottom|left)))\\s*:\\s*(?<unit>[\\w.]+);?", RegexOptions.IgnoreCase);
			Regex re2 = new Regex("(?<fullProperty>(?:(?<property>margin)-(?<position>top|right|bottom|left)))\\s*:\\s*(?<unit>[\\w.]+);?", RegexOptions.IgnoreCase);
			Regex re3 = new Regex("(?<fullProperty>(?<property>border)-(?<position>top|right|bottom|left)(?<property2>-(?:color)))\\s*:\\s*(?<unit>[#\\w.]+);?", RegexOptions.IgnoreCase);
			Regex re4 = new Regex("(?<fullProperty>(?<property>border)-(?<position>top|right|bottom|left)(?<property2>-(?:style)))\\s*:\\s*(?<unit>none|hidden|d(?:otted|ashed|ouble)|solid|groove|ridge|inset|outset);?", RegexOptions.IgnoreCase);
			Regex re5 = new Regex("(?<fullProperty>(?<property>border)-(?<position>top|right|bottom|left)(?<property2>-(?:width)))\\s*:\\s*(?<unit>[\\w.]+);?", RegexOptions.IgnoreCase);
			Regex re6 = new Regex("list-style-(?<style>type|image|position)\\s*:\\s*(?<unit>[^};]+);?", RegexOptions.IgnoreCase);
			Regex re7 = new Regex("font-(?:(?:(?<fontProperty>family\\b)\\s*:\\s*(?<fontPropertyValue>(?:\\b[a-zA-Z]+(-[a-zA-Z]+)?\\b|\\x22[^\\x22]+\\x22)(?:\\s*,\\s*(?:\\b[a-zA-Z]+(-[a-zA-Z]+)?\\b|\\x22[^\\x22]+\\x22))*)\\b)|(?:(?<fontProperty>style\\b)\\s*:\\s*(?<fontPropertyValue>normal|italic|oblique|inherit))|(?:(?<fontProperty>variant\\b)\\s*:\\s*(?<fontPropertyValue>normal|small-caps|inherit))|(?:(?<fontProperty>weight\\b)\\s*:\\s*(?<fontPropertyValue>normal|bold|(?:bold|light)er|[1-9]00|inherit))|(?:(?<fontProperty>size\\b)\\s*:\\s*(?<fontPropertyValue>(?:(?:xx?-)?(?:small|large))|medium|(?:\\d*\\.?\\d+(?:%|(p(?:[xct])|(?:[cem])m|in|ex))\\b)|inherit|\\b0\\b)))\\s*;?", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			Regex re8 = new Regex("background-(?:(?:(?<property>color)\\s*:\\s*(?<unit>transparent|inherit|(?:(?:\\#(?:[0-9A-F]{3}){1,2})|\\S+)))|(?:(?<property>image)\\s*:\\s*(?<unit>none|inherit|(?:url\\s*\\([^()]+\\))))|(?:(?<property>repeat)\\s*:\\s*(?<unit>no-repeat|inherit|repeat(?:-[xy])))|(?:(?<property>attachment)\\s*:\\s*(?<unit>scroll|inherit|fixed))|(?:(?<property>position)\\s*:\\s*(?<unit>((?<horizontal>left | center | right|(?:0|(?:(?:\\d*\\.?\\d+(?:p(?:[xct])|(?:[cem])m|%|in|ex)))))\\s+(?<vertical>top | center | bottom |(?:0|(?:(?:\\d*\\.?\\d+(?:p(?:[xct])|(?:[cem])m|%|in|ex))))))|((?<vertical>top | center | bottom )\\s+(?<horizontal>left | center | right ))|((?<horizontal>left | center | right )|(?<vertical>top | center | bottom )))));?", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
			MatchCollection matchCollection = regex.Matches(css);
			foreach (Match match in matchCollection)
			{
				string value = match.Value;
				MichaelAshRegexCompressor.HasAllPositions(re, ref value);
				MichaelAshRegexCompressor.HasAllPositions(re2, ref value);
				MichaelAshRegexCompressor.HasAllPositions(re3, ref value);
				MichaelAshRegexCompressor.HasAllPositions(re4, ref value);
				MichaelAshRegexCompressor.HasAllPositions(re5, ref value);
				MichaelAshRegexCompressor.HasAllListStyle(re6, ref value);
				MichaelAshRegexCompressor.HasAllFontProperties(re7, ref value);
				MichaelAshRegexCompressor.HasAllBackGroundProperties(re8, ref value);
				css = css.Replace(match.Value, value);
			}
			return css;
		}
		private static void HasAllBackGroundProperties(Regex re, ref string CSSText)
		{
			MatchCollection matchCollection = re.Matches(CSSText);
			int num = 5;
			if (matchCollection.Count == num)
			{
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					string value;
					if ((value = matchCollection[i].Groups["property"].Value) != null)
					{
						if (!(value == "color"))
						{
							if (!(value == "image"))
							{
								if (!(value == "repeat"))
								{
									if (!(value == "attachment"))
									{
										if (value == "position")
										{
											num2 += 16;
										}
									}
									else
									{
										num2 += 8;
									}
								}
								else
								{
									num2 += 4;
								}
							}
							else
							{
								num2 += 2;
							}
						}
						else
						{
							num2++;
						}
					}
				}
				if (num2 == 31)
				{
					CSSText = MichaelAshRegexCompressor.ShortHandBackGroundReplaceV2(matchCollection, re, CSSText);
				}
			}
		}
		private static void HasAllFontProperties(Regex re, ref string CSSText)
		{
			MatchCollection matchCollection = re.Matches(CSSText);
			int num = 5;
			if (matchCollection.Count == num)
			{
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					string value;
					if ((value = matchCollection[i].Groups["fontProperty"].Value) != null)
					{
						if (!(value == "style"))
						{
							if (!(value == "variant"))
							{
								if (!(value == "weight"))
								{
									if (!(value == "size"))
									{
										if (value == "family")
										{
											num2 += 16;
										}
									}
									else
									{
										num2 += 8;
									}
								}
								else
								{
									num2 += 4;
								}
							}
							else
							{
								num2 += 2;
							}
						}
						else
						{
							num2++;
						}
					}
				}
				if (num2 == 31)
				{
					CSSText = MichaelAshRegexCompressor.ShortHandFontReplaceV2(matchCollection, re, CSSText);
				}
			}
		}
		private static void HasAllListStyle(Regex re, ref string CSSText)
		{
			int num = 3;
			MatchCollection matchCollection = re.Matches(CSSText);
			if (matchCollection.Count == num)
			{
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					string value;
					if ((value = matchCollection[i].Groups["style"].Value) != null)
					{
						if (!(value == "type"))
						{
							if (!(value == "image"))
							{
								if (value == "position")
								{
									num2 += 4;
								}
							}
							else
							{
								num2 += 2;
							}
						}
						else
						{
							num2++;
						}
					}
				}
				if (num2 == 7)
				{
					CSSText = MichaelAshRegexCompressor.ShortHandListReplaceV2(matchCollection, re, CSSText);
				}
			}
		}
		private static void HasAllPositions(Regex re, ref string CSSText)
		{
			MatchCollection matchCollection = re.Matches(CSSText);
			if (matchCollection.Count == 4)
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					string value;
					if ((value = matchCollection[i].Groups["position"].Value) != null)
					{
						if (!(value == "top"))
						{
							if (!(value == "right"))
							{
								if (!(value == "bottom"))
								{
									if (value == "left")
									{
										num += 8;
									}
								}
								else
								{
									num += 4;
								}
							}
							else
							{
								num += 2;
							}
						}
						else
						{
							num++;
						}
					}
				}
				if (num == 15)
				{
					CSSText = MichaelAshRegexCompressor.ShortHandReplaceV2(matchCollection, re, CSSText);
				}
			}
		}
		private static string ShortHandFontReplaceV2(MatchCollection mcProperySet, Regex re, string InputText)
		{
			Regex regex = new Regex("line-height\\s*:\\s*((?:\\d*\\.?\\d+(?:%|(p(?:[xct])|(?:[cem])m|in|ex)\\b)?)|normal|inherit);?", RegexOptions.IgnoreCase);
			string text = string.Empty;
			string arg_17_0 = string.Empty;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			foreach (Match match in mcProperySet)
			{
				string value;
				if ((value = match.Groups[""].Value) != null)
				{
					if (!(value == "family"))
					{
						if (!(value == "size"))
						{
							if (value == "style" || value == "variant" || value == "weight")
							{
								if (match.Groups["fontPropertyValue"].Value != "normal")
								{
									text3 += string.Format(" {0}", match.Groups["fontPropertyValue"].Value);
								}
							}
						}
						else
						{
							if (regex.IsMatch(InputText))
							{
								Match match2 = regex.Match(InputText);
								if (match2.Groups[1].Value != "normal")
								{
									text2 = string.Format("/{0}", match2.Groups[1].Value);
								}
								InputText = regex.Replace(InputText, string.Empty);
							}
							text2 = string.Format(" {0}{1}", match.Groups["fontPropertyValue"].Value, text2);
							if (text2 == "medium")
							{
								text2 = string.Empty;
							}
						}
					}
					else
					{
						text = string.Format(" {0}", match.Groups["fontPropertyValue"].Value);
					}
				}
			}
			string text4 = string.Format("{0}{1}{2};", new object[]
			{
				text3,
				empty,
				empty2,
				text2,
				text
			});
			string value2 = string.Format("font:{0}", text4.Trim());
			string text5 = re.Replace(InputText, "");
			return text5.Insert(1, value2);
		}
		private static string ShortHandBackGroundReplaceV2(MatchCollection mcProperySet, Regex re, string InputText)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			string text5 = string.Empty;
			foreach (Match match in mcProperySet)
			{
				string value;
				if ((value = match.Groups["property"].Value) != null)
				{
					if (!(value == "color"))
					{
						if (!(value == "image"))
						{
							if (!(value == "repeat"))
							{
								if (!(value == "attachment"))
								{
									if (value == "position")
									{
										if (match.Groups["unit"].Value != "0% 0%")
										{
											text5 = string.Format(" {0}", match.Groups["unit"].Value);
										}
									}
								}
								else
								{
									if (match.Groups["unit"].Value != "scroll")
									{
										text4 = string.Format(" {0}", match.Groups["unit"].Value);
									}
								}
							}
							else
							{
								if (match.Groups["unit"].Value != "repeat")
								{
									text3 = string.Format(" {0}", match.Groups["unit"].Value);
								}
							}
						}
						else
						{
							if (match.Groups["unit"].Value != "none")
							{
								text2 = string.Format(" {0}", match.Groups["unit"].Value);
							}
						}
					}
					else
					{
						if (match.Groups["unit"].Value != "transparent")
						{
							text = string.Format(" {0}", match.Groups["unit"].Value);
						}
					}
				}
			}
			string text6 = string.Format("{0}{1}{2}{3}{4};", new object[]
			{
				text,
				text2,
				text3,
				text4,
				text5
			});
			string value2 = string.Format("background:{0}", text6.Trim());
			string text7 = re.Replace(InputText, "");
			return text7.Insert(1, value2);
		}
		private static string ShortHandReplaceV2(MatchCollection mcProperySet, Regex reTRBL1, string InputText)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			string text5 = string.Format("{0}{1}", mcProperySet[0].Groups["property"].Value, mcProperySet[0].Groups["property2"].Value);
			foreach (Match match in mcProperySet)
			{
				string value;
				if ((value = match.Groups["position"].Value) != null)
				{
					if (!(value == "top"))
					{
						if (!(value == "right"))
						{
							if (!(value == "bottom"))
							{
								if (value == "left")
								{
									text4 = match.Groups["unit"].Value;
								}
							}
							else
							{
								text3 = match.Groups["unit"].Value;
							}
						}
						else
						{
							text2 = match.Groups["unit"].Value;
						}
					}
					else
					{
						text = match.Groups["unit"].Value;
					}
				}
			}
			string value2 = string.Format("{0}:{1} {2} {3} {4};", new object[]
			{
				text5,
				text,
				text2,
				text3,
				text4
			});
			string text6 = reTRBL1.Replace(InputText, "");
			return text6.Insert(1, value2);
		}
		private static string ShortHandListReplaceV2(MatchCollection mcProperySet, Regex re, string InputText)
		{
			string arg = string.Empty;
			string arg2 = string.Empty;
			string arg3 = string.Empty;
			foreach (Match match in mcProperySet)
			{
				string value;
				if ((value = match.Groups["style"].Value) != null)
				{
					if (!(value == "type"))
					{
						if (!(value == "position"))
						{
							if (value == "style")
							{
								if (match.Groups["unit"].Value != "none")
								{
									arg3 = string.Format(" {0}", match.Groups["unit"].Value);
								}
							}
						}
						else
						{
							if (match.Groups["unit"].Value != "outside")
							{
								arg2 = string.Format(" {0}", match.Groups["unit"].Value);
							}
						}
					}
					else
					{
						if (match.Groups["unit"].Value != "disc")
						{
							arg = match.Groups["unit"].Value;
						}
					}
				}
			}
			string value2 = string.Format("list-style:{0}{1}{2};", arg, arg2, arg3);
			string text = re.Replace(InputText, "");
			return text.Insert(1, value2);
		}
	}
}
