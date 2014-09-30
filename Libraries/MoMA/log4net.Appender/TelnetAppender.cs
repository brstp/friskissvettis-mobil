using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace log4net.Appender
{
	public class TelnetAppender : AppenderSkeleton
	{
		protected class SocketHandler : IDisposable
		{
			protected class SocketClient : IDisposable
			{
				private Socket m_socket;
				private StreamWriter m_writer;
				public SocketClient(Socket socket)
				{
					this.m_socket = socket;
					try
					{
						this.m_writer = new StreamWriter(new NetworkStream(socket));
					}
					catch
					{
						this.Dispose();
						throw;
					}
				}
				public void Send(string message)
				{
					this.m_writer.Write(message);
					this.m_writer.Flush();
				}
				public void Dispose()
				{
					try
					{
						if (this.m_writer != null)
						{
							this.m_writer.Close();
							this.m_writer = null;
						}
					}
					catch
					{
					}
					if (this.m_socket != null)
					{
						try
						{
							this.m_socket.Shutdown(SocketShutdown.Both);
						}
						catch
						{
						}
						try
						{
							this.m_socket.Close();
						}
						catch
						{
						}
						this.m_socket = null;
					}
				}
			}
			private const int MAX_CONNECTIONS = 20;
			private Socket m_serverSocket;
			private ArrayList m_clients = new ArrayList();
			public bool HasConnections
			{
				get
				{
					ArrayList clients = this.m_clients;
					return clients != null && clients.Count > 0;
				}
			}
			public SocketHandler(int port)
			{
				this.m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.m_serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
				this.m_serverSocket.Listen(5);
				this.m_serverSocket.BeginAccept(new AsyncCallback(this.OnConnect), null);
			}
			public void Send(string message)
			{
				ArrayList clients = this.m_clients;
				foreach (TelnetAppender.SocketHandler.SocketClient socketClient in clients)
				{
					try
					{
						socketClient.Send(message);
					}
					catch (Exception)
					{
						socketClient.Dispose();
						this.RemoveClient(socketClient);
					}
				}
			}
			private void AddClient(TelnetAppender.SocketHandler.SocketClient client)
			{
				Monitor.Enter(this);
				try
				{
					ArrayList arrayList = (ArrayList)this.m_clients.Clone();
					arrayList.Add(client);
					this.m_clients = arrayList;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			private void RemoveClient(TelnetAppender.SocketHandler.SocketClient client)
			{
				Monitor.Enter(this);
				try
				{
					ArrayList arrayList = (ArrayList)this.m_clients.Clone();
					arrayList.Remove(client);
					this.m_clients = arrayList;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			private void OnConnect(IAsyncResult asyncResult)
			{
				try
				{
					Socket socket = this.m_serverSocket.EndAccept(asyncResult);
					LogLog.Debug("TelnetAppender: Accepting connection from [" + socket.RemoteEndPoint.ToString() + "]");
					TelnetAppender.SocketHandler.SocketClient socketClient = new TelnetAppender.SocketHandler.SocketClient(socket);
					int count = this.m_clients.Count;
					if (count < 20)
					{
						try
						{
							socketClient.Send("TelnetAppender v1.0 (" + (count + 1) + " active connections)\r\n\r\n");
							this.AddClient(socketClient);
						}
						catch
						{
							socketClient.Dispose();
						}
					}
					else
					{
						socketClient.Send("Sorry - Too many connections.\r\n");
						socketClient.Dispose();
					}
				}
				catch
				{
				}
				finally
				{
					if (this.m_serverSocket != null)
					{
						this.m_serverSocket.BeginAccept(new AsyncCallback(this.OnConnect), null);
					}
				}
			}
			public void Dispose()
			{
				ArrayList clients = this.m_clients;
				foreach (TelnetAppender.SocketHandler.SocketClient socketClient in clients)
				{
					socketClient.Dispose();
				}
				this.m_clients.Clear();
				Socket serverSocket = this.m_serverSocket;
				this.m_serverSocket = null;
				try
				{
					serverSocket.Shutdown(SocketShutdown.Both);
				}
				catch
				{
				}
				try
				{
					serverSocket.Close();
				}
				catch
				{
				}
			}
		}
		private TelnetAppender.SocketHandler m_handler;
		private int m_listeningPort = 23;
		public int Port
		{
			get
			{
				return this.m_listeningPort;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw SystemInfo.CreateArgumentOutOfRangeException("value", value, string.Concat(new string[]
					{
						"The value specified for Port is less than ",
						0.ToString(NumberFormatInfo.InvariantInfo),
						" or greater than ",
						65535.ToString(NumberFormatInfo.InvariantInfo),
						"."
					}));
				}
				this.m_listeningPort = value;
			}
		}
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}
		protected override void OnClose()
		{
			base.OnClose();
			if (this.m_handler != null)
			{
				this.m_handler.Dispose();
				this.m_handler = null;
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			try
			{
				LogLog.Debug("TelnetAppender: Creating SocketHandler to listen on port [" + this.m_listeningPort + "]");
				this.m_handler = new TelnetAppender.SocketHandler(this.m_listeningPort);
			}
			catch (Exception exception)
			{
				LogLog.Error("TelnetAppender: Failed to create SocketHandler", exception);
				throw;
			}
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (this.m_handler != null && this.m_handler.HasConnections)
			{
				this.m_handler.Send(base.RenderLoggingEvent(loggingEvent));
			}
		}
	}
}
