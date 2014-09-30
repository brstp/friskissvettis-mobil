using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
namespace EcmaScript.NET.Types.Cli
{
	[ComVisible(true)]
	public class CliObject : ScriptableObject, Wrapper, IIdEnumerable
	{
		private class EnumeratorBasedIdEnumeration : IdEnumeration
		{
			private IEnumerator enumerator = null;
			public EnumeratorBasedIdEnumeration(IEnumerator enumerator)
			{
				this.enumerator = enumerator;
			}
			public override object Current(Context cx)
			{
				return Context.CliToJS(cx, this.enumerator.Current, cx.topCallScope);
			}
			public override bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}
		}
		private const int JSTYPE_UNDEFINED = 0;
		private const int JSTYPE_NULL = 1;
		private const int JSTYPE_BOOLEAN = 2;
		private const int JSTYPE_NUMBER = 3;
		private const int JSTYPE_STRING = 4;
		private const int JSTYPE_CLI_CLASS = 5;
		private const int JSTYPE_CLI_OBJECT = 6;
		private const int JSTYPE_CLI_ARRAY = 7;
		private const int JSTYPE_OBJECT = 8;
		internal const sbyte CONVERSION_TRIVIAL = 1;
		internal const sbyte CONVERSION_NONTRIVIAL = 0;
		internal const sbyte CONVERSION_NONE = 99;
		private object m_Object = null;
		private CliType m_Type = null;
		public object Object
		{
			get
			{
				return this.m_Object;
			}
		}
		public override string ClassName
		{
			get
			{
				return "NativeCliObject";
			}
		}
		public object Unwrap()
		{
			return this.m_Object;
		}
		protected CliObject()
		{
			this.Init(this, base.GetType());
		}
		public CliObject(object obj)
		{
			this.Init(obj, obj.GetType());
		}
		public CliObject(object obj, Type type)
		{
			this.Init(obj, type);
		}
		protected void Init(object obj, Type type)
		{
			this.m_Object = obj;
			this.m_Type = CliType.GetNativeCliType(type);
		}
		public override object GetDefaultValue(Type hint)
		{
			object result;
			if (hint == null || hint == typeof(string))
			{
				result = this.m_Object.ToString();
			}
			else
			{
				string name;
				if (hint == typeof(bool))
				{
					name = "booleanValue";
				}
				else
				{
					if (!CliHelper.IsNumberType(hint))
					{
						throw Context.ReportRuntimeErrorById("msg.default.value", new object[0]);
					}
					name = "doubleValue";
				}
				object obj = this.Get(name, this);
				if (obj is IFunction)
				{
					IFunction function = (IFunction)obj;
					result = function.Call(Context.CurrentContext, function.ParentScope, this, ScriptRuntime.EmptyArgs);
				}
				else
				{
					if (CliHelper.IsNumberType(hint) && this.m_Object is bool)
					{
						result = (((bool)this.m_Object) ? 1.0 : 0.0);
					}
					else
					{
						result = this.m_Object.ToString();
					}
				}
			}
			return result;
		}
		public override object Put(int index, IScriptable start, object value)
		{
			object result;
			if (value is IdFunctionObject)
			{
				result = base.Put(index, start, value);
			}
			else
			{
				MemberInfo[] indexSetter = this.m_Type.IndexSetter;
				for (int i = 0; i < indexSetter.Length; i++)
				{
					MethodInfo methodInfo = (MethodInfo)indexSetter[i];
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 2 && CliHelper.IsNumberType(parameters[0].ParameterType))
					{
						methodInfo.Invoke(this.m_Object, new object[]
						{
							index,
							value
						});
						result = value;
						return result;
					}
				}
				result = base.Put(index, start, value);
			}
			return result;
		}
		public override object[] GetIds()
		{
			object[] result;
			if (this.m_Object is ICollection)
			{
				ICollection collection = (ICollection)this.m_Object;
				object[] array = new object[collection.Count];
				int num = collection.Count;
				while (--num >= 0)
				{
					array[num] = num;
				}
				result = array;
			}
			else
			{
				result = base.GetIds();
			}
			return result;
		}
		public override bool Has(int index, IScriptable start)
		{
			bool result;
			if (this.m_Object is ICollection)
			{
				ICollection collection = (ICollection)this.m_Object;
				result = (index >= 0 && index < collection.Count);
			}
			else
			{
				result = base.Has(index, start);
			}
			return result;
		}
		public override object Put(string name, IScriptable start, object value)
		{
			object result;
			if (value is IdFunctionObject)
			{
				result = base.Put(name, start, value);
			}
			else
			{
				PropertyInfo cachedProperty = this.m_Type.GetCachedProperty(name);
				if (cachedProperty != null)
				{
					if (!cachedProperty.CanWrite)
					{
						throw Context.ReportRuntimeErrorById("msg.undef.prop.write", new object[]
						{
							name,
							this.ClassName,
							value
						});
					}
					cachedProperty.SetValue(this.m_Object, Convert.ChangeType(value, cachedProperty.PropertyType), null);
					result = value;
				}
				else
				{
					FieldInfo cachedField = this.m_Type.GetCachedField(name);
					if (cachedField != null)
					{
						if (!cachedField.IsPublic)
						{
							throw Context.ReportRuntimeErrorById("msg.undef.prop.write", new object[]
							{
								name,
								this.ClassName
							});
						}
						cachedField.SetValue(this.m_Object, value);
						result = value;
					}
					else
					{
						MemberInfo[] indexSetter = this.m_Type.IndexSetter;
						for (int i = 0; i < indexSetter.Length; i++)
						{
							MethodInfo methodInfo = (MethodInfo)indexSetter[i];
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string))
							{
								methodInfo.Invoke(this.m_Object, new object[]
								{
									name,
									value
								});
								result = value;
								return result;
							}
						}
						result = base.Put(name, start, value);
					}
				}
			}
			return result;
		}
		public override object Get(int index, IScriptable start)
		{
			MemberInfo[] indexGetter = this.m_Type.IndexGetter;
			object result;
			for (int i = 0; i < indexGetter.Length; i++)
			{
				MethodInfo methodInfo = (MethodInfo)indexGetter[i];
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 1 && CliHelper.IsNumberType(parameters[0].ParameterType))
				{
					result = methodInfo.Invoke(this.m_Object, new object[]
					{
						index
					});
					return result;
				}
			}
			object obj = base.Get(index, start);
			if (obj != UniqueTag.NotFound)
			{
				result = obj;
				return result;
			}
			result = UniqueTag.NotFound;
			return result;
		}
		public override object Get(string name, IScriptable start)
		{
			object obj = base.Get(name, start);
			object result;
			if (obj != UniqueTag.NotFound || name == "Object")
			{
				result = obj;
			}
			else
			{
				if (this.m_Type.ClassAttribute != null)
				{
					CliMethodInfo functionsWithAttribute = this.m_Type.GetFunctionsWithAttribute(name);
					if (functionsWithAttribute != null)
					{
						result = functionsWithAttribute;
					}
					else
					{
						result = UniqueTag.NotFound;
					}
				}
				else
				{
					if (typeof(IReflect).IsAssignableFrom(this.m_Type.UnderlyingType))
					{
						MemberInfo[] member = ((IReflect)this.m_Object).GetMember(name, BindingFlags.Default);
						if (member.Length > 0)
						{
							if (member[0] is PropertyInfo)
							{
								result = ((PropertyInfo)member[0]).GetValue(this.m_Object, null);
							}
							else
							{
								if (member[0] is FieldInfo)
								{
									result = ((FieldInfo)member[0]).GetValue(this.m_Object);
								}
								else
								{
									result = new CliMethodInfo(name, member, null);
								}
							}
						}
						else
						{
							result = UniqueTag.NotFound;
						}
					}
					else
					{
						PropertyInfo cachedProperty = this.m_Type.GetCachedProperty(name);
						if (cachedProperty != null)
						{
							if (!cachedProperty.CanRead)
							{
								throw Context.ReportRuntimeErrorById("msg.undef.prop.read", new object[]
								{
									name,
									this.ClassName
								});
							}
							result = cachedProperty.GetValue(this.m_Object, null);
						}
						else
						{
							FieldInfo cachedField = this.m_Type.GetCachedField(name);
							if (cachedField != null)
							{
								if (!cachedField.IsPublic)
								{
									throw Context.ReportRuntimeErrorById("msg.undef.prop.read", new object[]
									{
										name,
										this.ClassName
									});
								}
								result = cachedField.GetValue(this.m_Object);
							}
							else
							{
								CliMethodInfo functions = this.m_Type.GetFunctions(name);
								if (functions != null)
								{
									result = functions;
								}
								else
								{
									MemberInfo[] indexGetter = this.m_Type.IndexGetter;
									for (int i = 0; i < indexGetter.Length; i++)
									{
										MethodInfo methodInfo = (MethodInfo)indexGetter[i];
										ParameterInfo[] parameters = methodInfo.GetParameters();
										if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
										{
											result = methodInfo.Invoke(this.m_Object, new object[]
											{
												name
											});
											return result;
										}
									}
									EventInfo cachedEvent = this.m_Type.GetCachedEvent(name);
									if (cachedEvent != null)
									{
										result = new CliEventInfo(cachedEvent)
										{
											ParentScope = this
										};
									}
									else
									{
										result = UniqueTag.NotFound;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public static bool CanConvert(object fromObj, Type to)
		{
			return CliObject.GetConversionWeight(fromObj, to) < 99;
		}
		internal static int GetConversionWeight(object fromObj, Type to)
		{
			int jSTypeCode = CliObject.GetJSTypeCode(fromObj);
			int result;
			switch (jSTypeCode)
			{
			case 0:
				if (to == typeof(string) || to == typeof(object))
				{
					result = 1;
					return result;
				}
				break;
			case 1:
				if (!to.IsPrimitive)
				{
					result = 1;
					return result;
				}
				break;
			case 2:
				if (to == typeof(bool))
				{
					result = 1;
					return result;
				}
				if (to == typeof(object))
				{
					result = 2;
					return result;
				}
				if (to == typeof(string))
				{
					result = 3;
					return result;
				}
				break;
			case 3:
				if (to.IsPrimitive)
				{
					if (to == typeof(double))
					{
						result = 1;
						return result;
					}
					if (to != typeof(bool))
					{
						result = 1 + CliObject.GetSizeRank(to);
						return result;
					}
				}
				else
				{
					if (to == typeof(string))
					{
						result = 9;
						return result;
					}
					if (to == typeof(object))
					{
						result = 10;
						return result;
					}
					if (CliHelper.IsNumberType(to))
					{
						result = 2;
						return result;
					}
				}
				break;
			case 4:
				if (to == typeof(string))
				{
					result = 1;
					return result;
				}
				if (to.IsInstanceOfType(fromObj))
				{
					result = 2;
					return result;
				}
				if (to.IsPrimitive)
				{
					if (to == typeof(char))
					{
						result = 3;
						return result;
					}
					if (to != typeof(bool))
					{
						result = 4;
						return result;
					}
				}
				break;
			case 5:
				if (to == typeof(Type))
				{
					result = 1;
					return result;
				}
				if (to == typeof(object))
				{
					result = 3;
					return result;
				}
				if (to == typeof(string))
				{
					result = 4;
					return result;
				}
				break;
			case 6:
			case 7:
			{
				object obj = fromObj;
				if (obj is Wrapper)
				{
					obj = ((Wrapper)obj).Unwrap();
				}
				if (to.IsInstanceOfType(obj))
				{
					result = 0;
					return result;
				}
				if (to == typeof(string))
				{
					result = 2;
					return result;
				}
				if (to.IsPrimitive && to != typeof(bool))
				{
					result = ((jSTypeCode == 7) ? 0 : (2 + CliObject.GetSizeRank(to)));
					return result;
				}
				break;
			}
			case 8:
				if (to == fromObj.GetType())
				{
					result = 1;
					return result;
				}
				if (to.IsArray)
				{
					if (fromObj is BuiltinArray)
					{
						result = 1;
						return result;
					}
				}
				else
				{
					if (to == typeof(object))
					{
						result = 2;
						return result;
					}
					if (to == typeof(string))
					{
						result = 3;
						return result;
					}
					if (to == typeof(DateTime))
					{
						if (fromObj is BuiltinDate)
						{
							result = 1;
							return result;
						}
					}
					else
					{
						if (to.IsInterface)
						{
							if (fromObj is IFunction)
							{
								if (to.GetMethods().Length == 1)
								{
									result = 1;
									return result;
								}
							}
							result = 11;
							return result;
						}
						if (to.IsPrimitive && to != typeof(bool))
						{
							result = 3 + CliObject.GetSizeRank(to);
							return result;
						}
					}
				}
				break;
			}
			result = 99;
			return result;
		}
		private static int GetSizeRank(Type aType)
		{
			int result;
			if (aType == typeof(double))
			{
				result = 1;
			}
			else
			{
				if (aType == typeof(float))
				{
					result = 2;
				}
				else
				{
					if (aType == typeof(long))
					{
						result = 3;
					}
					else
					{
						if (aType == typeof(int))
						{
							result = 4;
						}
						else
						{
							if (aType == typeof(short))
							{
								result = 5;
							}
							else
							{
								if (aType == typeof(char))
								{
									result = 6;
								}
								else
								{
									if (aType == typeof(sbyte))
									{
										result = 7;
									}
									else
									{
										if (aType == typeof(bool))
										{
											result = 99;
										}
										else
										{
											result = 8;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		private static int GetJSTypeCode(object value)
		{
			int result;
			if (value == null)
			{
				result = 1;
			}
			else
			{
				if (value == Undefined.Value)
				{
					result = 0;
				}
				else
				{
					if (value is string)
					{
						result = 4;
					}
					else
					{
						if (CliHelper.IsNumber(value))
						{
							result = 3;
						}
						else
						{
							if (value is bool)
							{
								result = 2;
							}
							else
							{
								if (value is IScriptable)
								{
									if (value is CliType)
									{
										result = 5;
									}
									else
									{
										if (value is CliArray)
										{
											result = 7;
										}
										else
										{
											if (value is Wrapper)
											{
												result = 6;
											}
											else
											{
												result = 8;
											}
										}
									}
								}
								else
								{
									if (value is Type)
									{
										result = 5;
									}
									else
									{
										if (value.GetType().IsArray)
										{
											result = 7;
										}
										else
										{
											result = 6;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		internal static object CoerceType(Type type, object value)
		{
			object result;
			if (value != null && value.GetType() == type)
			{
				result = value;
			}
			else
			{
				switch (CliObject.GetJSTypeCode(value))
				{
				case 0:
					if (type == typeof(string) || type == typeof(object))
					{
						result = "undefined";
						return result;
					}
					CliObject.reportConversionError("undefined", type);
					break;
				case 1:
					if (type.IsPrimitive)
					{
						CliObject.reportConversionError(value, type);
					}
					result = null;
					return result;
				case 2:
					if (type == typeof(bool) || type == typeof(bool) || type == typeof(object))
					{
						result = value;
						return result;
					}
					if (type == typeof(string))
					{
						result = value.ToString();
						return result;
					}
					CliObject.reportConversionError(value, type);
					break;
				case 3:
					if (type == typeof(string))
					{
						result = ScriptConvert.ToString(value);
						return result;
					}
					if (type == typeof(object))
					{
						result = CliObject.CoerceToNumber(typeof(double), value);
						return result;
					}
					if ((type.IsPrimitive && type != typeof(bool)) || CliHelper.IsNumberType(type))
					{
						result = CliObject.CoerceToNumber(type, value);
						return result;
					}
					CliObject.reportConversionError(value, type);
					break;
				case 4:
					if (type == typeof(string) || type.IsInstanceOfType(value))
					{
						result = value;
						return result;
					}
					if (type == typeof(char))
					{
						if (((string)value).Length == 1)
						{
							result = ((string)value)[0];
							return result;
						}
						result = CliObject.CoerceToNumber(type, value);
						return result;
					}
					else
					{
						if ((type.IsPrimitive && type != typeof(bool)) || CliHelper.IsNumberType(type))
						{
							result = CliObject.CoerceToNumber(type, value);
							return result;
						}
						CliObject.reportConversionError(value, type);
					}
					break;
				case 5:
					if (value is Wrapper)
					{
						value = ((Wrapper)value).Unwrap();
					}
					if (type == typeof(Type) || type == typeof(object))
					{
						result = value;
						return result;
					}
					if (type == typeof(string))
					{
						result = value.ToString();
						return result;
					}
					CliObject.reportConversionError(value, type);
					break;
				case 6:
				case 7:
					if (type.IsPrimitive)
					{
						if (type == typeof(bool))
						{
							CliObject.reportConversionError(value, type);
						}
						result = CliObject.CoerceToNumber(type, value);
						return result;
					}
					if (value is Wrapper)
					{
						value = ((Wrapper)value).Unwrap();
					}
					if (type == typeof(string))
					{
						result = value.ToString();
						return result;
					}
					if (type.IsInstanceOfType(value))
					{
						result = value;
						return result;
					}
					CliObject.reportConversionError(value, type);
					break;
				case 8:
					if (type == typeof(string))
					{
						result = ScriptConvert.ToString(value);
						return result;
					}
					if (type.IsPrimitive)
					{
						if (type == typeof(bool))
						{
							CliObject.reportConversionError(value, type);
						}
						result = CliObject.CoerceToNumber(type, value);
						return result;
					}
					if (type.IsInstanceOfType(value))
					{
						result = value;
						return result;
					}
					if (type == typeof(DateTime) && value is BuiltinDate)
					{
						double jSTimeValue = ((BuiltinDate)value).JSTimeValue;
						result = BuiltinDate.FromMilliseconds((double)((long)jSTimeValue));
						return result;
					}
					if (type.IsArray && value is BuiltinArray)
					{
						BuiltinArray builtinArray = (BuiltinArray)value;
						long length = builtinArray.getLength();
						Type elementType = type.GetElementType();
						object obj = Array.CreateInstance(elementType, (int)length);
						int num = 0;
						while ((long)num < length)
						{
							try
							{
								((Array)obj).SetValue(CliObject.CoerceType(elementType, builtinArray.Get(num, builtinArray)), num);
							}
							catch (EcmaScriptException)
							{
								CliObject.reportConversionError(value, type);
							}
							num++;
						}
						result = obj;
						return result;
					}
					if (value is Wrapper)
					{
						value = ((Wrapper)value).Unwrap();
						if (type.IsInstanceOfType(value))
						{
							result = value;
							return result;
						}
						CliObject.reportConversionError(value, type);
					}
					else
					{
						CliObject.reportConversionError(value, type);
					}
					break;
				}
				result = value;
			}
			return result;
		}
		private static object CoerceToNumber(Type type, object value)
		{
			Type type2 = value.GetType();
			object result;
			if (type == typeof(char))
			{
				if (type2 == typeof(char))
				{
					result = value;
				}
				else
				{
					result = (char)CliObject.toInteger(value, typeof(char), 0.0, 65535.0);
				}
			}
			else
			{
				if (type == typeof(object) || type == typeof(double))
				{
					result = ((type2 == typeof(double)) ? value : CliObject.toDouble(value));
				}
				else
				{
					if (type == typeof(float) || type == typeof(float))
					{
						if (type2 == typeof(float))
						{
							result = value;
						}
						else
						{
							double num = CliObject.toDouble(value);
							if (double.IsInfinity(num) || double.IsNaN(num) || num == 0.0)
							{
								result = (float)num;
							}
							else
							{
								double num2 = Math.Abs(num);
								if (num2 < 1.4012984643248171E-45)
								{
									result = (float)((num > 0.0) ? 0.0 : -0.0);
								}
								else
								{
									if (num2 > 3.4028234663852886E+38)
									{
										result = ((num > 0.0) ? float.PositiveInfinity : float.NegativeInfinity);
									}
									else
									{
										result = (float)num;
									}
								}
							}
						}
					}
					else
					{
						if (type == typeof(int))
						{
							if (type2 == typeof(int))
							{
								result = value;
							}
							else
							{
								result = (int)CliObject.toInteger(value, typeof(int), -2147483648.0, 2147483647.0);
							}
						}
						else
						{
							if (type == typeof(long))
							{
								if (type2 == typeof(long))
								{
									result = value;
								}
								else
								{
									result = CliObject.toInteger(value, typeof(long), -9.2233720368547758E+18, 9.2233720368547758E+18);
								}
							}
							else
							{
								if (type == typeof(short))
								{
									if (type2 == typeof(short))
									{
										result = value;
									}
									else
									{
										result = (short)CliObject.toInteger(value, typeof(short), -32768.0, 32767.0);
									}
								}
								else
								{
									if (type == typeof(byte) || type == typeof(sbyte))
									{
										if (type2 == typeof(byte))
										{
											result = value;
										}
										else
										{
											result = (sbyte)CliObject.toInteger(value, typeof(byte), -128.0, 127.0);
										}
									}
									else
									{
										result = CliObject.toDouble(value);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		private static double toDouble(object value)
		{
			double result;
			if (value is ValueType)
			{
				result = Convert.ToDouble(value);
			}
			else
			{
				if (value is string)
				{
					result = ScriptConvert.ToNumber((string)value);
				}
				else
				{
					if (value is IScriptable)
					{
						if (value is Wrapper)
						{
							result = CliObject.toDouble(((Wrapper)value).Unwrap());
						}
						else
						{
							result = ScriptConvert.ToNumber(value);
						}
					}
					else
					{
						MethodInfo methodInfo;
						try
						{
							methodInfo = value.GetType().GetMethod("doubleValue", new Type[0]);
						}
						catch (MethodAccessException)
						{
							methodInfo = null;
						}
						catch (SecurityException)
						{
							methodInfo = null;
						}
						if (methodInfo != null)
						{
							try
							{
								result = Convert.ToDouble(methodInfo.Invoke(value, null));
								return result;
							}
							catch (UnauthorizedAccessException)
							{
								CliObject.reportConversionError(value, typeof(double));
							}
							catch (TargetInvocationException)
							{
								CliObject.reportConversionError(value, typeof(double));
							}
						}
						result = ScriptConvert.ToNumber(value.ToString());
					}
				}
			}
			return result;
		}
		private static long toInteger(object value, Type type, double min, double max)
		{
			double num = CliObject.toDouble(value);
			if (double.IsInfinity(num) || double.IsNaN(num))
			{
				CliObject.reportConversionError(ScriptConvert.ToString(value), type);
			}
			if (num > 0.0)
			{
				num = Math.Floor(num);
			}
			else
			{
				num = Math.Ceiling(num);
			}
			if (num < min || num > max)
			{
				CliObject.reportConversionError(ScriptConvert.ToString(value), type);
			}
			return (long)num;
		}
		internal static void reportConversionError(object value, Type type)
		{
			throw Context.ReportRuntimeErrorById("msg.conversion.not.allowed", new object[]
			{
				Convert.ToString(value),
				CliHelper.ToSignature(type)
			});
		}
		public override string ToString()
		{
			return "[object " + this.ClassName + "]";
		}
		public IdEnumeration GetEnumeration(Context cx, bool enumValues)
		{
			IdEnumeration result;
			if (this.m_Object is IEnumerable)
			{
				result = new CliObject.EnumeratorBasedIdEnumeration(((IEnumerable)this.m_Object).GetEnumerator());
			}
			else
			{
				result = new IdEnumeration(this, cx, enumValues);
			}
			return result;
		}
	}
}
