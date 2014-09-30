using log4net.Layout;
using System;
namespace log4net.Util.TypeConverters
{
	internal class PatternLayoutConverter : IConvertFrom
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
				return new PatternLayout(text);
			}
			throw ConversionNotSupportedException.Create(typeof(PatternLayout), source);
		}
	}
}
