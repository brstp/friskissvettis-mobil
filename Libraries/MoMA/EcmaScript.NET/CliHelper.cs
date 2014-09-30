using System;
using System.Reflection;
using System.Text;
namespace EcmaScript.NET
{
	internal sealed class CliHelper
	{
		private CliHelper()
		{
		}
		internal static Type GetType(string className)
		{
			Type result;
			try
			{
				result = Type.GetType(className);
				return result;
			}
			catch
			{
			}
			result = null;
			return result;
		}
		internal static bool IsNegativeZero(double d)
		{
			return !double.IsNaN(d) && d == 0.0 && double.PositiveInfinity / d == double.NegativeInfinity;
		}
		internal static bool IsPositiveZero(double d)
		{
			return !double.IsNaN(d) && d == 0.0 && double.PositiveInfinity / d == double.PositiveInfinity;
		}
		internal new static bool Equals(object o1, object o2)
		{
			return (o1 == null && o2 == null) || (o1 != null && o2 != null && o1.Equals(o2));
		}
		internal static string ToSignature(ConstructorInfo ci)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(CliHelper.ToSignature(ci.DeclaringType));
			stringBuilder.Append(CliHelper.ToSignature('(', ci.GetParameters(), ')'));
			return stringBuilder.ToString();
		}
		internal static string ToSignature(PropertyInfo pi)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(CliHelper.ToSignature(pi.PropertyType));
			stringBuilder.Append(" ");
			stringBuilder.Append(CliHelper.ToSignature(pi.DeclaringType));
			stringBuilder.Append(".");
			stringBuilder.Append(pi.Name);
			stringBuilder.Append(CliHelper.ToSignature('[', pi.GetIndexParameters(), ']'));
			return stringBuilder.ToString();
		}
		internal static string ToSignature(FieldInfo fi)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(CliHelper.ToSignature(fi.FieldType));
			stringBuilder.Append(" ");
			stringBuilder.Append(CliHelper.ToSignature(fi.DeclaringType));
			stringBuilder.Append(".");
			stringBuilder.Append(fi.Name);
			return stringBuilder.ToString();
		}
		internal static string ToSignature(object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(CliHelper.ToSignature(args[0].GetType()));
			}
			return stringBuilder.ToString();
		}
		internal static string ToSignature(MemberInfo mi)
		{
			string result;
			if (mi is PropertyInfo)
			{
				result = CliHelper.ToSignature((PropertyInfo)mi);
			}
			else
			{
				if (mi is FieldInfo)
				{
					result = CliHelper.ToSignature((FieldInfo)mi);
				}
				else
				{
					if (mi is ConstructorInfo)
					{
						result = CliHelper.ToSignature((ConstructorInfo)mi);
					}
					else
					{
						if (mi is MethodInfo)
						{
							result = CliHelper.ToSignature((MethodInfo)mi);
						}
						else
						{
							result = "[unknown: " + mi.GetType().FullName + "]";
						}
					}
				}
			}
			return result;
		}
		internal static string ToSignature(char parenOpen, ParameterInfo[] pi, char parenClose)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(parenOpen);
			for (int i = 0; i < pi.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				if (pi[i].IsOut)
				{
					stringBuilder.Append("out ");
				}
				if (pi[i].IsIn)
				{
					stringBuilder.Append("in ");
				}
				if (CliHelper.IsParamsParameter(pi[i]))
				{
					stringBuilder.Append("params ");
				}
				stringBuilder.Append(CliHelper.ToSignature(pi[i].ParameterType));
				stringBuilder.Append(" ");
				stringBuilder.Append(pi[i].Name);
			}
			stringBuilder.Append(parenClose);
			return stringBuilder.ToString();
		}
		internal static string ToSignature(MethodInfo mi)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(CliHelper.ToSignature(mi.ReturnType));
			stringBuilder.Append(" ");
			stringBuilder.Append(CliHelper.ToSignature(mi.DeclaringType));
			stringBuilder.Append(".");
			stringBuilder.Append(mi.Name);
			stringBuilder.Append(CliHelper.ToSignature('(', mi.GetParameters(), ')'));
			return stringBuilder.ToString();
		}
		internal static bool HasParamsParameter(MethodBase mb)
		{
			return CliHelper.HasParamsParameter(mb.GetParameters());
		}
		internal static bool HasParamsParameter(ParameterInfo[] pis)
		{
			bool result;
			for (int i = 0; i < pis.Length; i++)
			{
				if (CliHelper.IsParamsParameter(pis[i]))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		internal static bool IsParamsParameter(ParameterInfo pi)
		{
			ParamArrayAttribute paramArrayAttribute = (ParamArrayAttribute)CliHelper.GetCustomAttribute(typeof(ParamArrayAttribute), pi);
			return paramArrayAttribute != null;
		}
		internal static string ToSignature(Type type)
		{
			string result;
			if (type.IsArray)
			{
				string text = CliHelper.ToSignature(type.GetElementType());
				for (int i = 0; i < type.GetArrayRank(); i++)
				{
					text += "[]";
				}
				result = text;
			}
			else
			{
				if (type == typeof(short))
				{
					result = "short";
				}
				else
				{
					if (type == typeof(ushort))
					{
						result = "ushort";
					}
					else
					{
						if (type == typeof(int))
						{
							result = "int";
						}
						else
						{
							if (type == typeof(uint))
							{
								result = "uint";
							}
							else
							{
								if (type == typeof(ulong))
								{
									result = "ulong";
								}
								else
								{
									if (type == typeof(long))
									{
										result = "long";
									}
									else
									{
										if (type == typeof(void))
										{
											result = "void";
										}
										else
										{
											if (type == typeof(bool))
											{
												result = "bool";
											}
											else
											{
												if (type == typeof(double))
												{
													result = "double";
												}
												else
												{
													if (type == typeof(decimal))
													{
														result = "decimal";
													}
													else
													{
														if (type == typeof(object))
														{
															result = "object";
														}
														else
														{
															result = type.FullName;
														}
													}
												}
											}
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
		internal static bool IsNumberType(Type type)
		{
			return type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
		}
		internal static bool IsNumber(object value)
		{
			return value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
		}
		internal static object CreateInstance(Type cl)
		{
			object result;
			try
			{
				result = Activator.CreateInstance(cl);
				return result;
			}
			catch
			{
			}
			result = null;
			return result;
		}
		internal static object GetCustomAttribute(Type type, Type attribute)
		{
			object[] customAttributes = type.GetCustomAttributes(attribute, true);
			object result;
			if (customAttributes.Length < 1)
			{
				result = null;
			}
			else
			{
				result = customAttributes[0];
			}
			return result;
		}
		internal static object GetCustomAttribute(Type type, MemberInfo mi)
		{
			object result = null;
			object[] customAttributes = mi.GetCustomAttributes(type, true);
			if (customAttributes.Length > 0)
			{
				result = customAttributes[0];
			}
			return result;
		}
		internal static object GetCustomAttribute(Type type, ParameterInfo pi)
		{
			object[] customAttributes = pi.GetCustomAttributes(type, true);
			object result;
			if (customAttributes.Length < 1)
			{
				result = null;
			}
			else
			{
				result = customAttributes[0];
			}
			return result;
		}
		internal static Type[] GetParameterTypes(ParameterInfo[] parameters)
		{
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			return array;
		}
	}
}
