using System;
namespace log4net.Util.TypeConverters
{
	internal class BooleanConverter : IConvertFrom
	{
		public bool CanConvertFrom(Type sourceType)
		{
			return sourceType == typeof(string);
		}
		public object ConvertFrom(object source)
		{
			string text = source as string;
			if (text != null)
			{
				return bool.Parse(text);
			}
			throw ConversionNotSupportedException.Create(typeof(bool), source);
		}
	}
}
