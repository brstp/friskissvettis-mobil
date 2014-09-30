using log4net.Core;
using System;
namespace log4net.Filter
{
	public abstract class FilterSkeleton : IFilter, IOptionHandler
	{
		private IFilter m_next;
		public IFilter Next
		{
			get
			{
				return this.m_next;
			}
			set
			{
				this.m_next = value;
			}
		}
		public virtual void ActivateOptions()
		{
		}
		public abstract FilterDecision Decide(LoggingEvent loggingEvent);
	}
}
