using log4net.Appender;
using log4net.Core;
using System;
namespace log4net.Util
{
	public class AppenderAttachedImpl : IAppenderAttachable
	{
		private AppenderCollection m_appenderList;
		private IAppender[] m_appenderArray;
		public AppenderCollection Appenders
		{
			get
			{
				AppenderCollection result;
				if (this.m_appenderList == null)
				{
					result = AppenderCollection.EmptyCollection;
				}
				else
				{
					result = AppenderCollection.ReadOnly(this.m_appenderList);
				}
				return result;
			}
		}
		public int AppendLoopOnAppenders(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			int result;
			if (this.m_appenderList == null)
			{
				result = 0;
			}
			else
			{
				if (this.m_appenderArray == null)
				{
					this.m_appenderArray = this.m_appenderList.ToArray();
				}
				IAppender[] appenderArray = this.m_appenderArray;
				for (int i = 0; i < appenderArray.Length; i++)
				{
					IAppender appender = appenderArray[i];
					try
					{
						appender.DoAppend(loggingEvent);
					}
					catch (Exception exception)
					{
						LogLog.Error("AppenderAttachedImpl: Failed to append to appender [" + appender.Name + "]", exception);
					}
				}
				result = this.m_appenderList.Count;
			}
			return result;
		}
		public int AppendLoopOnAppenders(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
			{
				throw new ArgumentNullException("loggingEvents");
			}
			if (loggingEvents.Length == 0)
			{
				throw new ArgumentException("loggingEvents array must not be empty", "loggingEvents");
			}
			int result;
			if (loggingEvents.Length == 1)
			{
				result = this.AppendLoopOnAppenders(loggingEvents[0]);
			}
			else
			{
				if (this.m_appenderList == null)
				{
					result = 0;
				}
				else
				{
					if (this.m_appenderArray == null)
					{
						this.m_appenderArray = this.m_appenderList.ToArray();
					}
					IAppender[] appenderArray = this.m_appenderArray;
					for (int i = 0; i < appenderArray.Length; i++)
					{
						IAppender appender = appenderArray[i];
						try
						{
							AppenderAttachedImpl.CallAppend(appender, loggingEvents);
						}
						catch (Exception exception)
						{
							LogLog.Error("AppenderAttachedImpl: Failed to append to appender [" + appender.Name + "]", exception);
						}
					}
					result = this.m_appenderList.Count;
				}
			}
			return result;
		}
		private static void CallAppend(IAppender appender, LoggingEvent[] loggingEvents)
		{
			IBulkAppender bulkAppender = appender as IBulkAppender;
			if (bulkAppender != null)
			{
				bulkAppender.DoAppend(loggingEvents);
			}
			else
			{
				for (int i = 0; i < loggingEvents.Length; i++)
				{
					LoggingEvent loggingEvent = loggingEvents[i];
					appender.DoAppend(loggingEvent);
				}
			}
		}
		public void AddAppender(IAppender newAppender)
		{
			if (newAppender == null)
			{
				throw new ArgumentNullException("newAppender");
			}
			this.m_appenderArray = null;
			if (this.m_appenderList == null)
			{
				this.m_appenderList = new AppenderCollection(1);
			}
			if (!this.m_appenderList.Contains(newAppender))
			{
				this.m_appenderList.Add(newAppender);
			}
		}
		public IAppender GetAppender(string name)
		{
			IAppender result;
			if (this.m_appenderList != null && name != null)
			{
				foreach (IAppender current in this.m_appenderList)
				{
					if (name == current.Name)
					{
						result = current;
						return result;
					}
				}
			}
			result = null;
			return result;
		}
		public void RemoveAllAppenders()
		{
			if (this.m_appenderList != null)
			{
				foreach (IAppender current in this.m_appenderList)
				{
					try
					{
						current.Close();
					}
					catch (Exception exception)
					{
						LogLog.Error("AppenderAttachedImpl: Failed to Close appender [" + current.Name + "]", exception);
					}
				}
				this.m_appenderList = null;
				this.m_appenderArray = null;
			}
		}
		public IAppender RemoveAppender(IAppender appender)
		{
			if (appender != null && this.m_appenderList != null)
			{
				this.m_appenderList.Remove(appender);
				if (this.m_appenderList.Count == 0)
				{
					this.m_appenderList = null;
				}
				this.m_appenderArray = null;
			}
			return appender;
		}
		public IAppender RemoveAppender(string name)
		{
			return this.RemoveAppender(this.GetAppender(name));
		}
	}
}
