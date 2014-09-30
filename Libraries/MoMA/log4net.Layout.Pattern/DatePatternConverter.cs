using log4net.Core;
using log4net.DateFormatter;
using log4net.Util;
using System;
using System.Globalization;
using System.IO;
namespace log4net.Layout.Pattern
{
	internal class DatePatternConverter : PatternLayoutConverter, IOptionHandler
	{
		protected IDateFormatter m_dateFormatter;
		public void ActivateOptions()
		{
			string text = this.Option;
			if (text == null)
			{
				text = "ISO8601";
			}
			if (string.Compare(text, "ISO8601", true, CultureInfo.InvariantCulture) == 0)
			{
				this.m_dateFormatter = new Iso8601DateFormatter();
			}
			else
			{
				if (string.Compare(text, "ABSOLUTE", true, CultureInfo.InvariantCulture) == 0)
				{
					this.m_dateFormatter = new AbsoluteTimeDateFormatter();
				}
				else
				{
					if (string.Compare(text, "DATE", true, CultureInfo.InvariantCulture) == 0)
					{
						this.m_dateFormatter = new DateTimeDateFormatter();
					}
					else
					{
						try
						{
							this.m_dateFormatter = new SimpleDateFormatter(text);
						}
						catch (Exception exception)
						{
							LogLog.Error("DatePatternConverter: Could not instantiate SimpleDateFormatter with [" + text + "]", exception);
							this.m_dateFormatter = new Iso8601DateFormatter();
						}
					}
				}
			}
		}
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			try
			{
				this.m_dateFormatter.FormatDate(loggingEvent.TimeStamp, writer);
			}
			catch (Exception exception)
			{
				LogLog.Error("DatePatternConverter: Error occurred while converting date.", exception);
			}
		}
	}
}