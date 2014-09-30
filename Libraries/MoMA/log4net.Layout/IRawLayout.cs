using log4net.Core;
using log4net.Util.TypeConverters;
using System;
namespace log4net.Layout
{
	[TypeConverter(typeof(RawLayoutConverter))]
	public interface IRawLayout
	{
		object Format(LoggingEvent loggingEvent);
	}
}
