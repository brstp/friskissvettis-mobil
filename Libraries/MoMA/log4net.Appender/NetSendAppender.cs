using log4net.Core;
using log4net.Util;
using System;
using System.Runtime.InteropServices;
namespace log4net.Appender
{
	public class NetSendAppender : AppenderSkeleton
	{
		private string m_server;
		private string m_sender;
		private string m_recipient;
		private SecurityContext m_securityContext;
		public string Sender
		{
			get
			{
				return this.m_sender;
			}
			set
			{
				this.m_sender = value;
			}
		}
		public string Recipient
		{
			get
			{
				return this.m_recipient;
			}
			set
			{
				this.m_recipient = value;
			}
		}
		public string Server
		{
			get
			{
				return this.m_server;
			}
			set
			{
				this.m_server = value;
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
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			if (this.Recipient == null)
			{
				throw new ArgumentNullException("Recipient", "The required property 'Recipient' was not specified.");
			}
			if (this.m_securityContext == null)
			{
				this.m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			NativeError nativeError = null;
			string text = base.RenderLoggingEvent(loggingEvent);
			using (this.m_securityContext.Impersonate(this))
			{
				int num = NetSendAppender.NetMessageBufferSend(this.Server, this.Recipient, this.Sender, text, text.Length * Marshal.SystemDefaultCharSize);
				if (num != 0)
				{
					nativeError = NativeError.GetError(num);
				}
			}
			if (nativeError != null)
			{
				this.ErrorHandler.Error(string.Concat(new string[]
				{
					nativeError.ToString(),
					" (Params: Server=",
					this.Server,
					", Recipient=",
					this.Recipient,
					", Sender=",
					this.Sender,
					")"
				}));
			}
		}
		[DllImport("netapi32.dll", SetLastError = true)]
		protected static extern int NetMessageBufferSend([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.LPWStr)] string msgName, [MarshalAs(UnmanagedType.LPWStr)] string fromName, [MarshalAs(UnmanagedType.LPWStr)] string buffer, int bufferSize);
	}
}
