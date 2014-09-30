using log4net.Core;
using log4net.Util;
using System;
using System.Runtime.InteropServices;
namespace log4net.Appender
{
	public class LocalSyslogAppender : AppenderSkeleton
	{
		public enum SyslogSeverity
		{
			Emergency,
			Alert,
			Critical,
			Error,
			Warning,
			Notice,
			Informational,
			Debug
		}
		public enum SyslogFacility
		{
			Kernel,
			User,
			Mail,
			Daemons,
			Authorization,
			Syslog,
			Printer,
			News,
			Uucp,
			Clock,
			Authorization2,
			Ftp,
			Ntp,
			Audit,
			Alert,
			Clock2,
			Local0,
			Local1,
			Local2,
			Local3,
			Local4,
			Local5,
			Local6,
			Local7
		}
		public class LevelSeverity : LevelMappingEntry
		{
			private LocalSyslogAppender.SyslogSeverity m_severity;
			public LocalSyslogAppender.SyslogSeverity Severity
			{
				get
				{
					return this.m_severity;
				}
				set
				{
					this.m_severity = value;
				}
			}
		}
		private LocalSyslogAppender.SyslogFacility m_facility = LocalSyslogAppender.SyslogFacility.User;
		private string m_identity;
		private IntPtr m_handleToIdentity = IntPtr.Zero;
		private LevelMapping m_levelMapping = new LevelMapping();
		public string Identity
		{
			get
			{
				return this.m_identity;
			}
			set
			{
				this.m_identity = value;
			}
		}
		public LocalSyslogAppender.SyslogFacility Facility
		{
			get
			{
				return this.m_facility;
			}
			set
			{
				this.m_facility = value;
			}
		}
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		public void AddMapping(LocalSyslogAppender.LevelSeverity mapping)
		{
			this.m_levelMapping.Add(mapping);
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			this.m_levelMapping.ActivateOptions();
			string text = this.m_identity;
			if (text == null)
			{
				text = SystemInfo.ApplicationFriendlyName;
			}
			this.m_handleToIdentity = Marshal.StringToHGlobalAnsi(text);
			LocalSyslogAppender.openlog(this.m_handleToIdentity, 1, this.m_facility);
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			int priority = LocalSyslogAppender.GeneratePriority(this.m_facility, this.GetSeverity(loggingEvent.Level));
			string message = base.RenderLoggingEvent(loggingEvent);
			LocalSyslogAppender.syslog(priority, "%s", message);
		}
		protected override void OnClose()
		{
			base.OnClose();
			try
			{
				LocalSyslogAppender.closelog();
			}
			catch (DllNotFoundException)
			{
			}
			if (this.m_handleToIdentity != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_handleToIdentity);
			}
		}
		protected virtual LocalSyslogAppender.SyslogSeverity GetSeverity(Level level)
		{
			LocalSyslogAppender.LevelSeverity levelSeverity = this.m_levelMapping.Lookup(level) as LocalSyslogAppender.LevelSeverity;
			LocalSyslogAppender.SyslogSeverity result;
			if (levelSeverity != null)
			{
				result = levelSeverity.Severity;
			}
			else
			{
				if (level >= Level.Alert)
				{
					result = LocalSyslogAppender.SyslogSeverity.Alert;
				}
				else
				{
					if (level >= Level.Critical)
					{
						result = LocalSyslogAppender.SyslogSeverity.Critical;
					}
					else
					{
						if (level >= Level.Error)
						{
							result = LocalSyslogAppender.SyslogSeverity.Error;
						}
						else
						{
							if (level >= Level.Warn)
							{
								result = LocalSyslogAppender.SyslogSeverity.Warning;
							}
							else
							{
								if (level >= Level.Notice)
								{
									result = LocalSyslogAppender.SyslogSeverity.Notice;
								}
								else
								{
									if (level >= Level.Info)
									{
										result = LocalSyslogAppender.SyslogSeverity.Informational;
									}
									else
									{
										result = LocalSyslogAppender.SyslogSeverity.Debug;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		private static int GeneratePriority(LocalSyslogAppender.SyslogFacility facility, LocalSyslogAppender.SyslogSeverity severity)
		{
			return (int)(facility * LocalSyslogAppender.SyslogFacility.Uucp + (int)severity);
		}
		[DllImport("libc")]
		private static extern void openlog(IntPtr ident, int option, LocalSyslogAppender.SyslogFacility facility);
		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern void syslog(int priority, string format, string message);
		[DllImport("libc")]
		private static extern void closelog();
	}
}
