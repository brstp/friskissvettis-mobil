using EcmaScript.NET.Collections;
using EcmaScript.NET.Types;
using System;
using System.Globalization;
using System.IO;
using System.Text;
namespace EcmaScript.NET
{
	internal class TokenStream
	{
		private const int EOF_CHAR = -1;
		private const int Id_break = 118;
		private const int Id_case = 113;
		private const int Id_continue = 119;
		private const int Id_default = 114;
		private const int Id_delete = 31;
		private const int Id_do = 116;
		private const int Id_else = 111;
		private const int Id_export = 108;
		private const int Id_false = 43;
		private const int Id_for = 117;
		private const int Id_function = 107;
		private const int Id_if = 110;
		private const int Id_in = 51;
		private const int Id_new = 30;
		private const int Id_null = 41;
		private const int Id_return = 4;
		private const int Id_switch = 112;
		private const int Id_this = 42;
		private const int Id_true = 44;
		private const int Id_typeof = 32;
		private const int Id_var = 120;
		private const int Id_void = 124;
		private const int Id_while = 115;
		private const int Id_with = 121;
		private const int Id_abstract = 125;
		private const int Id_boolean = 125;
		private const int Id_byte = 125;
		private const int Id_catch = 122;
		private const int Id_char = 125;
		private const int Id_class = 125;
		private const int Id_const = 125;
		private const int Id_debugger = 156;
		private const int Id_double = 125;
		private const int Id_enum = 125;
		private const int Id_extends = 125;
		private const int Id_final = 125;
		private const int Id_finally = 123;
		private const int Id_float = 125;
		private const int Id_goto = 125;
		private const int Id_implements = 125;
		private const int Id_import = 109;
		private const int Id_instanceof = 52;
		private const int Id_int = 125;
		private const int Id_interface = 125;
		private const int Id_long = 125;
		private const int Id_native = 125;
		private const int Id_package = 125;
		private const int Id_private = 125;
		private const int Id_protected = 125;
		private const int Id_public = 125;
		private const int Id_short = 125;
		private const int Id_static = 125;
		private const int Id_super = 125;
		private const int Id_synchronized = 125;
		private const int Id_throw = 49;
		private const int Id_throws = 125;
		private const int Id_transient = 125;
		private const int Id_try = 79;
		private const int Id_volatile = 125;
		private bool dirtyLine;
		internal string regExpFlags;
		private string str = "";
		private double dNumber;
		private char[] stringBuffer = new char[128];
		private int stringBufferTop;
		private ObjToIntMap allStrings = new ObjToIntMap(50);
		private int[] ungetBuffer = new int[3];
		private int ungetCursor;
		private bool hitEOF = false;
		private int lineStart = 0;
		private int lineno;
		private int lineEndChar = -1;
		private string sourceString;
		private StreamReader sourceReader;
		private char[] sourceBuffer;
		private int sourceEnd;
		private int sourceCursor;
		private bool xmlIsAttribute;
		private bool xmlIsTagContent;
		private int xmlOpenTagsCount;
		private Parser parser;
		internal int Lineno
		{
			get
			{
				return this.lineno;
			}
		}
		internal string String
		{
			get
			{
				return this.str;
			}
		}
		internal double Number
		{
			get
			{
				return this.dNumber;
			}
		}
		internal int Token
		{
			get
			{
				int num;
				bool flag;
				int num2;
				string text;
				while (true)
				{
					do
					{
						num = this.Char;
						if (num == -1)
						{
							goto Block_1;
						}
						if (num == 10)
						{
							goto Block_2;
						}
					}
					while (TokenStream.isJSSpace(num));
					if (num != 45)
					{
						this.dirtyLine = true;
					}
					if (num == 64)
					{
						goto Block_5;
					}
					flag = false;
					bool flag2;
					if (num == 92)
					{
						num = this.Char;
						if (num == 117)
						{
							flag2 = true;
							flag = true;
							this.stringBufferTop = 0;
						}
						else
						{
							flag2 = false;
							this.ungetChar(num);
							num = 92;
						}
					}
					else
					{
						char c = (char)num;
						flag2 = (char.IsLetter(c) || c == '$' || c == '_');
						if (flag2)
						{
							this.stringBufferTop = 0;
							this.addToString(num);
						}
					}
					if (flag2)
					{
						goto Block_11;
					}
					if (TokenStream.isDigit(num) || (num == 46 && TokenStream.isDigit(this.peekChar())))
					{
						goto Block_25;
					}
					if (num == 34 || num == 39)
					{
						goto Block_48;
					}
					num2 = num;
					switch (num2)
					{
					case 33:
						goto IL_9FC;
					case 34:
					case 35:
					case 36:
					case 39:
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 57:
						goto IL_E40;
					case 37:
						goto IL_D5C;
					case 38:
						goto IL_97F;
					case 40:
						goto IL_890;
					case 41:
						goto IL_898;
					case 42:
						goto IL_B83;
					case 43:
						goto IL_D88;
					case 44:
						goto IL_8A0;
					case 45:
						if (this.matchChar(61))
						{
							goto Block_102;
						}
						if (!this.matchChar(45))
						{
							goto IL_E2D;
						}
						if (this.dirtyLine)
						{
							goto IL_E24;
						}
						if (this.matchChar(62))
						{
							this.skipLine();
							continue;
						}
						goto IL_E23;
					case 46:
						goto IL_8D7;
					case 47:
					{
						if (this.matchChar(47))
						{
							this.skipLine();
							continue;
						}
						if (!this.matchChar(42))
						{
							goto IL_D38;
						}
						bool flag3 = false;
						StringBuilder stringBuilder = new StringBuilder();
						while (true)
						{
							num = this.Char;
							if (num == -1)
							{
								goto Block_88;
							}
							stringBuilder.Append((char)num);
							if (num == 42)
							{
								flag3 = true;
							}
							else
							{
								if (num == 47)
								{
									if (flag3)
									{
										break;
									}
								}
								else
								{
									flag3 = false;
								}
							}
						}
						stringBuilder.Remove(stringBuilder.Length - 2, 2);
						text = stringBuilder.ToString();
						string text2 = text.Trim();
						if (text.StartsWith("!"))
						{
							goto Block_92;
						}
						if (text2.StartsWith("@cc_on") || text2.StartsWith("@if") || text2.StartsWith("@elif") || text2.StartsWith("@else") || text2.StartsWith("@end"))
						{
							goto Block_97;
						}
						continue;
					}
					case 58:
						goto IL_8B0;
					case 59:
						goto IL_868;
					case 60:
						if (!this.matchChar(33))
						{
							goto IL_A93;
						}
						if (!this.matchChar(45))
						{
							goto IL_A89;
						}
						if (this.matchChar(45))
						{
							this.skipLine();
							continue;
						}
						goto IL_A7F;
					case 61:
						goto IL_9BE;
					case 62:
						goto IL_AEF;
					case 63:
						goto IL_8A8;
					}
					goto Block_59;
				}
				Block_1:
				int result = 0;
				return result;
				Block_2:
				this.dirtyLine = false;
				result = 1;
				return result;
				Block_5:
				result = 145;
				return result;
				Block_11:
				bool flag4 = flag;
				while (true)
				{
					if (flag)
					{
						int num3 = 0;
						for (int num4 = 0; num4 != 4; num4++)
						{
							num = this.Char;
							num3 = ScriptConvert.XDigitToInt(num, num3);
							if (num3 < 0)
							{
								break;
							}
						}
						if (num3 < 0)
						{
							break;
						}
						this.addToString(num3);
						flag = false;
					}
					else
					{
						num = this.Char;
						if (num == 92)
						{
							num = this.Char;
							if (num != 117)
							{
								goto IL_200;
							}
							flag = true;
							flag4 = true;
						}
						else
						{
							if (num == -1 || !TokenStream.IsJavaIdentifierPart((char)num))
							{
								goto Block_18;
							}
							this.addToString(num);
						}
					}
				}
				this.parser.AddError("msg.invalid.escape");
				result = -1;
				return result;
				IL_200:
				this.parser.AddError("msg.illegal.character");
				result = -1;
				return result;
				Block_18:
				this.ungetChar(num);
				string stringFromBuffer = this.StringFromBuffer;
				if (!flag4)
				{
					int num5 = TokenStream.stringToKeyword(stringFromBuffer);
					if (num5 != 0)
					{
						if (num5 != 125)
						{
							result = num5;
							return result;
						}
						if (!this.parser.compilerEnv.isReservedKeywordAsIdentifier())
						{
							result = num5;
							return result;
						}
						this.parser.AddWarning("msg.reserved.keyword", stringFromBuffer);
					}
				}
				this.str = (string)this.allStrings.intern(stringFromBuffer);
				result = 38;
				return result;
				Block_25:
				this.stringBufferTop = 0;
				int num6 = 10;
				if (num == 48)
				{
					num = this.Char;
					if (num == 120 || num == 88)
					{
						num6 = 16;
						num = this.Char;
					}
					else
					{
						if (TokenStream.isDigit(num))
						{
							num6 = 8;
						}
						else
						{
							this.addToString(48);
						}
					}
				}
				if (num6 == 16)
				{
					while (0 <= ScriptConvert.XDigitToInt(num, 0))
					{
						this.addToString(num);
						num = this.Char;
					}
				}
				else
				{
					while (48 <= num && num <= 57)
					{
						if (num6 == 8 && num >= 56)
						{
							this.parser.AddWarning("msg.bad.octal.literal", (num == 56) ? "8" : "9");
							num6 = 10;
						}
						this.addToString(num);
						num = this.Char;
					}
				}
				bool flag5 = true;
				if (num6 == 10 && (num == 46 || num == 101 || num == 69))
				{
					flag5 = false;
					if (num == 46)
					{
						do
						{
							this.addToString(num);
							num = this.Char;
						}
						while (TokenStream.isDigit(num));
					}
					if (num == 101 || num == 69)
					{
						this.addToString(num);
						num = this.Char;
						if (num == 43 || num == 45)
						{
							this.addToString(num);
							num = this.Char;
						}
						if (!TokenStream.isDigit(num))
						{
							this.parser.AddError("msg.missing.exponent");
							result = -1;
							return result;
						}
						do
						{
							this.addToString(num);
							num = this.Char;
						}
						while (TokenStream.isDigit(num));
					}
				}
				this.ungetChar(num);
				string stringFromBuffer2 = this.StringFromBuffer;
				double num7;
				if (num6 == 10 && !flag5)
				{
					try
					{
						num7 = double.Parse(stringFromBuffer2, BuiltinNumber.NumberFormatter);
					}
					catch (OverflowException)
					{
						if (stringFromBuffer2[0] == '-')
						{
							num7 = double.NegativeInfinity;
						}
						else
						{
							num7 = double.PositiveInfinity;
						}
					}
					catch (Exception)
					{
						this.parser.AddError("msg.caught.nfe");
						result = -1;
						return result;
					}
				}
				else
				{
					num7 = ScriptConvert.ToNumber(stringFromBuffer2, 0, num6);
				}
				this.dNumber = num7;
				result = 39;
				return result;
				Block_48:
				int num8 = num;
				this.stringBufferTop = 0;
				for (num = this.Char; num != num8; num = this.Char)
				{
					if (num == 10 || num == -1)
					{
						this.ungetChar(num);
						this.parser.AddError("msg.unterminated.string.lit");
						result = -1;
						return result;
					}
					if (num == 92)
					{
						num = this.Char;
						num2 = num;
						if (num2 <= 92)
						{
							if (num2 != 10)
							{
								if (num2 != 92)
								{
									goto IL_72F;
								}
								goto IL_714;
							}
						}
						else
						{
							switch (num2)
							{
							case 98:
							case 100:
							case 102:
								goto IL_714;
							case 99:
							case 101:
								goto IL_72F;
							default:
								switch (num2)
								{
								case 110:
								case 114:
								case 116:
								case 117:
								case 118:
								case 120:
									goto IL_714;
								case 111:
								case 112:
								case 113:
								case 115:
								case 119:
									goto IL_72F;
								default:
									goto IL_72F;
								}
								break;
							}
						}
						IL_757:
						goto IL_767;
						IL_714:
						this.addToString(92);
						this.addToString(num);
						goto IL_757;
						IL_72F:
						if (TokenStream.isDigit(num))
						{
							this.addToString(92);
						}
						this.addToString(num);
					}
					else
					{
						this.addToString(num);
					}
					IL_767:;
				}
				stringFromBuffer = this.StringFromBuffer;
				this.str = (string)this.allStrings.intern(stringFromBuffer);
				result = 40;
				return result;
				Block_59:
				switch (num2)
				{
				case 91:
					result = 81;
					return result;
				case 92:
					goto IL_E40;
				case 93:
					result = 82;
					return result;
				case 94:
					if (this.matchChar(61))
					{
						result = 90;
						return result;
					}
					result = 10;
					return result;
				default:
					switch (num2)
					{
					case 123:
						result = 83;
						return result;
					case 124:
						if (this.matchChar(124))
						{
							result = 102;
							return result;
						}
						if (this.matchChar(61))
						{
							result = 89;
							return result;
						}
						result = 9;
						return result;
					case 125:
						result = 84;
						return result;
					case 126:
						result = 27;
						return result;
					default:
						goto IL_E40;
					}
					break;
				}
				IL_868:
				result = 80;
				return result;
				IL_890:
				result = 85;
				return result;
				IL_898:
				result = 86;
				return result;
				IL_8A0:
				result = 87;
				return result;
				IL_8A8:
				result = 100;
				return result;
				IL_8B0:
				if (this.matchChar(58))
				{
					result = 142;
					return result;
				}
				result = 101;
				return result;
				IL_8D7:
				if (this.matchChar(46))
				{
					result = 141;
					return result;
				}
				if (this.matchChar(40))
				{
					result = 144;
					return result;
				}
				result = 106;
				return result;
				IL_97F:
				if (this.matchChar(38))
				{
					result = 103;
					return result;
				}
				if (this.matchChar(61))
				{
					result = 91;
					return result;
				}
				result = 11;
				return result;
				IL_9BE:
				if (!this.matchChar(61))
				{
					result = 88;
					return result;
				}
				if (this.matchChar(61))
				{
					result = 45;
					return result;
				}
				result = 12;
				return result;
				IL_9FC:
				if (!this.matchChar(61))
				{
					result = 26;
					return result;
				}
				if (this.matchChar(61))
				{
					result = 46;
					return result;
				}
				result = 13;
				return result;
				IL_A7F:
				this.ungetChar(45);
				IL_A89:
				this.ungetChar(33);
				IL_A93:
				if (this.matchChar(60))
				{
					if (this.matchChar(61))
					{
						result = 92;
						return result;
					}
					result = 18;
					return result;
				}
				else
				{
					if (this.matchChar(61))
					{
						result = 15;
						return result;
					}
					result = 14;
					return result;
				}
				IL_AEF:
				if (this.matchChar(62))
				{
					if (this.matchChar(62))
					{
						if (this.matchChar(61))
						{
							result = 94;
							return result;
						}
						result = 20;
						return result;
					}
					else
					{
						if (this.matchChar(61))
						{
							result = 93;
							return result;
						}
						result = 19;
						return result;
					}
				}
				else
				{
					if (this.matchChar(61))
					{
						result = 17;
						return result;
					}
					result = 16;
					return result;
				}
				IL_B83:
				if (this.matchChar(61))
				{
					result = 97;
					return result;
				}
				result = 23;
				return result;
				Block_88:
				this.parser.AddError("msg.unterminated.comment");
				result = -1;
				return result;
				Block_92:
				this.str = text.Substring(1);
				result = 155;
				return result;
				Block_97:
				this.str = text;
				result = 154;
				return result;
				IL_D38:
				if (this.matchChar(61))
				{
					result = 98;
					return result;
				}
				result = 24;
				return result;
				IL_D5C:
				if (this.matchChar(61))
				{
					result = 99;
					return result;
				}
				result = 25;
				return result;
				IL_D88:
				if (this.matchChar(61))
				{
					result = 95;
					return result;
				}
				if (this.matchChar(43))
				{
					result = 104;
					return result;
				}
				result = 21;
				return result;
				Block_102:
				num = 96;
				goto IL_E32;
				IL_E23:
				IL_E24:
				num = 105;
				goto IL_E32;
				IL_E2D:
				num = 22;
				IL_E32:
				this.dirtyLine = true;
				result = num;
				return result;
				IL_E40:
				this.parser.AddError("msg.illegal.character");
				result = -1;
				return result;
			}
		}
		internal bool XMLAttribute
		{
			get
			{
				return this.xmlIsAttribute;
			}
		}
		internal int FirstXMLToken
		{
			get
			{
				this.xmlOpenTagsCount = 0;
				this.xmlIsAttribute = false;
				this.xmlIsTagContent = false;
				this.ungetChar(60);
				return this.NextXMLToken;
			}
		}
		internal int NextXMLToken
		{
			get
			{
				this.stringBufferTop = 0;
				int result;
				for (int num = this.Char; num != -1; num = this.Char)
				{
					int num2;
					if (this.xmlIsTagContent)
					{
						num2 = num;
						if (num2 <= 39)
						{
							switch (num2)
							{
							case 9:
							case 10:
							case 13:
								break;
							case 11:
							case 12:
								goto IL_159;
							default:
								switch (num2)
								{
								case 32:
									goto IL_14C;
								case 33:
									goto IL_159;
								case 34:
									break;
								default:
									if (num2 != 39)
									{
										goto IL_159;
									}
									break;
								}
								this.addToString(num);
								if (!this.readQuotedString(num))
								{
									result = -1;
									return result;
								}
								goto IL_16D;
							}
							IL_14C:
							this.addToString(num);
						}
						else
						{
							if (num2 != 47)
							{
								switch (num2)
								{
								case 61:
									this.addToString(num);
									this.xmlIsAttribute = true;
									break;
								case 62:
									this.addToString(num);
									this.xmlIsTagContent = false;
									this.xmlIsAttribute = false;
									break;
								default:
									if (num2 != 123)
									{
										goto IL_159;
									}
									this.ungetChar(num);
									this.str = this.StringFromBuffer;
									result = 143;
									return result;
								}
							}
							else
							{
								this.addToString(num);
								if (this.peekChar() == 62)
								{
									num = this.Char;
									this.addToString(num);
									this.xmlIsTagContent = false;
									this.xmlOpenTagsCount--;
								}
							}
						}
						IL_16D:
						if (!this.xmlIsTagContent && this.xmlOpenTagsCount == 0)
						{
							this.str = this.StringFromBuffer;
							result = 146;
							return result;
						}
						goto IL_472;
						IL_159:
						this.addToString(num);
						this.xmlIsAttribute = false;
						goto IL_16D;
					}
					num2 = num;
					if (num2 != 60)
					{
						if (num2 == 123)
						{
							this.ungetChar(num);
							this.str = this.StringFromBuffer;
							result = 143;
							return result;
						}
						this.addToString(num);
					}
					else
					{
						this.addToString(num);
						num = this.peekChar();
						num2 = num;
						if (num2 != 33)
						{
							if (num2 != 47)
							{
								if (num2 != 63)
								{
									this.xmlIsTagContent = true;
									this.xmlOpenTagsCount++;
								}
								else
								{
									num = this.Char;
									this.addToString(num);
									if (!this.readPI())
									{
										result = -1;
										return result;
									}
								}
							}
							else
							{
								num = this.Char;
								this.addToString(num);
								if (this.xmlOpenTagsCount == 0)
								{
									this.stringBufferTop = 0;
									this.str = null;
									this.parser.AddError("msg.XML.bad.form");
									result = -1;
									return result;
								}
								this.xmlIsTagContent = true;
								this.xmlOpenTagsCount--;
							}
						}
						else
						{
							num = this.Char;
							this.addToString(num);
							num = this.peekChar();
							num2 = num;
							if (num2 != 45)
							{
								if (num2 != 91)
								{
									if (!this.readEntity())
									{
										result = -1;
										return result;
									}
								}
								else
								{
									num = this.Char;
									this.addToString(num);
									if (this.Char != 67 || this.Char != 68 || this.Char != 65 || this.Char != 84 || this.Char != 65 || this.Char != 91)
									{
										this.stringBufferTop = 0;
										this.str = null;
										this.parser.AddError("msg.XML.bad.form");
										result = -1;
										return result;
									}
									this.addToString(67);
									this.addToString(68);
									this.addToString(65);
									this.addToString(84);
									this.addToString(65);
									this.addToString(91);
									if (!this.readCDATA())
									{
										result = -1;
										return result;
									}
								}
							}
							else
							{
								num = this.Char;
								this.addToString(num);
								num = this.Char;
								if (num != 45)
								{
									this.stringBufferTop = 0;
									this.str = null;
									this.parser.AddError("msg.XML.bad.form");
									result = -1;
									return result;
								}
								this.addToString(num);
								if (!this.readXmlComment())
								{
									result = -1;
									return result;
								}
							}
						}
					}
					IL_472:;
				}
				this.stringBufferTop = 0;
				this.str = null;
				this.parser.AddError("msg.XML.bad.form");
				result = -1;
				return result;
			}
		}
		private string StringFromBuffer
		{
			get
			{
				return new string(this.stringBuffer, 0, this.stringBufferTop);
			}
		}
		private int Char
		{
			get
			{
				int result;
				if (this.ungetCursor != 0)
				{
					result = this.ungetBuffer[--this.ungetCursor];
				}
				else
				{
					int num;
					while (true)
					{
						if (this.sourceString != null)
						{
							if (this.sourceCursor == this.sourceEnd)
							{
								break;
							}
							num = (int)this.sourceString[this.sourceCursor++];
						}
						else
						{
							if (this.sourceCursor == this.sourceEnd)
							{
								if (!this.fillSourceBuffer())
								{
									goto Block_5;
								}
							}
							num = (int)this.sourceBuffer[this.sourceCursor++];
						}
						if (this.lineEndChar >= 0)
						{
							if (this.lineEndChar == 13 && num == 10)
							{
								this.lineEndChar = 10;
								continue;
							}
							this.lineEndChar = -1;
							this.lineStart = this.sourceCursor - 1;
							this.lineno++;
						}
						if (num <= 127)
						{
							goto Block_9;
						}
						if (!TokenStream.isJSFormatChar(num))
						{
							goto IL_19A;
						}
					}
					this.hitEOF = true;
					result = -1;
					return result;
					Block_5:
					this.hitEOF = true;
					result = -1;
					return result;
					Block_9:
					if (num == 10 || num == 13)
					{
						this.lineEndChar = num;
						num = 10;
					}
					goto IL_1B7;
					IL_19A:
					if (ScriptRuntime.isJSLineTerminator(num))
					{
						this.lineEndChar = num;
						num = 10;
					}
					IL_1B7:
					result = num;
				}
				return result;
			}
		}
		internal int Offset
		{
			get
			{
				int num = this.sourceCursor - this.lineStart;
				if (this.lineEndChar >= 0)
				{
					num--;
				}
				return num;
			}
		}
		internal string Line
		{
			get
			{
				string result;
				if (this.sourceString != null)
				{
					int num = this.sourceCursor;
					if (this.lineEndChar >= 0)
					{
						num--;
					}
					else
					{
						while (num != this.sourceEnd)
						{
							int c = (int)this.sourceString[num];
							if (ScriptRuntime.isJSLineTerminator(c))
							{
								break;
							}
							num++;
						}
					}
					result = this.sourceString.Substring(this.lineStart, num - this.lineStart);
				}
				else
				{
					int num2 = this.sourceCursor - this.lineStart;
					if (this.lineEndChar >= 0)
					{
						num2--;
					}
					else
					{
						while (true)
						{
							int num3 = this.lineStart + num2;
							if (num3 == this.sourceEnd)
							{
								try
								{
									if (!this.fillSourceBuffer())
									{
										break;
									}
								}
								catch (IOException)
								{
									break;
								}
								num3 = this.lineStart + num2;
							}
							int c = (int)this.sourceBuffer[num3];
							if (ScriptRuntime.isJSLineTerminator(c))
							{
								break;
							}
							num2++;
						}
					}
					result = new string(this.sourceBuffer, this.lineStart, num2);
				}
				return result;
			}
		}
		internal TokenStream(Parser parser, StreamReader sourceReader, string sourceString, int lineno)
		{
			this.parser = parser;
			this.lineno = lineno;
			if (sourceReader != null)
			{
				if (sourceString != null)
				{
					Context.CodeBug();
				}
				this.sourceReader = sourceReader;
				this.sourceBuffer = new char[512];
				this.sourceEnd = 0;
			}
			else
			{
				if (sourceString == null)
				{
					Context.CodeBug();
				}
				this.sourceString = sourceString;
				this.sourceEnd = sourceString.Length;
			}
			this.sourceCursor = 0;
		}
		internal string tokenToString(int token)
		{
			string result;
			if (EcmaScript.NET.Token.printTrees)
			{
				string text = EcmaScript.NET.Token.name(token);
				switch (token)
				{
				case 38:
				case 40:
					break;
				case 39:
					result = "NUMBER " + this.dNumber;
					return result;
				default:
					if (token != 47)
					{
						result = text;
						return result;
					}
					break;
				}
				result = text + " `" + this.str + "'";
			}
			else
			{
				result = "";
			}
			return result;
		}
		internal static bool isKeyword(string s)
		{
			return 0 != TokenStream.stringToKeyword(s);
		}
		private static int stringToKeyword(string name)
		{
			int num = 0;
			string text = null;
			switch (name.Length)
			{
			case 2:
			{
				int num2 = (int)name[1];
				if (num2 == 102)
				{
					if (name[0] == 'i')
					{
						num = 110;
						goto IL_9D3;
					}
				}
				else
				{
					if (num2 == 110)
					{
						if (name[0] == 'i')
						{
							num = 51;
							goto IL_9D3;
						}
					}
					else
					{
						if (num2 == 111)
						{
							if (name[0] == 'd')
							{
								num = 116;
								goto IL_9D3;
							}
						}
					}
				}
				break;
			}
			case 3:
			{
				char c = name[0];
				if (c <= 'i')
				{
					if (c != 'f')
					{
						if (c == 'i')
						{
							if (name[2] == 't' && name[1] == 'n')
							{
								num = 125;
								goto IL_9D3;
							}
						}
					}
					else
					{
						if (name[2] == 'r' && name[1] == 'o')
						{
							num = 117;
							goto IL_9D3;
						}
					}
				}
				else
				{
					if (c != 'n')
					{
						switch (c)
						{
						case 't':
							if (name[2] == 'y' && name[1] == 'r')
							{
								num = 79;
								goto IL_9D3;
							}
							break;
						case 'v':
							if (name[2] == 'r' && name[1] == 'a')
							{
								num = 120;
								goto IL_9D3;
							}
							break;
						}
					}
					else
					{
						if (name[2] == 'w' && name[1] == 'e')
						{
							num = 30;
							goto IL_9D3;
						}
					}
				}
				break;
			}
			case 4:
			{
				char c = name[0];
				switch (c)
				{
				case 'b':
					text = "byte";
					num = 125;
					break;
				case 'c':
				{
					int num2 = (int)name[3];
					if (num2 == 101)
					{
						if (name[2] == 's' && name[1] == 'a')
						{
							num = 113;
							goto IL_9D3;
						}
					}
					else
					{
						if (num2 == 114)
						{
							if (name[2] == 'a' && name[1] == 'h')
							{
								num = 125;
								goto IL_9D3;
							}
						}
					}
					break;
				}
				case 'd':
				case 'f':
					break;
				case 'e':
				{
					int num2 = (int)name[3];
					if (num2 == 101)
					{
						if (name[2] == 's' && name[1] == 'l')
						{
							num = 111;
							goto IL_9D3;
						}
					}
					else
					{
						if (num2 == 109)
						{
							if (name[2] == 'u' && name[1] == 'n')
							{
								num = 125;
								goto IL_9D3;
							}
						}
					}
					break;
				}
				case 'g':
					text = "goto";
					num = 125;
					break;
				default:
					switch (c)
					{
					case 'l':
						text = "long";
						num = 125;
						break;
					case 'm':
						break;
					case 'n':
						text = "null";
						num = 41;
						break;
					default:
						switch (c)
						{
						case 't':
						{
							int num2 = (int)name[3];
							if (num2 == 101)
							{
								if (name[2] == 'u' && name[1] == 'r')
								{
									num = 44;
									goto IL_9D3;
								}
							}
							else
							{
								if (num2 == 115)
								{
									if (name[2] == 'i' && name[1] == 'h')
									{
										num = 42;
										goto IL_9D3;
									}
								}
							}
							break;
						}
						case 'v':
							text = "void";
							num = 124;
							break;
						case 'w':
							text = "with";
							num = 121;
							break;
						}
						break;
					}
					break;
				}
				break;
			}
			case 5:
			{
				char c = name[2];
				if (c != 'a')
				{
					if (c != 'e')
					{
						switch (c)
						{
						case 'i':
							text = "while";
							num = 115;
							break;
						case 'l':
							text = "false";
							num = 43;
							break;
						case 'n':
						{
							int num2 = (int)name[0];
							if (num2 == 99)
							{
								text = "const";
								num = 125;
							}
							else
							{
								if (num2 == 102)
								{
									text = "final";
									num = 125;
								}
							}
							break;
						}
						case 'o':
						{
							int num2 = (int)name[0];
							if (num2 == 102)
							{
								text = "float";
								num = 125;
							}
							else
							{
								if (num2 == 115)
								{
									text = "short";
									num = 125;
								}
							}
							break;
						}
						case 'p':
							text = "super";
							num = 125;
							break;
						case 'r':
							text = "throw";
							num = 49;
							break;
						case 't':
							text = "catch";
							num = 122;
							break;
						}
					}
					else
					{
						text = "break";
						num = 118;
					}
				}
				else
				{
					text = "class";
					num = 125;
				}
				break;
			}
			case 6:
			{
				char c = name[1];
				if (c <= 'e')
				{
					if (c != 'a')
					{
						if (c == 'e')
						{
							int num2 = (int)name[0];
							if (num2 == 100)
							{
								text = "delete";
								num = 31;
							}
							else
							{
								if (num2 == 114)
								{
									text = "return";
									num = 4;
								}
							}
						}
					}
					else
					{
						text = "native";
						num = 125;
					}
				}
				else
				{
					if (c != 'h')
					{
						switch (c)
						{
						case 'm':
							text = "import";
							num = 109;
							break;
						case 'o':
							text = "double";
							num = 125;
							break;
						case 't':
							text = "static";
							num = 125;
							break;
						case 'u':
							text = "public";
							num = 125;
							break;
						case 'w':
							text = "switch";
							num = 112;
							break;
						case 'x':
							text = "export";
							num = 108;
							break;
						case 'y':
							text = "typeof";
							num = 32;
							break;
						}
					}
					else
					{
						text = "throws";
						num = 125;
					}
				}
				break;
			}
			case 7:
			{
				char c = name[1];
				if (c <= 'i')
				{
					if (c != 'a')
					{
						if (c != 'e')
						{
							if (c == 'i')
							{
								text = "finally";
								num = 123;
							}
						}
						else
						{
							text = "default";
							num = 114;
						}
					}
					else
					{
						text = "package";
						num = 125;
					}
				}
				else
				{
					if (c != 'o')
					{
						if (c != 'r')
						{
							if (c == 'x')
							{
								text = "extends";
								num = 125;
							}
						}
						else
						{
							text = "private";
							num = 125;
						}
					}
					else
					{
						text = "boolean";
						num = 125;
					}
				}
				break;
			}
			case 8:
			{
				char c = name[0];
				switch (c)
				{
				case 'a':
					text = "abstract";
					num = 125;
					break;
				case 'b':
				case 'e':
					break;
				case 'c':
					text = "continue";
					num = 119;
					break;
				case 'd':
					text = "debugger";
					num = 156;
					break;
				case 'f':
					text = "function";
					num = 107;
					break;
				default:
					if (c == 'v')
					{
						text = "volatile";
						num = 125;
					}
					break;
				}
				break;
			}
			case 9:
			{
				int num2 = (int)name[0];
				if (num2 == 105)
				{
					text = "interface";
					num = 125;
				}
				else
				{
					if (num2 == 112)
					{
						text = "protected";
						num = 125;
					}
					else
					{
						if (num2 == 116)
						{
							text = "transient";
							num = 125;
						}
					}
				}
				break;
			}
			case 10:
			{
				int num2 = (int)name[1];
				if (num2 == 109)
				{
					text = "implements";
					num = 125;
				}
				else
				{
					if (num2 == 110)
					{
						text = "instanceof";
						num = 52;
					}
				}
				break;
			}
			case 12:
				text = "synchronized";
				num = 125;
				break;
			}
			if (text != null && text != name && !text.Equals(name))
			{
				num = 0;
			}
			IL_9D3:
			int result;
			if (num == 0)
			{
				result = 0;
			}
			else
			{
				result = (num & 255);
			}
			return result;
		}
		internal bool eof()
		{
			return this.hitEOF;
		}
		private static bool isAlpha(int c)
		{
			bool result;
			if (c <= 90)
			{
				result = (65 <= c);
			}
			else
			{
				result = (97 <= c && c <= 122);
			}
			return result;
		}
		internal static bool isDigit(int c)
		{
			return 48 <= c && c <= 57;
		}
		internal static bool isJSSpace(int c)
		{
			bool result;
			if (c <= 127)
			{
				result = (c == 32 || c == 9 || c == 12 || c == 11);
			}
			else
			{
				result = (c == 160 || char.GetUnicodeCategory((char)c) == UnicodeCategory.SpaceSeparator);
			}
			return result;
		}
		private static bool isJSFormatChar(int c)
		{
			return c > 127 && char.GetUnicodeCategory((char)c) == UnicodeCategory.Format;
		}
		internal void readRegExp(int startToken)
		{
			this.stringBufferTop = 0;
			if (startToken == 98)
			{
				this.addToString(61);
			}
			else
			{
				if (startToken != 24)
				{
					Context.CodeBug();
				}
			}
			bool flag = false;
			int @char;
			while ((@char = this.Char) != 47 || flag)
			{
				if (@char == 10 || @char == -1)
				{
					this.ungetChar(@char);
					throw this.parser.ReportError("msg.unterminated.re.lit");
				}
				if (@char == 92)
				{
					this.addToString(@char);
					@char = this.Char;
				}
				else
				{
					if (@char == 91)
					{
						flag = true;
					}
					else
					{
						if (@char == 93)
						{
							flag = false;
						}
					}
				}
				this.addToString(@char);
			}
			int num = this.stringBufferTop;
			while (true)
			{
				if (this.matchChar(103))
				{
					this.addToString(103);
				}
				else
				{
					if (this.matchChar(105))
					{
						this.addToString(105);
					}
					else
					{
						if (!this.matchChar(109))
						{
							break;
						}
						this.addToString(109);
					}
				}
			}
			if (TokenStream.isAlpha(this.peekChar()))
			{
				throw this.parser.ReportError("msg.invalid.re.flag");
			}
			this.str = new string(this.stringBuffer, 0, num);
			this.regExpFlags = new string(this.stringBuffer, num, this.stringBufferTop - num);
		}
		private bool readQuotedString(int quote)
		{
			bool result;
			for (int @char = this.Char; @char != -1; @char = this.Char)
			{
				this.addToString(@char);
				if (@char == quote)
				{
					result = true;
					return result;
				}
			}
			this.stringBufferTop = 0;
			this.str = null;
			this.parser.AddError("msg.XML.bad.form");
			result = false;
			return result;
		}
		private bool readXmlComment()
		{
			int @char = this.Char;
			bool result;
			while (@char != -1)
			{
				this.addToString(@char);
				if (@char == 45 && this.peekChar() == 45)
				{
					@char = this.Char;
					this.addToString(@char);
					if (this.peekChar() == 62)
					{
						@char = this.Char;
						this.addToString(@char);
						result = true;
						return result;
					}
				}
				else
				{
					@char = this.Char;
				}
			}
			this.stringBufferTop = 0;
			this.str = null;
			this.parser.AddError("msg.XML.bad.form");
			result = false;
			return result;
		}
		private bool readCDATA()
		{
			int @char = this.Char;
			bool result;
			while (@char != -1)
			{
				this.addToString(@char);
				if (@char == 93 && this.peekChar() == 93)
				{
					@char = this.Char;
					this.addToString(@char);
					if (this.peekChar() == 62)
					{
						@char = this.Char;
						this.addToString(@char);
						result = true;
						return result;
					}
				}
				else
				{
					@char = this.Char;
				}
			}
			this.stringBufferTop = 0;
			this.str = null;
			this.parser.AddError("msg.XML.bad.form");
			result = false;
			return result;
		}
		private bool readEntity()
		{
			int num = 1;
			bool result;
			for (int @char = this.Char; @char != -1; @char = this.Char)
			{
				this.addToString(@char);
				switch (@char)
				{
				case 60:
					num++;
					break;
				case 62:
					num--;
					if (num == 0)
					{
						result = true;
						return result;
					}
					break;
				}
			}
			this.stringBufferTop = 0;
			this.str = null;
			this.parser.AddError("msg.XML.bad.form");
			result = false;
			return result;
		}
		private bool readPI()
		{
			bool result;
			for (int @char = this.Char; @char != -1; @char = this.Char)
			{
				this.addToString(@char);
				if (@char == 63 && this.peekChar() == 62)
				{
					@char = this.Char;
					this.addToString(@char);
					result = true;
					return result;
				}
			}
			this.stringBufferTop = 0;
			this.str = null;
			this.parser.AddError("msg.XML.bad.form");
			result = false;
			return result;
		}
		private void addToString(int c)
		{
			int num = this.stringBufferTop;
			if (num == this.stringBuffer.Length)
			{
				char[] destinationArray = new char[this.stringBuffer.Length * 4];
				Array.Copy(this.stringBuffer, 0, destinationArray, 0, num);
				this.stringBuffer = destinationArray;
			}
			this.stringBuffer[num] = (char)c;
			this.stringBufferTop = num + 1;
		}
		private void ungetChar(int c)
		{
			if (this.ungetCursor != 0 && this.ungetBuffer[this.ungetCursor - 1] == 10)
			{
				Context.CodeBug();
			}
			this.ungetBuffer[this.ungetCursor++] = c;
		}
		private bool matchChar(int test)
		{
			int @char = this.Char;
			bool result;
			if (@char == test)
			{
				result = true;
			}
			else
			{
				this.ungetChar(@char);
				result = false;
			}
			return result;
		}
		private int peekChar()
		{
			int @char = this.Char;
			this.ungetChar(@char);
			return @char;
		}
		private void skipLine()
		{
			int @char;
			while ((@char = this.Char) != -1 && @char != 10)
			{
			}
			this.ungetChar(@char);
		}
		private bool fillSourceBuffer()
		{
			if (this.sourceString != null)
			{
				Context.CodeBug();
			}
			if (this.sourceEnd == this.sourceBuffer.Length)
			{
				if (this.lineStart != 0)
				{
					Array.Copy(this.sourceBuffer, this.lineStart, this.sourceBuffer, 0, this.sourceEnd - this.lineStart);
					this.sourceEnd -= this.lineStart;
					this.sourceCursor -= this.lineStart;
					this.lineStart = 0;
				}
				else
				{
					char[] destinationArray = new char[this.sourceBuffer.Length * 2];
					Array.Copy(this.sourceBuffer, 0, destinationArray, 0, this.sourceEnd);
					this.sourceBuffer = destinationArray;
				}
			}
			int num = this.sourceReader.Read(this.sourceBuffer, this.sourceEnd, this.sourceBuffer.Length - this.sourceEnd);
			bool result;
			if (num <= 0)
			{
				result = false;
			}
			else
			{
				this.sourceEnd += num;
				result = true;
			}
			return result;
		}
		internal static bool IsJavaIdentifierPart(char c)
		{
			bool result;
			if (char.IsLetterOrDigit(c))
			{
				result = true;
			}
			else
			{
				UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
				result = (unicodeCategory == UnicodeCategory.CurrencySymbol || unicodeCategory == UnicodeCategory.ConnectorPunctuation || unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.NonSpacingMark || TokenStream.IsIdentifierIgnorable(c));
			}
			return result;
		}
		internal static bool IsIdentifierIgnorable(char c)
		{
			return (c >= '\0' && c <= '\b') || (c >= '\u000e' && c <= '\u001b') || (c >= '\u007f' && c <= '\u009f') || char.GetUnicodeCategory(c) == UnicodeCategory.Format;
		}
		internal static bool IsJavaIdentifierStart(char c)
		{
			bool result;
			if (char.IsLetter(c))
			{
				result = true;
			}
			else
			{
				UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
				result = (unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.CurrencySymbol || unicodeCategory == UnicodeCategory.ConnectorPunctuation);
			}
			return result;
		}
	}
}
