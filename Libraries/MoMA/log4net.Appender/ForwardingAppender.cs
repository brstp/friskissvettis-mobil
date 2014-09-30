using log4net.Core;
using log4net.Util;
using System;
using System.Threading;
namespace log4net.Appender
{
	public class ForwardingAppender : AppenderSkeleton, IAppenderAttachable
	{
		private AppenderAttachedImpl m_appenderAttachedImpl;
		public virtual AppenderCollection Appenders
		{
			get
			{
				Monitor.Enter(this);
				AppenderCollection result;
				try
				{
					if (this.m_appenderAttachedImpl == null)
					{
						result = AppenderCollection.EmptyCollection;
					}
					else
					{
						result = this.m_appenderAttachedImpl.Appenders;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		protected override void OnClose()
		{
			Monitor.Enter(this);
			try
			{
				if (this.m_appenderAttachedImpl != null)
				{
					this.m_appenderAttachedImpl.RemoveAllAppenders();
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (this.m_appenderAttachedImpl != null)
			{
				this.m_appenderAttachedImpl.AppendLoopOnAppenders(loggingEvent);
			}
		}
		protected override void Append(LoggingEvent[] loggingEvents)
		{
			if (this.m_appenderAttachedImpl != null)
			{
				this.m_appenderAttachedImpl.AppendLoopOnAppenders(loggingEvents);
			}
		}
		public virtual void AddAppender(IAppender newAppender)
		{
			if (newAppender == null)
			{
				throw new ArgumentNullException("newAppender");
			}
			Monitor.Enter(this);
			try
			{
				if (this.m_appenderAttachedImpl == null)
				{
					this.m_appenderAttachedImpl = new AppenderAttachedImpl();
				}
				this.m_appenderAttachedImpl.AddAppender(newAppender);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public virtual IAppender GetAppender(string name)
		{
			Monitor.Enter(this);
			IAppender result;
			try
			{
				if (this.m_appenderAttachedImpl == null || name == null)
				{
					result = null;
				}
				else
				{
					result = this.m_appenderAttachedImpl.GetAppender(name);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public virtual void RemoveAllAppenders()
		{
			Monitor.Enter(this);
			try
			{
				if (this.m_appenderAttachedImpl != null)
				{
					this.m_appenderAttachedImpl.RemoveAllAppenders();
					this.m_appenderAttachedImpl = null;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public virtual IAppender RemoveAppender(IAppender appender)
		{
			Monitor.Enter(this);
			IAppender result;
			try
			{
				if (appender != null && this.m_appenderAttachedImpl != null)
				{
					result = this.m_appenderAttachedImpl.RemoveAppender(appender);
					return result;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			result = null;
			return result;
		}
		public virtual IAppender RemoveAppender(string name)
		{
			Monitor.Enter(this);
			IAppender result;
			try
			{
				if (name != null && this.m_appenderAttachedImpl != null)
				{
					result = this.m_appenderAttachedImpl.RemoveAppender(name);
					return result;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			result = null;
			return result;
		}
	}
}
