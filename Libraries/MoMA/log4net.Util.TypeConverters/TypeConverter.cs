using System;
namespace log4net.Util.TypeConverters
{
	internal class TypeConverter : IConvertFrom
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
				return SystemInfo.GetTypeFromString(text, true, true);
			}
			throw ConversionNotSupportedException.Create(typeof(Type), source);
		}
	}
}
