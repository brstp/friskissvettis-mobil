using System;
namespace log4net.Repository.Hierarchy
{
	public class LoggerCreationEventArgs : EventArgs
	{
		private Logger m_log;
		public Logger Logger
		{
			get
			{
				return this.m_log;
			}
		}
		public LoggerCreationEventArgs(Logger log)
		{
			this.m_log = log;
		}
	}
}
