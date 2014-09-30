using System;
namespace log4net.Util.TypeConverters
{
	internal class PatternStringConverter : IConvertTo, IConvertFrom
	{
		public bool CanConvertTo(Type targetType)
		{
			return typeof(string).IsAssignableFrom(targetType);
		}
		public object ConvertTo(object source, Type targetType)
		{
			PatternString patternString = source as PatternString;
			if (patternString != null && this.CanConvertTo(targetType))
			{
				return patternString.Format();
			}
			throw ConversionNotSupportedException.Create(targetType, source);
		}
		public bool CanConvertFrom(Type sourceType)
		{
			return sourceType == typeof(string);
		}
		public object ConvertFrom(object source)
		{
			string text = source as string;
			if (text != null)
			{
				return new PatternString(text);
			}
			throw ConversionNotSupportedException.Create(typeof(PatternString), source);
		}
	}
}
