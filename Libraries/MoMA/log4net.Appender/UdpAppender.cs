using log4net.Core;
using log4net.Util;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace log4net.Appender
{
	public class UdpAppender : AppenderSkeleton
	{
		private IPAddress m_remoteAddress;
		private int m_remotePort;
		private IPEndPoint m_remoteEndPoint;
		private int m_localPort;
		private UdpClient m_client;
		private Encoding m_encoding = Encoding.Default;
		public IPAddress RemoteAddress
		{
			get
			{
				return this.m_remoteAddress;
			}
			set
			{
				this.m_remoteAddress = value;
			}
		}
		public int RemotePort
		{
			get
			{
				return this.m_remotePort;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw SystemInfo.CreateArgumentOutOfRangeException("value", value, string.Concat(new string[]
					{
						"The value specified is less than ",
						0.ToString(NumberFormatInfo.InvariantInfo),
						" or greater than ",
						65535.ToString(NumberFormatInfo.InvariantInfo),
						"."
					}));
				}
				this.m_remotePort = value;
			}
		}
		public int LocalPort
		{
			get
			{
				return this.m_localPort;
			}
			set
			{
				if (value != 0 && (value < 0 || value > 65535))
				{
					throw SystemInfo.CreateArgumentOutOfRangeException("value", value, string.Concat(new string[]
					{
						"The value specified is less than ",
						0.ToString(NumberFormatInfo.InvariantInfo),
						" or greater than ",
						65535.ToString(NumberFormatInfo.InvariantInfo),
						"."
					}));
				}
				this.m_localPort = value;
			}
		}
		public Encoding Encoding
		{
			get
			{
				return this.m_encoding;
			}
			set
			{
				this.m_encoding = value;
			}
		}
		protected UdpClient Client
		{
			get
			{
				return this.m_client;
			}
			set
			{
				this.m_client = value;
			}
		}
		protected IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_remoteEndPoint;
			}
			set
			{
				this.m_remoteEndPoint = value;
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
			if (this.RemoteAddress == null)
			{
				throw new ArgumentNullException("The required property 'Address' was not specified.");
			}
			if (this.RemotePort < 0 || this.RemotePort > 65535)
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("this.RemotePort", this.RemotePort, string.Concat(new string[]
				{
					"The RemotePort is less than ",
					0.ToString(NumberFormatInfo.InvariantInfo),
					" or greater than ",
					65535.ToString(NumberFormatInfo.InvariantInfo),
					"."
				}));
			}
			if (this.LocalPort != 0 && (this.LocalPort < 0 || this.LocalPort > 65535))
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("this.LocalPort", this.LocalPort, string.Concat(new string[]
				{
					"The LocalPort is less than ",
					0.ToString(NumberFormatInfo.InvariantInfo),
					" or greater than ",
					65535.ToString(NumberFormatInfo.InvariantInfo),
					"."
				}));
			}
			this.RemoteEndPoint = new IPEndPoint(this.RemoteAddress, this.RemotePort);
			this.InitializeClientConnection();
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			try
			{
				byte[] bytes = this.m_encoding.GetBytes(base.RenderLoggingEvent(loggingEvent).ToCharArray());
				this.Client.Send(bytes, bytes.Length, this.RemoteEndPoint);
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error(string.Concat(new object[]
				{
					"Unable to send logging event to remote host ",
					this.RemoteAddress.ToString(),
					" on port ",
					this.RemotePort,
					"."
				}), e, ErrorCode.WriteFailure);
			}
		}
		protected override void OnClose()
		{
			base.OnClose();
			if (this.Client != null)
			{
				this.Client.Close();
				this.Client = null;
			}
		}
		protected virtual void InitializeClientConnection()
		{
			try
			{
				if (this.LocalPort == 0)
				{
					this.Client = new UdpClient();
				}
				else
				{
					this.Client = new UdpClient(this.LocalPort);
				}
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error("Could not initialize the UdpClient connection on port " + this.LocalPort.ToString(NumberFormatInfo.InvariantInfo) + ".", e, ErrorCode.GenericFailure);
				this.Client = null;
			}
		}
	}
}