using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using log4net.Util;
using System;
using System.Runtime.Remoting;
namespace log4net.Plugin
{
	public class RemoteLoggingServerPlugin : PluginSkeleton
	{
		private class RemoteLoggingSinkImpl : MarshalByRefObject, RemotingAppender.IRemoteLoggingSink
		{
			private readonly ILoggerRepository m_repository;
			public RemoteLoggingSinkImpl(ILoggerRepository repository)
			{
				this.m_repository = repository;
			}
			public void LogEvents(LoggingEvent[] events)
			{
				if (events != null)
				{
					for (int i = 0; i < events.Length; i++)
					{
						LoggingEvent loggingEvent = events[i];
						if (loggingEvent != null)
						{
							this.m_repository.Log(loggingEvent);
						}
					}
				}
			}
			public override object InitializeLifetimeService()
			{
				return null;
			}
		}
		private RemoteLoggingServerPlugin.RemoteLoggingSinkImpl m_sink;
		private string m_sinkUri;
		public virtual string SinkUri
		{
			get
			{
				return this.m_sinkUri;
			}
			set
			{
				this.m_sinkUri = value;
			}
		}
		public RemoteLoggingServerPlugin() : base("RemoteLoggingServerPlugin:Unset URI")
		{
		}
		public RemoteLoggingServerPlugin(string sinkUri) : base("RemoteLoggingServerPlugin:" + sinkUri)
		{
			this.m_sinkUri = sinkUri;
		}
		public override void Attach(ILoggerRepository repository)
		{
			base.Attach(repository);
			this.m_sink = new RemoteLoggingServerPlugin.RemoteLoggingSinkImpl(repository);
			try
			{
				RemotingServices.Marshal(this.m_sink, this.m_sinkUri, typeof(RemotingAppender.IRemoteLoggingSink));
			}
			catch (Exception exception)
			{
				LogLog.Error("RemoteLoggingServerPlugin: Failed to Marshal remoting sink", exception);
			}
		}
		public override void Shutdown()
		{
			RemotingServices.Disconnect(this.m_sink);
			this.m_sink = null;
			base.Shutdown();
		}
	}
}
