using log4net.Core;
using log4net.Util.TypeConverters;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
namespace log4net.Util
{
	public sealed class OptionConverter
	{
		private const string DELIM_START = "${";
		private const char DELIM_STOP = '}';
		private const int DELIM_START_LEN = 2;
		private const int DELIM_STOP_LEN = 1;
		private OptionConverter()
		{
		}
		public static bool ToBoolean(string argValue, bool defaultValue)
		{
			bool result;
			if (argValue != null && argValue.Length > 0)
			{
				try
				{
					result = bool.Parse(argValue);
					return result;
				}
				catch (Exception exception)
				{
					LogLog.Error("OptionConverter: [" + argValue + "] is not in proper bool form.", exception);
				}
			}
			result = defaultValue;
			return result;
		}
		public static long ToFileSize(string argValue, long defaultValue)
		{
			long result;
			if (argValue == null)
			{
				result = defaultValue;
			}
			else
			{
				string text = argValue.Trim().ToUpper(CultureInfo.InvariantCulture);
				long num = 1L;
				int length;
				if ((length = text.IndexOf("KB")) != -1)
				{
					num = 1024L;
					text = text.Substring(0, length);
				}
				else
				{
					if ((length = text.IndexOf("MB")) != -1)
					{
						num = 1048576L;
						text = text.Substring(0, length);
					}
					else
					{
						if ((length = text.IndexOf("GB")) != -1)
						{
							num = 1073741824L;
							text = text.Substring(0, length);
						}
					}
				}
				if (text != null)
				{
					text = text.Trim();
					long num2;
					if (SystemInfo.TryParse(text, out num2))
					{
						result = num2 * num;
						return result;
					}
					LogLog.Error("OptionConverter: [" + text + "] is not in the correct file size syntax.");
				}
				result = defaultValue;
			}
			return result;
		}
		public static object ConvertStringTo(Type target, string txt)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			object result;
			if (typeof(string) == target || typeof(object) == target)
			{
				result = txt;
			}
			else
			{
				IConvertFrom convertFrom = ConverterRegistry.GetConvertFrom(target);
				if (convertFrom != null && convertFrom.CanConvertFrom(typeof(string)))
				{
					result = convertFrom.ConvertFrom(txt);
				}
				else
				{
					if (target.IsEnum)
					{
						result = OptionConverter.ParseEnum(target, txt, true);
					}
					else
					{
						MethodInfo method = target.GetMethod("Parse", new Type[]
						{
							typeof(string)
						});
						if (method != null)
						{
							result = method.Invoke(null, BindingFlags.InvokeMethod, null, new object[]
							{
								txt
							}, CultureInfo.InvariantCulture);
						}
						else
						{
							result = null;
						}
					}
				}
			}
			return result;
		}
		public static bool CanConvertTypeTo(Type sourceType, Type targetType)
		{
			bool result;
			if (sourceType == null || targetType == null)
			{
				result = false;
			}
			else
			{
				if (targetType.IsAssignableFrom(sourceType))
				{
					result = true;
				}
				else
				{
					IConvertTo convertTo = ConverterRegistry.GetConvertTo(sourceType, targetType);
					if (convertTo != null)
					{
						if (convertTo.CanConvertTo(targetType))
						{
							result = true;
							return result;
						}
					}
					IConvertFrom convertFrom = ConverterRegistry.GetConvertFrom(targetType);
					if (convertFrom != null)
					{
						if (convertFrom.CanConvertFrom(sourceType))
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
			}
			return result;
		}
		public static object ConvertTypeTo(object sourceInstance, Type targetType)
		{
			Type type = sourceInstance.GetType();
			object result;
			if (!targetType.IsAssignableFrom(type))
			{
				IConvertTo convertTo = ConverterRegistry.GetConvertTo(type, targetType);
				if (convertTo != null)
				{
					if (convertTo.CanConvertTo(targetType))
					{
						result = convertTo.ConvertTo(sourceInstance, targetType);
						return result;
					}
				}
				IConvertFrom convertFrom = ConverterRegistry.GetConvertFrom(targetType);
				if (convertFrom != null)
				{
					if (convertFrom.CanConvertFrom(type))
					{
						result = convertFrom.ConvertFrom(sourceInstance);
						return result;
					}
				}
				throw new ArgumentException(string.Concat(new string[]
				{
					"Cannot convert source object [",
					sourceInstance.ToString(),
					"] to target type [",
					targetType.Name,
					"]"
				}), "sourceInstance");
			}
			result = sourceInstance;
			return result;
		}
		public static object InstantiateByClassName(string className, Type superClass, object defaultValue)
		{
			object result;
			if (className != null)
			{
				try
				{
					Type typeFromString = SystemInfo.GetTypeFromString(className, true, true);
					if (!superClass.IsAssignableFrom(typeFromString))
					{
						LogLog.Error(string.Concat(new string[]
						{
							"OptionConverter: A [",
							className,
							"] object is not assignable to a [",
							superClass.FullName,
							"] variable."
						}));
						result = defaultValue;
						return result;
					}
					result = Activator.CreateInstance(typeFromString);
					return result;
				}
				catch (Exception exception)
				{
					LogLog.Error("OptionConverter: Could not instantiate class [" + className + "].", exception);
				}
			}
			result = defaultValue;
			return result;
		}
		public static string SubstituteVariables(string value, IDictionary props)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2;
			while (true)
			{
				num2 = value.IndexOf("${", num);
				if (num2 == -1)
				{
					break;
				}
				stringBuilder.Append(value.Substring(num, num2 - num));
				int num3 = value.IndexOf('}', num2);
				if (num3 == -1)
				{
					goto Block_3;
				}
				num2 += 2;
				string key = value.Substring(num2, num3 - num2);
				string text = props[key] as string;
				if (text != null)
				{
					stringBuilder.Append(text);
				}
				num = num3 + 1;
			}
			string result;
			if (num == 0)
			{
				result = value;
			}
			else
			{
				stringBuilder.Append(value.Substring(num, value.Length - num));
				result = stringBuilder.ToString();
			}
			return result;
			Block_3:
			throw new LogException(string.Concat(new object[]
			{
				"[",
				value,
				"] has no closing brace. Opening brace at position [",
				num2,
				"]"
			}));
		}
		private static object ParseEnum(Type enumType, string value, bool ignoreCase)
		{
			return Enum.Parse(enumType, value, ignoreCase);
		}
	}
}
