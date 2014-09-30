using log4net.Core;
using System;
using System.Text.RegularExpressions;
namespace log4net.Filter
{
	public class StringMatchFilter : FilterSkeleton
	{
		protected bool m_acceptOnMatch = true;
		protected string m_stringToMatch;
		protected string m_stringRegexToMatch;
		protected Regex m_regexToMatch;
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
		public string StringToMatch
		{
			get
			{
				return this.m_stringToMatch;
			}
			set
			{
				this.m_stringToMatch = value;
			}
		}
		public string RegexToMatch
		{
			get
			{
				return this.m_stringRegexToMatch;
			}
			set
			{
				this.m_stringRegexToMatch = value;
			}
		}
		public override void ActivateOptions()
		{
			if (this.m_stringRegexToMatch != null)
			{
				this.m_regexToMatch = new Regex(this.m_stringRegexToMatch, RegexOptions.Compiled);
			}
		}
		public override FilterDecision Decide(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			string renderedMessage = loggingEvent.RenderedMessage;
			FilterDecision result;
			if (renderedMessage == null || (this.m_stringToMatch == null && this.m_regexToMatch == null))
			{
				result = FilterDecision.Neutral;
			}
			else
			{
				if (this.m_regexToMatch != null)
				{
					if (!this.m_regexToMatch.Match(renderedMessage).Success)
					{
						result = FilterDecision.Neutral;
					}
					else
					{
						if (this.m_acceptOnMatch)
						{
							result = FilterDecision.Accept;
						}
						else
						{
							result = FilterDecision.Deny;
						}
					}
				}
				else
				{
					if (this.m_stringToMatch != null)
					{
						if (renderedMessage.IndexOf(this.m_stringToMatch) == -1)
						{
							result = FilterDecision.Neutral;
						}
						else
						{
							if (this.m_acceptOnMatch)
							{
								result = FilterDecision.Accept;
							}
							else
							{
								result = FilterDecision.Deny;
							}
						}
					}
					else
					{
						result = FilterDecision.Neutral;
					}
				}
			}
			return result;
		}
	}
}
