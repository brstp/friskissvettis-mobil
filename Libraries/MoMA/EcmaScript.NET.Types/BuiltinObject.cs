using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Types
{
	[ComVisible(true)]
	public class BuiltinObject : IdScriptableObject
	{
		private const int Id_constructor = 1;
		private const int Id_toString = 2;
		private const int Id_toLocaleString = 3;
		private const int Id_valueOf = 4;
		private const int Id_hasOwnProperty = 5;
		private const int Id_propertyIsEnumerable = 6;
		private const int Id_isPrototypeOf = 7;
		private const int Id_toSource = 8;
		private const int Id___defineGetter__ = 9;
		private const int Id___defineSetter__ = 10;
		private const int Id___lookupGetter__ = 11;
		private const int Id___lookupSetter__ = 12;
		private const int MAX_PROTOTYPE_ID = 12;
		private static readonly object OBJECT_TAG = new object();
		public override string ClassName
		{
			get
			{
				return "Object";
			}
		}
		internal static void Init(IScriptable scope, bool zealed)
		{
			BuiltinObject builtinObject = new BuiltinObject();
			builtinObject.ExportAsJSClass(12, scope, zealed);
		}
		public override string ToString()
		{
			return ScriptRuntime.DefaultObjectToString(this);
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
				name = "toLocaleString";
				break;
			case 4:
				arity = 0;
				name = "valueOf";
				break;
			case 5:
				arity = 1;
				name = "hasOwnProperty";
				break;
			case 6:
				arity = 1;
				name = "propertyIsEnumerable";
				break;
			case 7:
				arity = 1;
				name = "isPrototypeOf";
				break;
			case 8:
				arity = 0;
				name = "toSource";
				break;
			case 9:
				arity = 2;
				name = "__defineGetter__";
				break;
			case 10:
				arity = 2;
				name = "__defineSetter__";
				break;
			case 11:
				arity = 2;
				name = "__lookupGetter__";
				break;
			case 12:
				arity = 2;
				name = "__lookupSetter__";
				break;
			default:
				throw new ArgumentException(Convert.ToString(id));
			}
			base.InitPrototypeMethod(BuiltinObject.OBJECT_TAG, id, name, arity);
		}
		public override object ExecIdCall(IdFunctionObject f, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			object result;
			if (!f.HasTag(BuiltinObject.OBJECT_TAG))
			{
				result = base.ExecIdCall(f, cx, scope, thisObj, args);
			}
			else
			{
				int methodId = f.MethodId;
				switch (methodId)
				{
				case 1:
					if (thisObj != null)
					{
						result = f.Construct(cx, scope, args);
					}
					else
					{
						if (args.Length == 0 || args[0] == null || args[0] == Undefined.Value)
						{
							result = new BuiltinObject();
						}
						else
						{
							result = ScriptConvert.ToObject(cx, scope, args[0]);
						}
					}
					break;
				case 2:
				case 3:
					if (cx.HasFeature(Context.Features.ToStringAsSource))
					{
						string text = ScriptRuntime.defaultObjectToSource(cx, scope, thisObj, args);
						int length = text.Length;
						if (length != 0 && text[0] == '(' && text[length - 1] == ')')
						{
							text = text.Substring(1, length - 1 - 1);
						}
						result = text;
					}
					else
					{
						result = ScriptRuntime.DefaultObjectToString(thisObj);
					}
					break;
				case 4:
					result = thisObj;
					break;
				case 5:
				{
					bool flag;
					if (args.Length == 0)
					{
						flag = false;
					}
					else
					{
						string text = ScriptRuntime.ToStringIdOrIndex(cx, args[0]);
						if (text == null)
						{
							int index = ScriptRuntime.lastIndexResult(cx);
							flag = thisObj.Has(index, thisObj);
						}
						else
						{
							flag = thisObj.Has(text, thisObj);
						}
					}
					result = flag;
					break;
				}
				case 6:
				{
					bool flag;
					if (args.Length == 0)
					{
						flag = false;
					}
					else
					{
						string text = ScriptRuntime.ToStringIdOrIndex(cx, args[0]);
						if (text == null)
						{
							int index = ScriptRuntime.lastIndexResult(cx);
							flag = thisObj.Has(index, thisObj);
							if (flag && thisObj is ScriptableObject)
							{
								ScriptableObject scriptableObject = (ScriptableObject)thisObj;
								int attributes = scriptableObject.GetAttributes(index);
								flag = ((attributes & 2) == 0);
							}
						}
						else
						{
							flag = thisObj.Has(text, thisObj);
							if (flag && thisObj is ScriptableObject)
							{
								ScriptableObject scriptableObject = (ScriptableObject)thisObj;
								int attributes = scriptableObject.GetAttributes(text);
								flag = ((attributes & 2) == 0);
							}
						}
					}
					result = flag;
					break;
				}
				case 7:
				{
					bool flag = false;
					if (args.Length != 0 && args[0] is IScriptable)
					{
						IScriptable scriptable = (IScriptable)args[0];
						while (true)
						{
							scriptable = scriptable.GetPrototype();
							if (scriptable == thisObj)
							{
								break;
							}
							if (scriptable == null)
							{
								goto IL_32D;
							}
						}
						flag = true;
						IL_32D:;
					}
					result = flag;
					break;
				}
				case 8:
					result = ScriptRuntime.defaultObjectToSource(cx, scope, thisObj, args);
					break;
				case 9:
				case 10:
				{
					if (args.Length < 2 || (args.Length > 0 && !(args[1] is ICallable)))
					{
						object value = (args.Length > 1) ? args[1] : Undefined.Value;
						throw ScriptRuntime.NotFunctionError(value);
					}
					string text2 = ScriptRuntime.ToStringIdOrIndex(cx, args[0]);
					int index = (text2 != null) ? text2.GetHashCode() : ScriptRuntime.lastIndexResult(cx);
					ScriptableObject scriptableObject = thisObj as ScriptableObject;
					if (scriptableObject == null)
					{
						throw ScriptRuntime.TypeError("this is not a scriptable object.");
					}
					ICallable callable = (ICallable)args[1];
					if (methodId == 9)
					{
						if (text2 == null)
						{
							scriptableObject.DefineGetter(index, callable);
						}
						else
						{
							scriptableObject.DefineGetter(text2, callable);
						}
					}
					else
					{
						if (text2 == null)
						{
							scriptableObject.DefineSetter(index, callable);
						}
						else
						{
							scriptableObject.DefineSetter(text2, callable);
						}
					}
					result = Undefined.Value;
					break;
				}
				case 11:
				case 12:
					if (args.Length < 1)
					{
						result = Undefined.Value;
					}
					else
					{
						string text2 = ScriptRuntime.ToStringIdOrIndex(cx, args[0]);
						int arg_38E_0 = (text2 != null) ? text2.GetHashCode() : ScriptRuntime.lastIndexResult(cx);
						ScriptableObject scriptableObject = thisObj as ScriptableObject;
						if (scriptableObject == null)
						{
							throw ScriptRuntime.TypeError("this is not a scriptable object.");
						}
						if (methodId == 11)
						{
							result = scriptableObject.LookupGetter(text2);
						}
						else
						{
							result = scriptableObject.LookupSetter(text2);
						}
					}
					break;
				default:
					throw new ArgumentException(Convert.ToString(methodId));
				}
			}
			return result;
		}
		protected internal override int FindPrototypeId(string s)
		{
			int result = 0;
			string text = null;
			int length = s.Length;
			switch (length)
			{
			case 7:
				text = "valueOf";
				result = 4;
				break;
			case 8:
			{
				int num = (int)s[3];
				if (num == 111)
				{
					text = "toSource";
					result = 8;
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
			case 9:
			case 10:
			case 12:
			case 15:
				break;
			case 11:
				text = "constructor";
				result = 1;
				break;
			case 13:
				text = "isPrototypeOf";
				result = 7;
				break;
			case 14:
			{
				int num = (int)s[0];
				if (num == 104)
				{
					text = "hasOwnProperty";
					result = 5;
				}
				else
				{
					if (num == 116)
					{
						text = "toLocaleString";
						result = 3;
					}
				}
				break;
			}
			case 16:
			{
				int num = (int)s[2];
				if (num == 100)
				{
					num = (int)s[8];
					if (num == 71)
					{
						text = "__defineGetter__";
						result = 9;
					}
					else
					{
						if (num == 83)
						{
							text = "__defineSetter__";
							result = 10;
						}
					}
				}
				else
				{
					if (num == 108)
					{
						num = (int)s[8];
						if (num == 71)
						{
							text = "__lookupGetter__";
							result = 11;
						}
						else
						{
							if (num == 83)
							{
								text = "__lookupSetter__";
								result = 12;
							}
						}
					}
				}
				break;
			}
			default:
				if (length == 20)
				{
					text = "propertyIsEnumerable";
					result = 6;
				}
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
