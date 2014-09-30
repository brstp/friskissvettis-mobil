using log4net.Core;
using System;
using System.Collections;
using System.Globalization;
namespace log4net.Util
{
	public sealed class PatternParser
	{
		private sealed class StringLengthComparer : IComparer
		{
			public static readonly PatternParser.StringLengthComparer Instance = new PatternParser.StringLengthComparer();
			private StringLengthComparer()
			{
			}
			public int Compare(object x, object y)
			{
				string text = x as string;
				string text2 = y as string;
				int result;
				if (text == null && text2 == null)
				{
					result = 0;
				}
				else
				{
					if (text == null)
					{
						result = 1;
					}
					else
					{
						if (text2 == null)
						{
							result = -1;
						}
						else
						{
							result = text2.Length.CompareTo(text.Length);
						}
					}
				}
				return result;
			}
		}
		private const char ESCAPE_CHAR = '%';
		private PatternConverter m_head;
		private PatternConverter m_tail;
		private string m_pattern;
		private Hashtable m_patternConverters = new Hashtable();
		public Hashtable PatternConverters
		{
			get
			{
				return this.m_patternConverters;
			}
		}
		public PatternParser(string pattern)
		{
			this.m_pattern = pattern;
		}
		public PatternConverter Parse()
		{
			string[] matches = this.BuildCache();
			this.ParseInternal(this.m_pattern, matches);
			return this.m_head;
		}
		private string[] BuildCache()
		{
			string[] array = new string[this.m_patternConverters.Keys.Count];
			this.m_patternConverters.Keys.CopyTo(array, 0);
			Array.Sort(array, 0, array.Length, PatternParser.StringLengthComparer.Instance);
			return array;
		}
		private void ParseInternal(string pattern, string[] matches)
		{
			int i = 0;
			while (i < pattern.Length)
			{
				int num = pattern.IndexOf('%', i);
				if (num < 0 || num == pattern.Length - 1)
				{
					this.ProcessLiteral(pattern.Substring(i));
					i = pattern.Length;
				}
				else
				{
					if (pattern[num + 1] == '%')
					{
						this.ProcessLiteral(pattern.Substring(i, num - i + 1));
						i = num + 2;
					}
					else
					{
						this.ProcessLiteral(pattern.Substring(i, num - i));
						i = num + 1;
						FormattingInfo formattingInfo = new FormattingInfo();
						if (i < pattern.Length)
						{
							if (pattern[i] == '-')
							{
								formattingInfo.LeftAlign = true;
								i++;
							}
						}
						while (i < pattern.Length && char.IsDigit(pattern[i]))
						{
							if (formattingInfo.Min < 0)
							{
								formattingInfo.Min = 0;
							}
							formattingInfo.Min = formattingInfo.Min * 10 + int.Parse(pattern[i].ToString(CultureInfo.InvariantCulture), NumberFormatInfo.InvariantInfo);
							i++;
						}
						if (i < pattern.Length)
						{
							if (pattern[i] == '.')
							{
								i++;
							}
						}
						while (i < pattern.Length && char.IsDigit(pattern[i]))
						{
							if (formattingInfo.Max == 2147483647)
							{
								formattingInfo.Max = 0;
							}
							formattingInfo.Max = formattingInfo.Max * 10 + int.Parse(pattern[i].ToString(CultureInfo.InvariantCulture), NumberFormatInfo.InvariantInfo);
							i++;
						}
						int num2 = pattern.Length - i;
						for (int j = 0; j < matches.Length; j++)
						{
							if (matches[j].Length <= num2)
							{
								if (string.Compare(pattern, i, matches[j], 0, matches[j].Length, false, CultureInfo.InvariantCulture) == 0)
								{
									i += matches[j].Length;
									string option = null;
									if (i < pattern.Length)
									{
										if (pattern[i] == '{')
										{
											i++;
											int num3 = pattern.IndexOf('}', i);
											if (num3 >= 0)
											{
												option = pattern.Substring(i, num3 - i);
												i = num3 + 1;
											}
										}
									}
									this.ProcessConverter(matches[j], option, formattingInfo);
									break;
								}
							}
						}
					}
				}
			}
		}
		private void ProcessLiteral(string text)
		{
			if (text.Length > 0)
			{
				this.ProcessConverter("literal", text, new FormattingInfo());
			}
		}
		private void ProcessConverter(string converterName, string option, FormattingInfo formattingInfo)
		{
			LogLog.Debug(string.Concat(new object[]
			{
				"PatternParser: Converter [",
				converterName,
				"] Option [",
				option,
				"] Format [min=",
				formattingInfo.Min,
				",max=",
				formattingInfo.Max,
				",leftAlign=",
				formattingInfo.LeftAlign,
				"]"
			}));
			Type type = (Type)this.m_patternConverters[converterName];
			if (type == null)
			{
				LogLog.Error("PatternParser: Unknown converter name [" + converterName + "] in conversion pattern.");
			}
			else
			{
				PatternConverter patternConverter = null;
				try
				{
					patternConverter = (PatternConverter)Activator.CreateInstance(type);
				}
				catch (Exception ex)
				{
					LogLog.Error("PatternParser: Failed to create instance of Type [" + type.FullName + "] using default constructor. Exception: " + ex.ToString());
				}
				patternConverter.FormattingInfo = formattingInfo;
				patternConverter.Option = option;
				IOptionHandler optionHandler = patternConverter as IOptionHandler;
				if (optionHandler != null)
				{
					optionHandler.ActivateOptions();
				}
				this.AddConverter(patternConverter);
			}
		}
		private void AddConverter(PatternConverter pc)
		{
			if (this.m_head == null)
			{
				this.m_tail = pc;
				this.m_head = pc;
			}
			else
			{
				this.m_tail = this.m_tail.SetNext(pc);
			}
		}
	}
}
