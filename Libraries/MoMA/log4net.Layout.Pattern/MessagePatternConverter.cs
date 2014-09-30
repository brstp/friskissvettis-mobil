using log4net.Core;
using System;
using System.IO;
namespace log4net.Layout.Pattern
{
	internal sealed class MessagePatternConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			loggingEvent.WriteRenderedMessage(writer);
		}
	}
}
