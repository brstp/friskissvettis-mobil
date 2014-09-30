using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET.Types.RegExp
{
	[ComVisible(true)]
	public class RegExpImpl : RegExpProxy
	{
		internal string input;
		internal bool multiline;
		internal SubString[] parens;
		internal SubString lastMatch;
		internal SubString lastParen;
		internal SubString leftContext;
		internal SubString rightContext;
		public virtual bool IsRegExp(IScriptable obj)
		{
			return obj is BuiltinRegExp;
		}
		public virtual object Compile(Context cx, string source, string flags)
		{
			return BuiltinRegExp.compileRE(source, flags, false);
		}
		public virtual IScriptable Wrap(Context cx, IScriptable scope, object compiled)
		{
			return new BuiltinRegExp(scope, compiled);
		}
		public virtual object Perform(Context cx, IScriptable scope, IScriptable thisObj, object[] args, RegExpActions actionType)
		{
			GlobData globData = new GlobData();
			globData.mode = actionType;
			object result;
			switch (actionType)
			{
			case RegExpActions.Match:
			{
				globData.optarg = 1;
				object obj = RegExpImpl.matchOrReplace(cx, scope, thisObj, args, this, globData, false);
				result = ((globData.arrayobj == null) ? obj : globData.arrayobj);
				break;
			}
			case RegExpActions.Replace:
			{
				object obj2 = (args.Length < 2) ? Undefined.Value : args[1];
				string text = null;
				IFunction lambda = null;
				if (obj2 is IFunction)
				{
					lambda = (IFunction)obj2;
				}
				else
				{
					text = ScriptConvert.ToString(obj2);
				}
				globData.optarg = 2;
				globData.lambda = lambda;
				globData.repstr = text;
				globData.dollar = ((text == null) ? -1 : text.IndexOf('$'));
				globData.charBuf = null;
				globData.leftIndex = 0;
				object obj3 = RegExpImpl.matchOrReplace(cx, scope, thisObj, args, this, globData, true);
				SubString subString = this.rightContext;
				if (globData.charBuf == null)
				{
					if (globData.global || obj3 == null || !obj3.Equals(true))
					{
						result = globData.str;
						break;
					}
					SubString subString2 = this.leftContext;
					RegExpImpl.replace_glob(globData, cx, scope, this, subString2.index, subString2.length);
				}
				globData.charBuf.Append(subString.charArray, subString.index, subString.length);
				result = globData.charBuf.ToString();
				break;
			}
			case RegExpActions.Search:
				globData.optarg = 1;
				result = RegExpImpl.matchOrReplace(cx, scope, thisObj, args, this, globData, false);
				break;
			default:
				throw Context.CodeBug();
			}
			return result;
		}
		private static object matchOrReplace(Context cx, IScriptable scope, IScriptable thisObj, object[] args, RegExpImpl reImpl, GlobData data, bool forceFlat)
		{
			string text = ScriptConvert.ToString(thisObj);
			data.str = text;
			IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(scope);
			BuiltinRegExp builtinRegExp;
			if (args.Length == 0)
			{
				object regexpCompiled = BuiltinRegExp.compileRE("", "", false);
				builtinRegExp = new BuiltinRegExp(topLevelScope, regexpCompiled);
			}
			else
			{
				if (args[0] is BuiltinRegExp)
				{
					builtinRegExp = (BuiltinRegExp)args[0];
				}
				else
				{
					string text2 = ScriptConvert.ToString(args[0]);
					string global;
					if (data.optarg < args.Length)
					{
						args[0] = text2;
						global = ScriptConvert.ToString(args[data.optarg]);
					}
					else
					{
						global = null;
					}
					object regexpCompiled = BuiltinRegExp.compileRE(text2, global, forceFlat);
					builtinRegExp = new BuiltinRegExp(topLevelScope, regexpCompiled);
				}
			}
			data.regexp = builtinRegExp;
			data.global = ((builtinRegExp.Flags & 1) != 0);
			int[] array = new int[1];
			int[] array2 = array;
			object obj = null;
			if (data.mode == RegExpActions.Search)
			{
				obj = builtinRegExp.executeRegExp(cx, scope, reImpl, text, array2, 0);
				if (obj != null && obj.Equals(true))
				{
					obj = reImpl.leftContext.length;
				}
				else
				{
					obj = -1;
				}
			}
			else
			{
				if (data.global)
				{
					builtinRegExp.lastIndex = 0.0;
					int num = 0;
					while (array2[0] <= text.Length)
					{
						obj = builtinRegExp.executeRegExp(cx, scope, reImpl, text, array2, 0);
						if (obj == null || !obj.Equals(true))
						{
							break;
						}
						if (data.mode == RegExpActions.Match)
						{
							RegExpImpl.match_glob(data, cx, scope, num, reImpl);
						}
						else
						{
							if (data.mode != RegExpActions.Replace)
							{
								Context.CodeBug();
							}
							SubString subString = reImpl.lastMatch;
							int leftIndex = data.leftIndex;
							int leftlen = subString.index - leftIndex;
							data.leftIndex = subString.index + subString.length;
							RegExpImpl.replace_glob(data, cx, scope, reImpl, leftIndex, leftlen);
						}
						if (reImpl.lastMatch.length == 0)
						{
							if (array2[0] == text.Length)
							{
								break;
							}
							array2[0]++;
						}
						num++;
					}
				}
				else
				{
					obj = builtinRegExp.executeRegExp(cx, scope, reImpl, text, array2, (data.mode == RegExpActions.Replace) ? 0 : 1);
				}
			}
			return obj;
		}
		public virtual int FindSplit(Context cx, IScriptable scope, string target, string separator, IScriptable reObj, int[] ip, int[] matchlen, bool[] matched, string[][] parensp)
		{
			int num = ip[0];
			int length = target.Length;
			Context.Versions version = cx.Version;
			BuiltinRegExp builtinRegExp = (BuiltinRegExp)reObj;
			int num2;
			while (true)
			{
				num2 = ip[0];
				ip[0] = num;
				object obj = builtinRegExp.executeRegExp(cx, scope, this, target, ip, 0);
				if (obj == null || !obj.Equals(true))
				{
					break;
				}
				num = ip[0];
				ip[0] = num2;
				matched[0] = true;
				SubString subString = this.lastMatch;
				matchlen[0] = subString.length;
				if (matchlen[0] != 0)
				{
					goto IL_106;
				}
				if (num != ip[0])
				{
					goto IL_105;
				}
				if (num == length)
				{
					goto Block_5;
				}
				num++;
			}
			ip[0] = num2;
			matchlen[0] = 1;
			matched[0] = false;
			int result = length;
			return result;
			Block_5:
			int num3;
			if (version == Context.Versions.JS1_2)
			{
				matchlen[0] = 1;
				num3 = num;
			}
			else
			{
				num3 = -1;
			}
			goto IL_11C;
			IL_105:
			IL_106:
			num3 = num - matchlen[0];
			IL_11C:
			int num4 = (this.parens == null) ? 0 : this.parens.Length;
			parensp[0] = new string[num4];
			for (int i = 0; i < num4; i++)
			{
				SubString parenSubString = this.getParenSubString(i);
				parensp[0][i] = parenSubString.ToString();
			}
			result = num3;
			return result;
		}
		internal virtual SubString getParenSubString(int i)
		{
			SubString result;
			if (this.parens != null && i < this.parens.Length)
			{
				SubString subString = this.parens[i];
				if (subString != null)
				{
					result = subString;
					return result;
				}
			}
			result = SubString.EmptySubString;
			return result;
		}
		private static void match_glob(GlobData mdata, Context cx, IScriptable scope, int count, RegExpImpl reImpl)
		{
			if (mdata.arrayobj == null)
			{
				IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(scope);
				mdata.arrayobj = ScriptRuntime.NewObject(cx, topLevelScope, "Array", null);
			}
			SubString subString = reImpl.lastMatch;
			string value = subString.ToString();
			mdata.arrayobj.Put(count, mdata.arrayobj, value);
		}
		private static void replace_glob(GlobData rdata, Context cx, IScriptable scope, RegExpImpl reImpl, int leftIndex, int leftlen)
		{
			string text;
			int num2;
			if (rdata.lambda != null)
			{
				SubString[] array = reImpl.parens;
				int num = (array == null) ? 0 : array.Length;
				object[] array2 = new object[num + 3];
				array2[0] = reImpl.lastMatch.ToString();
				for (int i = 0; i < num; i++)
				{
					SubString subString = array[i];
					if (subString != null)
					{
						array2[i + 1] = subString.ToString();
					}
					else
					{
						array2[i + 1] = Undefined.Value;
					}
				}
				array2[num + 1] = reImpl.leftContext.length;
				array2[num + 2] = rdata.str;
				if (reImpl != cx.RegExpProxy)
				{
					Context.CodeBug();
				}
				cx.RegExpProxy = new RegExpImpl
				{
					multiline = reImpl.multiline,
					input = reImpl.input
				};
				try
				{
					IScriptable topLevelScope = ScriptableObject.GetTopLevelScope(scope);
					object val = rdata.lambda.Call(cx, topLevelScope, topLevelScope, array2);
					text = ScriptConvert.ToString(val);
				}
				finally
				{
					cx.RegExpProxy = reImpl;
				}
				num2 = text.Length;
			}
			else
			{
				text = null;
				num2 = rdata.repstr.Length;
				if (rdata.dollar >= 0)
				{
					int[] array3 = new int[1];
					int num3 = rdata.dollar;
					do
					{
						SubString subString = RegExpImpl.interpretDollar(cx, reImpl, rdata.repstr, num3, array3);
						if (subString != null)
						{
							num2 += subString.length - array3[0];
							num3 += array3[0];
						}
						else
						{
							num3++;
						}
						num3 = rdata.repstr.IndexOf('$', num3);
					}
					while (num3 >= 0);
				}
			}
			int num4 = leftlen + num2 + reImpl.rightContext.length;
			StringBuilder stringBuilder = rdata.charBuf;
			if (stringBuilder == null)
			{
				stringBuilder = new StringBuilder(num4);
				rdata.charBuf = stringBuilder;
			}
			else
			{
				stringBuilder.EnsureCapacity(rdata.charBuf.Length + num4);
			}
			stringBuilder.Append(reImpl.leftContext.charArray, leftIndex, leftlen);
			if (rdata.lambda != null)
			{
				stringBuilder.Append(text);
			}
			else
			{
				RegExpImpl.do_replace(rdata, cx, reImpl);
			}
		}
		private static SubString interpretDollar(Context cx, RegExpImpl res, string da, int dp, int[] skip)
		{
			if (da[dp] != '$')
			{
				Context.CodeBug();
			}
			Context.Versions version = cx.Version;
			SubString result;
			if (version != Context.Versions.Default && version <= Context.Versions.JS1_4)
			{
				if (dp > 0 && da[dp - 1] == '\\')
				{
					result = null;
					return result;
				}
			}
			int length = da.Length;
			if (dp + 1 >= length)
			{
				result = null;
			}
			else
			{
				char c = da[dp + 1];
				if (BuiltinRegExp.isDigit(c))
				{
					int num;
					int num2;
					if (version != Context.Versions.Default && version <= Context.Versions.JS1_4)
					{
						if (c == '0')
						{
							result = null;
							return result;
						}
						num = 0;
						num2 = dp;
						while (++num2 < length && BuiltinRegExp.isDigit(c = da[num2]))
						{
							int num3 = 10 * num + (int)(c - '0');
							if (num3 < num)
							{
								break;
							}
							num = num3;
						}
					}
					else
					{
						int num4 = (res.parens == null) ? 0 : res.parens.Length;
						num = (int)(c - '0');
						if (num > num4)
						{
							result = null;
							return result;
						}
						num2 = dp + 2;
						if (dp + 2 < length)
						{
							c = da[dp + 2];
							if (BuiltinRegExp.isDigit(c))
							{
								int num3 = 10 * num + (int)(c - '0');
								if (num3 <= num4)
								{
									num2++;
									num = num3;
								}
							}
						}
						if (num == 0)
						{
							result = null;
							return result;
						}
					}
					num--;
					skip[0] = num2 - dp;
					result = res.getParenSubString(num);
				}
				else
				{
					skip[0] = 2;
					char c2 = c;
					switch (c2)
					{
					case '$':
						result = new SubString("$");
						return result;
					case '%':
						break;
					case '&':
						result = res.lastMatch;
						return result;
					case '\'':
						result = res.rightContext;
						return result;
					default:
						if (c2 == '+')
						{
							result = res.lastParen;
							return result;
						}
						if (c2 == '`')
						{
							if (version == Context.Versions.JS1_2)
							{
								res.leftContext.index = 0;
								res.leftContext.length = res.lastMatch.index;
							}
							result = res.leftContext;
							return result;
						}
						break;
					}
					result = null;
				}
			}
			return result;
		}
		private static void do_replace(GlobData rdata, Context cx, RegExpImpl regExpImpl)
		{
			StringBuilder charBuf = rdata.charBuf;
			int num = 0;
			string repstr = rdata.repstr;
			int num2 = rdata.dollar;
			if (num2 != -1)
			{
				int[] array = new int[1];
				do
				{
					int num3 = num2 - num;
					charBuf.Append(repstr.Substring(num, num2 - num));
					num = num2;
					SubString subString = RegExpImpl.interpretDollar(cx, regExpImpl, repstr, num2, array);
					if (subString != null)
					{
						num3 = subString.length;
						if (num3 > 0)
						{
							charBuf.Append(subString.charArray, subString.index, num3);
						}
						num += array[0];
						num2 += array[0];
					}
					else
					{
						num2++;
					}
					num2 = repstr.IndexOf('$', num2);
				}
				while (num2 >= 0);
			}
			int length = repstr.Length;
			if (length > num)
			{
				charBuf.Append(repstr.Substring(num, length - num));
			}
		}
	}
}
