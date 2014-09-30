using log4net.Core;
using System;
using System.Runtime.InteropServices;
namespace log4net.Appender
{
	public class OutputDebugStringAppender : AppenderSkeleton
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
			OutputDebugStringAppender.OutputDebugString(base.RenderLoggingEvent(loggingEvent));
		}
		[DllImport("Kernel32.dll")]
		protected static extern void OutputDebugString(string message);
	}
}
