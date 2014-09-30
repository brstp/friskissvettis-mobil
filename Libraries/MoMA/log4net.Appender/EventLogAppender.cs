using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;
using System.Diagnostics;
using System.Threading;
namespace log4net.Appender
{
	public class EventLogAppender : AppenderSkeleton
	{
		public class Level2EventLogEntryType : LevelMappingEntry
		{
			private EventLogEntryType m_entryType;
			public EventLogEntryType EventLogEntryType
			{
				get
				{
					return this.m_entryType;
				}
				set
				{
					this.m_entryType = value;
				}
			}
		}
		private string m_logName;
		private string m_applicationName;
		private string m_machineName;
		private LevelMapping m_levelMapping = new LevelMapping();
		private SecurityContext m_securityContext;
		public string LogName
		{
			get
			{
				return this.m_logName;
			}
			set
			{
				this.m_logName = value;
			}
		}
		public string ApplicationName
		{
			get
			{
				return this.m_applicationName;
			}
			set
			{
				this.m_applicationName = value;
			}
		}
		public string MachineName
		{
			get
			{
				return this.m_machineName;
			}
			set
			{
			}
		}
		public SecurityContext SecurityContext
		{
			get
			{
				return this.m_securityContext;
			}
			set
			{
				this.m_securityContext = value;
			}
		}
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		public EventLogAppender()
		{
			this.m_applicationName = Thread.GetDomain().FriendlyName;
			this.m_logName = "Application";
			this.m_machineName = ".";
		}
		[Obsolete("Instead use the default constructor and set the Layout property")]
		public EventLogAppender(ILayout layout) : this()
		{
			this.Layout = layout;
		}
		public void AddMapping(EventLogAppender.Level2EventLogEntryType mapping)
		{
			this.m_levelMapping.Add(mapping);
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			if (this.m_securityContext == null)
			{
				this.m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			bool flag = false;
			string text = null;
			using (this.SecurityContext.Impersonate(this))
			{
				flag = EventLog.SourceExists(this.m_applicationName);
				if (flag)
				{
					text = EventLog.LogNameFromSourceName(this.m_applicationName, this.m_machineName);
				}
			}
			if (flag && text != this.m_logName)
			{
				LogLog.Debug(string.Concat(new string[]
				{
					"EventLogAppender: Changing event source [",
					this.m_applicationName,
					"] from log [",
					text,
					"] to log [",
					this.m_logName,
					"]"
				}));
			}
			else
			{
				if (!flag)
				{
					LogLog.Debug(string.Concat(new string[]
					{
						"EventLogAppender: Creating event source Source [",
						this.m_applicationName,
						"] in log ",
						this.m_logName,
						"]"
					}));
				}
			}
			string text2 = null;
			using (this.SecurityContext.Impersonate(this))
			{
				if (flag && text != this.m_logName)
				{
					EventLog.DeleteEventSource(this.m_applicationName, this.m_machineName);
					EventLogAppender.CreateEventSource(this.m_applicationName, this.m_logName, this.m_machineName);
					text2 = EventLog.LogNameFromSourceName(this.m_applicationName, this.m_machineName);
				}
				else
				{
					if (!flag)
					{
						EventLogAppender.CreateEventSource(this.m_applicationName, this.m_logName, this.m_machineName);
						text2 = EventLog.LogNameFromSourceName(this.m_applicationName, this.m_machineName);
					}
				}
			}
			this.m_levelMapping.ActivateOptions();
			LogLog.Debug(string.Concat(new string[]
			{
				"EventLogAppender: Source [",
				this.m_applicationName,
				"] is registered to log [",
				text2,
				"]"
			}));
		}
		private static void CreateEventSource(string source, string logName, string machineName)
		{
			EventLog.CreateEventSource(new EventSourceCreationData(source, logName)
			{
				MachineName = machineName
			});
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			int eventID = 0;
			object obj = loggingEvent.LookupProperty("EventID");
			if (obj != null)
			{
				if (obj is int)
				{
					eventID = (int)obj;
				}
				else
				{
					string text = obj as string;
					if (text != null && text.Length > 0)
					{
						int num;
						if (SystemInfo.TryParse(text, out num))
						{
							eventID = num;
						}
						else
						{
							this.ErrorHandler.Error("Unable to parse event ID property [" + text + "].");
						}
					}
				}
			}
			try
			{
				string text2 = base.RenderLoggingEvent(loggingEvent);
				if (text2.Length > 32000)
				{
					text2 = text2.Substring(0, 32000);
				}
				EventLogEntryType entryType = this.GetEntryType(loggingEvent.Level);
				using (this.SecurityContext.Impersonate(this))
				{
					EventLog.WriteEntry(this.m_applicationName, text2, entryType, eventID);
				}
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error(string.Concat(new string[]
				{
					"Unable to write to event log [",
					this.m_logName,
					"] using source [",
					this.m_applicationName,
					"]"
				}), e);
			}
		}
		protected virtual EventLogEntryType GetEntryType(Level level)
		{
			EventLogAppender.Level2EventLogEntryType level2EventLogEntryType = this.m_levelMapping.Lookup(level) as EventLogAppender.Level2EventLogEntryType;
			EventLogEntryType result;
			if (level2EventLogEntryType != null)
			{
				result = level2EventLogEntryType.EventLogEntryType;
			}
			else
			{
				if (level >= Level.Error)
				{
					result = EventLogEntryType.Error;
				}
				else
				{
					if (level == Level.Warn)
					{
						result = EventLogEntryType.Warning;
					}
					else
					{
						result = EventLogEntryType.Information;
					}
				}
			}
			return result;
		}
	}
}
