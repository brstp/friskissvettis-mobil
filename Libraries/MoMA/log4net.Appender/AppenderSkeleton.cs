using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Threading;
namespace log4net.Appender
{
	public abstract class AppenderSkeleton : IBulkAppender, IAppender, IOptionHandler
	{
		private const int c_renderBufferSize = 256;
		private const int c_renderBufferMaxCapacity = 1024;
		private ILayout m_layout;
		private string m_name;
		private Level m_threshold;
		private IErrorHandler m_errorHandler;
		private IFilter m_headFilter;
		private IFilter m_tailFilter;
		private bool m_closed = false;
		private bool m_recursiveGuard = false;
		private ReusableStringWriter m_renderWriter = null;
		public Level Threshold
		{
			get
			{
				return this.m_threshold;
			}
			set
			{
				this.m_threshold = value;
			}
		}
		public virtual IErrorHandler ErrorHandler
		{
			get
			{
				return this.m_errorHandler;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					if (value == null)
					{
						LogLog.Warn("AppenderSkeleton: You have tried to set a null error-handler.");
					}
					else
					{
						this.m_errorHandler = value;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		public virtual IFilter FilterHead
		{
			get
			{
				return this.m_headFilter;
			}
		}
		public virtual ILayout Layout
		{
			get
			{
				return this.m_layout;
			}
			set
			{
				this.m_layout = value;
			}
		}
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}
		protected virtual bool RequiresLayout
		{
			get
			{
				return false;
			}
		}
		protected AppenderSkeleton()
		{
			this.m_errorHandler = new OnlyOnceErrorHandler(base.GetType().Name);
		}
		~AppenderSkeleton()
		{
			if (!this.m_closed)
			{
				LogLog.Debug("AppenderSkeleton: Finalizing appender named [" + this.m_name + "].");
				this.Close();
			}
		}
		public virtual void ActivateOptions()
		{
		}
		public void Close()
		{
			Monitor.Enter(this);
			try
			{
				if (!this.m_closed)
				{
					this.OnClose();
					this.m_closed = true;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void DoAppend(LoggingEvent loggingEvent)
		{
			Monitor.Enter(this);
			try
			{
				if (this.m_closed)
				{
					this.ErrorHandler.Error("Attempted to append to closed appender named [" + this.m_name + "].");
				}
				else
				{
					if (!this.m_recursiveGuard)
					{
						try
						{
							this.m_recursiveGuard = true;
							if (this.FilterEvent(loggingEvent) && this.PreAppendCheck())
							{
								this.Append(loggingEvent);
							}
						}
						catch (Exception e)
						{
							this.ErrorHandler.Error("Failed in DoAppend", e);
						}
						finally
						{
							this.m_recursiveGuard = false;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void DoAppend(LoggingEvent[] loggingEvents)
		{
			Monitor.Enter(this);
			try
			{
				if (this.m_closed)
				{
					this.ErrorHandler.Error("Attempted to append to closed appender named [" + this.m_name + "].");
				}
				else
				{
					if (!this.m_recursiveGuard)
					{
						try
						{
							this.m_recursiveGuard = true;
							ArrayList arrayList = new ArrayList(loggingEvents.Length);
							for (int i = 0; i < loggingEvents.Length; i++)
							{
								LoggingEvent loggingEvent = loggingEvents[i];
								if (this.FilterEvent(loggingEvent))
								{
									arrayList.Add(loggingEvent);
								}
							}
							if (arrayList.Count > 0 && this.PreAppendCheck())
							{
								this.Append((LoggingEvent[])arrayList.ToArray(typeof(LoggingEvent)));
							}
						}
						catch (Exception e)
						{
							this.ErrorHandler.Error("Failed in Bulk DoAppend", e);
						}
						finally
						{
							this.m_recursiveGuard = false;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		protected virtual bool FilterEvent(LoggingEvent loggingEvent)
		{
			bool result;
			if (!this.IsAsSevereAsThreshold(loggingEvent.Level))
			{
				result = false;
			}
			else
			{
				IFilter filter = this.FilterHead;
				while (filter != null)
				{
					switch (filter.Decide(loggingEvent))
					{
					case FilterDecision.Deny:
						result = false;
						return result;
					case FilterDecision.Neutral:
						filter = filter.Next;
						break;
					case FilterDecision.Accept:
						filter = null;
						break;
					}
				}
				result = true;
			}
			return result;
		}
		public virtual void AddFilter(IFilter filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter param must not be null");
			}
			if (this.m_headFilter == null)
			{
				this.m_tailFilter = filter;
				this.m_headFilter = filter;
			}
			else
			{
				this.m_tailFilter.Next = filter;
				this.m_tailFilter = filter;
			}
		}
		public virtual void ClearFilters()
		{
			this.m_headFilter = (this.m_tailFilter = null);
		}
		protected virtual bool IsAsSevereAsThreshold(Level level)
		{
			return this.m_threshold == null || level >= this.m_threshold;
		}
		protected virtual void OnClose()
		{
		}
		protected abstract void Append(LoggingEvent loggingEvent);
		protected virtual void Append(LoggingEvent[] loggingEvents)
		{
			for (int i = 0; i < loggingEvents.Length; i++)
			{
				LoggingEvent loggingEvent = loggingEvents[i];
				this.Append(loggingEvent);
			}
		}
		protected virtual bool PreAppendCheck()
		{
			bool result;
			if (this.m_layout == null && this.RequiresLayout)
			{
				this.ErrorHandler.Error("AppenderSkeleton: No layout set for the appender named [" + this.m_name + "].");
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
		protected string RenderLoggingEvent(LoggingEvent loggingEvent)
		{
			if (this.m_renderWriter == null)
			{
				this.m_renderWriter = new ReusableStringWriter(CultureInfo.InvariantCulture);
			}
			this.m_renderWriter.Reset(1024, 256);
			this.RenderLoggingEvent(this.m_renderWriter, loggingEvent);
			return this.m_renderWriter.ToString();
		}
		protected void RenderLoggingEvent(TextWriter writer, LoggingEvent loggingEvent)
		{
			if (this.m_layout == null)
			{
				throw new InvalidOperationException("A layout must be set");
			}
			if (this.m_layout.IgnoresException)
			{
				string exceptionString = loggingEvent.GetExceptionString();
				if (exceptionString != null && exceptionString.Length > 0)
				{
					this.m_layout.Format(writer, loggingEvent);
					writer.WriteLine(exceptionString);
				}
				else
				{
					this.m_layout.Format(writer, loggingEvent);
				}
			}
			else
			{
				this.m_layout.Format(writer, loggingEvent);
			}
		}
	}
}
