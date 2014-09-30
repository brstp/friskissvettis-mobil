using log4net.Core;
using System;
using System.IO;
namespace log4net.Layout.Pattern
{
	internal sealed class UserNamePatternConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			writer.Write(loggingEvent.UserName);
		}
	}
}
