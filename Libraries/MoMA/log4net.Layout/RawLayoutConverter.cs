using log4net.Util.TypeConverters;
using System;
namespace log4net.Layout
{
	public class RawLayoutConverter : IConvertFrom
	{
		public bool CanConvertFrom(Type sourceType)
		{
			return typeof(ILayout).IsAssignableFrom(sourceType);
		}
		public object ConvertFrom(object source)
		{
			ILayout layout = source as ILayout;
			if (layout != null)
			{
				return new Layout2RawLayoutAdapter(layout);
			}
			throw ConversionNotSupportedException.Create(typeof(IRawLayout), source);
		}
	}
}
