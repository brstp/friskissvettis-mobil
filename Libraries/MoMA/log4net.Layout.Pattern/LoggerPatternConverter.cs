using log4net.Core;
using System;
namespace log4net.Layout.Pattern
{
	internal sealed class LoggerPatternConverter : NamedPatternConverter
	{
		protected override string GetFullyQualifiedName(LoggingEvent loggingEvent)
		{
			return loggingEvent.LoggerName;
		}
	}
}
