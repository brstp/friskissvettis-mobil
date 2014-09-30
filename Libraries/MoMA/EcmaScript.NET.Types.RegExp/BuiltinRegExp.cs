using EcmaScript.NET.Helpers;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace EcmaScript.NET.Types.RegExp
{
	[ComVisible(true)]
	public class BuiltinRegExp : IdScriptableObject, IFunction, IScriptable, ICallable
	{
		public const int JSREG_GLOB = 1;
		public const int JSREG_FOLD = 2;
		public const int JSREG_MULTILINE = 4;
		public const int TEST = 0;
		public const int MATCH = 1;
		public const int PREFIX = 2;
		public const sbyte REOP_EMPTY = 0;
		public const sbyte REOP_ALT = 1;
		public const sbyte REOP_BOL = 2;
		public const sbyte REOP_EOL = 3;
		public const sbyte REOP_WBDRY = 4;
		public const sbyte REOP_WNONBDRY = 5;
		public const sbyte REOP_QUANT = 6;
		public const sbyte REOP_STAR = 7;
		public const sbyte REOP_PLUS = 8;
		public const sbyte REOP_OPT = 9;
		public const sbyte REOP_LPAREN = 10;
		public const sbyte REOP_RPAREN = 11;
		public const sbyte REOP_DOT = 12;
		public const sbyte REOP_CCLASS = 13;
		public const sbyte REOP_DIGIT = 14;
		public const sbyte REOP_NONDIGIT = 15;
		public const sbyte REOP_ALNUM = 16;
		public const sbyte REOP_NONALNUM = 17;
		public const sbyte REOP_SPACE = 18;
		public const sbyte REOP_NONSPACE = 19;
		public const sbyte REOP_BACKREF = 20;
		public const sbyte REOP_FLAT = 21;
		public const sbyte REOP_FLAT1 = 22;
		public const sbyte REOP_JUMP = 23;
		public const sbyte REOP_DOTSTAR = 24;
		public const sbyte REOP_ANCHOR = 25;
		public const sbyte REOP_EOLONLY = 26;
		public const sbyte REOP_UCFLAT = 27;
		public const sbyte REOP_UCFLAT1 = 28;
		public const sbyte REOP_UCCLASS = 29;
		public const sbyte REOP_NUCCLASS = 30;
		public const sbyte REOP_BACKREFi = 31;
		public const sbyte REOP_FLATi = 32;
		public const sbyte REOP_FLAT1i = 33;
		public const sbyte REOP_UCFLATi = 34;
		public const sbyte REOP_UCFLAT1i = 35;
		public const sbyte REOP_ANCHOR1 = 36;
		public const sbyte REOP_NCCLASS = 37;
		public const sbyte REOP_DOTSTARMIN = 38;
		public const sbyte REOP_LPARENNON = 39;
		public const sbyte REOP_RPARENNON = 40;
		public const sbyte REOP_ASSERT = 41;
		public const sbyte REOP_ASSERT_NOT = 42;
		public const sbyte REOP_ASSERTTEST = 43;
		public const sbyte REOP_ASSERTNOTTEST = 44;
		public const sbyte REOP_MINIMALSTAR = 45;
		public const sbyte REOP_MINIMALPLUS = 46;
		public const sbyte REOP_MINIMALOPT = 47;
		public const sbyte REOP_MINIMALQUANT = 48;
		public const sbyte REOP_ENDCHILD = 49;
		public const sbyte REOP_CLASS = 50;
		public const sbyte REOP_REPEAT = 51;
		public const sbyte REOP_MINIMALREPEAT = 52;
		public const sbyte REOP_END = 53;
		private const int OFFSET_LEN = 2;
		private const int INDEX_LEN = 2;
		private const int Id_lastIndex = 1;
		private const int Id_source = 2;
		private const int Id_global = 3;
		private const int Id_ignoreCase = 4;
		private const int Id_multiline = 5;
		private const int MAX_INSTANCE_ID = 5;
		private const int Id_compile = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int Id_exec = 4;
		private const int Id_test = 5;
		private const int Id_prefix = 6;
		private const int MAX_PROTOTYPE_ID = 6;
		private static readonly object REGEXP_TAG = new object();
		private static bool debug = false;
		private RECompiled re;
		internal double lastIndex;
		public override string ClassName
		{
			get
			{
				return "RegExp";
			}
		}
		internal virtual int Flags
		{
			get
			{
				return this.re.flags;
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return 5;
			}
		}
		private static string DebugNameOp(sbyte op)
		{
			FieldInfo[] fields = typeof(BuiltinRegExp).GetFields();
			string result;
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.Name.StartsWith("REOP_"))
				{
					sbyte b = (sbyte)fieldInfo.GetValue(typeof(BuiltinRegExp));
					if (b == op)
					{
						result = fieldInfo.Name;
						return result;
					}
				}
			}
			result = "<undefined>";
			return result;
		}
		public static void Init(IScriptable scope, bool zealed)
		{
			BuiltinRegExp builtinRegExp = new BuiltinRegExp();
			builtinRegExp.re = (RECompiled)BuiltinRegExp.compileRE("", null, false);
			builtinRegExp.ActivatePrototypeMap(6);
			builtinRegExp.ParentScope = scope;
			builtinRegExp.SetPrototype(ScriptableObject.GetObjectPrototype(scope));
			BuiltinRegExpCtor builtinRegExpCtor = new BuiltinRegExpCtor();
			ScriptRuntime.setFunctionProtoAndParent(builtinRegExpCtor, scope);
			builtinRegExpCtor.ImmunePrototypeProperty = builtinRegExp;
			if (zealed)
			{
				builtinRegExp.SealObject();
				builtinRegExpCtor.SealObject();
			}
			ScriptableObject.DefineProperty(scope, "RegExp", builtinRegExpCtor, 2);
		}
		internal BuiltinRegExp(IScriptable scope, object regexpCompiled)
		{
			this.re = (RECompiled)regexpCompiled;
			this.lastIndex = 0.0;
			ScriptRuntime.setObjectProtoAndParent(this, scope);
		}
		public virtual object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			return this.execSub(cx, scope, args, 1);
		}
		public virtual IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			return (IScriptable)this.execSub(cx, scope, args, 1);
		}
		internal virtual IScriptable compile(Context cx, IScriptable scope, object[] args)
		{
			IScriptable result;
			if (args.Length > 0 && args[0] is BuiltinRegExp)
			{
				if (args.Length > 1 && args[1] != Undefined.Value)
				{
					throw ScriptRuntime.TypeErrorById("msg.bad.regexp.compile", new string[0]);
				}
				BuiltinRegExp builtinRegExp = (BuiltinRegExp)args[0];
				this.re = builtinRegExp.re;
				this.lastIndex = builtinRegExp.lastIndex;
				result = this;
			}
			else
			{
				string str = (args.Length == 0) ? "" : ScriptConvert.ToString(args[0]);
				string global = (args.Length > 1 && args[1] != Undefined.Value) ? ScriptConvert.ToString(args[1]) : null;
				this.re = (RECompiled)BuiltinRegExp.compileRE(str, global, false);
				this.lastIndex = 0.0;
				result = this;
			}
			return result;
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('/');
			if (this.re.source.Length != 0)
			{
				stringBuilder.Append(this.re.source);
			}
			else
			{
				stringBuilder.Append("(?:)");
			}
			stringBuilder.Append('/');
			if ((this.re.flags & 1) != 0)
			{
				stringBuilder.Append('g');
			}
			if ((this.re.flags & 2) != 0)
			{
				stringBuilder.Append('i');
			}
			if ((this.re.flags & 4) != 0)
			{
				stringBuilder.Append('m');
			}
			return stringBuilder.ToString();
		}
		internal BuiltinRegExp()
		{
		}
		private static RegExpImpl getImpl(Context cx)
		{
			return (RegExpImpl)cx.RegExpProxy;
		}
		private object execSub(Context cx, IScriptable scopeObj, object[] args, int matchType)
		{
			RegExpImpl impl = BuiltinRegExp.getImpl(cx);
			string text;
			if (args.Length == 0)
			{
				text = impl.input;
				if (text == null)
				{
					BuiltinRegExp.reportError("msg.no.re.input.for", this.ToString());
				}
			}
			else
			{
				text = ScriptConvert.ToString(args[0]);
			}
			double num = ((this.re.flags & 1) != 0) ? this.lastIndex : 0.0;
			object obj;
			if (num < 0.0 || (double)text.Length < num)
			{
				this.lastIndex = 0.0;
				obj = null;
			}
			else
			{
				int[] array = new int[]
				{
					(int)num
				};
				obj = this.executeRegExp(cx, scopeObj, impl, text, array, matchType);
				if ((this.re.flags & 1) != 0)
				{
					this.lastIndex = (double)((obj == null || obj == Undefined.Value) ? 0 : array[0]);
				}
			}
			return obj;
		}
		internal static object compileRE(string str, string global, bool flat)
		{
			RECompiled rECompiled = new RECompiled();
			rECompiled.source = str.ToCharArray();
			int length = str.Length;
			int num = 0;
			if (global != null)
			{
				for (int i = 0; i < global.Length; i++)
				{
					char c = global[i];
					if (c == 'g')
					{
						num |= 1;
					}
					else
					{
						if (c == 'i')
						{
							num |= 2;
						}
						else
						{
							if (c == 'm')
							{
								num |= 4;
							}
							else
							{
								BuiltinRegExp.reportError("msg.invalid.re.flag", Convert.ToString(c));
							}
						}
					}
				}
			}
			rECompiled.flags = num;
			CompilerState compilerState = new CompilerState(rECompiled.source, length, num);
			object result;
			if (flat && length > 0)
			{
				if (BuiltinRegExp.debug)
				{
					Console.Out.WriteLine("flat = \"" + str + "\"");
				}
				compilerState.result = new RENode(21);
				compilerState.result.chr = compilerState.cpbegin[0];
				compilerState.result.length = length;
				compilerState.result.flatIndex = 0;
				compilerState.progLength += 5;
			}
			else
			{
				if (!BuiltinRegExp.parseDisjunction(compilerState))
				{
					result = null;
					return result;
				}
			}
			rECompiled.program = new sbyte[compilerState.progLength + 1];
			if (compilerState.classCount != 0)
			{
				rECompiled.classList = new RECharSet[compilerState.classCount];
				rECompiled.classCount = compilerState.classCount;
			}
			int num2 = BuiltinRegExp.emitREBytecode(compilerState, rECompiled, 0, compilerState.result);
			rECompiled.program[num2++] = 53;
			if (BuiltinRegExp.debug)
			{
				Console.Out.WriteLine("Prog. length = " + num2);
				for (int i = 0; i < num2; i++)
				{
					Console.Out.Write(BuiltinRegExp.DebugNameOp(rECompiled.program[i]));
					if (i < num2 - 1)
					{
						Console.Out.Write(", ");
					}
				}
				Console.Out.WriteLine();
			}
			rECompiled.parenCount = compilerState.parenCount;
			sbyte b = rECompiled.program[0];
			switch (b)
			{
			case 21:
				goto IL_2F6;
			case 22:
				break;
			default:
				if (b != 28)
				{
					switch (b)
					{
					case 32:
						goto IL_2F6;
					case 33:
						goto IL_2DC;
					case 34:
						goto IL_318;
					case 35:
						break;
					default:
						goto IL_318;
					}
				}
				rECompiled.anchorCh = (int)((ushort)BuiltinRegExp.getIndex(rECompiled.program, 1));
				goto IL_318;
			}
			IL_2DC:
			rECompiled.anchorCh = (int)((ushort)((int)rECompiled.program[1] & 255));
			goto IL_318;
			IL_2F6:
			int index = BuiltinRegExp.getIndex(rECompiled.program, 1);
			rECompiled.anchorCh = (int)rECompiled.source[index];
			IL_318:
			if (BuiltinRegExp.debug)
			{
				if (rECompiled.anchorCh >= 0)
				{
					Console.Out.WriteLine("Anchor ch = '" + (char)rECompiled.anchorCh + "'");
				}
			}
			result = rECompiled;
			return result;
		}
		internal static bool isDigit(char c)
		{
			return '0' <= c && c <= '9';
		}
		private static bool isWord(char c)
		{
			return char.IsLetter(c) || BuiltinRegExp.isDigit(c) || c == '_';
		}
		private static bool isLineTerm(char c)
		{
			return ScriptRuntime.isJSLineTerminator((int)c);
		}
		private static bool isREWhiteSpace(int c)
		{
			return c == 32 || c == 9 || c == 10 || c == 13 || c == 8232 || c == 8233 || c == 12 || c == 11 || c == 160 || char.GetUnicodeCategory((char)c) == UnicodeCategory.SpaceSeparator;
		}
		private static char upcase(char ch)
		{
			char result;
			if (ch < '\u0080')
			{
				if ('a' <= ch && ch <= 'z')
				{
					result = (char)((int)ch + -32);
				}
				else
				{
					result = ch;
				}
			}
			else
			{
				char c = char.ToUpper(ch);
				if (ch >= '\u0080' && c < '\u0080')
				{
					result = ch;
				}
				else
				{
					result = c;
				}
			}
			return result;
		}
		private static char downcase(char ch)
		{
			char result;
			if (ch < '\u0080')
			{
				if ('A' <= ch && ch <= 'Z')
				{
					result = ch + ' ';
				}
				else
				{
					result = ch;
				}
			}
			else
			{
				char c = char.ToLower(ch);
				if (ch >= '\u0080' && c < '\u0080')
				{
					result = ch;
				}
				else
				{
					result = c;
				}
			}
			return result;
		}
		private static int toASCIIHexDigit(int c)
		{
			int result;
			if (c < 48)
			{
				result = -1;
			}
			else
			{
				if (c <= 57)
				{
					result = c - 48;
				}
				else
				{
					c |= 32;
					if (97 <= c && c <= 102)
					{
						result = c - 97 + 10;
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}
		private static bool parseDisjunction(CompilerState state)
		{
			bool result;
			using (new StackOverflowVerifier(1024))
			{
				if (!BuiltinRegExp.parseAlternative(state))
				{
					result = false;
				}
				else
				{
					char[] cpbegin = state.cpbegin;
					int cp = state.cp;
					if (cp != cpbegin.Length && cpbegin[cp] == '|')
					{
						state.cp++;
						RENode rENode = new RENode(1);
						rENode.kid = state.result;
						if (!BuiltinRegExp.parseDisjunction(state))
						{
							result = false;
							return result;
						}
						rENode.kid2 = state.result;
						state.result = rENode;
						state.progLength += 9;
					}
					result = true;
				}
			}
			return result;
		}
		private static bool parseAlternative(CompilerState state)
		{
			RENode rENode = null;
			RENode rENode2 = null;
			char[] cpbegin = state.cpbegin;
			bool result;
			while (state.cp != state.cpend && cpbegin[state.cp] != '|' && (state.parenNesting == 0 || cpbegin[state.cp] != ')'))
			{
				if (!BuiltinRegExp.parseTerm(state))
				{
					result = false;
					return result;
				}
				if (rENode == null)
				{
					rENode = state.result;
				}
				else
				{
					if (rENode2 == null)
					{
						rENode.next = state.result;
						rENode2 = state.result;
						while (rENode2.next != null)
						{
							rENode2 = rENode2.next;
						}
					}
					else
					{
						rENode2.next = state.result;
						rENode2 = rENode2.next;
						while (rENode2.next != null)
						{
							rENode2 = rENode2.next;
						}
					}
				}
			}
			if (rENode == null)
			{
				state.result = new RENode(0);
			}
			else
			{
				state.result = rENode;
			}
			result = true;
			return result;
		}
		private static bool calculateBitmapSize(CompilerState state, RENode target, char[] src, int index, int end)
		{
			char c = '\0';
			int num = 0;
			bool flag = false;
			target.bmsize = 0;
			bool result;
			if (index == end)
			{
				result = true;
			}
			else
			{
				if (src[index] == '^')
				{
					index++;
				}
				while (index != end)
				{
					int num2 = 2;
					char c2 = src[index];
					int num3;
					if (c2 != '\\')
					{
						num3 = (int)src[index++];
					}
					else
					{
						index++;
						char c3 = src[index++];
						c2 = c3;
						if (c2 <= 'D')
						{
							switch (c2)
							{
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							{
								int num4 = (int)(c3 - '0');
								c3 = src[index];
								if ('0' <= c3 && c3 <= '7')
								{
									index++;
									num4 = 8 * num4 + (int)(c3 - '0');
									c3 = src[index];
									if ('0' <= c3 && c3 <= '7')
									{
										index++;
										int num5 = 8 * num4 + (int)(c3 - '0');
										if (num5 <= 255)
										{
											num4 = num5;
										}
										else
										{
											index--;
										}
									}
								}
								num3 = num4;
								break;
							}
							default:
								if (c2 != 'D')
								{
									goto IL_30F;
								}
								goto IL_242;
							}
						}
						else
						{
							if (c2 == 'S' || c2 == 'W')
							{
								goto IL_242;
							}
							switch (c2)
							{
							case 'b':
								num3 = 8;
								goto IL_318;
							case 'c':
								if (index + 1 < end && char.IsLetter(src[index + 1]))
								{
									num3 = (int)((ushort)(src[index++] & '\u001f'));
								}
								else
								{
									num3 = 92;
								}
								goto IL_318;
							case 'd':
								if (flag)
								{
									BuiltinRegExp.reportError("msg.bad.range", "");
									result = false;
									return result;
								}
								num3 = 57;
								goto IL_318;
							case 'e':
							case 'g':
							case 'h':
							case 'i':
							case 'j':
							case 'k':
							case 'l':
							case 'm':
							case 'o':
							case 'p':
							case 'q':
								goto IL_30F;
							case 'f':
								num3 = 12;
								goto IL_318;
							case 'n':
								num3 = 10;
								goto IL_318;
							case 'r':
								num3 = 13;
								goto IL_318;
							case 's':
							case 'w':
								goto IL_242;
							case 't':
								num3 = 9;
								goto IL_318;
							case 'u':
								num2 += 2;
								break;
							case 'v':
								num3 = 11;
								goto IL_318;
							case 'x':
								break;
							default:
								goto IL_30F;
							}
							int num4 = 0;
							int num5 = 0;
							while (num5 < num2 && index < end)
							{
								c3 = src[index++];
								num4 = ScriptConvert.XDigitToInt((int)c3, num4);
								if (num4 < 0)
								{
									index -= num5 + 1;
									num4 = 92;
									break;
								}
								num5++;
							}
							num3 = num4;
						}
						IL_318:
						goto IL_32C;
						IL_30F:
						num3 = (int)c3;
						goto IL_318;
						IL_242:
						if (flag)
						{
							BuiltinRegExp.reportError("msg.bad.range", "");
							result = false;
							return result;
						}
						target.bmsize = 65535;
						result = true;
						return result;
					}
					IL_32C:
					if (flag)
					{
						if ((int)c > num3)
						{
							BuiltinRegExp.reportError("msg.bad.range", "");
							result = false;
							return result;
						}
						flag = false;
					}
					else
					{
						if (index < end - 1)
						{
							if (src[index] == '-')
							{
								index++;
								flag = true;
								c = (char)num3;
								continue;
							}
						}
					}
					if ((state.flags & 2) != 0)
					{
						char c4 = BuiltinRegExp.upcase((char)num3);
						char c5 = BuiltinRegExp.downcase((char)num3);
						num3 = (int)((c4 >= c5) ? c4 : c5);
					}
					if (num3 > num)
					{
						num = num3;
					}
				}
				target.bmsize = num;
				result = true;
			}
			return result;
		}
		private static void doFlat(CompilerState state, char c)
		{
			state.result = new RENode(21);
			state.result.chr = c;
			state.result.length = 1;
			state.result.flatIndex = -1;
			state.progLength += 3;
		}
		private static int getDecimalValue(char c, CompilerState state, int maxValue, string overflowMessageId)
		{
			bool flag = false;
			int cp = state.cp;
			char[] cpbegin = state.cpbegin;
			int num = (int)(c - '0');
			while (state.cp != state.cpend)
			{
				c = cpbegin[state.cp];
				if (!BuiltinRegExp.isDigit(c))
				{
					break;
				}
				if (!flag)
				{
					int num2 = (int)(c - '0');
					if (num < (maxValue - num2) / 10)
					{
						num = num * 10 + num2;
					}
					else
					{
						flag = true;
						num = maxValue;
					}
				}
				state.cp++;
			}
			if (flag)
			{
				BuiltinRegExp.reportError(overflowMessageId, new string(cpbegin, cp, state.cp - cp));
			}
			return num;
		}
		private static bool parseTerm(CompilerState state)
		{
			char[] cpbegin = state.cpbegin;
			char c = cpbegin[state.cp++];
			int num = 2;
			int parenCount = state.parenCount;
			int cp = state.cp;
			char c2 = c;
			bool result;
			switch (c2)
			{
			case '$':
				state.result = new RENode(3);
				state.progLength++;
				result = true;
				return result;
			case '%':
			case '&':
			case '\'':
			case ',':
			case '-':
				goto IL_9AC;
			case '(':
			{
				RENode rENode = null;
				int num2 = state.cp;
				if (state.cp + 1 < state.cpend && cpbegin[state.cp] == '?' && ((c = cpbegin[state.cp + 1]) == '=' || c == '!' || c == ':'))
				{
					state.cp += 2;
					if (c == '=')
					{
						rENode = new RENode(41);
						state.progLength += 4;
					}
					else
					{
						if (c == '!')
						{
							rENode = new RENode(42);
							state.progLength += 4;
						}
					}
				}
				else
				{
					rENode = new RENode(10);
					state.progLength += 6;
					rENode.parenIndex = state.parenCount++;
				}
				state.parenNesting++;
				if (!BuiltinRegExp.parseDisjunction(state))
				{
					result = false;
					return result;
				}
				if (state.cp == state.cpend || cpbegin[state.cp] != ')')
				{
					BuiltinRegExp.reportError("msg.unterm.paren", "");
					result = false;
					return result;
				}
				state.cp++;
				state.parenNesting--;
				if (rENode != null)
				{
					rENode.kid = state.result;
					state.result = rENode;
				}
				goto IL_9F7;
			}
			case ')':
				BuiltinRegExp.reportError("msg.re.unmatched.right.paren", "");
				result = false;
				return result;
			case '*':
			case '+':
				break;
			case '.':
				state.result = new RENode(12);
				state.progLength++;
				goto IL_9F7;
			default:
				if (c2 != '?')
				{
					switch (c2)
					{
					case '[':
					{
						state.result = new RENode(50);
						int num2 = state.cp;
						state.result.startIndex = num2;
						while (state.cp < state.cpend)
						{
							if (cpbegin[state.cp] == '\\')
							{
								state.cp++;
							}
							else
							{
								if (cpbegin[state.cp] == ']')
								{
									state.result.kidlen = state.cp - num2;
									state.result.index = state.classCount++;
									if (!BuiltinRegExp.calculateBitmapSize(state, state.result, cpbegin, num2, state.cp++))
									{
										result = false;
										return result;
									}
									state.progLength += 3;
									goto IL_9F7;
								}
							}
							state.cp++;
						}
						BuiltinRegExp.reportError("msg.unterm.class", "");
						result = false;
						return result;
					}
					case '\\':
						if (state.cp < state.cpend)
						{
							c = cpbegin[state.cp++];
							c2 = c;
							if (c2 <= 'S')
							{
								switch (c2)
								{
								case '0':
								{
									int num3 = 0;
									while (state.cp < state.cpend)
									{
										c = cpbegin[state.cp];
										if (c < '0' || c > '7')
										{
											break;
										}
										state.cp++;
										int num4 = 8 * num3 + (int)(c - '0');
										if (num4 > 255)
										{
											break;
										}
										num3 = num4;
									}
									c = (char)num3;
									BuiltinRegExp.doFlat(state, c);
									goto IL_651;
								}
								case '1':
								case '2':
								case '3':
								case '4':
								case '5':
								case '6':
								case '7':
								case '8':
								case '9':
								{
									int num2 = state.cp - 1;
									int num3 = BuiltinRegExp.getDecimalValue(c, state, 65535, "msg.overlarge.backref");
									if (num3 > 9 && num3 > state.parenCount)
									{
										state.cp = num2;
										num3 = 0;
										while (state.cp < state.cpend)
										{
											c = cpbegin[state.cp];
											if (c < '0' || c > '7')
											{
												break;
											}
											state.cp++;
											int num4 = 8 * num3 + (int)(c - '0');
											if (num4 > 255)
											{
												break;
											}
											num3 = num4;
										}
										c = (char)num3;
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									}
									state.result = new RENode(20);
									state.result.parenIndex = num3 - 1;
									state.progLength += 3;
									goto IL_651;
								}
								case ':':
								case ';':
								case '<':
								case '=':
								case '>':
								case '?':
								case '@':
								case 'A':
								case 'C':
									break;
								case 'B':
									state.result = new RENode(5);
									state.progLength++;
									result = true;
									return result;
								case 'D':
									state.result = new RENode(15);
									state.progLength++;
									goto IL_651;
								default:
									if (c2 == 'S')
									{
										state.result = new RENode(19);
										state.progLength++;
										goto IL_651;
									}
									break;
								}
							}
							else
							{
								if (c2 != 'W')
								{
									switch (c2)
									{
									case 'b':
										state.result = new RENode(4);
										state.progLength++;
										result = true;
										return result;
									case 'c':
										if (state.cp < state.cpend && char.IsLetter(cpbegin[state.cp]))
										{
											c = (cpbegin[state.cp++] & '\u001f');
										}
										else
										{
											state.cp--;
											c = '\\';
										}
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									case 'd':
										state.result = new RENode(14);
										state.progLength++;
										goto IL_651;
									case 'e':
									case 'g':
									case 'h':
									case 'i':
									case 'j':
									case 'k':
									case 'l':
									case 'm':
									case 'o':
									case 'p':
									case 'q':
										goto IL_606;
									case 'f':
										c = '\f';
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									case 'n':
										c = '\n';
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									case 'r':
										c = '\r';
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									case 's':
										state.result = new RENode(18);
										state.progLength++;
										goto IL_651;
									case 't':
										c = '\t';
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									case 'u':
										num += 2;
										break;
									case 'v':
										c = '\v';
										BuiltinRegExp.doFlat(state, c);
										goto IL_651;
									case 'w':
										state.result = new RENode(16);
										state.progLength++;
										goto IL_651;
									case 'x':
										break;
									default:
										goto IL_606;
									}
									int num5 = 0;
									int num6 = 0;
									while (num6 < num && state.cp < state.cpend)
									{
										c = cpbegin[state.cp++];
										num5 = ScriptConvert.XDigitToInt((int)c, num5);
										if (num5 < 0)
										{
											state.cp -= num6 + 2;
											num5 = (int)cpbegin[state.cp++];
											break;
										}
										num6++;
									}
									c = (char)num5;
									BuiltinRegExp.doFlat(state, c);
									goto IL_651;
								}
								state.result = new RENode(17);
								state.progLength++;
								goto IL_651;
							}
							IL_606:
							state.result = new RENode(21);
							state.result.chr = c;
							state.result.length = 1;
							state.result.flatIndex = state.cp - 1;
							state.progLength += 3;
							IL_651:
							goto IL_9F7;
						}
						BuiltinRegExp.reportError("msg.trail.backslash", "");
						result = false;
						return result;
					case ']':
						goto IL_9AC;
					case '^':
						state.result = new RENode(2);
						state.progLength++;
						result = true;
						return result;
					default:
						goto IL_9AC;
					}
				}
				break;
			}
			BuiltinRegExp.reportError("msg.bad.quant", Convert.ToString(cpbegin[state.cp - 1]));
			result = false;
			return result;
			IL_9AC:
			state.result = new RENode(21);
			state.result.chr = c;
			state.result.length = 1;
			state.result.flatIndex = state.cp - 1;
			state.progLength += 3;
			IL_9F7:
			RENode result2 = state.result;
			if (state.cp == state.cpend)
			{
				result = true;
			}
			else
			{
				bool flag = false;
				c2 = cpbegin[state.cp];
				switch (c2)
				{
				case '*':
					state.result = new RENode(6);
					state.result.min = 0;
					state.result.max = -1;
					state.progLength += 8;
					flag = true;
					break;
				case '+':
					state.result = new RENode(6);
					state.result.min = 1;
					state.result.max = -1;
					state.progLength += 8;
					flag = true;
					break;
				default:
					if (c2 != '?')
					{
						if (c2 == '{')
						{
							int num7 = -1;
							int cp2 = state.cp;
							c = cpbegin[++state.cp];
							if (BuiltinRegExp.isDigit(c))
							{
								state.cp++;
								int decimalValue = BuiltinRegExp.getDecimalValue(c, state, 65535, "msg.overlarge.min");
								c = cpbegin[state.cp];
								if (c == ',')
								{
									c = cpbegin[++state.cp];
									if (BuiltinRegExp.isDigit(c))
									{
										state.cp++;
										num7 = BuiltinRegExp.getDecimalValue(c, state, 65535, "msg.overlarge.max");
										c = cpbegin[state.cp];
										if (decimalValue > num7)
										{
											BuiltinRegExp.reportError("msg.max.lt.min", Convert.ToString(cpbegin[state.cp]));
											result = false;
											return result;
										}
									}
								}
								else
								{
									num7 = decimalValue;
								}
								if (c == '}')
								{
									state.result = new RENode(6);
									state.result.min = decimalValue;
									state.result.max = num7;
									state.progLength += 12;
									flag = true;
									if (state.cp + 1 != state.cpend)
									{
										char c3 = cpbegin[state.cp + 1];
										if (c3 == '{')
										{
											string text = string.Empty;
											int num6 = 2;
											while (state.cp + num6 != state.cpend)
											{
												if (cpbegin[state.cp + num6] == '}')
												{
													break;
												}
												text += cpbegin[state.cp + num6];
												num6++;
											}
											BuiltinRegExp.reportError("msg.bad.quant", text);
										}
									}
								}
							}
							if (!flag)
							{
								state.cp = cp2;
							}
						}
					}
					else
					{
						state.result = new RENode(6);
						state.result.min = 0;
						state.result.max = 1;
						state.progLength += 8;
						flag = true;
					}
					break;
				}
				if (!flag)
				{
					result = true;
				}
				else
				{
					state.cp++;
					state.result.kid = result2;
					state.result.parenIndex = parenCount;
					state.result.parenCount = state.parenCount - parenCount;
					if (state.cp < state.cpend && cpbegin[state.cp] == '?')
					{
						state.cp++;
						state.result.greedy = false;
					}
					else
					{
						state.result.greedy = true;
					}
					result = true;
				}
			}
			return result;
		}
		private static void resolveForwardJump(sbyte[] array, int from, int pc)
		{
			if (from > pc)
			{
				throw Context.CodeBug();
			}
			BuiltinRegExp.addIndex(array, from, pc - from);
		}
		private static int getOffset(sbyte[] array, int pc)
		{
			return BuiltinRegExp.getIndex(array, pc);
		}
		private static int addIndex(sbyte[] array, int pc, int index)
		{
			if (index < 0)
			{
				throw Context.CodeBug();
			}
			if (index > 65535)
			{
				throw Context.ReportRuntimeError("Too complex regexp");
			}
			array[pc] = (sbyte)(index >> 8);
			array[pc + 1] = (sbyte)index;
			return pc + 2;
		}
		private static int getIndex(sbyte[] array, int pc)
		{
			return ((int)array[pc] & 255) << 8 | ((int)array[pc + 1] & 255);
		}
		private static int emitREBytecode(CompilerState state, RECompiled re, int pc, RENode t)
		{
			sbyte[] program = re.program;
			while (t != null)
			{
				program[pc++] = t.op;
				sbyte op = t.op;
				if (op <= 10)
				{
					switch (op)
					{
					case 0:
						pc--;
						break;
					case 1:
					{
						RENode kid = t.kid2;
						int from = pc;
						pc += 2;
						pc = BuiltinRegExp.emitREBytecode(state, re, pc, t.kid);
						program[pc++] = 23;
						int from2 = pc;
						pc += 2;
						BuiltinRegExp.resolveForwardJump(program, from, pc);
						pc = BuiltinRegExp.emitREBytecode(state, re, pc, kid);
						program[pc++] = 23;
						from = pc;
						pc += 2;
						BuiltinRegExp.resolveForwardJump(program, from2, pc);
						BuiltinRegExp.resolveForwardJump(program, from, pc);
						break;
					}
					default:
						if (op != 6)
						{
							if (op == 10)
							{
								pc = BuiltinRegExp.addIndex(program, pc, t.parenIndex);
								pc = BuiltinRegExp.emitREBytecode(state, re, pc, t.kid);
								program[pc++] = 11;
								pc = BuiltinRegExp.addIndex(program, pc, t.parenIndex);
							}
						}
						else
						{
							if (t.min == 0 && t.max == -1)
							{
								program[pc - 1] = (t.greedy ? 7 : 45);
							}
							else
							{
								if (t.min == 0 && t.max == 1)
								{
									program[pc - 1] = (t.greedy ? 9 : 47);
								}
								else
								{
									if (t.min == 1 && t.max == -1)
									{
										program[pc - 1] = (t.greedy ? 8 : 46);
									}
									else
									{
										if (!t.greedy)
										{
											program[pc - 1] = 48;
										}
										pc = BuiltinRegExp.addIndex(program, pc, t.min);
										pc = BuiltinRegExp.addIndex(program, pc, t.max + 1);
									}
								}
							}
							pc = BuiltinRegExp.addIndex(program, pc, t.parenCount);
							pc = BuiltinRegExp.addIndex(program, pc, t.parenIndex);
							int from2 = pc;
							pc += 2;
							pc = BuiltinRegExp.emitREBytecode(state, re, pc, t.kid);
							program[pc++] = 49;
							BuiltinRegExp.resolveForwardJump(program, from2, pc);
						}
						break;
					}
				}
				else
				{
					switch (op)
					{
					case 20:
						pc = BuiltinRegExp.addIndex(program, pc, t.parenIndex);
						break;
					case 21:
						if (t.flatIndex != -1)
						{
							while (t.next != null && t.next.op == 21 && t.flatIndex + t.length == t.next.flatIndex)
							{
								t.length += t.next.length;
								t.next = t.next.next;
							}
						}
						if (t.flatIndex != -1 && t.length > 1)
						{
							if ((state.flags & 2) != 0)
							{
								program[pc - 1] = 32;
							}
							else
							{
								program[pc - 1] = 21;
							}
							pc = BuiltinRegExp.addIndex(program, pc, t.flatIndex);
							pc = BuiltinRegExp.addIndex(program, pc, t.length);
						}
						else
						{
							if (t.chr < 'Ā')
							{
								if ((state.flags & 2) != 0)
								{
									program[pc - 1] = 33;
								}
								else
								{
									program[pc - 1] = 22;
								}
								program[pc++] = (sbyte)t.chr;
							}
							else
							{
								if ((state.flags & 2) != 0)
								{
									program[pc - 1] = 35;
								}
								else
								{
									program[pc - 1] = 28;
								}
								pc = BuiltinRegExp.addIndex(program, pc, (int)t.chr);
							}
						}
						break;
					default:
						switch (op)
						{
						case 41:
						{
							int from2 = pc;
							pc += 2;
							pc = BuiltinRegExp.emitREBytecode(state, re, pc, t.kid);
							program[pc++] = 43;
							BuiltinRegExp.resolveForwardJump(program, from2, pc);
							break;
						}
						case 42:
						{
							int from2 = pc;
							pc += 2;
							pc = BuiltinRegExp.emitREBytecode(state, re, pc, t.kid);
							program[pc++] = 44;
							BuiltinRegExp.resolveForwardJump(program, from2, pc);
							break;
						}
						default:
							if (op == 50)
							{
								pc = BuiltinRegExp.addIndex(program, pc, t.index);
								re.classList[t.index] = new RECharSet(t.bmsize, t.startIndex, t.kidlen);
							}
							break;
						}
						break;
					}
				}
				IL_4C8:
				t = t.next;
				continue;
				goto IL_4C8;
			}
			return pc;
		}
		private static void pushProgState(REGlobalData gData, int min, int max, REBackTrackData backTrackLastToSave, int continuation_pc, int continuation_op)
		{
			gData.stateStackTop = new REProgState(gData.stateStackTop, min, max, gData.cp, backTrackLastToSave, continuation_pc, continuation_op);
		}
		private static REProgState popProgState(REGlobalData gData)
		{
			REProgState stateStackTop = gData.stateStackTop;
			gData.stateStackTop = stateStackTop.previous;
			return stateStackTop;
		}
		private static void pushBackTrackState(REGlobalData gData, sbyte op, int target)
		{
			gData.backTrackStackTop = new REBackTrackData(gData, (int)op, target);
		}
		private static bool flatNMatcher(REGlobalData gData, int matchChars, int length, char[] chars, int end)
		{
			bool result;
			if (gData.cp + length > end)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < length; i++)
				{
					if (gData.regexp.source[matchChars + i] != chars[gData.cp + i])
					{
						result = false;
						return result;
					}
				}
				gData.cp += length;
				result = true;
			}
			return result;
		}
		private static bool flatNIMatcher(REGlobalData gData, int matchChars, int length, char[] chars, int end)
		{
			bool result;
			if (gData.cp + length > end)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < length; i++)
				{
					if (BuiltinRegExp.upcase(gData.regexp.source[matchChars + i]) != BuiltinRegExp.upcase(chars[gData.cp + i]))
					{
						result = false;
						return result;
					}
				}
				gData.cp += length;
				result = true;
			}
			return result;
		}
		private static bool backrefMatcher(REGlobalData gData, int parenIndex, char[] chars, int end)
		{
			int num = gData.parens_index(parenIndex);
			bool result;
			if (num == -1)
			{
				result = true;
			}
			else
			{
				int num2 = gData.parens_length(parenIndex);
				if (gData.cp + num2 > end)
				{
					result = false;
				}
				else
				{
					if ((gData.regexp.flags & 2) != 0)
					{
						for (int i = 0; i < num2; i++)
						{
							if (BuiltinRegExp.upcase(chars[num + i]) != BuiltinRegExp.upcase(chars[gData.cp + i]))
							{
								result = false;
								return result;
							}
						}
					}
					else
					{
						for (int i = 0; i < num2; i++)
						{
							if (chars[num + i] != chars[gData.cp + i])
							{
								result = false;
								return result;
							}
						}
					}
					gData.cp += num2;
					result = true;
				}
			}
			return result;
		}
		private static void addCharacterToCharSet(RECharSet cs, char c)
		{
			int num = (int)(c / '\b');
			if ((int)c > cs.length)
			{
				throw new ApplicationException();
			}
			sbyte[] expr_2C_cp_0 = cs.bits;
			int expr_2C_cp_1 = num;
			expr_2C_cp_0[expr_2C_cp_1] |= (sbyte)(1 << (int)(c & '\a'));
		}
		private static void addCharacterRangeToCharSet(RECharSet cs, char c1, char c2)
		{
			int num = (int)(c1 / '\b');
			int num2 = (int)(c2 / '\b');
			if ((int)c2 > cs.length || c1 > c2)
			{
				throw new ApplicationException();
			}
			c1 &= '\a';
			c2 &= '\a';
			if (num == num2)
			{
				sbyte[] expr_58_cp_0 = cs.bits;
				int expr_58_cp_1 = num;
				expr_58_cp_0[expr_58_cp_1] |= (sbyte)(255 >> (int)('\a' - (c2 - c1)) << (int)c1);
			}
			else
			{
				sbyte[] expr_86_cp_0 = cs.bits;
				int expr_86_cp_1 = num;
				expr_86_cp_0[expr_86_cp_1] |= (sbyte)(255 << (int)c1);
				for (int i = num + 1; i < num2; i++)
				{
					cs.bits[i] = -1;
				}
				sbyte[] expr_C4_cp_0 = cs.bits;
				int expr_C4_cp_1 = num2;
				expr_C4_cp_0[expr_C4_cp_1] |= (sbyte)(255 >> (int)('\a' - c2));
			}
		}
		private static void processCharSet(REGlobalData gData, RECharSet charSet)
		{
			Monitor.Enter(charSet);
			try
			{
				if (!charSet.converted)
				{
					BuiltinRegExp.processCharSetImpl(gData, charSet);
					charSet.converted = true;
				}
			}
			finally
			{
				Monitor.Exit(charSet);
			}
		}
		private static void processCharSetImpl(REGlobalData gData, RECharSet charSet)
		{
			int num = charSet.startIndex;
			int num2 = num + charSet.strlength;
			char c = '\0';
			bool flag = false;
			charSet.sense = true;
			int num3 = charSet.length / 8 + 1;
			charSet.bits = new sbyte[num3];
			if (num != num2)
			{
				if (gData.regexp.source[num] == '^')
				{
					charSet.sense = false;
					num++;
				}
				while (num != num2)
				{
					int num4 = 2;
					char c2 = gData.regexp.source[num];
					char c3;
					if (c2 != '\\')
					{
						c3 = gData.regexp.source[num++];
					}
					else
					{
						num++;
						char c4 = gData.regexp.source[num++];
						c2 = c4;
						if (c2 <= 'D')
						{
							switch (c2)
							{
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							{
								int num5 = (int)(c4 - '0');
								c4 = gData.regexp.source[num];
								if ('0' <= c4 && c4 <= '7')
								{
									num++;
									num5 = 8 * num5 + (int)(c4 - '0');
									c4 = gData.regexp.source[num];
									if ('0' <= c4 && c4 <= '7')
									{
										num++;
										int i = 8 * num5 + (int)(c4 - '0');
										if (i <= 255)
										{
											num5 = i;
										}
										else
										{
											num--;
										}
									}
								}
								c3 = (char)num5;
								break;
							}
							default:
								if (c2 != 'D')
								{
									goto IL_475;
								}
								BuiltinRegExp.addCharacterRangeToCharSet(charSet, '\0', '/');
								BuiltinRegExp.addCharacterRangeToCharSet(charSet, ':', (char)charSet.length);
								continue;
							}
						}
						else
						{
							int i;
							if (c2 == 'S')
							{
								for (i = charSet.length; i >= 0; i--)
								{
									if (!BuiltinRegExp.isREWhiteSpace(i))
									{
										BuiltinRegExp.addCharacterToCharSet(charSet, (char)i);
									}
								}
								continue;
							}
							if (c2 == 'W')
							{
								for (i = charSet.length; i >= 0; i--)
								{
									if (!BuiltinRegExp.isWord((char)i))
									{
										BuiltinRegExp.addCharacterToCharSet(charSet, (char)i);
									}
								}
								continue;
							}
							switch (c2)
							{
							case 'b':
								c3 = '\b';
								goto IL_47E;
							case 'c':
								if (num + 1 < num2 && BuiltinRegExp.isWord(gData.regexp.source[num + 1]))
								{
									c3 = (gData.regexp.source[num++] & '\u001f');
								}
								else
								{
									num--;
									c3 = '\\';
								}
								goto IL_47E;
							case 'd':
								BuiltinRegExp.addCharacterRangeToCharSet(charSet, '0', '9');
								continue;
							case 'e':
							case 'g':
							case 'h':
							case 'i':
							case 'j':
							case 'k':
							case 'l':
							case 'm':
							case 'o':
							case 'p':
							case 'q':
								goto IL_475;
							case 'f':
								c3 = '\f';
								goto IL_47E;
							case 'n':
								c3 = '\n';
								goto IL_47E;
							case 'r':
								c3 = '\r';
								goto IL_47E;
							case 's':
								for (i = charSet.length; i >= 0; i--)
								{
									if (BuiltinRegExp.isREWhiteSpace(i))
									{
										BuiltinRegExp.addCharacterToCharSet(charSet, (char)i);
									}
								}
								continue;
							case 't':
								c3 = '\t';
								goto IL_47E;
							case 'u':
								num4 += 2;
								break;
							case 'v':
								c3 = '\v';
								goto IL_47E;
							case 'w':
								for (i = charSet.length; i >= 0; i--)
								{
									if (BuiltinRegExp.isWord((char)i))
									{
										BuiltinRegExp.addCharacterToCharSet(charSet, (char)i);
									}
								}
								continue;
							case 'x':
								break;
							default:
								goto IL_475;
							}
							int num5 = 0;
							i = 0;
							while (i < num4 && num < num2)
							{
								c4 = gData.regexp.source[num++];
								int num6 = BuiltinRegExp.toASCIIHexDigit((int)c4);
								if (num6 < 0)
								{
									num -= i + 1;
									num5 = 92;
									break;
								}
								num5 = (num5 << 4 | num6);
								i++;
							}
							c3 = (char)num5;
						}
						IL_47E:
						goto IL_49B;
						IL_475:
						c3 = c4;
					}
					IL_49B:
					if (flag)
					{
						if ((gData.regexp.flags & 2) != 0)
						{
							BuiltinRegExp.addCharacterRangeToCharSet(charSet, BuiltinRegExp.upcase(c), BuiltinRegExp.upcase(c3));
							BuiltinRegExp.addCharacterRangeToCharSet(charSet, BuiltinRegExp.downcase(c), BuiltinRegExp.downcase(c3));
						}
						else
						{
							BuiltinRegExp.addCharacterRangeToCharSet(charSet, c, c3);
						}
						flag = false;
					}
					else
					{
						if ((gData.regexp.flags & 2) != 0)
						{
							BuiltinRegExp.addCharacterToCharSet(charSet, BuiltinRegExp.upcase(c3));
							BuiltinRegExp.addCharacterToCharSet(charSet, BuiltinRegExp.downcase(c3));
						}
						else
						{
							BuiltinRegExp.addCharacterToCharSet(charSet, c3);
						}
						if (num < num2 - 1)
						{
							if (gData.regexp.source[num] == '-')
							{
								num++;
								flag = true;
								c = c3;
							}
						}
					}
				}
			}
		}
		private static bool classMatcher(REGlobalData gData, RECharSet charSet, char ch)
		{
			if (!charSet.converted)
			{
				BuiltinRegExp.processCharSet(gData, charSet);
			}
			int num = (int)(ch / '\b');
			bool result;
			if (charSet.sense)
			{
				if (charSet.length == 0 || (int)ch > charSet.length || ((int)charSet.bits[num] & 1 << (int)(ch & '\a')) == 0)
				{
					result = false;
					return result;
				}
			}
			else
			{
				if (charSet.length != 0 && (int)ch <= charSet.length && ((int)charSet.bits[num] & 1 << (int)(ch & '\a')) != 0)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		private static bool executeREBytecode(REGlobalData gData, char[] chars, int end)
		{
			int num = 0;
			sbyte[] program = gData.regexp.program;
			bool flag = false;
			int num2 = 0;
			int num3 = 53;
			if (BuiltinRegExp.debug)
			{
				Console.Out.WriteLine(string.Concat(new object[]
				{
					"Input = \"",
					new string(chars),
					"\", start at ",
					gData.cp
				}));
			}
			int num4 = (int)program[num++];
			while (true)
			{
				if (BuiltinRegExp.debug)
				{
					Console.Out.WriteLine(string.Concat(new object[]
					{
						"Testing at ",
						gData.cp,
						", op = ",
						num4
					}));
				}
				switch (num4)
				{
				case 0:
					flag = true;
					goto IL_E49;
				case 1:
				{
					BuiltinRegExp.pushProgState(gData, 0, 0, null, num2, num3);
					int target = num + BuiltinRegExp.getOffset(program, num);
					sbyte op = program[target++];
					BuiltinRegExp.pushBackTrackState(gData, op, target);
					num += 2;
					num4 = (int)program[num++];
					continue;
				}
				case 2:
					if (gData.cp != 0)
					{
						if (!gData.multiline && (gData.regexp.flags & 4) == 0)
						{
							flag = false;
							goto IL_E49;
						}
						if (!BuiltinRegExp.isLineTerm(chars[gData.cp - 1]))
						{
							flag = false;
							goto IL_E49;
						}
					}
					flag = true;
					goto IL_E49;
				case 3:
					if (gData.cp != end)
					{
						if (!gData.multiline && (gData.regexp.flags & 4) == 0)
						{
							flag = false;
							goto IL_E49;
						}
						if (!BuiltinRegExp.isLineTerm(chars[gData.cp]))
						{
							flag = false;
							goto IL_E49;
						}
					}
					flag = true;
					goto IL_E49;
				case 4:
					flag = ((gData.cp == 0 || !BuiltinRegExp.isWord(chars[gData.cp - 1])) ^ (gData.cp >= end || !BuiltinRegExp.isWord(chars[gData.cp])));
					goto IL_E49;
				case 5:
					flag = ((gData.cp == 0 || !BuiltinRegExp.isWord(chars[gData.cp - 1])) ^ (gData.cp < end && BuiltinRegExp.isWord(chars[gData.cp])));
					goto IL_E49;
				case 6:
				case 7:
				case 8:
				case 9:
				case 45:
				case 46:
				case 47:
				case 48:
				{
					bool flag2 = false;
					int num5 = num4;
					switch (num5)
					{
					case 6:
						flag2 = true;
						goto IL_9C5;
					case 7:
						flag2 = true;
						goto IL_98C;
					case 8:
						flag2 = true;
						goto IL_99F;
					case 9:
						flag2 = true;
						goto IL_9B2;
					default:
						switch (num5)
						{
						case 45:
							goto IL_98C;
						case 46:
							goto IL_99F;
						case 47:
							goto IL_9B2;
						case 48:
							goto IL_9C5;
						}
						goto Block_46;
					}
					IL_9EC:
					int num6;
					int max;
					BuiltinRegExp.pushProgState(gData, num6, max, null, num2, num3);
					if (flag2)
					{
						num3 = 51;
						num2 = num;
						BuiltinRegExp.pushBackTrackState(gData, 51, num);
						num += 6;
						num4 = (int)program[num++];
					}
					else
					{
						if (num6 != 0)
						{
							num3 = 52;
							num2 = num;
							num += 6;
							num4 = (int)program[num++];
						}
						else
						{
							BuiltinRegExp.pushBackTrackState(gData, 52, num);
							BuiltinRegExp.popProgState(gData);
							num += 4;
							num += BuiltinRegExp.getOffset(program, num);
							num4 = (int)program[num++];
						}
					}
					continue;
					IL_9C5:
					num6 = BuiltinRegExp.getOffset(program, num);
					num += 2;
					max = BuiltinRegExp.getOffset(program, num) - 1;
					num += 2;
					goto IL_9EC;
					IL_9B2:
					num6 = 0;
					max = 1;
					goto IL_9EC;
					IL_99F:
					num6 = 1;
					max = -1;
					goto IL_9EC;
					IL_98C:
					num6 = 0;
					max = -1;
					goto IL_9EC;
				}
				case 10:
				{
					int index = BuiltinRegExp.getIndex(program, num);
					num += 2;
					gData.set_parens(index, gData.cp, 0);
					num4 = (int)program[num++];
					continue;
				}
				case 11:
				{
					int index = BuiltinRegExp.getIndex(program, num);
					num += 2;
					int num7 = gData.parens_index(index);
					gData.set_parens(index, num7, gData.cp - num7);
					if (index > gData.lastParen)
					{
						gData.lastParen = index;
					}
					num4 = (int)program[num++];
					continue;
				}
				case 12:
					flag = (gData.cp != end && !BuiltinRegExp.isLineTerm(chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 14:
					flag = (gData.cp != end && BuiltinRegExp.isDigit(chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 15:
					flag = (gData.cp != end && !BuiltinRegExp.isDigit(chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 16:
					flag = (gData.cp != end && BuiltinRegExp.isWord(chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 17:
					flag = (gData.cp != end && !BuiltinRegExp.isWord(chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 18:
					flag = (gData.cp != end && BuiltinRegExp.isREWhiteSpace((int)chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 19:
					flag = (gData.cp != end && !BuiltinRegExp.isREWhiteSpace((int)chars[gData.cp]));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				case 20:
				{
					int index = BuiltinRegExp.getIndex(program, num);
					num += 2;
					flag = BuiltinRegExp.backrefMatcher(gData, index, chars, end);
					goto IL_E49;
				}
				case 21:
				{
					int num8 = BuiltinRegExp.getIndex(program, num);
					num += 2;
					int index2 = BuiltinRegExp.getIndex(program, num);
					num += 2;
					flag = BuiltinRegExp.flatNMatcher(gData, num8, index2, chars, end);
					goto IL_E49;
				}
				case 22:
				{
					char c = (char)((int)program[num++] & 255);
					flag = (gData.cp != end && chars[gData.cp] == c);
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				}
				case 23:
				{
					REProgState rEProgState = BuiltinRegExp.popProgState(gData);
					num2 = rEProgState.continuation_pc;
					num3 = rEProgState.continuation_op;
					int num8 = BuiltinRegExp.getOffset(program, num);
					num += num8;
					num4 = (int)program[num++];
					continue;
				}
				case 28:
				{
					char c = (char)BuiltinRegExp.getIndex(program, num);
					num += 2;
					flag = (gData.cp != end && chars[gData.cp] == c);
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				}
				case 32:
				{
					int num8 = BuiltinRegExp.getIndex(program, num);
					num += 2;
					int index2 = BuiltinRegExp.getIndex(program, num);
					num += 2;
					flag = BuiltinRegExp.flatNIMatcher(gData, num8, index2, chars, end);
					goto IL_E49;
				}
				case 33:
				{
					char c = (char)((int)program[num++] & 255);
					flag = (gData.cp != end && BuiltinRegExp.upcase(chars[gData.cp]) == BuiltinRegExp.upcase(c));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				}
				case 35:
				{
					char c = (char)BuiltinRegExp.getIndex(program, num);
					num += 2;
					flag = (gData.cp != end && BuiltinRegExp.upcase(chars[gData.cp]) == BuiltinRegExp.upcase(c));
					if (flag)
					{
						gData.cp++;
					}
					goto IL_E49;
				}
				case 41:
				case 42:
				{
					BuiltinRegExp.pushProgState(gData, 0, 0, gData.backTrackStackTop, num2, num3);
					sbyte op2;
					if (num4 == 41)
					{
						op2 = 43;
					}
					else
					{
						op2 = 44;
					}
					BuiltinRegExp.pushBackTrackState(gData, op2, num + BuiltinRegExp.getOffset(program, num));
					num += 2;
					num4 = (int)program[num++];
					continue;
				}
				case 43:
				case 44:
				{
					REProgState rEProgState = BuiltinRegExp.popProgState(gData);
					gData.cp = rEProgState.index;
					gData.backTrackStackTop = rEProgState.backTrack;
					num2 = rEProgState.continuation_pc;
					num3 = rEProgState.continuation_op;
					if (flag)
					{
						flag = (num4 == 43);
					}
					else
					{
						if (num4 != 43)
						{
							flag = true;
						}
					}
					goto IL_E49;
				}
				case 49:
					num = num2;
					num4 = num3;
					continue;
				case 50:
				{
					int index3 = BuiltinRegExp.getIndex(program, num);
					num += 2;
					if (gData.cp != end)
					{
						if (BuiltinRegExp.classMatcher(gData, gData.regexp.classList[index3], chars[gData.cp]))
						{
							gData.cp++;
							flag = true;
							goto IL_E49;
						}
					}
					flag = false;
					goto IL_E49;
				}
				case 51:
				{
					REProgState rEProgState = BuiltinRegExp.popProgState(gData);
					if (!flag)
					{
						if (rEProgState.min == 0)
						{
							flag = true;
						}
						num2 = rEProgState.continuation_pc;
						num3 = rEProgState.continuation_op;
						num += 4;
						num += BuiltinRegExp.getOffset(program, num);
						goto IL_E49;
					}
					if (rEProgState.min == 0 && gData.cp == rEProgState.index)
					{
						flag = false;
						num2 = rEProgState.continuation_pc;
						num3 = rEProgState.continuation_op;
						num += 4;
						num += BuiltinRegExp.getOffset(program, num);
						goto IL_E49;
					}
					int num9 = rEProgState.min;
					int num10 = rEProgState.max;
					if (num9 != 0)
					{
						num9--;
					}
					if (num10 != -1)
					{
						num10--;
					}
					if (num10 == 0)
					{
						flag = true;
						num2 = rEProgState.continuation_pc;
						num3 = rEProgState.continuation_op;
						num += 4;
						num += BuiltinRegExp.getOffset(program, num);
						goto IL_E49;
					}
					BuiltinRegExp.pushProgState(gData, num9, num10, null, rEProgState.continuation_pc, rEProgState.continuation_op);
					num3 = 51;
					num2 = num;
					BuiltinRegExp.pushBackTrackState(gData, 51, num);
					int index4 = BuiltinRegExp.getIndex(program, num);
					num += 2;
					int index = BuiltinRegExp.getIndex(program, num);
					num += 4;
					num4 = (int)program[num++];
					for (int i = 0; i < index4; i++)
					{
						gData.set_parens(index + i, -1, 0);
					}
					continue;
				}
				case 52:
				{
					REProgState rEProgState = BuiltinRegExp.popProgState(gData);
					if (!flag)
					{
						if (rEProgState.max == -1 || rEProgState.max > 0)
						{
							BuiltinRegExp.pushProgState(gData, rEProgState.min, rEProgState.max, null, rEProgState.continuation_pc, rEProgState.continuation_op);
							num3 = 52;
							num2 = num;
							int index4 = BuiltinRegExp.getIndex(program, num);
							num += 2;
							int index = BuiltinRegExp.getIndex(program, num);
							num += 4;
							for (int i = 0; i < index4; i++)
							{
								gData.set_parens(index + i, -1, 0);
							}
							num4 = (int)program[num++];
							continue;
						}
						num2 = rEProgState.continuation_pc;
						num3 = rEProgState.continuation_op;
						goto IL_E49;
					}
					else
					{
						if (rEProgState.min == 0 && gData.cp == rEProgState.index)
						{
							flag = false;
							num2 = rEProgState.continuation_pc;
							num3 = rEProgState.continuation_op;
							goto IL_E49;
						}
						int num9 = rEProgState.min;
						int num10 = rEProgState.max;
						if (num9 != 0)
						{
							num9--;
						}
						if (num10 != -1)
						{
							num10--;
						}
						BuiltinRegExp.pushProgState(gData, num9, num10, null, rEProgState.continuation_pc, rEProgState.continuation_op);
						if (num9 != 0)
						{
							num3 = 52;
							num2 = num;
							int index4 = BuiltinRegExp.getIndex(program, num);
							num += 2;
							int index = BuiltinRegExp.getIndex(program, num);
							num += 4;
							for (int i = 0; i < index4; i++)
							{
								gData.set_parens(index + i, -1, 0);
							}
							num4 = (int)program[num++];
						}
						else
						{
							num2 = rEProgState.continuation_pc;
							num3 = rEProgState.continuation_op;
							BuiltinRegExp.pushBackTrackState(gData, 52, num);
							BuiltinRegExp.popProgState(gData);
							num += 4;
							num += BuiltinRegExp.getOffset(program, num);
							num4 = (int)program[num++];
						}
						continue;
					}
					break;
				}
				case 53:
					goto IL_E3B;
				}
				break;
				IL_E49:
				if (!flag)
				{
					REBackTrackData backTrackStackTop = gData.backTrackStackTop;
					if (backTrackStackTop == null)
					{
						goto IL_F0B;
					}
					gData.backTrackStackTop = backTrackStackTop.previous;
					gData.lastParen = backTrackStackTop.lastParen;
					if (backTrackStackTop.parens != null)
					{
						gData.parens = new long[backTrackStackTop.parens.Length];
						backTrackStackTop.parens.CopyTo(gData.parens, 0);
					}
					gData.cp = backTrackStackTop.cp;
					gData.stateStackTop = backTrackStackTop.stateStackTop;
					num3 = gData.stateStackTop.continuation_op;
					num2 = gData.stateStackTop.continuation_pc;
					num = backTrackStackTop.continuation_pc;
					num4 = backTrackStackTop.continuation_op;
				}
				else
				{
					num4 = (int)program[num++];
				}
			}
			goto IL_E43;
			Block_46:
			throw Context.CodeBug();
			IL_E3B:
			bool result = true;
			return result;
			IL_E43:
			throw Context.CodeBug();
			IL_F0B:
			result = false;
			return result;
		}
		private static bool matchRegExp(REGlobalData gData, RECompiled re, char[] chars, int start, int end, bool multiline)
		{
			if (re.parenCount != 0)
			{
				gData.parens = new long[re.parenCount];
			}
			else
			{
				gData.parens = null;
			}
			gData.backTrackStackTop = null;
			gData.stateStackTop = null;
			gData.multiline = multiline;
			gData.regexp = re;
			gData.lastParen = 0;
			int anchorCh = gData.regexp.anchorCh;
			int i = start;
			bool result;
			while (i <= end)
			{
				if (anchorCh >= 0)
				{
					while (i != end)
					{
						char c = chars[i];
						if ((int)c == anchorCh || ((gData.regexp.flags & 2) != 0 && BuiltinRegExp.upcase(c) == BuiltinRegExp.upcase((char)anchorCh)))
						{
							goto IL_E8;
						}
						i++;
					}
					result = false;
					return result;
				}
				IL_E8:
				gData.cp = i;
				for (int j = 0; j < re.parenCount; j++)
				{
					gData.set_parens(j, -1, 0);
				}
				bool flag = BuiltinRegExp.executeREBytecode(gData, chars, end);
				gData.backTrackStackTop = null;
				gData.stateStackTop = null;
				if (!flag)
				{
					i++;
					continue;
				}
				gData.skipped = i - start;
				result = true;
				return result;
			}
			result = false;
			return result;
		}
		internal virtual object executeRegExp(Context cx, IScriptable scopeObj, RegExpImpl res, string str, int[] indexp, int matchType)
		{
			REGlobalData rEGlobalData = new REGlobalData();
			int num = indexp[0];
			char[] array = str.ToCharArray();
			int num2 = array.Length;
			if (num > num2)
			{
				num = num2;
			}
			object result;
			if (!BuiltinRegExp.matchRegExp(rEGlobalData, this.re, array, num, num2, res.multiline))
			{
				if (matchType != 2)
				{
					result = null;
				}
				else
				{
					result = Undefined.Value;
				}
			}
			else
			{
				int num3 = rEGlobalData.cp;
				int num4 = num3;
				indexp[0] = num4;
				int num5 = num4 - (num + rEGlobalData.skipped);
				int num6 = num3;
				num3 -= num5;
				object obj;
				IScriptable scriptable;
				if (matchType == 0)
				{
					obj = true;
					scriptable = null;
				}
				else
				{
					IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(scopeObj);
					obj = ScriptRuntime.NewObject(cx, topLevelScope, "Array", null);
					scriptable = (IScriptable)obj;
					string value = new string(array, num3, num5);
					scriptable.Put(0, scriptable, value);
				}
				if (this.re.parenCount == 0)
				{
					res.parens = null;
					res.lastParen = SubString.EmptySubString;
				}
				else
				{
					SubString subString = null;
					res.parens = new SubString[this.re.parenCount];
					int i = 0;
					while (i < this.re.parenCount)
					{
						int num7 = rEGlobalData.parens_index(i);
						if (num7 != -1)
						{
							int len = rEGlobalData.parens_length(i);
							subString = new SubString(array, num7, len);
							res.parens[i] = subString;
							if (matchType != 0)
							{
								string value2 = subString.ToString();
								scriptable.Put(i + 1, scriptable, value2);
							}
						}
						else
						{
							if (matchType != 0)
							{
								scriptable.Put(i + 1, scriptable, Undefined.Value);
							}
						}
						IL_1E5:
						i++;
						continue;
						goto IL_1E5;
					}
					res.lastParen = subString;
				}
				if (matchType != 0)
				{
					scriptable.Put("index", scriptable, num + rEGlobalData.skipped);
					scriptable.Put("input", scriptable, str);
				}
				if (res.lastMatch == null)
				{
					res.lastMatch = new SubString();
					res.leftContext = new SubString();
					res.rightContext = new SubString();
				}
				res.lastMatch.charArray = array;
				res.lastMatch.index = num3;
				res.lastMatch.length = num5;
				res.leftContext.charArray = array;
				if (cx.Version == Context.Versions.JS1_2)
				{
					res.leftContext.index = num;
					res.leftContext.length = rEGlobalData.skipped;
				}
				else
				{
					res.leftContext.index = 0;
					res.leftContext.length = num + rEGlobalData.skipped;
				}
				res.rightContext.charArray = array;
				res.rightContext.index = num6;
				res.rightContext.length = num2 - num6;
				result = obj;
			}
			return result;
		}
		private static void reportError(string messageId, string arg)
		{
			string message = ScriptRuntime.GetMessage(messageId, new object[]
			{
				arg
			});
			throw ScriptRuntime.ConstructError("SyntaxError", message);
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int num = 0;
			string text = null;
			int length = s.Length;
			if (length == 6)
			{
				int num2 = (int)s[0];
				if (num2 == 103)
				{
					text = "global";
					num = 3;
				}
				else
				{
					if (num2 == 115)
					{
						text = "source";
						num = 2;
					}
				}
			}
			else
			{
				if (length == 9)
				{
					int num2 = (int)s[0];
					if (num2 == 108)
					{
						text = "lastIndex";
						num = 1;
					}
					else
					{
						if (num2 == 109)
						{
							text = "multiline";
							num = 5;
						}
					}
				}
				else
				{
					if (length == 10)
					{
						text = "ignoreCase";
						num = 4;
					}
				}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				num = 0;
			}
			int result;
			if (num == 0)
			{
				result = base.FindInstanceIdInfo(s);
			}
			else
			{
				int attributes;
				switch (num)
				{
				case 1:
					attributes = 6;
					break;
				case 2:
				case 3:
				case 4:
				case 5:
					attributes = 7;
					break;
				default:
					throw new ApplicationException();
				}
				result = IdScriptableObject.InstanceIdInfo(attributes, num);
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			switch (id)
			{
			case 1:
				result = "lastIndex";
				break;
			case 2:
				result = "source";
				break;
			case 3:
				result = "global";
				break;
			case 4:
				result = "ignoreCase";
				break;
			case 5:
				result = "multiline";
				break;
			default:
				result = base.GetInstanceIdName(id);
				break;
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			object result;
			switch (id)
			{
			case 1:
				result = this.lastIndex;
				break;
			case 2:
				result = new string(this.re.source);
				break;
			case 3:
				result = ((this.re.flags & 1) != 0);
				break;
			case 4:
				result = ((this.re.flags & 2) != 0);
				break;
			case 5:
				result = ((this.re.flags & 4) != 0);
				break;
			default:
				result = base.GetInstanceIdValue(id);
				break;
			}
			return result;
		}
		protected internal override void SetInstanceIdValue(int id, object value)
		{
			if (id == 1)
			{
				this.lastIndex = ScriptConvert.ToNumber(value);
			}
			else
			{
				base.SetInstanceIdValue(id, value);
			}
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
				arity = 1;
				name = "compile";
				break;
			case 2:
				arity = 0;
				name = "toString";
				break;
			case 3:
				arity = 0;
				name = "toSource";
				break;
			case 4:
				arity = 1;
				name = "exec";
				break;
			case 5:
				arity = 1;
				name = "test";
				break;
			case 6:
				arity = 1;
				name = "prefix";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinRegExp.REGEXP_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinRegExp.REGEXP_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
					result = BuiltinRegExp.realThis(thisObj, f).compile(cx, scope, args);
					break;
				case 2:
				case 3:
					result = BuiltinRegExp.realThis(thisObj, f).ToString();
					break;
				case 4:
					result = BuiltinRegExp.realThis(thisObj, f).execSub(cx, scope, args, 1);
					break;
				case 5:
				{
					object obj = BuiltinRegExp.realThis(thisObj, f).execSub(cx, scope, args, 0);
					result = true.Equals(obj);
					break;
				}
				case 6:
					result = BuiltinRegExp.realThis(thisObj, f).execSub(cx, scope, args, 2);
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private static BuiltinRegExp realThis(IScriptable thisObj, IdFunctionObject f)
		{
			if (!(thisObj is BuiltinRegExp))
			{
				throw IdScriptableObject.IncompatibleCallError(f);
			}
			return (BuiltinRegExp)thisObj;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 4:
			{
				int num = (int)s[0];
				if (num == 101)
				{
					text = "exec";
					result = 4;
				}
				else
				{
					if (num == 116)
					{
						text = "test";
						result = 5;
					}
				}
				break;
			}
			case 6:
				text = "prefix";
				result = 6;
				break;
			case 7:
				text = "compile";
				result = 1;
				break;
			case 8:
			{
				int num = (int)s[3];
				if (num == 111)
				{
					text = "toSource";
					result = 3;
				}
				else
				{
					if (num == 116)
					{
						text = "toString";
						result = 2;
					}
				}
				break;
			}
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
