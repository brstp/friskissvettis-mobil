using log4net.Core;
using log4net.Layout;
using System;
using System.Globalization;
namespace log4net.Appender
{
	public class ConsoleAppender : AppenderSkeleton
	{
		public const string ConsoleOut = "Console.Out";
		public const string ConsoleError = "Console.Error";
		private bool m_writeToErrorStream = false;
		public virtual string Target
		{
			get
			{
				return this.m_writeToErrorStream ? "Console.Error" : "Console.Out";
			}
			set
			{
				string strB = value.Trim();
				if (string.Compare("Console.Error", strB, true, CultureInfo.InvariantCulture) == 0)
				{
					this.m_writeToErrorStream = true;
				}
				else
				{
					this.m_writeToErrorStream = false;
				}
			}
		}
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		public ConsoleAppender()
		{
		}
		[Obsolete("Instead use the default constructor and set the Layout property")]
		public ConsoleAppender(ILayout layout) : this(layout, false)
		{
		}
		[Obsolete("Instead use the default constructor and set the Layout & Target properties")]
		public ConsoleAppender(ILayout layout, bool writeToErrorStream)
		{
			this.Layout = layout;
			this.m_writeToErrorStream = writeToErrorStream;
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (this.m_writeToErrorStream)
			{
				Console.Error.Write(base.RenderLoggingEvent(loggingEvent));
			}
			else
			{
				Console.Write(base.RenderLoggingEvent(loggingEvent));
			}
		}
	}
}
