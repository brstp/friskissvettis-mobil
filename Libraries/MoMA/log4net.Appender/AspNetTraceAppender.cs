using log4net.Core;
using System;
using System.Web;
namespace log4net.Appender
{
	public class AspNetTraceAppender : AppenderSkeleton
	{
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (HttpContext.Current != null)
			{
				if (HttpContext.Current.Trace.IsEnabled)
				{
					if (loggingEvent.Level >= Level.Warn)
					{
						HttpContext.Current.Trace.Warn(loggingEvent.LoggerName, base.RenderLoggingEvent(loggingEvent));
					}
					else
					{
						HttpContext.Current.Trace.Write(loggingEvent.LoggerName, base.RenderLoggingEvent(loggingEvent));
					}
				}
			}
		}
	}
}
