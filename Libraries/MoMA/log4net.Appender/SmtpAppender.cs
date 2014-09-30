using log4net.Core;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
namespace log4net.Appender
{
	public class SmtpAppender : BufferingAppenderSkeleton
	{
		public enum SmtpAuthentication
		{
			None,
			Basic,
			Ntlm
		}
		private string m_to;
		private string m_from;
		private string m_subject;
		private string m_smtpHost;
		private SmtpAppender.SmtpAuthentication m_authentication = SmtpAppender.SmtpAuthentication.None;
		private string m_username;
		private string m_password;
		private int m_port = 25;
		private MailPriority m_mailPriority = MailPriority.Normal;
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
		public string SmtpHost
		{
			get
			{
				return this.m_smtpHost;
			}
			set
			{
				this.m_smtpHost = value;
			}
		}
		[Obsolete("Use the BufferingAppenderSkeleton Fix methods")]
		public bool LocationInfo
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		public SmtpAppender.SmtpAuthentication Authentication
		{
			get
			{
				return this.m_authentication;
			}
			set
			{
				this.m_authentication = value;
			}
		}
		public string Username
		{
			get
			{
				return this.m_username;
			}
			set
			{
				this.m_username = value;
			}
		}
		public string Password
		{
			get
			{
				return this.m_password;
			}
			set
			{
				this.m_password = value;
			}
		}
		public int Port
		{
			get
			{
				return this.m_port;
			}
			set
			{
				this.m_port = value;
			}
		}
		public MailPriority Priority
		{
			get
			{
				return this.m_mailPriority;
			}
			set
			{
				this.m_mailPriority = value;
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
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				string text = this.Layout.Header;
				if (text != null)
				{
					stringWriter.Write(text);
				}
				for (int i = 0; i < events.Length; i++)
				{
					base.RenderLoggingEvent(stringWriter, events[i]);
				}
				text = this.Layout.Footer;
				if (text != null)
				{
					stringWriter.Write(text);
				}
				this.SendEmail(stringWriter.ToString());
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error("Error occurred while sending e-mail notification.", e);
			}
		}
		protected virtual void SendEmail(string messageBody)
		{
			SmtpClient smtpClient = new SmtpClient();
			if (this.m_smtpHost != null && this.m_smtpHost.Length > 0)
			{
				smtpClient.Host = this.m_smtpHost;
			}
			smtpClient.Port = this.m_port;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			if (this.m_authentication == SmtpAppender.SmtpAuthentication.Basic)
			{
				smtpClient.Credentials = new NetworkCredential(this.m_username, this.m_password);
			}
			else
			{
				if (this.m_authentication == SmtpAppender.SmtpAuthentication.Ntlm)
				{
					smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
				}
			}
			smtpClient.Send(new MailMessage
			{
				Body = messageBody,
				From = new MailAddress(this.m_from),
				To = 
				{
					this.m_to
				},
				Subject = this.m_subject,
				Priority = this.m_mailPriority
			});
		}
	}
}
