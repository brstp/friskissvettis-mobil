using log4net.Core;
using System;
using System.Collections;
using System.Threading;
namespace log4net.Appender
{
	public class RemotingAppender : BufferingAppenderSkeleton
	{
		public interface IRemoteLoggingSink
		{
			void LogEvents(LoggingEvent[] events);
		}
		private string m_sinkUrl;
		private RemotingAppender.IRemoteLoggingSink m_sinkObj;
		private int m_queuedCallbackCount = 0;
		private ManualResetEvent m_workQueueEmptyEvent = new ManualResetEvent(true);
		public string Sink
		{
			get
			{
				return this.m_sinkUrl;
			}
			set
			{
				this.m_sinkUrl = value;
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			IDictionary dictionary = new Hashtable();
			dictionary["typeFilterLevel"] = "Full";
			this.m_sinkObj = (RemotingAppender.IRemoteLoggingSink)Activator.GetObject(typeof(RemotingAppender.IRemoteLoggingSink), this.m_sinkUrl, dictionary);
		}
		protected override void SendBuffer(LoggingEvent[] events)
		{
			this.BeginAsyncSend();
			if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendBufferCallback), events))
			{
				this.EndAsyncSend();
				this.ErrorHandler.Error("RemotingAppender [" + base.Name + "] failed to ThreadPool.QueueUserWorkItem logging events in SendBuffer.");
			}
		}
		protected override void OnClose()
		{
			base.OnClose();
			if (!this.m_workQueueEmptyEvent.WaitOne(30000, false))
			{
				this.ErrorHandler.Error("RemotingAppender [" + base.Name + "] failed to send all queued events before close, in OnClose.");
			}
		}
		private void BeginAsyncSend()
		{
			this.m_workQueueEmptyEvent.Reset();
			Interlocked.Increment(ref this.m_queuedCallbackCount);
		}
		private void EndAsyncSend()
		{
			if (Interlocked.Decrement(ref this.m_queuedCallbackCount) <= 0)
			{
				this.m_workQueueEmptyEvent.Set();
			}
		}
		private void SendBufferCallback(object state)
		{
			try
			{
				LoggingEvent[] events = (LoggingEvent[])state;
				this.m_sinkObj.LogEvents(events);
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error("Failed in SendBufferCallback", e);
			}
			finally
			{
				this.EndAsyncSend();
			}
		}
	}
}
