using log4net.Core;
using System;
namespace log4net.Appender
{
	public interface IAppender
	{
		string Name
		{
			get;
			set;
		}
		void Close();
		void DoAppend(LoggingEvent loggingEvent);
	}
}
