using EcmaScript.NET.Types.E4X;
using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public class BuiltinGlobal : IIdFunctionCall
	{
		private const string URI_DECODE_RESERVED = ";/?:@&=+$,#";
		private const int Id_decodeURI = 1;
		private const int Id_decodeURIComponent = 2;
		private const int Id_encodeURI = 3;
		private const int Id_encodeURIComponent = 4;
		private const int Id_escape = 5;
		private const int Id_eval = 6;
		private const int Id_isFinite = 7;
		private const int Id_isNaN = 8;
		private const int Id_isXMLName = 9;
		private const int Id_parseFloat = 10;
		private const int Id_parseInt = 11;
		private const int Id_unescape = 12;
		private const int Id_uneval = 13;
		private const int LAST_SCOPE_FUNCTION_ID = 13;
		private const int Id_new_CommonError = 14;
		private static readonly object FTAG = new object();
		public static void Init(Context cx, IScriptable scope, bool zealed)
		{
			BuiltinGlobal idcall = new BuiltinGlobal();
			for (int i = 1; i <= 13; i++)
			{
				int arity = 1;
				string text;
				switch (i)
				{
				case 1:
					text = "decodeURI";
					break;
				case 2:
					text = "decodeURIComponent";
					break;
				case 3:
					text = "encodeURI";
					break;
				case 4:
					text = "encodeURIComponent";
					break;
				case 5:
					text = "escape";
					break;
				case 6:
					text = "eval";
					break;
				case 7:
					text = "isFinite";
					break;
				case 8:
					text = "isNaN";
					break;
				case 9:
					text = "isXMLName";
					break;
				case 10:
					text = "parseFloat";
					break;
				case 11:
					text = "parseInt";
					arity = 2;
					break;
				case 12:
					text = "unescape";
					break;
				case 13:
					text = "uneval";
					break;
				default:
					throw Context.CodeBug();
				}
				IdFunctionObject idFunctionObject = new IdFunctionObject(idcall, BuiltinGlobal.FTAG, i, text, arity, scope);
				if (zealed)
				{
					idFunctionObject.SealObject();
				}
				idFunctionObject.ExportAsScopeProperty();
			}
			ScriptableObject.DefineProperty(scope, "NaN", double.NaN, 2);
			ScriptableObject.DefineProperty(scope, "Infinity", double.PositiveInfinity, 2);
			ScriptableObject.DefineProperty(scope, "undefined", Undefined.Value, 2);
			string[] array = new string[]
			{
				"ConversionError",
				"EvalError",
				"RangeError",
				"ReferenceError",
				"SyntaxError",
				"TypeError",
				"URIError",
				"InternalError",
				"JavaException"
			};
			for (int j = 0; j < array.Length; j++)
			{
				string text = array[j];
				IScriptable scriptable = ScriptRuntime.NewObject(cx, scope, "Error", ScriptRuntime.EmptyArgs);
				scriptable.Put("name", scriptable, text);
				if (zealed)
				{
					if (scriptable is ScriptableObject)
					{
						((ScriptableObject)scriptable).SealObject();
					}
				}
				IdFunctionObject idFunctionObject2 = new IdFunctionObject(idcall, BuiltinGlobal.FTAG, 14, text, 1, scope);
				idFunctionObject2.MarkAsConstructor(scriptable);
				if (zealed)
				{
					idFunctionObject2.SealObject();
				}
				idFunctionObject2.ExportAsScopeProperty();
			}
		}
		public virtual object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (f.HasTag(BuiltinGlobal.FTAG))
			{
				int methodId = f.MethodId;
				object result;
				switch (methodId)
				{
				case 1:
				case 2:
				{
					string str = ScriptConvert.ToString(args, 0);
					result = BuiltinGlobal.decode(str, methodId == 1);
					break;
				}
				case 3:
				case 4:
				{
					string str = ScriptConvert.ToString(args, 0);
					result = BuiltinGlobal.encode(str, methodId == 3);
					break;
				}
				case 5:
					result = this.js_escape(args);
					break;
				case 6:
					result = this.ImplEval(cx, scope, thisObj, args);
					break;
				case 7:
				{
					bool flag;
					if (args.Length < 1)
					{
						flag = false;
					}
					else
					{
						double num = ScriptConvert.ToNumber(args[0]);
						flag = (!double.IsNaN(num) && num != double.PositiveInfinity && num != double.NegativeInfinity);
					}
					result = flag;
					break;
				}
				case 8:
				{
					bool flag;
					if (args.Length < 1)
					{
						flag = true;
					}
					else
					{
						double num = ScriptConvert.ToNumber(args[0]);
						flag = double.IsNaN(num);
					}
					result = flag;
					break;
				}
				case 9:
				{
					object value = (args.Length == 0) ? Undefined.Value : args[0];
					XMLLib xMLLib = XMLLib.ExtractFromScope(scope);
					result = xMLLib.IsXMLName(cx, value);
					break;
				}
				case 10:
					result = this.js_parseFloat(args);
					break;
				case 11:
					result = this.js_parseInt(args);
					break;
				case 12:
					result = this.js_unescape(args);
					break;
				case 13:
				{
					object value2 = (args.Length != 0) ? args[0] : Undefined.Value;
					result = ScriptRuntime.uneval(cx, scope, value2);
					break;
				}
				case 14:
					result = BuiltinError.make(cx, scope, f, args);
					break;
				default:
					goto IL_209;
				}
				return result;
			}
			IL_209:
			throw f.Unknown();
		}
		private object js_parseInt(object[] args)
		{
			string text = ScriptConvert.ToString(args, 0);
			int num = ScriptConvert.ToInt32(args, 1);
			int length = text.Length;
			object result;
			if (length == 0)
			{
				result = double.NaN;
			}
			else
			{
				bool flag = false;
				int num2 = 0;
				char c;
				do
				{
					c = text[num2];
					if (!char.IsWhiteSpace(c))
					{
						break;
					}
					num2++;
				}
				while (num2 < length);
				if (c == '+' || (flag = (c == '-')))
				{
					num2++;
				}
				if (num == 0)
				{
					num = -1;
				}
				else
				{
					if (num < 2 || num > 36)
					{
						result = double.NaN;
						return result;
					}
					if (num == 16 && length - num2 > 1 && text[num2] == '0')
					{
						c = text[num2 + 1];
						if (c == 'x' || c == 'X')
						{
							num2 += 2;
						}
					}
				}
				if (num == -1)
				{
					num = 10;
					if (length - num2 > 1 && text[num2] == '0')
					{
						c = text[num2 + 1];
						if (c == 'x' || c == 'X')
						{
							num = 16;
							num2 += 2;
						}
						else
						{
							if ('0' <= c && c <= '9')
							{
								num = 8;
								num2++;
							}
						}
					}
				}
				double num3 = ScriptConvert.ToNumber(text, num2, num);
				result = (flag ? (-num3) : num3);
			}
			return result;
		}
		private object js_parseFloat(object[] args)
		{
			object result;
			if (args.Length < 1)
			{
				result = double.NaN;
			}
			else
			{
				string text = ScriptConvert.ToString(args[0]);
				int length = text.Length;
				int num = 0;
				while (num != length)
				{
					char c = text[num];
					if (!TokenStream.isJSSpace((int)c))
					{
						int i = num;
						if (c == '+' || c == '-')
						{
							i++;
							if (i == length)
							{
								result = double.NaN;
								return result;
							}
							c = text[i];
						}
						if (c != 'I')
						{
							int num2 = -1;
							int num3 = -1;
							while (i < length)
							{
								char c2 = text[i];
								switch (c2)
								{
								case '+':
								case '-':
									if (num3 != i - 1)
									{
										goto IL_269;
									}
									break;
								case ',':
								case '/':
								case ':':
								case ';':
								case '<':
								case '=':
								case '>':
								case '?':
								case '@':
								case 'A':
								case 'B':
								case 'C':
								case 'D':
									goto IL_264;
								case '.':
									if (num2 != -1)
									{
										goto IL_269;
									}
									num2 = i;
									break;
								case '0':
								case '1':
								case '2':
								case '3':
								case '4':
								case '5':
								case '6':
								case '7':
								case '8':
								case '9':
									break;
								case 'E':
									goto IL_22C;
								default:
									if (c2 != 'e')
									{
										goto IL_264;
									}
									goto IL_22C;
								}
								IL_26E:
								i++;
								continue;
								IL_22C:
								if (num3 == -1)
								{
									num3 = i;
									goto IL_26E;
								}
								IL_264:
								IL_269:
								break;
							}
							text = text.Substring(num, i - num);
							try
							{
								result = double.Parse(text, BuiltinNumber.NumberFormatter);
							}
							catch (OverflowException)
							{
								if (text[0] == '-')
								{
									result = double.NegativeInfinity;
								}
								else
								{
									result = double.PositiveInfinity;
								}
							}
							catch (Exception)
							{
								result = double.NaN;
							}
							return result;
						}
						if (i + 8 <= length && string.Compare(text, i, "Infinity", 0, 8) == 0)
						{
							double num4;
							if (text[num] == '-')
							{
								num4 = double.NegativeInfinity;
							}
							else
							{
								num4 = double.PositiveInfinity;
							}
							result = num4;
							return result;
						}
						result = double.NaN;
						return result;
					}
					else
					{
						num++;
					}
				}
				result = double.NaN;
			}
			return result;
		}
		private object js_escape(object[] args)
		{
			string text = ScriptConvert.ToString(args, 0);
			int num = 7;
			if (args.Length > 1)
			{
				double num2 = ScriptConvert.ToNumber(args[1]);
				if (double.IsNaN(num2) || (double)(num = (int)num2) != num2 || 0 != (num & -8))
				{
					throw Context.ReportRuntimeErrorById("msg.bad.esc.mask", new object[0]);
				}
			}
			StringBuilder stringBuilder = null;
			int num3 = 0;
			int length = text.Length;
			while (num3 != length)
			{
				int num4 = (int)text[num3];
				if (num != 0 && ((num4 >= 48 && num4 <= 57) || (num4 >= 65 && num4 <= 90) || (num4 >= 97 && num4 <= 122) || num4 == 64 || num4 == 42 || num4 == 95 || num4 == 45 || num4 == 46 || ((num & 4) != 0 && (num4 == 47 || num4 == 43))))
				{
					if (stringBuilder != null)
					{
						stringBuilder.Append((char)num4);
					}
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(length + 3);
						stringBuilder.Append(text);
						stringBuilder.Length = num3;
					}
					int num5;
					if (num4 < 256)
					{
						if (num4 == 32 && num == 2)
						{
							stringBuilder.Append('+');
							goto IL_234;
						}
						stringBuilder.Append('%');
						num5 = 2;
					}
					else
					{
						stringBuilder.Append('%');
						stringBuilder.Append('u');
						num5 = 4;
					}
					for (int i = (num5 - 1) * 4; i >= 0; i -= 4)
					{
						int num6 = 15 & num4 >> i;
						int num7 = (num6 < 10) ? (48 + num6) : (55 + num6);
						stringBuilder.Append((char)num7);
					}
				}
				IL_234:
				num3++;
				continue;
				goto IL_234;
			}
			return (stringBuilder == null) ? text : stringBuilder.ToString();
		}
		private object js_unescape(object[] args)
		{
			string text = ScriptConvert.ToString(args, 0);
			int num = text.IndexOf('%');
			if (num >= 0)
			{
				int length = text.Length;
				char[] array = text.ToCharArray();
				int num2 = num;
				int num3 = num;
				while (num3 != length)
				{
					char c = array[num3];
					num3++;
					if (c == '%' && num3 != length)
					{
						int num4;
						int num5;
						if (array[num3] == 'u')
						{
							num4 = num3 + 1;
							num5 = num3 + 5;
						}
						else
						{
							num4 = num3;
							num5 = num3 + 2;
						}
						if (num5 <= length)
						{
							int num6 = 0;
							for (int num7 = num4; num7 != num5; num7++)
							{
								num6 = ScriptConvert.XDigitToInt((int)array[num7], num6);
							}
							if (num6 >= 0)
							{
								c = (char)num6;
								num3 = num5;
							}
						}
					}
					array[num2] = c;
					num2++;
				}
				text = new string(array, 0, num2);
			}
			return text;
		}
		private object ImplEval(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (cx.Version == Context.Versions.JS1_4)
			{
				Context.ReportWarningById("msg.cant.call.indirect", new string[]
				{
					"eval"
				});
				return ScriptRuntime.evalSpecial(cx, scope, thisObj, args, string.Empty, 0);
			}
			throw ScriptRuntime.ConstructError("EvalError", ScriptRuntime.GetMessage("msg.cant.call.indirect", new object[]
			{
				"eval"
			}));
		}
		internal static bool isEvalFunction(object functionObj)
		{
			bool result;
			if (functionObj is IdFunctionObject)
			{
				IdFunctionObject idFunctionObject = (IdFunctionObject)functionObj;
				if (idFunctionObject.HasTag(BuiltinGlobal.FTAG) && idFunctionObject.MethodId == 6)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private static string encode(string str, bool fullUri)
		{
			sbyte[] array = null;
			StringBuilder stringBuilder = null;
			int num = 0;
			int length = str.Length;
			while (num != length)
			{
				char c = str[num];
				if (BuiltinGlobal.encodeUnescaped(c, fullUri))
				{
					if (stringBuilder != null)
					{
						stringBuilder.Append(c);
					}
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(length + 3);
						stringBuilder.Append(str);
						stringBuilder.Length = num;
						array = new sbyte[6];
					}
					if ('\udc00' <= c && c <= '\udfff')
					{
						throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
					}
					int ucs4Char;
					if (c < '\ud800' || '\udbff' < c)
					{
						ucs4Char = (int)c;
					}
					else
					{
						num++;
						if (num == length)
						{
							throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
						}
						char c2 = str[num];
						if ('\udc00' > c2 || c2 > '\udfff')
						{
							throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
						}
						ucs4Char = (int)(((int)(c - '\ud800') << 10) + (c2 - '\udc00')) + 65536;
					}
					int num2 = BuiltinGlobal.oneUcs4ToUtf8Char(array, ucs4Char);
					for (int i = 0; i < num2; i++)
					{
						int num3 = 255 & (int)array[i];
						stringBuilder.Append('%');
						stringBuilder.Append(BuiltinGlobal.toHexChar((int)((uint)num3 >> 4)));
						stringBuilder.Append(BuiltinGlobal.toHexChar(num3 & 15));
					}
				}
				num++;
			}
			return (stringBuilder == null) ? str : stringBuilder.ToString();
		}
		private static char toHexChar(int i)
		{
			if (i >> 4 != 0)
			{
				Context.CodeBug();
			}
			return (char)((i < 10) ? (i + 48) : (i - 10 + 97));
		}
		private static int unHex(char c)
		{
			int result;
			if ('A' <= c && c <= 'F')
			{
				result = (int)(c - 'A' + '\n');
			}
			else
			{
				if ('a' <= c && c <= 'f')
				{
					result = (int)(c - 'a' + '\n');
				}
				else
				{
					if ('0' <= c && c <= '9')
					{
						result = (int)(c - '0');
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}
		private static int unHex(char c1, char c2)
		{
			int num = BuiltinGlobal.unHex(c1);
			int num2 = BuiltinGlobal.unHex(c2);
			int result;
			if (num >= 0 && num2 >= 0)
			{
				result = (num << 4 | num2);
			}
			else
			{
				result = -1;
			}
			return result;
		}
		private static string decode(string str, bool fullUri)
		{
			char[] array = null;
			int length = 0;
			int num = 0;
			int length2 = str.Length;
			while (num != length2)
			{
				char c = str[num];
				if (c != '%')
				{
					if (array != null)
					{
						array[length++] = c;
					}
					num++;
				}
				else
				{
					if (array == null)
					{
						array = new char[length2];
						str.ToCharArray(0, num).CopyTo(array, 0);
						length = num;
					}
					int num2 = num;
					if (num + 3 > length2)
					{
						throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
					}
					int num3 = BuiltinGlobal.unHex(str[num + 1], str[num + 2]);
					if (num3 < 0)
					{
						throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
					}
					num += 3;
					if ((num3 & 128) == 0)
					{
						c = (char)num3;
					}
					else
					{
						if ((num3 & 192) == 128)
						{
							throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
						}
						int num4;
						int num5;
						int num6;
						if ((num3 & 32) == 0)
						{
							num4 = 1;
							num5 = (num3 & 31);
							num6 = 128;
						}
						else
						{
							if ((num3 & 16) == 0)
							{
								num4 = 2;
								num5 = (num3 & 15);
								num6 = 2048;
							}
							else
							{
								if ((num3 & 8) == 0)
								{
									num4 = 3;
									num5 = (num3 & 7);
									num6 = 65536;
								}
								else
								{
									if ((num3 & 4) == 0)
									{
										num4 = 4;
										num5 = (num3 & 3);
										num6 = 2097152;
									}
									else
									{
										if ((num3 & 2) != 0)
										{
											throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
										}
										num4 = 5;
										num5 = (num3 & 1);
										num6 = 67108864;
									}
								}
							}
						}
						if (num + 3 * num4 > length2)
						{
							throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
						}
						for (int num7 = 0; num7 != num4; num7++)
						{
							if (str[num] != '%')
							{
								throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
							}
							num3 = BuiltinGlobal.unHex(str[num + 1], str[num + 2]);
							if (num3 < 0 || (num3 & 192) != 128)
							{
								throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
							}
							num5 = (num5 << 6 | (num3 & 63));
							num += 3;
						}
						if (num5 < num6 || num5 == 65534 || num5 == 65535)
						{
							num5 = 65533;
						}
						if (num5 >= 65536)
						{
							num5 -= 65536;
							if (num5 > 1048575)
							{
								throw Context.ReportRuntimeErrorById("msg.bad.uri", new object[0]);
							}
							char c2 = (char)(((uint)num5 >> 10) + 55296u);
							c = (char)((num5 & 1023) + 56320);
							array[length++] = c2;
						}
						else
						{
							c = (char)num5;
						}
					}
					if (fullUri && ";/?:@&=+$,#".IndexOf(c) >= 0)
					{
						for (int num8 = num2; num8 != num; num8++)
						{
							array[length++] = str[num8];
						}
					}
					else
					{
						array[length++] = c;
					}
				}
			}
			return (array == null) ? str : new string(array, 0, length);
		}
		private static bool encodeUnescaped(char c, bool fullUri)
		{
			return ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z') || ('0' <= c && c <= '9') || "-_.!~*'()".IndexOf(c) >= 0 || (fullUri && ";/?:@&=+$,#".IndexOf(c) >= 0);
		}
		private static int oneUcs4ToUtf8Char(sbyte[] utf8Buffer, int ucs4Char)
		{
			int num = 1;
			if ((ucs4Char & -128) == 0)
			{
				utf8Buffer[0] = (sbyte)ucs4Char;
			}
			else
			{
				int num2 = (int)((uint)ucs4Char >> 11);
				num = 2;
				while (num2 != 0)
				{
					num2 = (int)((uint)num2 >> 5);
					num++;
				}
				int num3 = num;
				while (--num3 > 0)
				{
					utf8Buffer[num3] = (sbyte)((ucs4Char & 63) | 128);
					ucs4Char = (int)((uint)ucs4Char >> 6);
				}
				utf8Buffer[0] = (sbyte)(256 - (1 << 8 - num) + ucs4Char);
			}
			return num;
		}
	}
}
