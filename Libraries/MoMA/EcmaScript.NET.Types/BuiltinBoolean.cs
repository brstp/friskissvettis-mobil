using System;
namespace EcmaScript.NET.Types
{
	internal sealed class BuiltinBoolean : IdScriptableObject
	{
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int Id_valueOf = 4;
		private const int MAX_PROTOTYPE_ID = 4;
		private static readonly object BOOLEAN_TAG = new object();
		private bool booleanValue;
		public override string ClassName
		{
			get
			{
				return "Boolean";
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinBoolean builtinBoolean = new BuiltinBoolean(false);
			builtinBoolean.ExportAsJSClass(4, scope, zealed, 7);
		}
		private BuiltinBoolean(bool b)
		{
			this.booleanValue = b;
		}
		public override object GetDefaultValue(Type typeHint)
		{
			object result;
			if (typeHint == typeof(bool))
			{
				result = this.booleanValue;
			}
			else
			{
				result = base.GetDefaultValue(typeHint);
			}
			return result;
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
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinBoolean.BOOLEAN_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinBoolean.BOOLEAN_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				if (methodId == 1)
				{
					bool flag = ScriptConvert.ToBoolean(args, 0);
					if (thisObj == null)
					{
						result = new BuiltinBoolean(flag);
					}
					else
					{
						result = flag;
					}
				}
				else
				{
					if (!(thisObj is BuiltinBoolean))
					{
						throw IdScriptableObject.IncompatibleCallError(f);
					}
					bool flag2 = ((BuiltinBoolean)thisObj).booleanValue;
					switch (methodId)
					{
					case 2:
						result = (flag2 ? "true" : "false");
						break;
					case 3:
						result = (flag2 ? "(new Boolean(true))" : "(new Boolean(false))");
						break;
					case 4:
						result = flag2;
						break;
					default:
						throw new ArgumentException(Convert.ToString(methodId));
					}
				}
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			if (length == 7)
			{
				text = "valueOf";
				result = 4;
			}
			else
			{
				if (length == 8)
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
				}
				else
				{
					if (length == 11)
					{
						text = "constructor";
						result = 1;
					}
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
