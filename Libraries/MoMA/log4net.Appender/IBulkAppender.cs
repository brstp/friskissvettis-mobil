using log4net.Core;
using System;
namespace log4net.Appender
{
	public interface IBulkAppender : IAppender
	{
		void DoAppend(LoggingEvent[] loggingEvents);
	}
}
