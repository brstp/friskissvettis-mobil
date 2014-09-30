using log4net.Core;
using System;
namespace log4net.Filter
{
	public sealed class DenyAllFilter : FilterSkeleton
	{
		public override FilterDecision Decide(LoggingEvent loggingEvent)
		{
			return FilterDecision.Deny;
		}
	}
}
