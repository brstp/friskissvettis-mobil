using log4net.Core;
using log4net.Util;
using System;
using System.IO;
namespace log4net.Appender
{
	public class SmtpPickupDirAppender : BufferingAppenderSkeleton
	{
		private string m_to;
		private string m_from;
		private string m_subject;
		private string m_pickupDir;
		private SecurityContext m_securityContext;
		public string To
		{
			get
			{
				return this.m_to;
			}
			set
			{
				this.m_to = value;
			}
		}
		public string From
		{
			get
			{
				return this.m_from;
			}
			set
			{
				this.m_from = value;
			}
		}
		public string Subject
		{
			get
			{
				return this.m_subject;
			}
			set
			{
				this.m_subject = value;
			}
		}
		public string PickupDir
		{
			get
			{
				return this.m_pickupDir;
			}
			set
			{
				this.m_pickupDir = value;
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
		protected override void SendBuffer(LoggingEvent[] events)
		{
			try
			{
				string text = null;
				StreamWriter streamWriter = null;
				using (this.SecurityContext.Impersonate(this))
				{
					text = Path.Combine(this.m_pickupDir, SystemInfo.NewGuid().ToString("N"));
					streamWriter = File.CreateText(text);
				}
				if (streamWriter == null)
				{
					this.ErrorHandler.Error("Failed to create output file for writing [" + text + "]", null, ErrorCode.FileOpenFailure);
				}
				else
				{
					using (streamWriter)
					{
						streamWriter.WriteLine("To: " + this.m_to);
						streamWriter.WriteLine("From: " + this.m_from);
						streamWriter.WriteLine("Subject: " + this.m_subject);
						streamWriter.WriteLine("");
						string text2 = this.Layout.Header;
						if (text2 != null)
						{
							streamWriter.Write(text2);
						}
						for (int i = 0; i < events.Length; i++)
						{
							base.RenderLoggingEvent(streamWriter, events[i]);
						}
						text2 = this.Layout.Footer;
						if (text2 != null)
						{
							streamWriter.Write(text2);
						}
						streamWriter.WriteLine("");
						streamWriter.WriteLine(".");
					}
				}
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error("Error occurred while sending e-mail notification.", e);
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			if (this.m_securityContext == null)
			{
				this.m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			using (this.SecurityContext.Impersonate(this))
			{
				this.m_pickupDir = SmtpPickupDirAppender.ConvertToFullPath(this.m_pickupDir.Trim());
			}
		}
		protected static string ConvertToFullPath(string path)
		{
			return SystemInfo.ConvertToFullPath(path);
		}
	}
}
