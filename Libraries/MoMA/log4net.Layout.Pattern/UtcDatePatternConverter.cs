using log4net.Core;
using log4net.Util;
using System;
using System.IO;
namespace log4net.Layout.Pattern
{
	internal class UtcDatePatternConverter : DatePatternConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			try
			{
				this.m_dateFormatter.FormatDate(loggingEvent.TimeStamp.ToUniversalTime(), writer);
			}
			catch (Exception exception)
			{
				LogLog.Error("UtcDatePatternConverter: Error occurred while converting date.", exception);
			}
		}
	}
}
