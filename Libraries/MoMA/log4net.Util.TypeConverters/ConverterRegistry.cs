using log4net.Layout;
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading;
namespace log4net.Util.TypeConverters
{
	public sealed class ConverterRegistry
	{
		private static Hashtable s_type2converter;
		private ConverterRegistry()
		{
		}
		static ConverterRegistry()
		{
			ConverterRegistry.s_type2converter = new Hashtable();
			ConverterRegistry.AddConverter(typeof(bool), typeof(BooleanConverter));
			ConverterRegistry.AddConverter(typeof(Encoding), typeof(EncodingConverter));
			ConverterRegistry.AddConverter(typeof(Type), typeof(TypeConverter));
			ConverterRegistry.AddConverter(typeof(PatternLayout), typeof(PatternLayoutConverter));
			ConverterRegistry.AddConverter(typeof(PatternString), typeof(PatternStringConverter));
			ConverterRegistry.AddConverter(typeof(IPAddress), typeof(IPAddressConverter));
		}
		public static void AddConverter(Type destinationType, object converter)
		{
			if (destinationType != null && converter != null)
			{
				Hashtable obj;
				Monitor.Enter(obj = ConverterRegistry.s_type2converter);
				try
				{
					ConverterRegistry.s_type2converter[destinationType] = converter;
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}
		public static void AddConverter(Type destinationType, Type converterType)
		{
			ConverterRegistry.AddConverter(destinationType, ConverterRegistry.CreateConverterInstance(converterType));
		}
		public static IConvertTo GetConvertTo(Type sourceType, Type destinationType)
		{
			Hashtable obj;
			Monitor.Enter(obj = ConverterRegistry.s_type2converter);
			IConvertTo result;
			try
			{
				IConvertTo convertTo = ConverterRegistry.s_type2converter[sourceType] as IConvertTo;
				if (convertTo == null)
				{
					convertTo = (ConverterRegistry.GetConverterFromAttribute(sourceType) as IConvertTo);
					if (convertTo != null)
					{
						ConverterRegistry.s_type2converter[sourceType] = convertTo;
					}
				}
				result = convertTo;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static IConvertFrom GetConvertFrom(Type destinationType)
		{
			Hashtable obj;
			Monitor.Enter(obj = ConverterRegistry.s_type2converter);
			IConvertFrom result;
			try
			{
				IConvertFrom convertFrom = ConverterRegistry.s_type2converter[destinationType] as IConvertFrom;
				if (convertFrom == null)
				{
					convertFrom = (ConverterRegistry.GetConverterFromAttribute(destinationType) as IConvertFrom);
					if (convertFrom != null)
					{
						ConverterRegistry.s_type2converter[destinationType] = convertFrom;
					}
				}
				result = convertFrom;
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		private static object GetConverterFromAttribute(Type destinationType)
		{
			object[] customAttributes = destinationType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
			object result;
			if (customAttributes != null && customAttributes.Length > 0)
			{
				TypeConverterAttribute typeConverterAttribute = customAttributes[0] as TypeConverterAttribute;
				if (typeConverterAttribute != null)
				{
					Type typeFromString = SystemInfo.GetTypeFromString(destinationType, typeConverterAttribute.ConverterTypeName, false, true);
					result = ConverterRegistry.CreateConverterInstance(typeFromString);
					return result;
				}
			}
			result = null;
			return result;
		}
		private static object CreateConverterInstance(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType", "CreateConverterInstance cannot create instance, converterType is null");
			}
			object result;
			if (typeof(IConvertFrom).IsAssignableFrom(converterType) || typeof(IConvertTo).IsAssignableFrom(converterType))
			{
				try
				{
					result = Activator.CreateInstance(converterType);
					return result;
				}
				catch (Exception exception)
				{
					LogLog.Error("ConverterRegistry: Cannot CreateConverterInstance of type [" + converterType.FullName + "], Exception in call to Activator.CreateInstance", exception);
				}
			}
			else
			{
				LogLog.Error("ConverterRegistry: Cannot CreateConverterInstance of type [" + converterType.FullName + "], type does not implement IConvertFrom or IConvertTo");
			}
			result = null;
			return result;
		}
	}
}
