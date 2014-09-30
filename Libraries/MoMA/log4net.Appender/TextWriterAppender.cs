using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;
using System.IO;
using System.Threading;
namespace log4net.Appender
{
	public class TextWriterAppender : AppenderSkeleton
	{
		private QuietTextWriter m_qtw;
		private bool m_immediateFlush = true;
		public bool ImmediateFlush
		{
			get
			{
				return this.m_immediateFlush;
			}
			set
			{
				this.m_immediateFlush = value;
			}
		}
		public virtual TextWriter Writer
		{
			get
			{
				return this.m_qtw;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					this.Reset();
					if (value != null)
					{
						this.m_qtw = new QuietTextWriter(value, this.ErrorHandler);
						this.WriteHeader();
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		public override IErrorHandler ErrorHandler
		{
			get
			{
				return base.ErrorHandler;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					if (value == null)
					{
						LogLog.Warn("TextWriterAppender: You have tried to set a null error-handler.");
					}
					else
					{
						base.ErrorHandler = value;
						if (this.m_qtw != null)
						{
							this.m_qtw.ErrorHandler = value;
						}
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		protected QuietTextWriter QuietWriter
		{
			get
			{
				return this.m_qtw;
			}
			set
			{
				this.m_qtw = value;
			}
		}
		public TextWriterAppender()
		{
		}
		[Obsolete("Instead use the default constructor and set the Layout & Writer properties")]
		public TextWriterAppender(ILayout layout, Stream os) : this(layout, new StreamWriter(os))
		{
		}
		[Obsolete("Instead use the default constructor and set the Layout & Writer properties")]
		public TextWriterAppender(ILayout layout, TextWriter writer)
		{
			this.Layout = layout;
			this.Writer = writer;
		}
		protected override bool PreAppendCheck()
		{
			bool result;
			if (!base.PreAppendCheck())
			{
				result = false;
			}
			else
			{
				if (this.m_qtw == null)
				{
					this.PrepareWriter();
					if (this.m_qtw == null)
					{
						this.ErrorHandler.Error("No output stream or file set for the appender named [" + base.Name + "].");
						result = false;
						return result;
					}
				}
				if (this.m_qtw.Closed)
				{
					this.ErrorHandler.Error("Output stream for appender named [" + base.Name + "] has been closed.");
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			base.RenderLoggingEvent(this.m_qtw, loggingEvent);
			if (this.m_immediateFlush)
			{
				this.m_qtw.Flush();
			}
		}
		protected override void Append(LoggingEvent[] loggingEvents)
		{
			for (int i = 0; i < loggingEvents.Length; i++)
			{
				LoggingEvent loggingEvent = loggingEvents[i];
				base.RenderLoggingEvent(this.m_qtw, loggingEvent);
			}
			if (this.m_immediateFlush)
			{
				this.m_qtw.Flush();
			}
		}
		protected override void OnClose()
		{
			Monitor.Enter(this);
			try
			{
				this.Reset();
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		protected virtual void WriteFooterAndCloseWriter()
		{
			this.WriteFooter();
			this.CloseWriter();
		}
		protected virtual void CloseWriter()
		{
			if (this.m_qtw != null)
			{
				try
				{
					this.m_qtw.Close();
				}
				catch (Exception e)
				{
					this.ErrorHandler.Error("Could not close writer [" + this.m_qtw + "]", e);
				}
			}
		}
		protected virtual void Reset()
		{
			this.WriteFooterAndCloseWriter();
			this.m_qtw = null;
		}
		protected virtual void WriteFooter()
		{
			if (this.Layout != null && this.m_qtw != null && !this.m_qtw.Closed)
			{
				string footer = this.Layout.Footer;
				if (footer != null)
				{
					this.m_qtw.Write(footer);
				}
			}
		}
		protected virtual void WriteHeader()
		{
			if (this.Layout != null && this.m_qtw != null && !this.m_qtw.Closed)
			{
				string header = this.Layout.Header;
				if (header != null)
				{
					this.m_qtw.Write(header);
				}
			}
		}
		protected virtual void PrepareWriter()
		{
		}
	}
}
