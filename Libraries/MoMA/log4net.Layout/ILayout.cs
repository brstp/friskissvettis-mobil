using log4net.Core;
using System;
using System.IO;
namespace log4net.Layout
{
	public interface ILayout
	{
		string ContentType
		{
			get;
		}
		string Header
		{
			get;
		}
		string Footer
		{
			get;
		}
		bool IgnoresException
		{
			get;
		}
		void Format(TextWriter writer, LoggingEvent loggingEvent);
	}
}
