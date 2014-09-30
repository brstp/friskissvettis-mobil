using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace MoMA.Helpers
{
	public class ReflectionHelper
	{
		public static T GetAttributeFromProperty<T>(object obj, string propertyName, T defaultValue)
		{
			Type typeFromHandle = typeof(T);
			try
			{
				object value = obj.GetType().GetProperty(propertyName).GetCustomAttributes(typeFromHandle, false).FirstOrDefault<object>();
				defaultValue = ConversionHelper.Convert<T>(value, defaultValue);
			}
			catch
			{
			}
			return defaultValue;
		}
		public static List<Type> GetAllClasses(Assembly asm, string nameSpace, Type shouldBeThisInterface)
		{
			List<Type> list = new List<Type>();
			Type[] types = asm.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.Namespace != null && type.IsClass && type.Namespace.Equals(nameSpace))
				{
					list.Add(type);
				}
			}
			return list;
		}
		public static void InvokeGenericMethod(object classObj, string functionName, Type type, object[] parameters)
		{
			MethodInfo methodInfo = classObj.GetType().GetMethod(functionName).MakeGenericMethod(new Type[]
			{
				type
			});
			methodInfo.Invoke(classObj, parameters);
		}
		public static void InvokeGenericMethod(object classObj, string functionName, Type type)
		{
			ReflectionHelper.InvokeGenericMethod(classObj, functionName, type, new object[0]);
		}
	}
}
