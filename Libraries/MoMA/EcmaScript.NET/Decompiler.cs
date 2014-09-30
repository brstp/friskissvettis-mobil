using EcmaScript.NET.Collections;
using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Decompiler
	{
		public const int ONLY_BODY_FLAG = 1;
		public const int TO_SOURCE_FLAG = 2;
		public const int TO_STRING_FLAG = 4;
		public const int INITIAL_INDENT_PROP = 1;
		public const int INDENT_GAP_PROP = 2;
		public const int CASE_GAP_PROP = 3;
		private const int FUNCTION_END = 147;
		private char[] sourceBuffer = new char[128];
		private int sourceTop;
		private static bool printSource = false;
		internal string EncodedSource
		{
			get
			{
				return this.SourceToString(0);
			}
		}
		internal int CurrentOffset
		{
			get
			{
				return this.sourceTop;
			}
		}
		internal int MarkFunctionStart(int functionType)
		{
			int currentOffset = this.CurrentOffset;
			this.AddToken(107);
			this.Append((char)functionType);
			return currentOffset;
		}
		internal int MarkFunctionEnd(int functionStart)
		{
			int currentOffset = this.CurrentOffset;
			this.Append('\u0093');
			return currentOffset;
		}
		internal void AddToken(int token)
		{
			if (0 > token || token > 157)
			{
				throw new ArgumentException();
			}
			this.Append((char)token);
		}
		internal void AddEol(int token)
		{
			if (0 > token || token > 157)
			{
				throw new ArgumentException();
			}
			this.Append((char)token);
			this.Append('\u0001');
		}
		internal void AddName(string str)
		{
			this.AddToken(38);
			this.AppendString(str);
		}
		internal void AddString(string str)
		{
			this.AddToken(40);
			this.AppendString(str);
		}
		internal void AddRegexp(string regexp, string flags)
		{
			this.AddToken(47);
			this.AppendString(string.Concat(new object[]
			{
				'/',
				regexp,
				'/',
				flags
			}));
		}
		internal void AddJScriptConditionalComment(string str)
		{
			this.AddToken(154);
			this.AppendString(str);
		}
		internal void AddPreservedComment(string str)
		{
			this.AddToken(155);
			this.AppendString(str);
		}
		internal void AddNumber(double n)
		{
			this.AddToken(39);
			long num = (long)n;
			if ((double)num != n)
			{
				num = BitConverter.DoubleToInt64Bits(n);
				this.Append('D');
				this.Append((char)(num >> 48));
				this.Append((char)(num >> 32));
				this.Append((char)(num >> 16));
				this.Append((char)num);
			}
			else
			{
				if (num < 0L)
				{
					Context.CodeBug();
				}
				if (num <= 65535L)
				{
					this.Append('S');
					this.Append((char)num);
				}
				else
				{
					this.Append('J');
					this.Append((char)(num >> 48));
					this.Append((char)(num >> 32));
					this.Append((char)(num >> 16));
					this.Append((char)num);
				}
			}
		}
		private void AppendString(string str)
		{
			int length = str.Length;
			int num = 1;
			if (length >= 32768)
			{
				num = 2;
			}
			int num2 = this.sourceTop + num + length;
			if (num2 > this.sourceBuffer.Length)
			{
				this.IncreaseSourceCapacity(num2);
			}
			if (length >= 32768)
			{
				this.sourceBuffer[this.sourceTop] = (char)(32768u | (uint)length >> 16);
				this.sourceTop++;
			}
			this.sourceBuffer[this.sourceTop] = (char)length;
			this.sourceTop++;
			str.ToCharArray(0, length).CopyTo(this.sourceBuffer, this.sourceTop);
			this.sourceTop = num2;
		}
		private void Append(char c)
		{
			if (this.sourceTop == this.sourceBuffer.Length)
			{
				this.IncreaseSourceCapacity(this.sourceTop + 1);
			}
			this.sourceBuffer[this.sourceTop] = c;
			this.sourceTop++;
		}
		private void IncreaseSourceCapacity(int minimalCapacity)
		{
			if (minimalCapacity <= this.sourceBuffer.Length)
			{
				Context.CodeBug();
			}
			int num = this.sourceBuffer.Length * 2;
			if (num < minimalCapacity)
			{
				num = minimalCapacity;
			}
			char[] destinationArray = new char[num];
			Array.Copy(this.sourceBuffer, 0, destinationArray, 0, this.sourceTop);
			this.sourceBuffer = destinationArray;
		}
		private string SourceToString(int offset)
		{
			if (offset < 0 || this.sourceTop < offset)
			{
				Context.CodeBug();
			}
			return new string(this.sourceBuffer, offset, this.sourceTop - offset);
		}
		public static string Decompile(string source, int flags, UintMap properties)
		{
			int length = source.Length;
			string result;
			if (length == 0)
			{
				result = "";
			}
			else
			{
				int num = properties.getInt(1, 0);
				if (num < 0)
				{
					throw new ArgumentException();
				}
				int @int = properties.getInt(2, 4);
				if (@int < 0)
				{
					throw new ArgumentException();
				}
				int int2 = properties.getInt(3, 2);
				if (int2 < 0)
				{
					throw new ArgumentException();
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = 0 != (flags & 1);
				bool flag2 = 0 != (flags & 2);
				bool flag3 = 0 != (flags & 4);
				if (Decompiler.printSource)
				{
					Console.Error.WriteLine("length:" + length);
					for (int i = 0; i < length; i++)
					{
						string text = null;
						if (Token.printNames)
						{
							text = Token.name((int)source[i]);
						}
						if (text == null)
						{
							text = "---";
						}
						string text2 = (text.Length > 7) ? "\t" : "\t\t";
						Console.Error.WriteLine(string.Concat(new object[]
						{
							text,
							text2,
							(int)source[i],
							"\t'",
							ScriptRuntime.escapeString(source.Substring(i, i + 1 - i)),
							"'"
						}));
					}
					Console.Error.WriteLine();
				}
				int num2 = 0;
				bool flag4 = false;
				int j = 0;
				int num3;
				if (source[j] == '\u0086')
				{
					j++;
					num3 = -1;
				}
				else
				{
					num3 = (int)source[j + 1];
				}
				if (!flag2)
				{
					if (!flag3)
					{
						stringBuilder.Append('\n');
					}
					for (int k = 0; k < num; k++)
					{
						stringBuilder.Append(' ');
					}
				}
				else
				{
					if (num3 == 2)
					{
						stringBuilder.Append('(');
					}
				}
				while (j < length)
				{
					switch (source[j])
					{
					case '\u0001':
						if (!flag2)
						{
							bool flag5 = true;
							if (!flag4)
							{
								flag4 = true;
								if (flag)
								{
									stringBuilder.Length = 0;
									num -= @int;
									flag5 = false;
								}
							}
							if (flag5)
							{
								stringBuilder.Append('\n');
							}
							if (j + 1 < length)
							{
								int l = 0;
								int num4 = (int)source[j + 1];
								if (num4 == 113 || num4 == 114)
								{
									l = @int - int2;
								}
								else
								{
									if (num4 == 84)
									{
										l = @int;
									}
									else
									{
										if (num4 == 38)
										{
											int sourceStringEnd = Decompiler.GetSourceStringEnd(source, j + 2);
											if (source[sourceStringEnd] == 'e')
											{
												l = @int;
											}
										}
									}
								}
								while (l < num)
								{
									stringBuilder.Append(' ');
									l++;
								}
							}
						}
						break;
					case '\u0002':
					case '\u0003':
					case '\u0005':
					case '\u0006':
					case '\a':
					case '\b':
					case '!':
					case '"':
					case '#':
					case '$':
					case '%':
					case '0':
					case '2':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case ':':
					case ';':
					case '<':
					case '=':
					case '>':
					case '?':
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'G':
					case 'H':
					case 'I':
					case 'J':
					case 'K':
					case 'L':
					case 'M':
					case 'N':
					case 'l':
					case 'm':
					case '}':
					case '~':
					case '\u007f':
					case '\u0080':
					case '\u0081':
					case '\u0082':
					case '\u0083':
					case '\u0084':
					case '\u0085':
					case '\u0086':
					case '\u0087':
					case '\u0088':
					case '\u0089':
					case '\u008a':
					case '\u008b':
					case '\u008c':
					case '\u008f':
					case '\u0092':
						goto IL_D40;
					case '\u0004':
						stringBuilder.Append("return");
						if (80 != Decompiler.GetNext(source, length, j))
						{
							stringBuilder.Append(' ');
						}
						break;
					case '\t':
						stringBuilder.Append(" | ");
						break;
					case '\n':
						stringBuilder.Append(" ^ ");
						break;
					case '\v':
						stringBuilder.Append(" & ");
						break;
					case '\f':
						stringBuilder.Append(" == ");
						break;
					case '\r':
						stringBuilder.Append(" != ");
						break;
					case '\u000e':
						stringBuilder.Append(" < ");
						break;
					case '\u000f':
						stringBuilder.Append(" <= ");
						break;
					case '\u0010':
						stringBuilder.Append(" > ");
						break;
					case '\u0011':
						stringBuilder.Append(" >= ");
						break;
					case '\u0012':
						stringBuilder.Append(" << ");
						break;
					case '\u0013':
						stringBuilder.Append(" >> ");
						break;
					case '\u0014':
						stringBuilder.Append(" >>> ");
						break;
					case '\u0015':
						stringBuilder.Append(" + ");
						break;
					case '\u0016':
						stringBuilder.Append(" - ");
						break;
					case '\u0017':
						stringBuilder.Append(" * ");
						break;
					case '\u0018':
						stringBuilder.Append(" / ");
						break;
					case '\u0019':
						stringBuilder.Append(" % ");
						break;
					case '\u001a':
						stringBuilder.Append('!');
						break;
					case '\u001b':
						stringBuilder.Append('~');
						break;
					case '\u001c':
						stringBuilder.Append('+');
						break;
					case '\u001d':
						stringBuilder.Append('-');
						break;
					case '\u001e':
						stringBuilder.Append("new ");
						break;
					case '\u001f':
						stringBuilder.Append("delete ");
						break;
					case ' ':
						stringBuilder.Append("typeof ");
						break;
					case '&':
					case '/':
						j = Decompiler.PrintSourceString(source, j + 1, false, stringBuilder);
						continue;
					case '\'':
						j = Decompiler.PrintSourceNumber(source, j + 1, stringBuilder);
						continue;
					case '(':
						j = Decompiler.PrintSourceString(source, j + 1, true, stringBuilder);
						continue;
					case ')':
						stringBuilder.Append("null");
						break;
					case '*':
						stringBuilder.Append("this");
						break;
					case '+':
						stringBuilder.Append("false");
						break;
					case ',':
						stringBuilder.Append("true");
						break;
					case '-':
						stringBuilder.Append(" === ");
						break;
					case '.':
						stringBuilder.Append(" !== ");
						break;
					case '1':
						stringBuilder.Append("throw ");
						break;
					case '3':
						stringBuilder.Append(" in ");
						break;
					case '4':
						stringBuilder.Append(" instanceof ");
						break;
					case '@':
						stringBuilder.Append(':');
						break;
					case 'O':
						stringBuilder.Append("try ");
						break;
					case 'P':
						stringBuilder.Append(';');
						if (1 != Decompiler.GetNext(source, length, j))
						{
							stringBuilder.Append(' ');
						}
						break;
					case 'Q':
						stringBuilder.Append('[');
						break;
					case 'R':
						stringBuilder.Append(']');
						break;
					case 'S':
						num2++;
						if (1 == Decompiler.GetNext(source, length, j))
						{
							num += @int;
						}
						stringBuilder.Append('{');
						break;
					case 'T':
						num2--;
						if (!flag || num2 != 0)
						{
							stringBuilder.Append('}');
							int next = Decompiler.GetNext(source, length, j);
							if (next <= 111)
							{
								if (next == 1)
								{
									goto IL_617;
								}
								if (next == 111)
								{
									goto IL_621;
								}
							}
							else
							{
								if (next == 115)
								{
									goto IL_621;
								}
								if (next == 147)
								{
									goto IL_617;
								}
							}
							break;
							IL_621:
							num -= @int;
							stringBuilder.Append(' ');
							break;
							IL_617:
							num -= @int;
						}
						break;
					case 'U':
						stringBuilder.Append('(');
						break;
					case 'V':
						stringBuilder.Append(')');
						if (83 == Decompiler.GetNext(source, length, j))
						{
							stringBuilder.Append(' ');
						}
						break;
					case 'W':
						stringBuilder.Append(", ");
						break;
					case 'X':
						stringBuilder.Append(" = ");
						break;
					case 'Y':
						stringBuilder.Append(" |= ");
						break;
					case 'Z':
						stringBuilder.Append(" ^= ");
						break;
					case '[':
						stringBuilder.Append(" &= ");
						break;
					case '\\':
						stringBuilder.Append(" <<= ");
						break;
					case ']':
						stringBuilder.Append(" >>= ");
						break;
					case '^':
						stringBuilder.Append(" >>>= ");
						break;
					case '_':
						stringBuilder.Append(" += ");
						break;
					case '`':
						stringBuilder.Append(" -= ");
						break;
					case 'a':
						stringBuilder.Append(" *= ");
						break;
					case 'b':
						stringBuilder.Append(" /= ");
						break;
					case 'c':
						stringBuilder.Append(" %= ");
						break;
					case 'd':
						stringBuilder.Append(" ? ");
						break;
					case 'e':
						if (1 == Decompiler.GetNext(source, length, j))
						{
							stringBuilder.Append(':');
						}
						else
						{
							stringBuilder.Append(" : ");
						}
						break;
					case 'f':
						stringBuilder.Append(" || ");
						break;
					case 'g':
						stringBuilder.Append(" && ");
						break;
					case 'h':
						stringBuilder.Append("++");
						break;
					case 'i':
						stringBuilder.Append("--");
						break;
					case 'j':
						stringBuilder.Append('.');
						break;
					case 'k':
						j++;
						stringBuilder.Append("function ");
						break;
					case 'n':
						stringBuilder.Append("if ");
						break;
					case 'o':
						stringBuilder.Append("else ");
						break;
					case 'p':
						stringBuilder.Append("switch ");
						break;
					case 'q':
						stringBuilder.Append("case ");
						break;
					case 'r':
						stringBuilder.Append("default");
						break;
					case 's':
						stringBuilder.Append("while ");
						break;
					case 't':
						stringBuilder.Append("do ");
						break;
					case 'u':
						stringBuilder.Append("for ");
						break;
					case 'v':
						stringBuilder.Append("break");
						if (38 == Decompiler.GetNext(source, length, j))
						{
							stringBuilder.Append(' ');
						}
						break;
					case 'w':
						stringBuilder.Append("continue");
						if (38 == Decompiler.GetNext(source, length, j))
						{
							stringBuilder.Append(' ');
						}
						break;
					case 'x':
						stringBuilder.Append("var ");
						break;
					case 'y':
						stringBuilder.Append("with ");
						break;
					case 'z':
						stringBuilder.Append("catch ");
						break;
					case '{':
						stringBuilder.Append("finally ");
						break;
					case '|':
						stringBuilder.Append("void ");
						break;
					case '\u008d':
						stringBuilder.Append("..");
						break;
					case '\u008e':
						stringBuilder.Append("::");
						break;
					case '\u0090':
						stringBuilder.Append(".(");
						break;
					case '\u0091':
						stringBuilder.Append('@');
						break;
					case '\u0093':
						break;
					default:
						goto IL_D40;
					}
					j++;
					continue;
					IL_D40:
					throw new ApplicationException();
				}
				if (!flag2)
				{
					if (!flag && !flag3)
					{
						stringBuilder.Append('\n');
					}
				}
				else
				{
					if (num3 == 2)
					{
						stringBuilder.Append(')');
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
		private static int GetNext(string source, int length, int i)
		{
			return (int)((i + 1 < length) ? source[i + 1] : '\0');
		}
		private static int GetSourceStringEnd(string source, int offset)
		{
			return Decompiler.PrintSourceString(source, offset, false, null);
		}
		private static int PrintSourceString(string source, int offset, bool asQuotedString, StringBuilder sb)
		{
			int num = (int)source[offset];
			offset++;
			if ((32768 & num) != 0)
			{
				num = ((32767 & num) << 16 | (int)source[offset]);
				offset++;
			}
			if (sb != null)
			{
				string text = source.Substring(offset, offset + num - offset);
				if (!asQuotedString)
				{
					sb.Append(text);
				}
				else
				{
					sb.Append('"');
					sb.Append(ScriptRuntime.escapeString(text));
					sb.Append('"');
				}
			}
			return offset + num;
		}
		private static int PrintSourceNumber(string source, int offset, StringBuilder sb)
		{
			double d = 0.0;
			char c = source[offset];
			offset++;
			if (c == 'S')
			{
				if (sb != null)
				{
					int num = (int)source[offset];
					d = (double)num;
				}
				offset++;
			}
			else
			{
				if (c != 'J' && c != 'D')
				{
					throw new ApplicationException();
				}
				if (sb != null)
				{
					long num2 = (long)((long)((ulong)source[offset]) << 48);
					num2 |= (long)((long)((ulong)source[offset + 1]) << 32);
					num2 |= (long)((long)((ulong)source[offset + 2]) << 16);
					num2 |= (long)((ulong)source[offset + 3]);
					if (c == 'J')
					{
						d = (double)num2;
					}
					else
					{
						d = BitConverter.Int64BitsToDouble(num2);
					}
				}
				offset += 4;
			}
			if (sb != null)
			{
				sb.Append(ScriptConvert.ToString(d, 10));
			}
			return offset;
		}
	}
}
