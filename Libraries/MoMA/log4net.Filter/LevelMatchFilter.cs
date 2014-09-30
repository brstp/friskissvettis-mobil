using log4net.Core;
using System;
namespace log4net.Filter
{
	public class LevelMatchFilter : FilterSkeleton
	{
		private bool m_acceptOnMatch = true;
		private Level m_levelToMatch;
		public bool AcceptOnMatch
		{
			get
			{
				return this.m_acceptOnMatch;
			}
			set
			{
				this.m_acceptOnMatch = value;
			}
		}
		public Level LevelToMatch
		{
			get
			{
				return this.m_levelToMatch;
			}
			set
			{
				this.m_levelToMatch = value;
			}
		}
		public override FilterDecision Decide(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			FilterDecision result;
			if (this.m_levelToMatch != null && this.m_levelToMatch == loggingEvent.Level)
			{
				result = (this.m_acceptOnMatch ? FilterDecision.Accept : FilterDecision.Deny);
			}
			else
			{
				result = FilterDecision.Neutral;
			}
			return result;
		}
	}
}
