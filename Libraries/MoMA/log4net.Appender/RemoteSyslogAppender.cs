using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
namespace log4net.Appender
{
	public class RemoteSyslogAppender : UdpAppender
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
			private RemoteSyslogAppender.SyslogSeverity m_severity;
			public RemoteSyslogAppender.SyslogSeverity Severity
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
		private const int DefaultSyslogPort = 514;
		private RemoteSyslogAppender.SyslogFacility m_facility = RemoteSyslogAppender.SyslogFacility.User;
		private PatternLayout m_identity;
		private LevelMapping m_levelMapping = new LevelMapping();
		public PatternLayout Identity
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
		public RemoteSyslogAppender.SyslogFacility Facility
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
		public RemoteSyslogAppender()
		{
			base.RemotePort = 514;
			base.RemoteAddress = IPAddress.Parse("127.0.0.1");
			base.Encoding = Encoding.ASCII;
		}
		public void AddMapping(RemoteSyslogAppender.LevelSeverity mapping)
		{
			this.m_levelMapping.Add(mapping);
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			try
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				int value = RemoteSyslogAppender.GeneratePriority(this.m_facility, this.GetSeverity(loggingEvent.Level));
				stringWriter.Write('<');
				stringWriter.Write(value);
				stringWriter.Write('>');
				if (this.m_identity != null)
				{
					this.m_identity.Format(stringWriter, loggingEvent);
				}
				else
				{
					stringWriter.Write(loggingEvent.Domain);
				}
				stringWriter.Write(": ");
				base.RenderLoggingEvent(stringWriter, loggingEvent);
				string text = stringWriter.ToString();
				byte[] bytes = base.Encoding.GetBytes(text.ToCharArray());
				base.Client.Send(bytes, bytes.Length, base.RemoteEndPoint);
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error(string.Concat(new object[]
				{
					"Unable to send logging event to remote syslog ",
					base.RemoteAddress.ToString(),
					" on port ",
					base.RemotePort,
					"."
				}), e, ErrorCode.WriteFailure);
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			this.m_levelMapping.ActivateOptions();
		}
		protected virtual RemoteSyslogAppender.SyslogSeverity GetSeverity(Level level)
		{
			RemoteSyslogAppender.LevelSeverity levelSeverity = this.m_levelMapping.Lookup(level) as RemoteSyslogAppender.LevelSeverity;
			RemoteSyslogAppender.SyslogSeverity result;
			if (levelSeverity != null)
			{
				result = levelSeverity.Severity;
			}
			else
			{
				if (level >= Level.Alert)
				{
					result = RemoteSyslogAppender.SyslogSeverity.Alert;
				}
				else
				{
					if (level >= Level.Critical)
					{
						result = RemoteSyslogAppender.SyslogSeverity.Critical;
					}
					else
					{
						if (level >= Level.Error)
						{
							result = RemoteSyslogAppender.SyslogSeverity.Error;
						}
						else
						{
							if (level >= Level.Warn)
							{
								result = RemoteSyslogAppender.SyslogSeverity.Warning;
							}
							else
							{
								if (level >= Level.Notice)
								{
									result = RemoteSyslogAppender.SyslogSeverity.Notice;
								}
								else
								{
									if (level >= Level.Info)
									{
										result = RemoteSyslogAppender.SyslogSeverity.Informational;
									}
									else
									{
										result = RemoteSyslogAppender.SyslogSeverity.Debug;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public static int GeneratePriority(RemoteSyslogAppender.SyslogFacility facility, RemoteSyslogAppender.SyslogSeverity severity)
		{
			if (facility < RemoteSyslogAppender.SyslogFacility.Kernel || facility > RemoteSyslogAppender.SyslogFacility.Local7)
			{
				throw new ArgumentException("SyslogFacility out of range", "facility");
			}
			if (severity < RemoteSyslogAppender.SyslogSeverity.Emergency || severity > RemoteSyslogAppender.SyslogSeverity.Debug)
			{
				throw new ArgumentException("SyslogSeverity out of range", "severity");
			}
			return (int)(facility * RemoteSyslogAppender.SyslogFacility.Uucp + (int)severity);
		}
	}
}
