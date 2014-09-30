using log4net.Core;
using System;
namespace log4net.Layout
{
	public class RawPropertyLayout : IRawLayout
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
		public virtual object Format(LoggingEvent loggingEvent)
		{
			return loggingEvent.LookupProperty(this.m_key);
		}
	}
}
