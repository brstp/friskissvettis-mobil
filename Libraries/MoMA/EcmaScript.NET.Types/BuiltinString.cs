using System;
using System.Text;
namespace EcmaScript.NET.Types
{
	internal sealed class BuiltinString : IdScriptableObject
	{
		private const int Id_length = 1;
		private const int MAX_INSTANCE_ID = 1;
		private const int ConstructorId_fromCharCode = -1;
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int Id_valueOf = 4;
		private const int Id_charAt = 5;
		private const int Id_charCodeAt = 6;
		private const int Id_indexOf = 7;
		private const int Id_lastIndexOf = 8;
		private const int Id_split = 9;
		private const int Id_substring = 10;
		private const int Id_toLowerCase = 11;
		private const int Id_toUpperCase = 12;
		private const int Id_substr = 13;
		private const int Id_concat = 14;
		private const int Id_slice = 15;
		private const int Id_bold = 16;
		private const int Id_italics = 17;
		private const int Id_fixed = 18;
		private const int Id_strike = 19;
		private const int Id_small = 20;
		private const int Id_big = 21;
		private const int Id_blink = 22;
		private const int Id_sup = 23;
		private const int Id_sub = 24;
		private const int Id_fontsize = 25;
		private const int Id_fontcolor = 26;
		private const int Id_link = 27;
		private const int Id_anchor = 28;
		private const int Id_equals = 29;
		private const int Id_equalsIgnoreCase = 30;
		private const int Id_match = 31;
		private const int Id_search = 32;
		private const int Id_replace = 33;
		private const int MAX_PROTOTYPE_ID = 33;
		private static readonly object STRING_TAG = new object();
		private string m_Value;
		public override string ClassName
		{
			get
			{
				return "String";
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return 1;
			}
		}
		internal int Length
		{
			get
			{
				return this.m_Value.Length;
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinString builtinString = new BuiltinString("");
			builtinString.ExportAsJSClass(33, scope, zealed, 7);
		}
		private BuiltinString(string s)
		{
			this.m_Value = s;
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int result;
			if (s.Equals("length"))
			{
				result = IdScriptableObject.InstanceIdInfo(7, 1);
			}
			else
			{
				result = base.FindInstanceIdInfo(s);
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			string result;
			if (id == 1)
			{
				result = "length";
			}
			else
			{
				result = base.GetInstanceIdName(id);
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			object result;
			if (id == 1)
			{
				result = this.m_Value.Length;
			}
			else
			{
				result = base.GetInstanceIdValue(id);
			}
			return result;
		}
		protected internal override void FillConstructorProperties(IdFunctionObject ctor)
		{
			this.AddIdFunctionProperty(ctor, BuiltinString.STRING_TAG, -1, "fromCharCode", 1);
			base.FillConstructorProperties(ctor);
		}
		protected internal override void InitPrototypeId(int id)
		{
			int arity;
			string name;
			switch (id)
			{
			case 1:
				arity = 1;
				name = "constructor";
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
				arity = 0;
				name = "valueOf";
				break;
			case 5:
				arity = 1;
				name = "charAt";
				break;
			case 6:
				arity = 1;
				name = "charCodeAt";
				break;
			case 7:
				arity = 1;
				name = "indexOf";
				break;
			case 8:
				arity = 1;
				name = "lastIndexOf";
				break;
			case 9:
				arity = 2;
				name = "split";
				break;
			case 10:
				arity = 2;
				name = "substring";
				break;
			case 11:
				arity = 0;
				name = "toLowerCase";
				break;
			case 12:
				arity = 0;
				name = "toUpperCase";
				break;
			case 13:
				arity = 2;
				name = "substr";
				break;
			case 14:
				arity = 1;
				name = "concat";
				break;
			case 15:
				arity = 2;
				name = "slice";
				break;
			case 16:
				arity = 0;
				name = "bold";
				break;
			case 17:
				arity = 0;
				name = "italics";
				break;
			case 18:
				arity = 0;
				name = "fixed";
				break;
			case 19:
				arity = 0;
				name = "strike";
				break;
			case 20:
				arity = 0;
				name = "small";
				break;
			case 21:
				arity = 0;
				name = "big";
				break;
			case 22:
				arity = 0;
				name = "blink";
				break;
			case 23:
				arity = 0;
				name = "sup";
				break;
			case 24:
				arity = 0;
				name = "sub";
				break;
			case 25:
				arity = 0;
				name = "fontsize";
				break;
			case 26:
				arity = 0;
				name = "fontcolor";
				break;
			case 27:
				arity = 0;
				name = "link";
				break;
			case 28:
				arity = 0;
				name = "anchor";
				break;
			case 29:
				arity = 1;
				name = "equals";
				break;
			case 30:
				arity = 1;
				name = "equalsIgnoreCase";
				break;
			case 31:
				arity = 1;
				name = "match";
				break;
			case 32:
				arity = 1;
				name = "search";
				break;
			case 33:
				arity = 1;
				name = "replace";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinString.STRING_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (f.HasTag(BuiltinString.STRING_TAG))
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case -1:
				{
					int num = args.Length;
					if (num < 1)
					{
						result = "";
						return result;
					}
					StringBuilder stringBuilder = new StringBuilder(num);
					for (int num2 = 0; num2 != num; num2++)
					{
						stringBuilder.Append(ScriptConvert.ToUint16(args[num2]));
					}
					result = stringBuilder.ToString();
					return result;
				}
				case 1:
				{
					string text = (args.Length >= 1) ? ScriptConvert.ToString(args[0]) : "";
					if (thisObj == null)
					{
						result = new BuiltinString(text);
						return result;
					}
					result = text;
					return result;
				}
				case 2:
				case 4:
					result = BuiltinString.RealThis(thisObj, f).m_Value;
					return result;
				case 3:
				{
					string text = BuiltinString.RealThis(thisObj, f).m_Value;
					result = "(new String(\"" + ScriptRuntime.escapeString(text) + "\"))";
					return result;
				}
				case 5:
				case 6:
				{
					string text2 = ScriptConvert.ToString(thisObj);
					double num3 = ScriptConvert.ToInteger(args, 0);
					if (num3 < 0.0 || num3 >= (double)text2.Length)
					{
						if (methodId == 5)
						{
							result = "";
							return result;
						}
						result = double.NaN;
						return result;
					}
					else
					{
						char c = text2[(int)num3];
						if (methodId == 5)
						{
							result = Convert.ToString(c);
							return result;
						}
						result = (int)c;
						return result;
					}
					break;
				}
				case 7:
					result = BuiltinString.js_indexOf(ScriptConvert.ToString(thisObj), args);
					return result;
				case 8:
					result = BuiltinString.js_lastIndexOf(ScriptConvert.ToString(thisObj), args);
					return result;
				case 9:
					result = BuiltinString.ImplSplit(cx, scope, ScriptConvert.ToString(thisObj), args);
					return result;
				case 10:
					result = BuiltinString.js_substring(cx, ScriptConvert.ToString(thisObj), args);
					return result;
				case 11:
					result = ScriptConvert.ToString(thisObj).ToLower();
					return result;
				case 12:
					result = ScriptConvert.ToString(thisObj).ToUpper();
					return result;
				case 13:
					result = BuiltinString.js_substr(ScriptConvert.ToString(thisObj), args);
					return result;
				case 14:
					result = BuiltinString.js_concat(ScriptConvert.ToString(thisObj), args);
					return result;
				case 15:
					result = BuiltinString.js_slice(ScriptConvert.ToString(thisObj), args);
					return result;
				case 16:
					result = BuiltinString.Tagify(thisObj, "b", null, null);
					return result;
				case 17:
					result = BuiltinString.Tagify(thisObj, "i", null, null);
					return result;
				case 18:
					result = BuiltinString.Tagify(thisObj, "tt", null, null);
					return result;
				case 19:
					result = BuiltinString.Tagify(thisObj, "strike", null, null);
					return result;
				case 20:
					result = BuiltinString.Tagify(thisObj, "small", null, null);
					return result;
				case 21:
					result = BuiltinString.Tagify(thisObj, "big", null, null);
					return result;
				case 22:
					result = BuiltinString.Tagify(thisObj, "blink", null, null);
					return result;
				case 23:
					result = BuiltinString.Tagify(thisObj, "sup", null, null);
					return result;
				case 24:
					result = BuiltinString.Tagify(thisObj, "sub", null, null);
					return result;
				case 25:
					result = BuiltinString.Tagify(thisObj, "font", "size", args);
					return result;
				case 26:
					result = BuiltinString.Tagify(thisObj, "font", "color", args);
					return result;
				case 27:
					result = BuiltinString.Tagify(thisObj, "a", "href", args);
					return result;
				case 28:
					result = BuiltinString.Tagify(thisObj, "a", "name", args);
					return result;
				case 29:
				case 30:
				{
					string text3 = ScriptConvert.ToString(thisObj);
					string text4 = ScriptConvert.ToString(args, 0);
					result = ((methodId == 29) ? text3.Equals(text4) : text3.ToUpper().Equals(text4.ToUpper()));
					return result;
				}
				case 31:
				case 32:
				case 33:
				{
					RegExpActions actionType;
					if (methodId == 31)
					{
						actionType = RegExpActions.Match;
					}
					else
					{
						if (methodId == 32)
						{
							actionType = RegExpActions.Search;
						}
						else
						{
							actionType = RegExpActions.Replace;
						}
					}
					result = cx.regExpProxy.Perform(cx, scope, thisObj, args, actionType);
					return result;
				}
				}
				throw new ArgumentException(Convert.ToString(methodId));
			}
			result = base.ExecIdCall(f, cx, scope, thisObj, args);
			return result;
		}
		private static BuiltinString RealThis(IScriptable thisObj, IdFunctionObject f)
		{
			if (!(thisObj is BuiltinString))
			{
				throw IdScriptableObject.IncompatibleCallError(f);
			}
			return (BuiltinString)thisObj;
		}
		private static string Tagify(object thisObj, string tag, string attribute, object[] args)
		{
			string value = ScriptConvert.ToString(thisObj);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('<');
			stringBuilder.Append(tag);
			if (attribute != null)
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(attribute);
				stringBuilder.Append("=\"");
				stringBuilder.Append(ScriptConvert.ToString(args, 0));
				stringBuilder.Append('"');
			}
			stringBuilder.Append('>');
			stringBuilder.Append(value);
			stringBuilder.Append("</");
			stringBuilder.Append(tag);
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}
		public override string ToString()
		{
			return this.m_Value;
		}
		public override object Get(int index, IScriptable start)
		{
			object result;
			if (0 <= index && index < this.m_Value.Length)
			{
				result = this.m_Value.Substring(index, index + 1 - index);
			}
			else
			{
				result = base.Get(index, start);
			}
			return result;
		}
		public override object Put(int index, IScriptable start, object value)
		{
			object result;
			if (0 <= index && index < this.m_Value.Length)
			{
				result = Undefined.Value;
			}
			else
			{
				result = base.Put(index, start, value);
			}
			return result;
		}
		private static int js_indexOf(string target, object[] args)
		{
			string value = ScriptConvert.ToString(args, 0);
			double num = ScriptConvert.ToInteger(args, 1);
			int result;
			if (num > (double)target.Length)
			{
				result = -1;
			}
			else
			{
				if (num < 0.0)
				{
					num = 0.0;
				}
				result = target.IndexOf(value, (int)num);
			}
			return result;
		}
		private static int js_lastIndexOf(string target, object[] args)
		{
			string text = ScriptConvert.ToString(args, 0);
			double num = ScriptConvert.ToNumber(args, 1);
			if (double.IsNaN(num) || num > (double)target.Length)
			{
				num = (double)target.Length;
			}
			else
			{
				if (num < 0.0)
				{
					num = 0.0;
				}
			}
			return BuiltinString.lastIndexOf(target.ToCharArray(), 0, target.Length, text.ToCharArray(), 0, text.Length, (int)num);
		}
		private static int lastIndexOf(char[] source, int sourceOffset, int sourceCount, char[] target, int targetOffset, int targetCount, int fromIndex)
		{
			int num = sourceCount - targetCount;
			int result;
			if (fromIndex < 0)
			{
				result = -1;
			}
			else
			{
				if (fromIndex > num)
				{
					fromIndex = num;
				}
				if (targetCount == 0)
				{
					result = fromIndex;
				}
				else
				{
					int num2 = targetOffset + targetCount - 1;
					char c = target[num2];
					int num3 = sourceOffset + targetCount - 1;
					int num4 = num3 + fromIndex;
					int num5;
					while (true)
					{
						IL_65:
						while (num4 >= num3 && source[num4] != c)
						{
							num4--;
						}
						if (num4 < num3)
						{
							break;
						}
						int i = num4 - 1;
						num5 = i - (targetCount - 1);
						int num6 = num2 - 1;
						while (i > num5)
						{
							if (source[i--] != target[num6--])
							{
								num4--;
								goto IL_65;
							}
						}
						goto Block_8;
					}
					result = -1;
					return result;
					Block_8:
					result = num5 - sourceOffset + 1;
				}
			}
			return result;
		}
		private static int find_split(Context cx, IScriptable scope, string target, string separator, Context.Versions version, RegExpProxy reProxy, IScriptable re, int[] ip, int[] matchlen, bool[] matched, string[][] parensp)
		{
			int num = ip[0];
			int length = target.Length;
			int result;
			if (version == Context.Versions.JS1_2 && re == null && separator.Length == 1 && separator[0] == ' ')
			{
				if (num == 0)
				{
					while (num < length && char.IsWhiteSpace(target[num]))
					{
						num++;
					}
					ip[0] = num;
				}
				if (num == length)
				{
					result = -1;
				}
				else
				{
					while (num < length && !char.IsWhiteSpace(target[num]))
					{
						num++;
					}
					int num2 = num;
					while (num2 < length && char.IsWhiteSpace(target[num2]))
					{
						num2++;
					}
					matchlen[0] = num2 - num;
					result = num;
				}
			}
			else
			{
				if (num > length)
				{
					result = -1;
				}
				else
				{
					if (re != null)
					{
						result = reProxy.FindSplit(cx, scope, target, separator, re, ip, matchlen, matched, parensp);
					}
					else
					{
						if (version != Context.Versions.Default && version < Context.Versions.JS1_3 && length == 0)
						{
							result = -1;
						}
						else
						{
							if (separator.Length == 0)
							{
								if (version == Context.Versions.JS1_2)
								{
									if (num == length)
									{
										matchlen[0] = 1;
										result = num;
									}
									else
									{
										result = num + 1;
									}
								}
								else
								{
									result = ((num == length) ? -1 : (num + 1));
								}
							}
							else
							{
								if (ip[0] >= length)
								{
									result = length;
								}
								else
								{
									num = target.IndexOf(separator, ip[0]);
									result = ((num != -1) ? num : length);
								}
							}
						}
					}
				}
			}
			return result;
		}
		private static object ImplSplit(Context cx, IScriptable scope, string target, object[] args)
		{
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(scope);
			IScriptable scriptable = ScriptRuntime.NewObject(cx, topLevelScope, "Array", null);
			object result;
			if (args.Length < 1)
			{
				scriptable.Put(0, scriptable, target);
				result = scriptable;
			}
			else
			{
				bool flag = args.Length > 1 && args[1] != Undefined.Value;
				long num = 0L;
				if (flag)
				{
					num = ScriptConvert.ToUint32(args[1]);
					if (num > (long)target.Length)
					{
						num = (long)(1 + target.Length);
					}
				}
				string text = null;
				int[] array = new int[1];
				IScriptable scriptable2 = null;
				RegExpProxy regExpProxy = null;
				if (args[0] is IScriptable)
				{
					regExpProxy = cx.RegExpProxy;
					if (regExpProxy != null)
					{
						IScriptable scriptable3 = (IScriptable)args[0];
						if (regExpProxy.IsRegExp(scriptable3))
						{
							scriptable2 = scriptable3;
						}
					}
				}
				if (scriptable2 == null)
				{
					text = ScriptConvert.ToString(args[0]);
					array[0] = text.Length;
				}
				int[] array2 = new int[1];
				int[] array3 = array2;
				int num2 = 0;
				bool[] array4 = new bool[1];
				bool[] matched = array4;
				string[][] array5 = new string[1][];
				string[][] parensp = array5;
				Context.Versions version = cx.Version;
				int num3;
				while ((num3 = BuiltinString.find_split(cx, scope, target, text, version, regExpProxy, scriptable2, array3, array, matched, parensp)) >= 0)
				{
					if ((flag && (long)num2 >= num) || num3 > target.Length)
					{
						break;
					}
					string value;
					if (target.Length == 0)
					{
						value = target;
					}
					else
					{
						value = target.Substring(array3[0], num3 - array3[0]);
					}
					scriptable.Put(num2, scriptable, value);
					num2++;
					array3[0] = num3 + array[0];
					if (version < Context.Versions.JS1_3 && version != Context.Versions.Default)
					{
						if (!flag && array3[0] == target.Length)
						{
							break;
						}
					}
				}
				result = scriptable;
			}
			return result;
		}
		private static string js_substring(Context cx, string target, object[] args)
		{
			int length = target.Length;
			double num = ScriptConvert.ToInteger(args, 0);
			if (num < 0.0)
			{
				num = 0.0;
			}
			else
			{
				if (num > (double)length)
				{
					num = (double)length;
				}
			}
			double num2;
			if (args.Length <= 1 || args[1] == Undefined.Value)
			{
				num2 = (double)length;
			}
			else
			{
				num2 = ScriptConvert.ToInteger(args[1]);
				if (num2 < 0.0)
				{
					num2 = 0.0;
				}
				else
				{
					if (num2 > (double)length)
					{
						num2 = (double)length;
					}
				}
				if (num2 < num)
				{
					if (cx.Version != Context.Versions.JS1_2)
					{
						double num3 = num;
						num = num2;
						num2 = num3;
					}
					else
					{
						num2 = num;
					}
				}
			}
			return target.Substring((int)num, (int)num2 - (int)num);
		}
		private static string js_substr(string target, object[] args)
		{
			string result;
			if (args.Length < 1)
			{
				result = target;
			}
			else
			{
				double num = ScriptConvert.ToInteger(args[0]);
				int length = target.Length;
				if (num < 0.0)
				{
					num += (double)length;
					if (num < 0.0)
					{
						num = 0.0;
					}
				}
				else
				{
					if (num > (double)length)
					{
						num = (double)length;
					}
				}
				double num2;
				if (args.Length == 1)
				{
					num2 = (double)length;
				}
				else
				{
					num2 = ScriptConvert.ToInteger(args[1]);
					if (num2 < 0.0)
					{
						num2 = 0.0;
					}
					num2 += num;
					if (num2 > (double)length)
					{
						num2 = (double)length;
					}
				}
				result = target.Substring((int)num, (int)num2 - (int)num);
			}
			return result;
		}
		private static string js_concat(string target, object[] args)
		{
			int num = args.Length;
			string result;
			if (num == 0)
			{
				result = target;
			}
			else
			{
				if (num == 1)
				{
					string str = ScriptConvert.ToString(args[0]);
					result = target + str;
				}
				else
				{
					int num2 = target.Length;
					string[] array = new string[num];
					for (int num3 = 0; num3 != num; num3++)
					{
						string text = ScriptConvert.ToString(args[num3]);
						array[num3] = text;
						num2 += text.Length;
					}
					StringBuilder stringBuilder = new StringBuilder(num2);
					stringBuilder.Append(target);
					for (int num3 = 0; num3 != num; num3++)
					{
						stringBuilder.Append(array[num3]);
					}
					result = stringBuilder.ToString();
				}
			}
			return result;
		}
		private static string js_slice(string target, object[] args)
		{
			string result;
			if (args.Length != 0)
			{
				double num = ScriptConvert.ToInteger(args[0]);
				int length = target.Length;
				if (num < 0.0)
				{
					num += (double)length;
					if (num < 0.0)
					{
						num = 0.0;
					}
				}
				else
				{
					if (num > (double)length)
					{
						num = (double)length;
					}
				}
				double num2;
				if (args.Length == 1)
				{
					num2 = (double)length;
				}
				else
				{
					num2 = ScriptConvert.ToInteger(args[1]);
					if (num2 < 0.0)
					{
						num2 += (double)length;
						if (num2 < 0.0)
						{
							num2 = 0.0;
						}
					}
					else
					{
						if (num2 > (double)length)
						{
							num2 = (double)length;
						}
					}
					if (num2 < num)
					{
						num2 = num;
					}
				}
				result = target.Substring((int)num, (int)num2 - (int)num);
			}
			else
			{
				result = target;
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			switch (s.Length)
			{
			case 3:
			{
				int num = (int)s[2];
				if (num == 98)
				{
					if (s[0] == 's' && s[1] == 'u')
					{
						result = 24;
						return result;
					}
				}
				else
				{
					if (num == 103)
					{
						if (s[0] == 'b' && s[1] == 'i')
						{
							result = 21;
							return result;
						}
					}
					else
					{
						if (num == 112)
						{
							if (s[0] == 's' && s[1] == 'u')
							{
								result = 23;
								return result;
							}
						}
					}
				}
				break;
			}
			case 4:
			{
				int num = (int)s[0];
				if (num == 98)
				{
					text = "bold";
					result = 16;
				}
				else
				{
					if (num == 108)
					{
						text = "link";
						result = 27;
					}
				}
				break;
			}
			case 5:
			{
				char c = s[4];
				switch (c)
				{
				case 'd':
					text = "fixed";
					result = 18;
					break;
				case 'e':
					text = "slice";
					result = 15;
					break;
				case 'f':
				case 'g':
				case 'i':
				case 'j':
					break;
				case 'h':
					text = "match";
					result = 31;
					break;
				case 'k':
					text = "blink";
					result = 22;
					break;
				case 'l':
					text = "small";
					result = 20;
					break;
				default:
					if (c == 't')
					{
						text = "split";
						result = 9;
					}
					break;
				}
				break;
			}
			case 6:
			{
				char c = s[1];
				if (c != 'e')
				{
					if (c != 'h')
					{
						switch (c)
						{
						case 'n':
							text = "anchor";
							result = 28;
							break;
						case 'o':
							text = "concat";
							result = 14;
							break;
						case 'q':
							text = "equals";
							result = 29;
							break;
						case 't':
							text = "strike";
							result = 19;
							break;
						case 'u':
							text = "substr";
							result = 13;
							break;
						}
					}
					else
					{
						text = "charAt";
						result = 5;
					}
				}
				else
				{
					text = "search";
					result = 32;
				}
				break;
			}
			case 7:
			{
				char c = s[1];
				if (c <= 'e')
				{
					if (c != 'a')
					{
						if (c == 'e')
						{
							text = "replace";
							result = 33;
						}
					}
					else
					{
						text = "valueOf";
						result = 4;
					}
				}
				else
				{
					if (c != 'n')
					{
						if (c == 't')
						{
							text = "italics";
							result = 17;
						}
					}
					else
					{
						text = "indexOf";
						result = 7;
					}
				}
				break;
			}
			case 8:
			{
				int num = (int)s[4];
				if (num == 114)
				{
					text = "toString";
					result = 2;
				}
				else
				{
					if (num == 115)
					{
						text = "fontsize";
						result = 25;
					}
					else
					{
						if (num == 117)
						{
							text = "toSource";
							result = 3;
						}
					}
				}
				break;
			}
			case 9:
			{
				int num = (int)s[0];
				if (num == 102)
				{
					text = "fontcolor";
					result = 26;
				}
				else
				{
					if (num == 115)
					{
						text = "substring";
						result = 10;
					}
				}
				break;
			}
			case 10:
				text = "charCodeAt";
				result = 6;
				break;
			case 11:
			{
				char c = s[2];
				if (c <= 'U')
				{
					if (c != 'L')
					{
						if (c == 'U')
						{
							text = "toUpperCase";
							result = 12;
						}
					}
					else
					{
						text = "toLowerCase";
						result = 11;
					}
				}
				else
				{
					if (c != 'n')
					{
						if (c == 's')
						{
							text = "lastIndexOf";
							result = 8;
						}
					}
					else
					{
						text = "constructor";
						result = 1;
					}
				}
				break;
			}
			case 16:
				text = "equalsIgnoreCase";
				result = 30;
				break;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
