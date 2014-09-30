using EcmaScript.NET.Collections;
using EcmaScript.NET.Helpers;
using EcmaScript.NET.Types;
using EcmaScript.NET.Types.Cli;
using EcmaScript.NET.Types.E4X;
using EcmaScript.NET.Types.RegExp;
using System;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class ScriptRuntime
	{
		public const int MAXSTACKSIZE = 1000;
		private const string XML_INIT_CLASS = "EcmaScript.NET.Xml.Impl.XMLLib";
		private const string DEFAULT_NS_TAG = "__default_namespace__";
		private static readonly object LIBRARY_SCOPE_KEY = new object();
		private static ResourceManager m_ResourceManager = null;
		public static readonly object[] EmptyArgs = new object[0];
		public static readonly string[] EmptyStrings = new string[0];
		protected internal ScriptRuntime()
		{
		}
		public static bool IsNativeRuntimeType(Type cl)
		{
			bool result;
			if (cl.IsPrimitive)
			{
				result = (cl != typeof(char));
			}
			else
			{
				result = (cl == typeof(string) || cl == typeof(bool) || CliHelper.IsNumberType(cl) || typeof(IScriptable).IsAssignableFrom(cl));
			}
			return result;
		}
		public static ScriptableObject InitStandardObjects(Context cx, ScriptableObject scope, bool zealed)
		{
			if (scope == null)
			{
				scope = new BuiltinObject();
			}
			scope.AssociateValue(ScriptRuntime.LIBRARY_SCOPE_KEY, scope);
			BaseFunction.Init(scope, zealed);
			BuiltinObject.Init(scope, zealed);
			IScriptable objectPrototype = ScriptableObject.GetObjectPrototype(scope);
			IScriptable functionPrototype = ScriptableObject.GetFunctionPrototype(scope);
			functionPrototype.SetPrototype(objectPrototype);
			if (scope.GetPrototype() == null)
			{
				scope.SetPrototype(objectPrototype);
			}
			BuiltinError.Init(scope, zealed);
			BuiltinGlobal.Init(cx, scope, zealed);
			if (scope is BuiltinGlobalObject)
			{
				((BuiltinGlobalObject)scope).Init(scope, zealed);
			}
			BuiltinArray.Init(scope, zealed);
			BuiltinString.Init(scope, zealed);
			BuiltinBoolean.Init(scope, zealed);
			BuiltinNumber.Init(scope, zealed);
			BuiltinDate.Init(scope, zealed);
			BuiltinMath.Init(scope, zealed);
			BuiltinWith.Init(scope, zealed);
			BuiltinCall.Init(scope, zealed);
			BuiltinScript.Init(scope, zealed);
			BuiltinRegExp.Init(scope, zealed);
			if (cx.HasFeature(Context.Features.E4x))
			{
				XMLLib.Init(scope, zealed);
			}
			Continuation.Init(scope, zealed);
			if (cx.HasFeature(Context.Features.NonEcmaItObject))
			{
				ScriptRuntime.InitItObject(cx, scope);
			}
			return scope;
		}
		private static void InitItObject(Context cx, ScriptableObject scope)
		{
			BuiltinObject builtinObject = new BuiltinObject();
			builtinObject.SetPrototype(scope);
			builtinObject.DefineProperty("color", Undefined.Value, 4);
			builtinObject.DefineProperty("height", Undefined.Value, 4);
			builtinObject.DefineProperty("width", Undefined.Value, 4);
			builtinObject.DefineProperty("funny", Undefined.Value, 4);
			builtinObject.DefineProperty("array", Undefined.Value, 4);
			builtinObject.DefineProperty("rdonly", Undefined.Value, 1);
			scope.DefineProperty("it", builtinObject, 4);
		}
		public static ScriptableObject getLibraryScopeOrNull(IScriptable scope)
		{
			return (ScriptableObject)ScriptableObject.GetTopScopeValue(scope, ScriptRuntime.LIBRARY_SCOPE_KEY);
		}
		public static bool isJSLineTerminator(int c)
		{
			return (c & 57296) == 0 && (c == 10 || c == 13 || c == 8232 || c == 8233);
		}
		public static object[] padArguments(object[] args, int count)
		{
			object[] result;
			if (count < args.Length)
			{
				result = args;
			}
			else
			{
				object[] array = new object[count];
				int i;
				for (i = 0; i < args.Length; i++)
				{
					array[i] = args[i];
				}
				while (i < count)
				{
					array[i] = Undefined.Value;
					i++;
				}
				result = array;
			}
			return result;
		}
		public static string escapeString(string s)
		{
			return ScriptRuntime.escapeString(s, '"');
		}
		public static string escapeString(string s, char escapeQuote)
		{
			if (escapeQuote != '"' && escapeQuote != '\'')
			{
				Context.CodeBug();
			}
			StringBuilder stringBuilder = null;
			int num = 0;
			int length = s.Length;
			while (num != length)
			{
				int num2 = (int)s[num];
				if (32 <= num2 && num2 <= 126 && num2 != (int)escapeQuote && num2 != 92)
				{
					if (stringBuilder != null)
					{
						stringBuilder.Append((char)num2);
					}
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(length + 3);
						stringBuilder.Append(s);
						stringBuilder.Length = num;
					}
					int num3 = -1;
					int num4 = num2;
					switch (num4)
					{
					case 8:
						num3 = 98;
						break;
					case 9:
						num3 = 116;
						break;
					case 10:
						num3 = 110;
						break;
					case 11:
						num3 = 118;
						break;
					case 12:
						num3 = 102;
						break;
					case 13:
						num3 = 114;
						break;
					default:
						if (num4 != 32)
						{
							if (num4 == 92)
							{
								num3 = 92;
							}
						}
						else
						{
							num3 = 32;
						}
						break;
					}
					if (num3 >= 0)
					{
						stringBuilder.Append('\\');
						stringBuilder.Append((char)num3);
					}
					else
					{
						if (num2 == (int)escapeQuote)
						{
							stringBuilder.Append('\\');
							stringBuilder.Append(escapeQuote);
						}
						else
						{
							int num5;
							if (num2 < 256)
							{
								stringBuilder.Append("\\x");
								num5 = 2;
							}
							else
							{
								stringBuilder.Append("\\u");
								num5 = 4;
							}
							for (int i = (num5 - 1) * 4; i >= 0; i -= 4)
							{
								int num6 = 15 & num2 >> i;
								int num7 = (num6 < 10) ? (48 + num6) : (87 + num6);
								stringBuilder.Append((char)num7);
							}
						}
					}
				}
				num++;
			}
			return (stringBuilder == null) ? s : stringBuilder.ToString();
		}
		internal static bool isValidIdentifierName(string s)
		{
			int length = s.Length;
			bool result;
			if (length == 0)
			{
				result = false;
			}
			else
			{
				if (!char.IsLetter(s[0]) && s[0].CompareTo('$') != 0 && s[0].CompareTo('_') != 0)
				{
					result = false;
				}
				else
				{
					for (int num = 1; num != length; num++)
					{
						if (!TokenStream.IsJavaIdentifierPart(s[num]))
						{
							result = false;
							return result;
						}
					}
					result = !TokenStream.isKeyword(s);
				}
			}
			return result;
		}
		internal static string DefaultObjectToString(IScriptable obj)
		{
			return "[object " + obj.ClassName + ']';
		}
		internal static string uneval(Context cx, IScriptable scope, object value)
		{
			string result;
			if (value == null)
			{
				result = "null";
			}
			else
			{
				if (value == Undefined.Value)
				{
					result = "undefined";
				}
				else
				{
					if (value is string)
					{
						string text = ScriptRuntime.escapeString((string)value);
						StringBuilder stringBuilder = new StringBuilder(text.Length + 2);
						stringBuilder.Append('"');
						stringBuilder.Append(text);
						stringBuilder.Append('"');
						result = stringBuilder.ToString();
					}
					else
					{
						if (CliHelper.IsNumber(value))
						{
							double num = Convert.ToDouble(value);
							if (num == 0.0 && 1.0 / num < 0.0)
							{
								result = "-0";
							}
							else
							{
								result = ScriptConvert.ToString(num);
							}
						}
						else
						{
							if (value is bool)
							{
								result = ScriptConvert.ToString(value);
							}
							else
							{
								if (value is IScriptable)
								{
									IScriptable scriptable = (IScriptable)value;
									object property = ScriptableObject.GetProperty(scriptable, "toSource");
									if (property is IFunction)
									{
										IFunction function = (IFunction)property;
										result = ScriptConvert.ToString(function.Call(cx, scope, scriptable, ScriptRuntime.EmptyArgs));
									}
									else
									{
										result = ScriptConvert.ToString(value);
									}
								}
								else
								{
									ScriptRuntime.WarnAboutNonJSObject(value);
									result = value.ToString();
								}
							}
						}
					}
				}
			}
			return result;
		}
		internal static string defaultObjectToSource(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			string result;
			using (new StackOverflowVerifier(1024))
			{
				bool flag;
				bool flag2;
				if (cx.iterating == null)
				{
					flag = true;
					flag2 = false;
					cx.iterating = new ObjToIntMap(31);
				}
				else
				{
					flag = false;
					flag2 = cx.iterating.has(thisObj);
				}
				StringBuilder stringBuilder = new StringBuilder(128);
				if (flag)
				{
					stringBuilder.Append("(");
				}
				stringBuilder.Append('{');
				try
				{
					if (!flag2)
					{
						cx.iterating.intern(thisObj);
						object[] ids = thisObj.GetIds();
						for (int i = 0; i < ids.Length; i++)
						{
							if (i > 0)
							{
								stringBuilder.Append(", ");
							}
							object obj = ids[i];
							object value;
							if (obj is int)
							{
								int num = (int)obj;
								value = thisObj.Get(num, thisObj);
								stringBuilder.Append(num);
							}
							else
							{
								string text = (string)obj;
								value = thisObj.Get(text, thisObj);
								if (ScriptRuntime.isValidIdentifierName(text))
								{
									stringBuilder.Append(text);
								}
								else
								{
									stringBuilder.Append('\'');
									stringBuilder.Append(ScriptRuntime.escapeString(text, '\''));
									stringBuilder.Append('\'');
								}
							}
							stringBuilder.Append(':');
							stringBuilder.Append(ScriptRuntime.uneval(cx, scope, value));
						}
					}
				}
				finally
				{
					if (flag)
					{
						cx.iterating = null;
					}
				}
				stringBuilder.Append('}');
				if (flag)
				{
					stringBuilder.Append(')');
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
		public static IScriptable NewObject(Context cx, IScriptable scope, string constructorName, object[] args)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			IFunction existingCtor = ScriptRuntime.getExistingCtor(cx, scope, constructorName);
			if (args == null)
			{
				args = ScriptRuntime.EmptyArgs;
			}
			return existingCtor.Construct(cx, scope, args);
		}
		public static object setDefaultNamespace(object ns, Context cx)
		{
			IScriptable scriptable = cx.currentActivationCall;
			if (scriptable == null)
			{
				scriptable = ScriptRuntime.getTopCallScope(cx);
			}
			XMLLib xMLLib = ScriptRuntime.CurrentXMLLib(cx);
			object value = xMLLib.ToDefaultXmlNamespace(cx, ns);
			if (!scriptable.Has("__default_namespace__", scriptable))
			{
				ScriptableObject.DefineProperty(scriptable, "__default_namespace__", value, 6);
			}
			else
			{
				scriptable.Put("__default_namespace__", scriptable, value);
			}
			return Undefined.Value;
		}
		public static object searchDefaultNamespace(Context cx)
		{
			IScriptable scriptable = cx.currentActivationCall;
			if (scriptable == null)
			{
				scriptable = ScriptRuntime.getTopCallScope(cx);
			}
			object obj;
			while (true)
			{
				IScriptable parentScope = scriptable.ParentScope;
				if (parentScope == null)
				{
					break;
				}
				obj = scriptable.Get("__default_namespace__", scriptable);
				if (obj != UniqueTag.NotFound)
				{
					goto Block_4;
				}
				scriptable = parentScope;
			}
			obj = ScriptableObject.GetProperty(scriptable, "__default_namespace__");
			object result;
			if (obj == UniqueTag.NotFound)
			{
				result = null;
				return result;
			}
			Block_4:
			result = obj;
			return result;
		}
		public static object getTopLevelProp(IScriptable scope, string id)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			return ScriptableObject.GetProperty(scope, id);
		}
		internal static IFunction getExistingCtor(Context cx, IScriptable scope, string constructorName)
		{
			object property = ScriptableObject.GetProperty(scope, constructorName);
			if (property is IFunction)
			{
				return (IFunction)property;
			}
			if (property == UniqueTag.NotFound)
			{
				throw Context.ReportRuntimeErrorById("msg.ctor.not.found", new object[]
				{
					constructorName
				});
			}
			throw Context.ReportRuntimeErrorById("msg.not.ctor", new object[]
			{
				constructorName
			});
		}
		private static long indexFromString(string str)
		{
			int length = str.Length;
			long result;
			if (length > 0)
			{
				int num = 0;
				bool flag = false;
				int num2 = (int)str[0];
				if (num2 == 45)
				{
					if (length > 1)
					{
						num2 = (int)str[1];
						num = 1;
						flag = true;
					}
				}
				num2 -= 48;
				if (0 <= num2 && num2 <= 9 && length <= (flag ? 11 : 10))
				{
					int num3 = -num2;
					int num4 = 0;
					num++;
					if (num3 != 0)
					{
						while (num != length && 0 <= (num2 = (int)(str[num] - '0')) && num2 <= 9)
						{
							num4 = num3;
							num3 = 10 * num3 - num2;
							num++;
						}
					}
					if (num == length && (num4 > -214748364 || (num4 == -214748364 && num2 <= (flag ? 8 : 7))))
					{
						result = (long)(-1 & (flag ? num3 : (-num3)));
						return result;
					}
				}
			}
			result = -1L;
			return result;
		}
		public static long testUint32String(string str)
		{
			int length = str.Length;
			long result;
			if (1 <= length && length <= 10)
			{
				int num = (int)str[0];
				num -= 48;
				if (num == 0)
				{
					result = ((length == 1) ? 0L : -1L);
					return result;
				}
				if (1 <= num && num <= 9)
				{
					long num2 = (long)num;
					for (int num3 = 1; num3 != length; num3++)
					{
						num = (int)(str[num3] - '0');
						if (0 > num || num > 9)
						{
							result = -1L;
							return result;
						}
						num2 = 10L * num2 + (long)num;
					}
					if ((ulong)num2 >> 32 == 0uL)
					{
						result = num2;
						return result;
					}
				}
			}
			result = -1L;
			return result;
		}
		internal static object getIndexObject(string s)
		{
			long num = ScriptRuntime.indexFromString(s);
			object result;
			if (num >= 0L)
			{
				result = (int)num;
			}
			else
			{
				result = s;
			}
			return result;
		}
		internal static object getIndexObject(double d)
		{
			int num = (int)d;
			object result;
			if ((double)num == d)
			{
				result = num;
			}
			else
			{
				result = ScriptConvert.ToString(d);
			}
			return result;
		}
		internal static string ToStringIdOrIndex(Context cx, object id)
		{
			string result;
			if (CliHelper.IsNumber(id))
			{
				double num = Convert.ToDouble(id);
				int num2 = (int)num;
				if ((double)num2 == num)
				{
					ScriptRuntime.storeIndexResult(cx, num2);
					result = null;
				}
				else
				{
					result = ScriptConvert.ToString(id);
				}
			}
			else
			{
				string text;
				if (id is string)
				{
					text = (string)id;
				}
				else
				{
					text = ScriptConvert.ToString(id);
				}
				long num3 = ScriptRuntime.indexFromString(text);
				if (num3 >= 0L)
				{
					ScriptRuntime.storeIndexResult(cx, (int)num3);
					result = null;
				}
				else
				{
					result = text;
				}
			}
			return result;
		}
		public static object getObjectElem(object obj, object elem, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefReadError(obj, elem);
			}
			return ScriptRuntime.getObjectElem(scriptable, elem, cx);
		}
		public static object getObjectElem(IScriptable obj, object elem, Context cx)
		{
			object result;
			if (obj is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)obj;
				result = xMLObject.EcmaGet(cx, elem);
			}
			else
			{
				string text = ScriptRuntime.ToStringIdOrIndex(cx, elem);
				object obj2;
				if (text == null)
				{
					int index = ScriptRuntime.lastIndexResult(cx);
					obj2 = ScriptableObject.GetProperty(obj, index);
				}
				else
				{
					obj2 = ScriptableObject.GetProperty(obj, text);
				}
				if (obj2 == UniqueTag.NotFound)
				{
					obj2 = Undefined.Value;
				}
				result = obj2;
			}
			return result;
		}
		public static object getObjectProp(object obj, string property, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefReadError(obj, property);
			}
			return ScriptRuntime.getObjectProp(scriptable, property, cx);
		}
		public static object getObjectProp(IScriptable obj, string property, Context cx)
		{
			object result;
			if (obj is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)obj;
				result = xMLObject.EcmaGet(cx, property);
			}
			else
			{
				object obj2 = ScriptableObject.GetProperty(obj, property);
				if (obj2 == UniqueTag.NotFound)
				{
					obj2 = Undefined.Value;
				}
				result = obj2;
			}
			return result;
		}
		public static object getObjectIndex(object obj, double dblIndex, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefReadError(obj, ScriptConvert.ToString(dblIndex));
			}
			int num = (int)dblIndex;
			object result;
			if ((double)num == dblIndex)
			{
				result = ScriptRuntime.getObjectIndex(scriptable, num, cx);
			}
			else
			{
				string property = ScriptConvert.ToString(dblIndex);
				result = ScriptRuntime.getObjectProp(scriptable, property, cx);
			}
			return result;
		}
		public static object getObjectIndex(IScriptable obj, int index, Context cx)
		{
			object result;
			if (obj is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)obj;
				result = xMLObject.EcmaGet(cx, index);
			}
			else
			{
				object obj2 = ScriptableObject.GetProperty(obj, index);
				if (obj2 == UniqueTag.NotFound)
				{
					obj2 = Undefined.Value;
				}
				result = obj2;
			}
			return result;
		}
		public static object setObjectElem(object obj, object elem, object value, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefWriteError(obj, elem, value);
			}
			return ScriptRuntime.setObjectElem(scriptable, elem, value, cx);
		}
		public static object setObjectElem(IScriptable obj, object elem, object value, Context cx)
		{
			object result;
			if (obj is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)obj;
				xMLObject.EcmaPut(cx, elem, value);
				result = value;
			}
			else
			{
				string text = ScriptRuntime.ToStringIdOrIndex(cx, elem);
				if (text == null)
				{
					int index = ScriptRuntime.lastIndexResult(cx);
					ScriptableObject.PutProperty(obj, index, value);
				}
				else
				{
					ScriptableObject.PutProperty(obj, text, value);
				}
				result = value;
			}
			return result;
		}
		public static object setObjectProp(object obj, string property, object value, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefWriteError(obj, property, value);
			}
			return ScriptRuntime.setObjectProp(scriptable, property, value, cx);
		}
		public static object setObjectProp(IScriptable obj, string property, object value, Context cx)
		{
			object result;
			if (obj is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)obj;
				xMLObject.EcmaPut(cx, property, value);
				result = value;
			}
			else
			{
				result = ScriptableObject.PutProperty(obj, property, value);
			}
			return result;
		}
		public static object setObjectIndex(object obj, double dblIndex, object value, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefWriteError(obj, Convert.ToString(dblIndex), value);
			}
			int num = (int)dblIndex;
			object result;
			if ((double)num == dblIndex)
			{
				result = ScriptRuntime.setObjectIndex(scriptable, num, value, cx);
			}
			else
			{
				string property = ScriptConvert.ToString(dblIndex);
				result = ScriptRuntime.setObjectProp(scriptable, property, value, cx);
			}
			return result;
		}
		public static object setObjectIndex(IScriptable obj, int index, object value, Context cx)
		{
			object result;
			if (obj is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)obj;
				xMLObject.EcmaPut(cx, index, value);
				result = value;
			}
			else
			{
				result = ScriptableObject.PutProperty(obj, index, value);
			}
			return result;
		}
		public static bool deleteObjectElem(IScriptable target, object elem, Context cx)
		{
			bool result;
			if (target is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)target;
				result = xMLObject.EcmaDelete(cx, elem);
			}
			else
			{
				string text = ScriptRuntime.ToStringIdOrIndex(cx, elem);
				if (text == null)
				{
					int index = ScriptRuntime.lastIndexResult(cx);
					result = ScriptableObject.DeleteProperty(target, index);
				}
				else
				{
					result = ScriptableObject.DeleteProperty(target, text);
				}
			}
			return result;
		}
		public static bool hasObjectElem(IScriptable target, object elem, Context cx)
		{
			bool result;
			if (target is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)target;
				result = xMLObject.EcmaHas(cx, elem);
			}
			else
			{
				string text = ScriptRuntime.ToStringIdOrIndex(cx, elem);
				if (text == null)
				{
					int index = ScriptRuntime.lastIndexResult(cx);
					result = ScriptableObject.HasProperty(target, index);
				}
				else
				{
					result = ScriptableObject.HasProperty(target, text);
				}
			}
			return result;
		}
		public static object refGet(IRef rf, Context cx)
		{
			return rf.Get(cx);
		}
		public static object refSet(IRef rf, object value, Context cx)
		{
			return rf.Set(cx, value);
		}
		public static object refDel(IRef rf, Context cx)
		{
			return rf.Delete(cx);
		}
		internal static bool isSpecialProperty(string s)
		{
			return s.Equals("__proto__") || s.Equals("__parent__");
		}
		public static IRef specialRef(object obj, string specialProperty, Context cx)
		{
			return SpecialRef.createSpecial(cx, obj, specialProperty);
		}
		public static object delete(object obj, object id, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				string text = (id == null) ? "null" : id.ToString();
				throw ScriptRuntime.TypeErrorById("msg.undef.prop.delete", new string[]
				{
					ScriptConvert.ToString(obj),
					text
				});
			}
			bool flag = ScriptRuntime.deleteObjectElem(scriptable, id, cx);
			return flag;
		}
		public static object name(Context cx, IScriptable scope, string name)
		{
			IScriptable parentScope = scope.ParentScope;
			object result;
			if (parentScope == null)
			{
				object obj = ScriptRuntime.topScopeName(cx, scope, name);
				if (obj == UniqueTag.NotFound)
				{
					throw ScriptRuntime.NotFoundError(scope, name);
				}
				result = obj;
			}
			else
			{
				result = ScriptRuntime.nameOrFunction(cx, scope, parentScope, name, false);
			}
			return result;
		}
		private static object nameOrFunction(Context cx, IScriptable scope, IScriptable parentScope, string name, bool asFunctionCall)
		{
			IScriptable value = scope;
			XMLObject xMLObject = null;
			IScriptable prototype;
			XMLObject xMLObject2;
			object obj;
			while (true)
			{
				if (scope is BuiltinWith)
				{
					prototype = scope.GetPrototype();
					if (prototype is XMLObject)
					{
						xMLObject2 = (XMLObject)prototype;
						if (xMLObject2.EcmaHas(cx, name))
						{
							break;
						}
						if (xMLObject == null)
						{
							xMLObject = xMLObject2;
						}
					}
					else
					{
						obj = ScriptableObject.GetProperty(prototype, name);
						if (obj != UniqueTag.NotFound)
						{
							goto Block_5;
						}
					}
				}
				else
				{
					if (scope is BuiltinCall)
					{
						obj = scope.Get(name, scope);
						if (obj != UniqueTag.NotFound)
						{
							goto Block_7;
						}
					}
					else
					{
						obj = ScriptableObject.GetProperty(scope, name);
						if (obj != UniqueTag.NotFound)
						{
							goto Block_9;
						}
					}
				}
				scope = parentScope;
				parentScope = parentScope.ParentScope;
				if (parentScope == null)
				{
					goto Block_10;
				}
			}
			value = xMLObject2;
			obj = xMLObject2.EcmaGet(cx, name);
			goto IL_190;
			Block_5:
			value = prototype;
			goto IL_190;
			Block_7:
			if (asFunctionCall)
			{
				value = ScriptableObject.GetTopLevelScope(parentScope);
			}
			goto IL_190;
			Block_9:
			value = scope;
			goto IL_190;
			Block_10:
			obj = ScriptRuntime.topScopeName(cx, scope, name);
			if (obj == UniqueTag.NotFound)
			{
				if (xMLObject == null || asFunctionCall)
				{
					throw ScriptRuntime.NotFoundError(scope, name);
				}
				obj = xMLObject.EcmaGet(cx, name);
			}
			value = scope;
			IL_190:
			if (asFunctionCall)
			{
				if (!(obj is ICallable))
				{
					throw ScriptRuntime.NotFunctionError(obj, name);
				}
				ScriptRuntime.storeScriptable(cx, value);
			}
			return obj;
		}
		private static object topScopeName(Context cx, IScriptable scope, string name)
		{
			if (cx.useDynamicScope)
			{
				scope = ScriptRuntime.checkDynamicScope(cx.topCallScope, scope);
			}
			return ScriptableObject.GetProperty(scope, name);
		}
		public static IScriptable bind(Context cx, IScriptable scope, string id)
		{
			IScriptable scriptable = null;
			IScriptable parentScope = scope.ParentScope;
			IScriptable result;
			if (parentScope != null)
			{
				while (scope is BuiltinWith)
				{
					IScriptable prototype = scope.GetPrototype();
					if (prototype is XMLObject)
					{
						XMLObject xMLObject = (XMLObject)prototype;
						if (xMLObject.EcmaHas(cx, id))
						{
							result = xMLObject;
							return result;
						}
						if (scriptable == null)
						{
							scriptable = xMLObject;
						}
					}
					else
					{
						if (ScriptableObject.HasProperty(prototype, id))
						{
							result = prototype;
							return result;
						}
					}
					scope = parentScope;
					parentScope = parentScope.ParentScope;
					if (parentScope == null)
					{
						goto IL_103;
					}
				}
				while (!ScriptableObject.HasProperty(scope, id))
				{
					scope = parentScope;
					parentScope = parentScope.ParentScope;
					if (parentScope == null)
					{
						goto IL_103;
					}
				}
				result = scope;
				return result;
			}
			IL_103:
			if (cx.useDynamicScope)
			{
				scope = ScriptRuntime.checkDynamicScope(cx.topCallScope, scope);
			}
			if (ScriptableObject.HasProperty(scope, id))
			{
				result = scope;
			}
			else
			{
				result = scriptable;
			}
			return result;
		}
		public static object setName(IScriptable bound, object value, Context cx, IScriptable scope, string id)
		{
			if (bound != null)
			{
				if (bound is XMLObject)
				{
					XMLObject xMLObject = (XMLObject)bound;
					xMLObject.EcmaPut(cx, id, value);
				}
				else
				{
					ScriptableObject.PutProperty(bound, id, value);
				}
			}
			else
			{
				if (cx.HasFeature(Context.Features.StrictVars))
				{
					throw Context.ReportRuntimeErrorById("msg.assn.create.strict", new object[]
					{
						id
					});
				}
				bound = ScriptableObject.GetTopLevelScope(scope);
				if (cx.useDynamicScope)
				{
					bound = ScriptRuntime.checkDynamicScope(cx.topCallScope, bound);
				}
				bound.Put(id, bound, value);
			}
			return value;
		}
		public static ICallable getNameFunctionAndThis(string name, Context cx, IScriptable scope)
		{
			IScriptable parentScope = scope.ParentScope;
			ICallable result;
			if (parentScope == null)
			{
				object obj = ScriptRuntime.topScopeName(cx, scope, name);
				if (!(obj is ICallable))
				{
					if (obj == UniqueTag.NotFound)
					{
						throw ScriptRuntime.NotFoundError(scope, name);
					}
					throw ScriptRuntime.NotFunctionError(obj, name);
				}
				else
				{
					ScriptRuntime.storeScriptable(cx, scope);
					result = (ICallable)obj;
				}
			}
			else
			{
				result = (ICallable)ScriptRuntime.nameOrFunction(cx, scope, parentScope, name, true);
			}
			return result;
		}
		public static ICallable GetElemFunctionAndThis(object obj, object elem, Context cx)
		{
			string text = ScriptRuntime.ToStringIdOrIndex(cx, elem);
			ICallable result;
			if (text != null)
			{
				result = ScriptRuntime.getPropFunctionAndThis(obj, text, cx);
			}
			else
			{
				int num = ScriptRuntime.lastIndexResult(cx);
				IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
				if (scriptable == null)
				{
					throw ScriptRuntime.UndefCallError(obj, Convert.ToString(num));
				}
				object property;
				while (true)
				{
					property = ScriptableObject.GetProperty(scriptable, num);
					if (property != UniqueTag.NotFound)
					{
						break;
					}
					if (!(scriptable is XMLObject))
					{
						break;
					}
					XMLObject xMLObject = (XMLObject)scriptable;
					IScriptable extraMethodSource = xMLObject.GetExtraMethodSource(cx);
					if (extraMethodSource == null)
					{
						break;
					}
					scriptable = extraMethodSource;
				}
				if (!(property is ICallable))
				{
					throw ScriptRuntime.NotFunctionError(property, elem);
				}
				ScriptRuntime.storeScriptable(cx, scriptable);
				result = (ICallable)property;
			}
			return result;
		}
		public static ICallable getPropFunctionAndThis(object obj, string property, Context cx)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefCallError(obj, property);
			}
			object property2;
			while (true)
			{
				property2 = ScriptableObject.GetProperty(scriptable, property);
				if (property2 != UniqueTag.NotFound)
				{
					break;
				}
				if (!(scriptable is XMLObject))
				{
					break;
				}
				XMLObject xMLObject = (XMLObject)scriptable;
				IScriptable extraMethodSource = xMLObject.GetExtraMethodSource(cx);
				if (extraMethodSource == null)
				{
					break;
				}
				scriptable = extraMethodSource;
			}
			if (property2 == UniqueTag.NotFound)
			{
			}
			if (!(property2 is ICallable))
			{
				throw ScriptRuntime.NotFunctionError(property2, property);
			}
			ScriptRuntime.storeScriptable(cx, scriptable);
			return (ICallable)property2;
		}
		public static ICallable getValueFunctionAndThis(object value, Context cx)
		{
			if (!(value is ICallable))
			{
				throw ScriptRuntime.NotFunctionError(value);
			}
			ICallable callable = (ICallable)value;
			IScriptable scriptable;
			if (callable is IScriptable)
			{
				scriptable = ((IScriptable)callable).ParentScope;
			}
			else
			{
				if (cx.topCallScope == null)
				{
					throw new Exception();
				}
				scriptable = cx.topCallScope;
			}
			if (scriptable.ParentScope != null)
			{
				if (!(scriptable is BuiltinWith))
				{
					if (scriptable is BuiltinCall)
					{
						scriptable = ScriptableObject.GetTopLevelScope(scriptable);
					}
				}
			}
			ScriptRuntime.storeScriptable(cx, scriptable);
			return callable;
		}
		public static IRef callRef(ICallable function, IScriptable thisObj, object[] args, Context cx)
		{
			if (!(function is IRefCallable))
			{
				string message = ScriptRuntime.GetMessage("msg.no.ref.from.function", new object[]
				{
					ScriptConvert.ToString(function)
				});
				throw ScriptRuntime.ConstructError("ReferenceError", message);
			}
			IRefCallable refCallable = (IRefCallable)function;
			IRef @ref = refCallable.RefCall(cx, thisObj, args);
			if (@ref == null)
			{
				throw new ApplicationException(refCallable.GetType().FullName + ".refCall() returned null");
			}
			return @ref;
		}
		public static IScriptable NewObject(object fun, Context cx, IScriptable scope, object[] args)
		{
			if (!(fun is IFunction))
			{
				throw ScriptRuntime.NotFunctionError(fun);
			}
			IFunction function = (IFunction)fun;
			return function.Construct(cx, scope, args);
		}
		public static object callSpecial(Context cx, ICallable fun, IScriptable thisObj, object[] args, IScriptable scope, IScriptable callerThis, int callType, string filename, int lineNumber)
		{
			object result;
			if (callType == 1)
			{
				if (BuiltinGlobal.isEvalFunction(fun))
				{
					result = ScriptRuntime.evalSpecial(cx, scope, callerThis, args, filename, lineNumber);
					return result;
				}
			}
			else
			{
				if (callType != 2)
				{
					throw Context.CodeBug();
				}
				if (BuiltinWith.IsWithFunction(fun))
				{
					throw Context.ReportRuntimeErrorById("msg.only.from.new", new object[]
					{
						"With"
					});
				}
			}
			result = fun.Call(cx, scope, thisObj, args);
			return result;
		}
		public static object newSpecial(Context cx, object fun, object[] args, IScriptable scope, int callType)
		{
			object result;
			if (callType == 1)
			{
				if (BuiltinGlobal.isEvalFunction(fun))
				{
					throw ScriptRuntime.TypeErrorById("msg.not.ctor", new string[]
					{
						"eval"
					});
				}
			}
			else
			{
				if (callType != 2)
				{
					throw Context.CodeBug();
				}
				if (BuiltinWith.IsWithFunction(fun))
				{
					result = BuiltinWith.NewWithSpecial(cx, scope, args);
					return result;
				}
			}
			result = ScriptRuntime.NewObject(fun, cx, scope, args);
			return result;
		}
		public static object applyOrCall(bool isApply, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			int num = args.Length;
			ICallable callable;
			if (thisObj is ICallable)
			{
				callable = (ICallable)thisObj;
			}
			else
			{
				object defaultValue = thisObj.GetDefaultValue(typeof(IFunction));
				if (!(defaultValue is ICallable))
				{
					throw ScriptRuntime.NotFunctionError(defaultValue, thisObj);
				}
				callable = (ICallable)defaultValue;
			}
			IScriptable scriptable = null;
			if (num != 0)
			{
				scriptable = ScriptConvert.ToObjectOrNull(cx, args[0]);
			}
			if (scriptable == null)
			{
				scriptable = ScriptRuntime.getTopCallScope(cx);
			}
			object[] array;
			if (isApply)
			{
				if (num <= 1)
				{
					array = ScriptRuntime.EmptyArgs;
				}
				else
				{
					object obj = args[1];
					if (obj == null || obj == Undefined.Value)
					{
						array = ScriptRuntime.EmptyArgs;
					}
					else
					{
						if (!(obj is BuiltinArray) && !(obj is Arguments))
						{
							throw ScriptRuntime.TypeErrorById("msg.arg.isnt.array", new string[0]);
						}
						array = cx.GetElements((IScriptable)obj);
					}
				}
			}
			else
			{
				if (num <= 1)
				{
					array = ScriptRuntime.EmptyArgs;
				}
				else
				{
					array = new object[num - 1];
					Array.Copy(args, 1, array, 0, num - 1);
				}
			}
			return callable.Call(cx, scope, scriptable, array);
		}
		public static object evalSpecial(Context cx, IScriptable scope, object thisArg, object[] args, string filename, int lineNumber)
		{
			object result;
			if (args.Length < 1)
			{
				result = Undefined.Value;
			}
			else
			{
				object obj = args[0];
				if (!(obj is string))
				{
					if (cx.HasFeature(Context.Features.StrictEval))
					{
						throw Context.ReportRuntimeErrorById("msg.eval.nonstring.strict", new object[0]);
					}
					string message = ScriptRuntime.GetMessage("msg.eval.nonstring", new object[0]);
					Context.ReportWarning(message);
					result = obj;
				}
				else
				{
					if (filename == null)
					{
						int[] array = new int[1];
						filename = Context.GetSourcePositionFromStack(array);
						if (filename != null)
						{
							lineNumber = array[0];
						}
						else
						{
							filename = "";
						}
					}
					string sourceName = ScriptRuntime.makeUrlForGeneratedScript(true, filename, lineNumber);
					ErrorReporter compilationErrorReporter = DefaultErrorReporter.ForEval(cx.ErrorReporter);
					IScript script = cx.CompileString((string)obj, new Interpreter(), compilationErrorReporter, sourceName, 1, null);
					((InterpretedFunction)script).idata.evalScriptFlag = true;
					ICallable callable = (ICallable)script;
					result = callable.Call(cx, scope, (IScriptable)thisArg, ScriptRuntime.EmptyArgs);
				}
			}
			return result;
		}
		public static string Typeof(object value)
		{
			string result;
			if (value == null)
			{
				result = "object";
			}
			else
			{
				if (value == Undefined.Value)
				{
					result = "undefined";
				}
				else
				{
					if (value is IScriptable)
					{
						if (value is XMLObject)
						{
							result = "xml";
						}
						else
						{
							result = ((value is ICallable && !(value is BuiltinRegExp)) ? "function" : "object");
						}
					}
					else
					{
						if (value is string)
						{
							result = "string";
						}
						else
						{
							if (value is char || CliHelper.IsNumber(value))
							{
								result = "number";
							}
							else
							{
								if (!(value is bool))
								{
									throw ScriptRuntime.errorWithClassName("msg.invalid.type", value);
								}
								result = "boolean";
							}
						}
					}
				}
			}
			return result;
		}
		internal static ApplicationException errorWithClassName(string msg, object val)
		{
			return Context.ReportRuntimeErrorById(msg, new object[]
			{
				val.GetType().FullName
			});
		}
		public static string TypeofName(IScriptable scope, string id)
		{
			Context currentContext = Context.CurrentContext;
			IScriptable scriptable = ScriptRuntime.bind(currentContext, scope, id);
			string result;
			if (scriptable == null)
			{
				result = "undefined";
			}
			else
			{
				result = ScriptRuntime.Typeof(ScriptRuntime.getObjectProp(scriptable, id, currentContext));
			}
			return result;
		}
		public static object Add(object val1, object val2, Context cx)
		{
			object result;
			if (CliHelper.IsNumber(val1) && CliHelper.IsNumber(val2))
			{
				result = (double)val1 + (double)val2;
			}
			else
			{
				if (val1 is XMLObject)
				{
					object obj = ((XMLObject)val1).AddValues(cx, true, val2);
					if (obj != UniqueTag.NotFound)
					{
						result = obj;
						return result;
					}
				}
				if (val2 is XMLObject)
				{
					object obj = ((XMLObject)val2).AddValues(cx, false, val1);
					if (obj != UniqueTag.NotFound)
					{
						result = obj;
						return result;
					}
				}
				if (val1 is CliEventInfo)
				{
					result = ((CliEventInfo)val1).Add(val2, cx);
				}
				else
				{
					if (val1 is IScriptable)
					{
						val1 = ((IScriptable)val1).GetDefaultValue(null);
					}
					if (val2 is IScriptable)
					{
						val2 = ((IScriptable)val2).GetDefaultValue(null);
					}
					if (!(val1 is string) && !(val2 is string))
					{
						if (CliHelper.IsNumber(val1) && CliHelper.IsNumber(val2))
						{
							result = (double)val1 + (double)val2;
						}
						else
						{
							result = ScriptConvert.ToNumber(val1) + ScriptConvert.ToNumber(val2);
						}
					}
					else
					{
						result = ScriptConvert.ToString(val1) + ScriptConvert.ToString(val2);
					}
				}
			}
			return result;
		}
		public static object nameIncrDecr(IScriptable scopeChain, string id, int incrDecrMask)
		{
			IScriptable scriptable;
			object obj;
			while (true)
			{
				scriptable = scopeChain;
				do
				{
					obj = scriptable.Get(id, scopeChain);
					if (obj != UniqueTag.NotFound)
					{
						goto Block_0;
					}
					scriptable = scriptable.GetPrototype();
				}
				while (scriptable != null);
				scopeChain = scopeChain.ParentScope;
				if (scopeChain == null)
				{
					goto Block_2;
				}
			}
			Block_0:
			return ScriptRuntime.doScriptableIncrDecr(scriptable, id, scopeChain, obj, incrDecrMask);
			Block_2:
			throw ScriptRuntime.NotFoundError(scopeChain, id);
		}
		public static object propIncrDecr(object obj, string id, Context cx, int incrDecrMask)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.UndefReadError(obj, id);
			}
			IScriptable scriptable2 = scriptable;
			object obj2;
			while (true)
			{
				obj2 = scriptable2.Get(id, scriptable);
				if (obj2 != UniqueTag.NotFound)
				{
					break;
				}
				scriptable2 = scriptable2.GetPrototype();
				if (scriptable2 == null)
				{
					goto Block_3;
				}
			}
			object result = ScriptRuntime.doScriptableIncrDecr(scriptable2, id, scriptable, obj2, incrDecrMask);
			return result;
			Block_3:
			scriptable.Put(id, scriptable, double.NaN);
			result = double.NaN;
			return result;
		}
		private static object doScriptableIncrDecr(IScriptable target, string id, IScriptable protoChainStart, object value, int incrDecrMask)
		{
			bool flag = (incrDecrMask & 2) != 0;
			double num;
			if (CliHelper.IsNumber(value))
			{
				num = Convert.ToDouble(value);
			}
			else
			{
				num = ScriptConvert.ToNumber(value);
				if (flag)
				{
					value = num;
				}
			}
			if ((incrDecrMask & 1) == 0)
			{
				num += 1.0;
			}
			else
			{
				num -= 1.0;
			}
			object obj = num;
			target.Put(id, protoChainStart, obj);
			object result;
			if (flag)
			{
				result = value;
			}
			else
			{
				result = obj;
			}
			return result;
		}
		public static object elemIncrDecr(object obj, object index, Context cx, int incrDecrMask)
		{
			object obj2 = ScriptRuntime.getObjectElem(obj, index, cx);
			bool flag = (incrDecrMask & 2) != 0;
			double num;
			if (CliHelper.IsNumber(obj2))
			{
				num = Convert.ToDouble(obj2);
			}
			else
			{
				num = ScriptConvert.ToNumber(obj2);
				if (flag)
				{
					obj2 = num;
				}
			}
			if ((incrDecrMask & 1) == 0)
			{
				num += 1.0;
			}
			else
			{
				num -= 1.0;
			}
			object obj3 = num;
			ScriptRuntime.setObjectElem(obj, index, obj3, cx);
			object result;
			if (flag)
			{
				result = obj2;
			}
			else
			{
				result = obj3;
			}
			return result;
		}
		public static object refIncrDecr(IRef rf, Context cx, int incrDecrMask)
		{
			object obj = rf.Get(cx);
			bool flag = (incrDecrMask & 2) != 0;
			double num;
			if (CliHelper.IsNumber(obj))
			{
				num = Convert.ToDouble(obj);
			}
			else
			{
				num = ScriptConvert.ToNumber(obj);
				if (flag)
				{
					obj = num;
				}
			}
			if ((incrDecrMask & 1) == 0)
			{
				num += 1.0;
			}
			else
			{
				num -= 1.0;
			}
			rf.Set(cx, num);
			object result;
			if (flag)
			{
				result = obj;
			}
			else
			{
				result = num;
			}
			return result;
		}
		public static bool eq(object x, object y)
		{
			bool result;
			if (x == null || x == Undefined.Value)
			{
				if (y == null || y == Undefined.Value)
				{
					result = true;
				}
				else
				{
					if (y is ScriptableObject)
					{
						object obj = ((ScriptableObject)y).EquivalentValues(x);
						if (obj != UniqueTag.NotFound)
						{
							result = (bool)obj;
							return result;
						}
					}
					result = false;
				}
			}
			else
			{
				if (CliHelper.IsNumber(x))
				{
					result = ScriptRuntime.eqNumber(Convert.ToDouble(x), y);
				}
				else
				{
					if (x is string)
					{
						result = ScriptRuntime.eqString((string)x, y);
					}
					else
					{
						if (x is bool)
						{
							bool flag = (bool)x;
							if (y is bool)
							{
								result = (flag == (bool)y);
							}
							else
							{
								if (y is ScriptableObject)
								{
									object obj = ((ScriptableObject)y).EquivalentValues(x);
									if (obj != UniqueTag.NotFound)
									{
										result = (bool)obj;
										return result;
									}
								}
								result = ScriptRuntime.eqNumber(flag ? 1.0 : 0.0, y);
							}
						}
						else
						{
							if (x is IScriptable)
							{
								if (y is IScriptable)
								{
									if (x == y)
									{
										result = true;
									}
									else
									{
										if (x is ScriptableObject)
										{
											object obj = ((ScriptableObject)x).EquivalentValues(y);
											if (obj != UniqueTag.NotFound)
											{
												result = (bool)obj;
												return result;
											}
										}
										if (y is ScriptableObject)
										{
											object obj = ((ScriptableObject)y).EquivalentValues(x);
											if (obj != UniqueTag.NotFound)
											{
												result = (bool)obj;
												return result;
											}
										}
										result = (x is Wrapper && y is Wrapper && ((Wrapper)x).Unwrap() == ((Wrapper)y).Unwrap());
									}
								}
								else
								{
									if (y is bool)
									{
										if (x is ScriptableObject)
										{
											object obj = ((ScriptableObject)x).EquivalentValues(y);
											if (obj != UniqueTag.NotFound)
											{
												result = (bool)obj;
												return result;
											}
										}
										double x2 = ((bool)y) ? 1.0 : 0.0;
										result = ScriptRuntime.eqNumber(x2, x);
									}
									else
									{
										if (CliHelper.IsNumber(y))
										{
											result = ScriptRuntime.eqNumber(Convert.ToDouble(y), x);
										}
										else
										{
											result = (y is string && ScriptRuntime.eqString((string)y, x));
										}
									}
								}
							}
							else
							{
								ScriptRuntime.WarnAboutNonJSObject(x);
								result = (x == y);
							}
						}
					}
				}
			}
			return result;
		}
		internal static bool eqNumber(double x, object y)
		{
			bool result;
			while (y != null && y != Undefined.Value)
			{
				if (CliHelper.IsNumber(y))
				{
					result = (x == Convert.ToDouble(y));
				}
				else
				{
					if (y is string)
					{
						result = (x == ScriptConvert.ToNumber(y));
					}
					else
					{
						if (y is bool)
						{
							result = (x == (((bool)y) ? 1.0 : 0.0));
						}
						else
						{
							if (y is IScriptable)
							{
								if (y is ScriptableObject)
								{
									object value = x;
									object obj = ((ScriptableObject)y).EquivalentValues(value);
									if (obj != UniqueTag.NotFound)
									{
										result = (bool)obj;
										return result;
									}
								}
								y = ScriptConvert.ToPrimitive(y);
								continue;
							}
							ScriptRuntime.WarnAboutNonJSObject(y);
							result = false;
						}
					}
				}
				return result;
			}
			result = false;
			return result;
		}
		private static bool eqString(string x, object y)
		{
			bool result;
			while (y != null && y != Undefined.Value)
			{
				if (y is string)
				{
					result = x.Equals(y);
				}
				else
				{
					if (CliHelper.IsNumber(y))
					{
						result = (ScriptConvert.ToNumber(x) == Convert.ToDouble(y));
					}
					else
					{
						if (y is bool)
						{
							result = (ScriptConvert.ToNumber(x) == (((bool)y) ? 1.0 : 0.0));
						}
						else
						{
							if (y is IScriptable)
							{
								if (y is ScriptableObject)
								{
									object obj = ((ScriptableObject)y).EquivalentValues(x);
									if (obj != UniqueTag.NotFound)
									{
										result = (bool)obj;
										return result;
									}
								}
								y = ScriptConvert.ToPrimitive(y);
								continue;
							}
							ScriptRuntime.WarnAboutNonJSObject(y);
							result = false;
						}
					}
				}
				return result;
			}
			result = false;
			return result;
		}
		public static bool shallowEq(object x, object y)
		{
			bool result;
			if (x == y)
			{
				if (!CliHelper.IsNumber(x))
				{
					result = true;
				}
				else
				{
					double d = Convert.ToDouble(x);
					result = !double.IsNaN(d);
				}
			}
			else
			{
				if (x == null || x == Undefined.Value)
				{
					result = false;
				}
				else
				{
					if (CliHelper.IsNumber(x))
					{
						if (CliHelper.IsNumber(y))
						{
							result = (Convert.ToDouble(x) == Convert.ToDouble(y));
							return result;
						}
					}
					else
					{
						if (x is string)
						{
							if (y is string)
							{
								result = x.Equals(y);
								return result;
							}
						}
						else
						{
							if (x is bool)
							{
								if (y is bool)
								{
									result = x.Equals(y);
									return result;
								}
							}
							else
							{
								if (!(x is IScriptable))
								{
									ScriptRuntime.WarnAboutNonJSObject(x);
									result = (x == y);
									return result;
								}
								if (x is Wrapper && y is Wrapper)
								{
									result = (((Wrapper)x).Unwrap() == ((Wrapper)y).Unwrap());
									return result;
								}
							}
						}
					}
					result = false;
				}
			}
			return result;
		}
		public static bool InstanceOf(object a, object b, Context cx)
		{
			IScriptable scriptable = b as IScriptable;
			if (scriptable == null)
			{
				throw ScriptRuntime.TypeErrorById("msg.instanceof.not.object", new string[0]);
			}
			IScriptable scriptable2 = a as IScriptable;
			return scriptable2 != null && scriptable.HasInstance(scriptable2);
		}
		protected internal static bool jsDelegatesTo(IScriptable lhs, IScriptable rhs)
		{
			bool result;
			for (IScriptable prototype = lhs.GetPrototype(); prototype != null; prototype = prototype.GetPrototype())
			{
				if (prototype.Equals(rhs))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool In(object a, object b, Context cx)
		{
			if (!(b is IScriptable))
			{
				throw ScriptRuntime.TypeErrorById("msg.instanceof.not.object", new string[0]);
			}
			return ScriptRuntime.hasObjectElem((IScriptable)b, a, cx);
		}
		public static bool cmp_LT(object val1, object val2)
		{
			double num;
			double num2;
			bool result;
			if (CliHelper.IsNumber(val1) && CliHelper.IsNumber(val2))
			{
				num = Convert.ToDouble(val1);
				num2 = Convert.ToDouble(val2);
			}
			else
			{
				if (val1 is IScriptable)
				{
					val1 = ((IScriptable)val1).GetDefaultValue(typeof(long));
				}
				if (val2 is IScriptable)
				{
					val2 = ((IScriptable)val2).GetDefaultValue(typeof(long));
				}
				if (val1 is string && val2 is string)
				{
					result = (string.CompareOrdinal((string)val1, (string)val2) < 0);
					return result;
				}
				num = ScriptConvert.ToNumber(val1);
				num2 = ScriptConvert.ToNumber(val2);
			}
			result = (num < num2);
			return result;
		}
		public static bool cmp_LE(object val1, object val2)
		{
			double num;
			double num2;
			bool result;
			if (CliHelper.IsNumber(val1) && CliHelper.IsNumber(val2))
			{
				num = Convert.ToDouble(val1);
				num2 = Convert.ToDouble(val2);
			}
			else
			{
				if (val1 is IScriptable)
				{
					val1 = ((IScriptable)val1).GetDefaultValue(typeof(long));
				}
				if (val2 is IScriptable)
				{
					val2 = ((IScriptable)val2).GetDefaultValue(typeof(long));
				}
				if (val1 is string && val2 is string)
				{
					result = (string.CompareOrdinal((string)val1, (string)val2) <= 0);
					return result;
				}
				num = ScriptConvert.ToNumber(val1);
				num2 = ScriptConvert.ToNumber(val2);
			}
			result = (num <= num2);
			return result;
		}
		public static bool hasTopCall(Context cx)
		{
			return cx.topCallScope != null;
		}
		public static IScriptable getTopCallScope(Context cx)
		{
			IScriptable topCallScope = cx.topCallScope;
			if (topCallScope == null)
			{
				throw new ApplicationException();
			}
			return topCallScope;
		}
		public static object DoTopCall(ICallable callable, Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (scope == null)
			{
				throw new ArgumentException();
			}
			if (cx.topCallScope != null)
			{
				throw new ApplicationException();
			}
			cx.topCallScope = ScriptableObject.GetTopLevelScope(scope);
			cx.useDynamicScope = cx.HasFeature(Context.Features.DynamicScope);
			ContextFactory factory = cx.Factory;
			object result;
			try
			{
				result = factory.DoTopCall(callable, cx, scope, thisObj, args);
			}
			finally
			{
				cx.topCallScope = null;
				cx.cachedXMLLib = null;
				if (cx.currentActivationCall != null)
				{
					throw new ApplicationException("ActivationCall without exitActivationFunction() invokation.");
				}
			}
			return result;
		}
		internal static IScriptable checkDynamicScope(IScriptable possibleDynamicScope, IScriptable staticTopScope)
		{
			IScriptable result;
			if (possibleDynamicScope == staticTopScope)
			{
				result = possibleDynamicScope;
			}
			else
			{
				IScriptable scriptable = possibleDynamicScope;
				while (true)
				{
					scriptable = scriptable.GetPrototype();
					if (scriptable == staticTopScope)
					{
						break;
					}
					if (scriptable == null)
					{
						goto Block_3;
					}
				}
				result = possibleDynamicScope;
				return result;
				Block_3:
				result = staticTopScope;
			}
			return result;
		}
		public static void initScript(BuiltinFunction funObj, IScriptable thisObj, Context cx, IScriptable scope, bool evalScript)
		{
			if (cx.topCallScope == null)
			{
				throw new ApplicationException();
			}
			int paramAndVarCount = funObj.ParamAndVarCount;
			if (paramAndVarCount != 0)
			{
				IScriptable scriptable = scope;
				while (scriptable is BuiltinWith)
				{
					scriptable = scriptable.ParentScope;
				}
				int index = paramAndVarCount;
				while (index-- != 0)
				{
					string paramOrVarName = funObj.getParamOrVarName(index);
					if (!ScriptableObject.HasProperty(scope, paramOrVarName))
					{
						if (!evalScript)
						{
							ScriptableObject.DefineProperty(scriptable, paramOrVarName, Undefined.Value, 4);
						}
						else
						{
							scriptable.Put(paramOrVarName, scriptable, Undefined.Value);
						}
					}
				}
			}
		}
		public static IScriptable createFunctionActivation(BuiltinFunction funObj, IScriptable scope, object[] args)
		{
			return new BuiltinCall(funObj, scope, args);
		}
		public static void enterActivationFunction(Context cx, IScriptable activation)
		{
			if (cx.topCallScope == null)
			{
				throw new ApplicationException();
			}
			BuiltinCall builtinCall = (BuiltinCall)activation;
			builtinCall.parentActivationCall = cx.currentActivationCall;
			cx.currentActivationCall = builtinCall;
		}
		public static void exitActivationFunction(Context cx)
		{
			BuiltinCall currentActivationCall = cx.currentActivationCall;
			cx.currentActivationCall = currentActivationCall.parentActivationCall;
			currentActivationCall.parentActivationCall = null;
		}
		internal static BuiltinCall findFunctionActivation(Context cx, IFunction f)
		{
			BuiltinCall result;
			for (BuiltinCall builtinCall = cx.currentActivationCall; builtinCall != null; builtinCall = builtinCall.parentActivationCall)
			{
				if (builtinCall.function == f)
				{
					result = builtinCall;
					return result;
				}
			}
			result = null;
			return result;
		}
		public static IScriptable NewCatchScope(Exception t, IScriptable lastCatchScope, string exceptionName, Context cx, IScriptable scope)
		{
			bool flag;
			object obj;
			if (t is EcmaScriptThrow)
			{
				flag = false;
				obj = ((EcmaScriptThrow)t).Value;
			}
			else
			{
				flag = true;
				if (lastCatchScope != null)
				{
					BuiltinObject builtinObject = (BuiltinObject)lastCatchScope;
					obj = builtinObject.GetAssociatedValue(t);
					if (obj == null)
					{
						Context.CodeBug();
					}
				}
				else
				{
					Exception ex = null;
					EcmaScriptException ex2;
					string text;
					string text2;
					if (t is EcmaScriptError)
					{
						EcmaScriptError ecmaScriptError = (EcmaScriptError)t;
						ex2 = ecmaScriptError;
						text = ecmaScriptError.Name;
						text2 = ecmaScriptError.ErrorMessage;
					}
					else
					{
						if (!(t is EcmaScriptRuntimeException))
						{
							throw Context.CodeBug();
						}
						ex2 = (EcmaScriptRuntimeException)t;
						if (t.InnerException != null)
						{
							ex = t.InnerException;
							text = "JavaException";
							text2 = ex.GetType().FullName + ": " + ex.Message;
						}
						else
						{
							text = "InternalError";
							text2 = ex2.Message;
						}
					}
					string text3 = ex2.SourceName;
					if (text3 == null)
					{
						text3 = "";
					}
					int lineNumber = ex2.LineNumber;
					object[] args;
					if (lineNumber > 0)
					{
						args = new object[]
						{
							text2,
							text3,
							lineNumber
						};
					}
					else
					{
						args = new object[]
						{
							text2,
							text3
						};
					}
					IScriptable scriptable = cx.NewObject(scope, text, args);
					ScriptableObject.PutProperty(scriptable, "name", text);
					if (ex != null)
					{
						object value = cx.Wrap(scope, ex, null);
						ScriptableObject.DefineProperty(scriptable, "javaException", value, 5);
					}
					if (ex2 != null)
					{
						object value = cx.Wrap(scope, ex2, null);
						ScriptableObject.DefineProperty(scriptable, "rhinoException", value, 5);
					}
					obj = scriptable;
				}
			}
			BuiltinObject builtinObject2 = new BuiltinObject();
			builtinObject2.DefineProperty(exceptionName, obj, 4);
			if (flag)
			{
				builtinObject2.AssociateValue(t, obj);
			}
			return builtinObject2;
		}
		public static IScriptable enterWith(object obj, Context cx, IScriptable scope)
		{
			IScriptable scriptable = ScriptConvert.ToObjectOrNull(cx, obj);
			if (scriptable == null)
			{
				throw ScriptRuntime.TypeErrorById("msg.undef.with", new string[]
				{
					ScriptConvert.ToString(obj)
				});
			}
			IScriptable result;
			if (scriptable is XMLObject)
			{
				XMLObject xMLObject = (XMLObject)scriptable;
				result = xMLObject.EnterWith(scope);
			}
			else
			{
				result = new BuiltinWith(scope, scriptable);
			}
			return result;
		}
		public static IScriptable leaveWith(IScriptable scope)
		{
			BuiltinWith builtinWith = (BuiltinWith)scope;
			return builtinWith.ParentScope;
		}
		public static IScriptable enterDotQuery(object value, IScriptable scope)
		{
			if (!(value is XMLObject))
			{
				throw ScriptRuntime.NotXmlError(value);
			}
			XMLObject xMLObject = (XMLObject)value;
			return xMLObject.EnterDotQuery(scope);
		}
		public static object updateDotQuery(bool value, IScriptable scope)
		{
			BuiltinWith builtinWith = (BuiltinWith)scope;
			return builtinWith.UpdateDotQuery(value);
		}
		public static IScriptable leaveDotQuery(IScriptable scope)
		{
			BuiltinWith builtinWith = (BuiltinWith)scope;
			return builtinWith.ParentScope;
		}
		public static void setFunctionProtoAndParent(BaseFunction fn, IScriptable scope)
		{
			fn.ParentScope = scope;
			fn.SetPrototype(ScriptableObject.GetFunctionPrototype(scope));
		}
		public static void setObjectProtoAndParent(ScriptableObject obj, IScriptable scope)
		{
			scope = ScriptableObject.GetTopLevelScope(scope);
			obj.ParentScope = scope;
			IScriptable classPrototype = ScriptableObject.getClassPrototype(scope, obj.ClassName);
			obj.SetPrototype(classPrototype);
		}
		public static void initFunction(Context cx, IScriptable scope, BuiltinFunction function, int type, bool fromEvalCode)
		{
			if (type == 1)
			{
				string functionName = function.FunctionName;
				if (functionName != null && functionName.Length != 0)
				{
					if (!fromEvalCode)
					{
						ScriptableObject.DefineProperty(scope, functionName, function, 4);
					}
					else
					{
						scope.Put(functionName, scope, function);
					}
				}
			}
			else
			{
				if (type != 3)
				{
					throw Context.CodeBug();
				}
				string functionName = function.FunctionName;
				if (functionName != null && functionName.Length != 0)
				{
					while (scope is BuiltinWith)
					{
						scope = scope.ParentScope;
					}
					scope.Put(functionName, scope, function);
				}
			}
		}
		public static IScriptable newArrayLiteral(object[] objects, int[] skipIndexces, Context cx, IScriptable scope)
		{
			int num = objects.Length;
			int num2 = 0;
			if (skipIndexces != null)
			{
				num2 = skipIndexces.Length;
			}
			int num3 = num + num2;
			int num4 = num3;
			IScriptable scriptable;
			if (cx.Version == Context.Versions.JS1_2)
			{
				scriptable = cx.NewObject(scope, "Array", ScriptRuntime.EmptyArgs);
				ScriptableObject.PutProperty(scriptable, "length", num4);
			}
			else
			{
				scriptable = cx.NewObject(scope, "Array", new object[]
				{
					num4
				});
			}
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			while (num6 != num3)
			{
				if (num5 != num2 && skipIndexces[num5] == num6)
				{
					num5++;
				}
				else
				{
					ScriptableObject.PutProperty(scriptable, num6, objects[num7]);
					num7++;
				}
				num6++;
			}
			return scriptable;
		}
		public static IScriptable newObjectLiteral(object[] propertyIds, object[] propertyValues, Context cx, IScriptable scope)
		{
			IScriptable scriptable = cx.NewObject(scope);
			int num = 0;
			int num2 = propertyIds.Length;
			while (num != num2)
			{
				object obj = propertyIds[num];
				object obj2 = propertyValues[num];
				if (obj is Node.GetterPropertyLiteral)
				{
					BuiltinObject builtinObject = (BuiltinObject)scriptable;
					InterpretedFunction interpretedFunction = (InterpretedFunction)obj2;
					builtinObject.DefineGetter((string)((Node.GetterPropertyLiteral)obj).Property, interpretedFunction);
				}
				else
				{
					if (obj is Node.SetterPropertyLiteral)
					{
						BuiltinObject builtinObject = (BuiltinObject)scriptable;
						InterpretedFunction interpretedFunction = (InterpretedFunction)obj2;
						builtinObject.DefineSetter((string)((Node.SetterPropertyLiteral)obj).Property, interpretedFunction);
					}
					else
					{
						if (obj is string)
						{
							ScriptableObject.PutProperty(scriptable, (string)obj, obj2);
						}
						else
						{
							ScriptableObject.PutProperty(scriptable, (int)obj, obj2);
						}
					}
				}
				num++;
			}
			return scriptable;
		}
		public static bool isArrayObject(object obj)
		{
			return obj is BuiltinArray || obj is Arguments;
		}
		public static object[] getArrayElements(IScriptable obj)
		{
			Context currentContext = Context.CurrentContext;
			long lengthProperty = BuiltinArray.getLengthProperty(currentContext, obj);
			if (lengthProperty > 2147483647L)
			{
				throw new ArgumentException();
			}
			int num = (int)lengthProperty;
			object[] result;
			if (num == 0)
			{
				result = ScriptRuntime.EmptyArgs;
			}
			else
			{
				object[] array = new object[num];
				for (int i = 0; i < num; i++)
				{
					object property = ScriptableObject.GetProperty(obj, i);
					array[i] = ((property == UniqueTag.NotFound) ? Undefined.Value : property);
				}
				result = array;
			}
			return result;
		}
		internal static void checkDeprecated(Context cx, string name)
		{
			Context.Versions version = cx.Version;
			if (version >= Context.Versions.JS1_4 || version == Context.Versions.Default)
			{
				string message = ScriptRuntime.GetMessage("msg.deprec.ctor", new object[]
				{
					name
				});
				if (version != Context.Versions.Default)
				{
					throw Context.ReportRuntimeError(message);
				}
				Context.ReportWarning(message);
			}
		}
		public static string GetMessage(string messageId, params object[] arguments)
		{
			Context currentContext = Context.CurrentContext;
			CultureInfo culture = null;
			if (currentContext != null)
			{
				culture = currentContext.CurrentCulture;
			}
			if (ScriptRuntime.m_ResourceManager == null)
			{
				ScriptRuntime.m_ResourceManager = new ResourceManager("EcmaScript.NET.Resources.Messages", typeof(ScriptRuntime).Assembly);
			}
			string @string = ScriptRuntime.m_ResourceManager.GetString(messageId, culture);
			if (@string == null)
			{
				throw new ApplicationException("Missing no message resource found for message property " + messageId);
			}
			if (arguments == null)
			{
				arguments = new object[0];
			}
			string result;
			if (arguments.Length == 0)
			{
				result = @string;
			}
			else
			{
				result = string.Format(@string, arguments);
			}
			return result;
		}
		public static EcmaScriptError ConstructError(string error, string message)
		{
			int[] array = new int[1];
			string sourcePositionFromStack = Context.GetSourcePositionFromStack(array);
			return ScriptRuntime.ConstructError(error, message, sourcePositionFromStack, array[0], null, 0);
		}
		public static EcmaScriptError ConstructError(string error, string message, string sourceName, int lineNumber, string lineSource, int columnNumber)
		{
			return new EcmaScriptError(error, message, sourceName, lineNumber, lineSource, columnNumber);
		}
		public static EcmaScriptError TypeError(string message)
		{
			return ScriptRuntime.ConstructError("TypeError", message);
		}
		public static EcmaScriptError TypeErrorById(string messageId, params string[] args)
		{
			return ScriptRuntime.TypeError(ScriptRuntime.GetMessage(messageId, args));
		}
		public static ApplicationException UndefReadError(object obj, object id)
		{
			string text = (id == null) ? "null" : id.ToString();
			return ScriptRuntime.TypeErrorById("msg.undef.prop.read", new string[]
			{
				ScriptConvert.ToString(obj),
				text
			});
		}
		public static ApplicationException UndefCallError(object obj, object id)
		{
			string text = (id == null) ? "null" : id.ToString();
			return ScriptRuntime.TypeErrorById("msg.undef.method.call", new string[]
			{
				ScriptConvert.ToString(obj),
				text
			});
		}
		public static ApplicationException UndefWriteError(object obj, object id, object value)
		{
			string text = (id == null) ? "null" : id.ToString();
			string text2 = (value is IScriptable) ? value.ToString() : ScriptConvert.ToString(value);
			return ScriptRuntime.TypeErrorById("msg.undef.prop.write", new string[]
			{
				ScriptConvert.ToString(obj),
				text,
				text2
			});
		}
		public static ApplicationException NotFoundError(IScriptable obj, string property)
		{
			string message = ScriptRuntime.GetMessage("msg.is.not.defined", new object[]
			{
				property
			});
			throw ScriptRuntime.ConstructError("ReferenceError", message);
		}
		public static ApplicationException NotFunctionError(object value)
		{
			return ScriptRuntime.NotFunctionError(value, value);
		}
		public static ApplicationException NotFunctionError(object value, object messageHelper)
		{
			string text = (messageHelper == null) ? "null" : messageHelper.ToString();
			ApplicationException result;
			if (value == UniqueTag.NotFound)
			{
				result = ScriptRuntime.TypeErrorById("msg.function.not.found", new string[]
				{
					text
				});
			}
			else
			{
				result = ScriptRuntime.TypeErrorById("msg.isnt.function", new string[]
				{
					text,
					(value == null) ? "null" : value.GetType().FullName
				});
			}
			return result;
		}
		private static ApplicationException NotXmlError(object value)
		{
			throw ScriptRuntime.TypeErrorById("msg.isnt.xml.object", new string[]
			{
				ScriptConvert.ToString(value)
			});
		}
		internal static void WarnAboutNonJSObject(object nonJSObject)
		{
			string text = string.Concat(new object[]
			{
				"+++ USAGE WARNING: Missed Context.Wrap() conversion:\nRuntime detected object ",
				nonJSObject,
				" of class ",
				nonJSObject.GetType().FullName,
				" where it expected String, Number, Boolean or Scriptable instance. Please check your code for missig Context.Wrap() call."
			});
			Context.ReportWarning(text);
			Console.Error.WriteLine(text);
		}
		private static XMLLib CurrentXMLLib(Context cx)
		{
			if (cx.topCallScope == null)
			{
				throw new ApplicationException();
			}
			XMLLib xMLLib = cx.cachedXMLLib;
			if (xMLLib == null)
			{
				xMLLib = XMLLib.ExtractFromScope(cx.topCallScope);
				if (xMLLib == null)
				{
					throw new ApplicationException();
				}
				cx.cachedXMLLib = xMLLib;
			}
			return xMLLib;
		}
		public static string escapeAttributeValue(object value, Context cx)
		{
			XMLLib xMLLib = ScriptRuntime.CurrentXMLLib(cx);
			return xMLLib.EscapeAttributeValue(value);
		}
		public static string escapeTextValue(object value, Context cx)
		{
			XMLLib xMLLib = ScriptRuntime.CurrentXMLLib(cx);
			return xMLLib.EscapeTextValue(value);
		}
		public static IRef memberRef(object obj, object elem, Context cx, int memberTypeFlags)
		{
			if (!(obj is XMLObject))
			{
				throw ScriptRuntime.NotXmlError(obj);
			}
			XMLObject xMLObject = (XMLObject)obj;
			return xMLObject.MemberRef(cx, elem, memberTypeFlags);
		}
		public static IRef memberRef(object obj, object ns, object elem, Context cx, int memberTypeFlags)
		{
			if (!(obj is XMLObject))
			{
				throw ScriptRuntime.NotXmlError(obj);
			}
			XMLObject xMLObject = (XMLObject)obj;
			return xMLObject.MemberRef(cx, ns, elem, memberTypeFlags);
		}
		public static IRef nameRef(object name, Context cx, IScriptable scope, int memberTypeFlags)
		{
			XMLLib xMLLib = ScriptRuntime.CurrentXMLLib(cx);
			return xMLLib.NameRef(cx, name, scope, memberTypeFlags);
		}
		public static IRef nameRef(object ns, object name, Context cx, IScriptable scope, int memberTypeFlags)
		{
			XMLLib xMLLib = ScriptRuntime.CurrentXMLLib(cx);
			return xMLLib.NameRef(cx, ns, name, scope, memberTypeFlags);
		}
		private static void storeIndexResult(Context cx, int index)
		{
			cx.scratchIndex = index;
		}
		internal static int lastIndexResult(Context cx)
		{
			return cx.scratchIndex;
		}
		public static void storeUint32Result(Context cx, long value)
		{
			if ((ulong)value >> 32 != 0uL)
			{
				throw new ArgumentException();
			}
			cx.scratchUint32 = value;
		}
		public static long lastUint32Result(Context cx)
		{
			long scratchUint = cx.scratchUint32;
			if ((ulong)scratchUint >> 32 != 0uL)
			{
				throw new ApplicationException();
			}
			return scratchUint;
		}
		private static void storeScriptable(Context cx, IScriptable value)
		{
			if (cx.scratchScriptable != null)
			{
				throw new ApplicationException();
			}
			cx.scratchScriptable = value;
		}
		public static IScriptable lastStoredScriptable(Context cx)
		{
			IScriptable scratchScriptable = cx.scratchScriptable;
			cx.scratchScriptable = null;
			return scratchScriptable;
		}
		internal static string makeUrlForGeneratedScript(bool isEval, string masterScriptUrl, int masterScriptLine)
		{
			string result;
			if (isEval)
			{
				result = string.Concat(new object[]
				{
					masterScriptUrl,
					'#',
					masterScriptLine,
					"(eval)"
				});
			}
			else
			{
				result = string.Concat(new object[]
				{
					masterScriptUrl,
					'#',
					masterScriptLine,
					"(Function)"
				});
			}
			return result;
		}
		internal static bool isGeneratedScript(string sourceUrl)
		{
			return sourceUrl.IndexOf("(eval)") >= 0 || sourceUrl.IndexOf("(Function)") >= 0;
		}
	}
}
