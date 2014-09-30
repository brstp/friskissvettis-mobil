using log4net.Core;
using System;
namespace log4net.Filter
{
	public class PropertyFilter : StringMatchFilter
	{
		private string m_key;
		public string Key
		{
			get
			{
				return this.m_key;
			}
			set
			{
				this.m_key = value;
			}
		}
		public override FilterDecision Decide(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			FilterDecision result;
			if (this.m_key == null)
			{
				result = FilterDecision.Neutral;
			}
			else
			{
				object obj = loggingEvent.LookupProperty(this.m_key);
				string text = loggingEvent.Repository.RendererMap.FindAndRender(obj);
				if (text == null || (this.m_stringToMatch == null && this.m_regexToMatch == null))
				{
					result = FilterDecision.Neutral;
				}
				else
				{
					if (this.m_regexToMatch != null)
					{
						if (!this.m_regexToMatch.Match(text).Success)
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
							if (text.IndexOf(this.m_stringToMatch) == -1)
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
			}
			return result;
		}
	}
}
