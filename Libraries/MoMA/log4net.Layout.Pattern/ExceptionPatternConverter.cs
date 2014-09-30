using log4net.Core;
using System;
using System.IO;
namespace log4net.Layout.Pattern
{
	internal sealed class ExceptionPatternConverter : PatternLayoutConverter
	{
		public ExceptionPatternConverter()
		{
			this.IgnoresException = false;
		}
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			string exceptionString = loggingEvent.GetExceptionString();
			if (exceptionString != null && exceptionString.Length > 0)
			{
				writer.WriteLine(exceptionString);
			}
		}
	}
}
