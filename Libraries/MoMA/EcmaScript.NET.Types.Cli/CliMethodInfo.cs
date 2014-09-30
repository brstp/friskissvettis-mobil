using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET.Types.Cli
{
	[ComVisible(true)]
	public class CliMethodInfo : BaseFunction
	{
		private const int PREFERENCE_EQUAL = 0;
		private const int PREFERENCE_FIRST_ARG = 1;
		private const int PREFERENCE_SECOND_ARG = 2;
		private const int PREFERENCE_AMBIGUOUS = 3;
		private string m_Name = string.Empty;
		private MethodInfo[] m_MethodInfos = null;
		private object m_Target = null;
		private bool[] paramsParameters = null;
		public override string ClassName
		{
			get
			{
				return this.m_Name;
			}
		}
		public override string FunctionName
		{
			get
			{
				return this.m_Name;
			}
		}
		public CliMethodInfo(object target, string methodName)
		{
			this.Init(methodName, new MemberInfo[]
			{
				target.GetType().GetMethod(methodName)
			}, target);
		}
		public CliMethodInfo(string name, MemberInfo methodInfo, object target)
		{
			this.Init(name, new MemberInfo[]
			{
				methodInfo
			}, target);
		}
		public CliMethodInfo(string name, MemberInfo[] methodInfos, object target)
		{
			this.Init(name, methodInfos, target);
		}
		private void Init(string name, MemberInfo[] methodInfos, object target)
		{
			this.m_Name = name;
			this.m_MethodInfos = new MethodInfo[methodInfos.Length];
			Array.Copy(methodInfos, 0, this.m_MethodInfos, 0, this.m_MethodInfos.Length);
			this.m_Target = target;
			this.paramsParameters = new bool[methodInfos.Length];
			for (int i = 0; i < methodInfos.Length; i++)
			{
				this.paramsParameters[i] = CliHelper.HasParamsParameter(this.m_MethodInfos[i]);
			}
		}
		public override object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args)
		{
			if (this.m_MethodInfos.Length == 0)
			{
				throw new ApplicationException("No methods defined for call");
			}
			int num = CliMethodInfo.FindFunction(cx, this.m_MethodInfos, args, this.paramsParameters);
			if (num < 0)
			{
				Type declaringType = this.m_MethodInfos[0].DeclaringType;
				string text = string.Concat(new object[]
				{
					declaringType.FullName,
					'.',
					this.FunctionName,
					'(',
					CliMethodInfo.ScriptSignature(args),
					')'
				});
				throw Context.ReportRuntimeErrorById("msg.java.no_such_method", new object[]
				{
					text
				});
			}
			MethodBase methodBase = this.m_MethodInfos[num];
			ParameterInfo[] parameters = methodBase.GetParameters();
			object[] array = args;
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (this.paramsParameters[num] && i >= parameters.Length - 1)
				{
					Type parameterType = parameters[parameters.Length - 1].ParameterType;
					Type elementType = parameterType.GetElementType();
					object[] array2 = new object[args.Length - i];
					for (int j = i; j < args.Length; j++)
					{
						array2[j - i] = Context.JsToCli(obj, elementType);
					}
					args = new object[i + 1];
					args.CopyTo(args, 0);
					args[i] = array2;
				}
				else
				{
					Type parameterType2 = parameters[i].ParameterType;
					object obj2 = Context.JsToCli(obj, parameterType2);
					if (obj2 != obj)
					{
						if (array == args)
						{
							args = new object[args.Length];
							args.CopyTo(args, 0);
						}
						args[i] = obj2;
					}
				}
			}
			object obj3;
			if (!methodBase.IsStatic)
			{
				IScriptable scriptable = thisObj;
				Type declaringType = methodBase.DeclaringType;
				while (scriptable != null)
				{
					if (scriptable is Wrapper)
					{
						obj3 = ((Wrapper)scriptable).Unwrap();
						if (declaringType.IsInstanceOfType(obj3))
						{
							goto IL_2AF;
						}
					}
					scriptable = scriptable.GetPrototype();
				}
				throw Context.ReportRuntimeErrorById("msg.nonjava.method", new object[]
				{
					this.FunctionName,
					ScriptConvert.ToString(thisObj),
					declaringType.FullName
				});
			}
			obj3 = null;
			IL_2AF:
			object obj4 = null;
			try
			{
				obj4 = methodBase.Invoke(obj3, args);
			}
			catch (Exception e)
			{
				Context.ThrowAsScriptRuntimeEx(e);
			}
			Type type = methodBase.DeclaringType;
			if (methodBase is MethodInfo)
			{
				type = ((MethodInfo)methodBase).ReturnType;
			}
			object obj5 = cx.Wrap(scope, obj4, type);
			if (obj5 == null && type == Type.GetType("System.Void"))
			{
				obj5 = Undefined.Value;
			}
			return obj5;
		}
		internal static int FindFunction(Context cx, MethodBase[] methodsOrCtors, object[] args, bool[] paramsParameter)
		{
			int result;
			if (methodsOrCtors.Length == 0)
			{
				result = -1;
			}
			else
			{
				if (methodsOrCtors.Length == 1)
				{
					MethodBase methodBase = methodsOrCtors[0];
					ParameterInfo[] parameters = methodBase.GetParameters();
					int num = parameters.Length;
					if (num != args.Length)
					{
						if (!paramsParameter[0])
						{
							result = -1;
							return result;
						}
					}
					for (int i = 0; i != num; i++)
					{
						object fromObj = args[i];
						Type to;
						if (paramsParameter[0] && i >= parameters.Length - 1)
						{
							to = parameters[parameters.Length - 1].ParameterType.GetElementType();
						}
						else
						{
							to = parameters[i].ParameterType;
						}
						if (!CliObject.CanConvert(fromObj, to))
						{
							result = -1;
							return result;
						}
					}
					result = 0;
				}
				else
				{
					int num2 = -1;
					int[] array = null;
					int num3 = 0;
					for (int j = 0; j < methodsOrCtors.Length; j++)
					{
						MethodBase methodBase = methodsOrCtors[j];
						ParameterInfo[] parameters = methodBase.GetParameters();
						if (parameters.Length == args.Length)
						{
							for (int i = 0; i < parameters.Length; i++)
							{
								if (!CliObject.CanConvert(args[i], parameters[i].ParameterType))
								{
									goto IL_30D;
								}
							}
							if (num2 < 0)
							{
								num2 = j;
							}
							else
							{
								int num4 = 0;
								int num5 = 0;
								for (int i = -1; i != num3; i++)
								{
									int num6;
									if (i == -1)
									{
										num6 = num2;
									}
									else
									{
										num6 = array[i];
									}
									MethodBase methodBase2 = methodsOrCtors[num6];
									int num7 = CliMethodInfo.PreferSignature(args, parameters, methodBase2.GetParameters());
									if (num7 == 3)
									{
										break;
									}
									if (num7 == 1)
									{
										num4++;
									}
									else
									{
										if (num7 != 2)
										{
											if (num7 != 0)
											{
												Context.CodeBug();
											}
											if (methodBase2.IsStatic && methodBase2.DeclaringType.IsAssignableFrom(methodBase.DeclaringType))
											{
												if (i == -1)
												{
													num2 = j;
												}
												else
												{
													array[i] = j;
												}
											}
											goto IL_30D;
										}
										num5++;
									}
								}
								if (num4 == 1 + num3)
								{
									num2 = j;
									num3 = 0;
								}
								else
								{
									if (num5 != 1 + num3)
									{
										if (array == null)
										{
											array = new int[methodsOrCtors.Length - 1];
										}
										array[num3] = j;
										num3++;
									}
								}
							}
						}
						IL_30D:;
					}
					if (num2 < 0)
					{
						result = -1;
					}
					else
					{
						if (num3 == 0)
						{
							result = num2;
						}
						else
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int i = -1; i != num3; i++)
							{
								int num6;
								if (i == -1)
								{
									num6 = num2;
								}
								else
								{
									num6 = array[i];
								}
								stringBuilder.Append("\n    ");
								stringBuilder.Append(CliHelper.ToSignature(methodsOrCtors[num6]));
							}
							MethodBase methodBase3 = methodsOrCtors[num2];
							string name = methodBase3.Name;
							string fullName = methodBase3.DeclaringType.FullName;
							if (methodsOrCtors[0] is MethodInfo)
							{
								throw Context.ReportRuntimeErrorById("msg.constructor.ambiguous", new object[]
								{
									name,
									CliMethodInfo.ScriptSignature(args),
									stringBuilder.ToString()
								});
							}
							throw Context.ReportRuntimeErrorById("msg.method.ambiguous", new object[]
							{
								fullName,
								name,
								CliMethodInfo.ScriptSignature(args),
								stringBuilder.ToString()
							});
						}
					}
				}
			}
			return result;
		}
		internal static string ScriptSignature(object[] values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = 0; num != values.Length; num++)
			{
				object obj = values[num];
				string value;
				if (obj == null)
				{
					value = "null";
				}
				else
				{
					if (obj is bool)
					{
						value = "boolean";
					}
					else
					{
						if (obj is string)
						{
							value = "string";
						}
						else
						{
							if (CliHelper.IsNumber(obj))
							{
								value = "number";
							}
							else
							{
								if (obj is IScriptable)
								{
									if (obj is Undefined)
									{
										value = "undefined";
									}
									else
									{
										if (obj is Wrapper)
										{
											object obj2 = ((Wrapper)obj).Unwrap();
											value = obj2.GetType().FullName;
										}
										else
										{
											if (obj is IFunction)
											{
												value = "function";
											}
											else
											{
												value = "object";
											}
										}
									}
								}
								else
								{
									value = CliHelper.ToSignature(obj.GetType());
								}
							}
						}
					}
				}
				if (num != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
		private static int PreferSignature(object[] args, ParameterInfo[] sig1, ParameterInfo[] sig2)
		{
			int num = 0;
			for (int i = 0; i < args.Length; i++)
			{
				Type parameterType = sig1[i].ParameterType;
				Type parameterType2 = sig2[i].ParameterType;
				if (parameterType != parameterType2)
				{
					object fromObj = args[i];
					int conversionWeight = CliObject.GetConversionWeight(fromObj, parameterType);
					int conversionWeight2 = CliObject.GetConversionWeight(fromObj, parameterType2);
					int num2;
					if (conversionWeight < conversionWeight2)
					{
						num2 = 1;
					}
					else
					{
						if (conversionWeight > conversionWeight2)
						{
							num2 = 2;
						}
						else
						{
							if (conversionWeight == 0)
							{
								if (parameterType.IsAssignableFrom(parameterType2))
								{
									num2 = 2;
								}
								else
								{
									if (parameterType2.IsAssignableFrom(parameterType))
									{
										num2 = 1;
									}
									else
									{
										num2 = 3;
									}
								}
							}
							else
							{
								num2 = 3;
							}
						}
					}
					num |= num2;
					if (num == 3)
					{
						break;
					}
				}
			}
			return num;
		}
		public override object GetDefaultValue(Type typeHint)
		{
			object result;
			if (typeHint == typeof(string))
			{
				result = this.ToString();
			}
			else
			{
				result = base.GetDefaultValue(typeHint);
			}
			return result;
		}
		public override string ToString()
		{
			string str = "function " + this.m_Name + "() \n";
			str += "{/*\n";
			MethodInfo[] methodInfos = this.m_MethodInfos;
			for (int i = 0; i < methodInfos.Length; i++)
			{
				MethodInfo mi = methodInfos[i];
				str = str + CliHelper.ToSignature(mi) + "\n";
			}
			return str + "*/}";
		}
	}
}
