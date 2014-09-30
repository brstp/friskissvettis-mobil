using System;
using System.Text;
namespace EcmaScript.NET.Types
{
	internal sealed class BuiltinError : IdScriptableObject
	{
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toSource = 3;
		private const int MAX_PROTOTYPE_ID = 3;
		private static readonly object ERROR_TAG = new object();
		public override string ClassName
		{
			get
			{
				return "Error";
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinError builtinError = new BuiltinError();
			ScriptableObject.PutProperty(builtinError, "name", "Error");
			ScriptableObject.PutProperty(builtinError, "message", "");
			ScriptableObject.PutProperty(builtinError, "fileName", "");
			ScriptableObject.PutProperty(builtinError, "lineNumber", 0);
			ScriptableObject.PutProperty(builtinError, "stack", "NOT IMPLEMENTED");
			builtinError.ExportAsJSClass(3, scope, zealed, 7);
		}
		internal static BuiltinError make(Context cx, IScriptable scope, IdFunctionObject ctorObj, object[] args)
		{
			IScriptable prototype = (IScriptable)ctorObj.Get("prototype", ctorObj);
			BuiltinError builtinError = new BuiltinError();
			builtinError.SetPrototype(prototype);
			builtinError.ParentScope = scope;
			if (args.Length >= 1)
			{
				ScriptableObject.PutProperty(builtinError, "message", ScriptConvert.ToString(args[0]));
				if (args.Length >= 2)
				{
					ScriptableObject.PutProperty(builtinError, "fileName", args[1]);
					if (args.Length >= 3)
					{
						int num = ScriptConvert.ToInt32(args[2]);
						ScriptableObject.PutProperty(builtinError, "lineNumber", num);
					}
				}
			}
			return builtinError;
		}
		public override string ToString()
		{
			return BuiltinError.js_toString(this);
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
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinError.ERROR_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinError.ERROR_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
					result = BuiltinError.make(cx, scope, f, args);
					break;
				case 2:
					result = BuiltinError.js_toString(thisObj);
					break;
				case 3:
					result = BuiltinError.js_toSource(cx, scope, thisObj);
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		private static string js_toString(IScriptable thisObj)
		{
			return BuiltinError.getString(thisObj, "name") + ": " + BuiltinError.getString(thisObj, "message");
		}
		private static string js_toSource(Context cx, IScriptable scope, IScriptable thisObj)
		{
			object obj = ScriptableObject.GetProperty(thisObj, "name");
			object obj2 = ScriptableObject.GetProperty(thisObj, "message");
			object obj3 = ScriptableObject.GetProperty(thisObj, "fileName");
			object property = ScriptableObject.GetProperty(thisObj, "lineNumber");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(new ");
			if (obj == UniqueTag.NotFound)
			{
				obj = Undefined.Value;
			}
			stringBuilder.Append(ScriptConvert.ToString(obj));
			stringBuilder.Append("(");
			if (obj2 != UniqueTag.NotFound || obj3 != UniqueTag.NotFound || property != UniqueTag.NotFound)
			{
				if (obj2 == UniqueTag.NotFound)
				{
					obj2 = "";
				}
				stringBuilder.Append(ScriptRuntime.uneval(cx, scope, obj2));
				if (obj3 != UniqueTag.NotFound || property != UniqueTag.NotFound)
				{
					stringBuilder.Append(", ");
					if (obj3 == UniqueTag.NotFound)
					{
						obj3 = "";
					}
					stringBuilder.Append(ScriptRuntime.uneval(cx, scope, obj3));
					if (property != UniqueTag.NotFound)
					{
						int num = ScriptConvert.ToInt32(property);
						if (num != 0)
						{
							stringBuilder.Append(", ");
							stringBuilder.Append(ScriptConvert.ToString((double)num));
						}
					}
				}
			}
			stringBuilder.Append("))");
			return stringBuilder.ToString();
		}
		private static string getString(IScriptable obj, string id)
		{
			object property = ScriptableObject.GetProperty(obj, id);
			string result;
			if (property == UniqueTag.NotFound)
			{
				result = "";
			}
			else
			{
				result = ScriptConvert.ToString(property);
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
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
			if (text != null && text != s && !text.Equals(s))
			{
				result = 0;
			}
			return result;
		}
	}
}
