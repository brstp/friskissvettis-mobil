using log4net.Core;
using System;
namespace log4net.Util
{
	public class OnlyOnceErrorHandler : IErrorHandler
	{
		private bool m_firstTime = true;
		private readonly string m_prefix;
		private bool IsEnabled
		{
			get
			{
				bool result;
				if (this.m_firstTime)
				{
					this.m_firstTime = false;
					result = true;
				}
				else
				{
					result = (LogLog.InternalDebugging && !LogLog.QuietMode);
				}
				return result;
			}
		}
		public OnlyOnceErrorHandler()
		{
			this.m_prefix = "";
		}
		public OnlyOnceErrorHandler(string prefix)
		{
			this.m_prefix = prefix;
		}
		public void Error(string message, Exception e, ErrorCode errorCode)
		{
			if (this.IsEnabled)
			{
				LogLog.Error("[" + this.m_prefix + "] " + message, e);
			}
		}
		public void Error(string message, Exception e)
		{
			if (this.IsEnabled)
			{
				LogLog.Error("[" + this.m_prefix + "] " + message, e);
			}
		}
		public void Error(string message)
		{
			if (this.IsEnabled)
			{
				LogLog.Error("[" + this.m_prefix + "] " + message);
			}
		}
	}
}
