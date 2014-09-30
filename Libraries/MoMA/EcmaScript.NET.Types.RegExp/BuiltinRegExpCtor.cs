using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class BuiltinRegExpCtor : BaseFunction
	{
		private const int Id_multiline = 1;
		private const int Id_STAR = 2;
		private const int Id_input = 3;
		private const int Id_UNDERSCORE = 4;
		private const int Id_lastMatch = 5;
		private const int Id_AMPERSAND = 6;
		private const int Id_lastParen = 7;
		private const int Id_PLUS = 8;
		private const int Id_leftContext = 9;
		private const int Id_BACK_QUOTE = 10;
		private const int Id_rightContext = 11;
		private const int Id_QUOTE = 12;
		private const int DOLLAR_ID_BASE = 12;
		private const int Id_DOLLAR_1 = 13;
		private const int Id_DOLLAR_2 = 14;
		private const int Id_DOLLAR_3 = 15;
		private const int Id_DOLLAR_4 = 16;
		private const int Id_DOLLAR_5 = 17;
		private const int Id_DOLLAR_6 = 18;
		private const int Id_DOLLAR_7 = 19;
		private const int Id_DOLLAR_8 = 20;
		private const int Id_DOLLAR_9 = 21;
		private const int MAX_INSTANCE_ID = 21;
		public override string FunctionName
		{
			get
			{
				return "RegExp";
			}
		}
		private static RegExpImpl Impl
		{
			get
			{
				Context currentContext = Context.CurrentContext;
				return (RegExpImpl)currentContext.RegExpProxy;
			}
		}
		protected internal override int MaxInstanceId
		{
			get
			{
				return base.MaxInstanceId + 21;
			}
		}
		internal BuiltinRegExpCtor()
		{
		}
		public override object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (args.Length > 0 && args[0] is BuiltinRegExp && (args.Length == 1 || args[1] == Undefined.Value))
			{
				result = args[0];
			}
			else
			{
				result = this.Construct(cx, scope, args);
			}
			return result;
		}
		public override IScriptable Construct(Context cx, IScriptable scope, object[] args)
		{
			BuiltinRegExp builtinRegExp = new BuiltinRegExp();
			builtinRegExp.compile(cx, scope, args);
			ScriptRuntime.setObjectProtoAndParent(builtinRegExp, scope);
			return builtinRegExp;
		}
		protected internal override int FindInstanceIdInfo(string s)
		{
			int num = 0;
			string text = null;
			switch (s.Length)
			{
			case 2:
			{
				char c = s[1];
				switch (c)
				{
				case '&':
					if (s[0] == '$')
					{
						num = 6;
						goto IL_389;
					}
					break;
				case '\'':
					if (s[0] == '$')
					{
						num = 12;
						goto IL_389;
					}
					break;
				case '(':
				case ')':
				case ',':
				case '-':
				case '.':
				case '/':
				case '0':
					break;
				case '*':
					if (s[0] == '$')
					{
						num = 2;
						goto IL_389;
					}
					break;
				case '+':
					if (s[0] == '$')
					{
						num = 8;
						goto IL_389;
					}
					break;
				case '1':
					if (s[0] == '$')
					{
						num = 13;
						goto IL_389;
					}
					break;
				case '2':
					if (s[0] == '$')
					{
						num = 14;
						goto IL_389;
					}
					break;
				case '3':
					if (s[0] == '$')
					{
						num = 15;
						goto IL_389;
					}
					break;
				case '4':
					if (s[0] == '$')
					{
						num = 16;
						goto IL_389;
					}
					break;
				case '5':
					if (s[0] == '$')
					{
						num = 17;
						goto IL_389;
					}
					break;
				case '6':
					if (s[0] == '$')
					{
						num = 18;
						goto IL_389;
					}
					break;
				case '7':
					if (s[0] == '$')
					{
						num = 19;
						goto IL_389;
					}
					break;
				case '8':
					if (s[0] == '$')
					{
						num = 20;
						goto IL_389;
					}
					break;
				case '9':
					if (s[0] == '$')
					{
						num = 21;
						goto IL_389;
					}
					break;
				default:
					if (c == '_')
					{
						if (s[0] == '$')
						{
							num = 4;
							goto IL_389;
						}
					}
					break;
				}
				break;
			}
			case 5:
				text = "input";
				num = 3;
				break;
			case 9:
			{
				int num2 = (int)s[4];
				if (num2 == 77)
				{
					text = "lastMatch";
					num = 5;
				}
				else
				{
					if (num2 == 80)
					{
						text = "lastParen";
						num = 7;
					}
					else
					{
						if (num2 == 105)
						{
							text = "multiline";
							num = 1;
						}
					}
				}
				break;
			}
			case 10:
				text = "BACK_QUOTE";
				num = 10;
				break;
			case 11:
				text = "leftContext";
				num = 9;
				break;
			case 12:
				text = "rightContext";
				num = 11;
				break;
			}
			if (text != null && text != s && !text.Equals(s))
			{
				num = 0;
			}
			IL_389:
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
				case 2:
				case 3:
				case 4:
					attributes = 4;
					break;
				default:
					attributes = 5;
					break;
				}
				result = IdScriptableObject.InstanceIdInfo(attributes, base.MaxInstanceId + num);
			}
			return result;
		}
		protected internal override string GetInstanceIdName(int id)
		{
			int num = id - base.MaxInstanceId;
			string result;
			if (1 <= num && num <= 21)
			{
				switch (num)
				{
				case 1:
					result = "multiline";
					break;
				case 2:
					result = "$*";
					break;
				case 3:
					result = "input";
					break;
				case 4:
					result = "$_";
					break;
				case 5:
					result = "lastMatch";
					break;
				case 6:
					result = "$&";
					break;
				case 7:
					result = "lastParen";
					break;
				case 8:
					result = "$+";
					break;
				case 9:
					result = "leftContext";
					break;
				case 10:
					result = "$`";
					break;
				case 11:
					result = "rightContext";
					break;
				case 12:
					result = "$'";
					break;
				default:
				{
					int num2 = num - 12 - 1;
					char[] value = new char[]
					{
						'$',
						(char)(49 + num2)
					};
					result = new string(value);
					break;
				}
				}
			}
			else
			{
				result = base.GetInstanceIdName(id);
			}
			return result;
		}
		protected internal override object GetInstanceIdValue(int id)
		{
			int num = id - base.MaxInstanceId;
			object result;
			if (1 <= num && num <= 21)
			{
				RegExpImpl impl = BuiltinRegExpCtor.Impl;
				object obj;
				switch (num)
				{
				case 1:
				case 2:
					result = impl.multiline;
					return result;
				case 3:
				case 4:
					obj = impl.input;
					break;
				case 5:
				case 6:
					obj = impl.lastMatch;
					break;
				case 7:
				case 8:
					obj = impl.lastParen;
					break;
				case 9:
				case 10:
					obj = impl.leftContext;
					break;
				case 11:
				case 12:
					obj = impl.rightContext;
					break;
				default:
				{
					int i = num - 12 - 1;
					obj = impl.getParenSubString(i);
					break;
				}
				}
				result = ((obj == null) ? "" : obj.ToString());
			}
			else
			{
				result = base.GetInstanceIdValue(id);
			}
			return result;
		}
		protected internal override void SetInstanceIdValue(int id, object value)
		{
			switch (id - base.MaxInstanceId)
			{
			case 1:
			case 2:
				BuiltinRegExpCtor.Impl.multiline = ScriptConvert.ToBoolean(value);
				break;
			case 3:
			case 4:
				BuiltinRegExpCtor.Impl.input = ScriptConvert.ToString(value);
				break;
			default:
				base.SetInstanceIdValue(id, value);
				break;
			}
		}
	}
}
