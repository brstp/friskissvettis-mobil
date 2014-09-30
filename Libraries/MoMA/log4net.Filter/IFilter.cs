using log4net.Core;
using System;
namespace log4net.Filter
{
	public interface IFilter : IOptionHandler
	{
		IFilter Next
		{
			get;
			set;
		}
		FilterDecision Decide(LoggingEvent loggingEvent);
	}
}
