using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Threading;
namespace log4net.Appender
{
	public abstract class BufferingAppenderSkeleton : AppenderSkeleton
	{
		private const int DEFAULT_BUFFER_SIZE = 512;
		private int m_bufferSize = 512;
		private CyclicBuffer m_cb;
		private ITriggeringEventEvaluator m_evaluator;
		private bool m_lossy = false;
		private ITriggeringEventEvaluator m_lossyEvaluator;
		private FixFlags m_fixFlags = FixFlags.All;
		private readonly bool m_eventMustBeFixed;
		public bool Lossy
		{
			get
			{
				return this.m_lossy;
			}
			set
			{
				this.m_lossy = value;
			}
		}
		public int BufferSize
		{
			get
			{
				return this.m_bufferSize;
			}
			set
			{
				this.m_bufferSize = value;
			}
		}
		public ITriggeringEventEvaluator Evaluator
		{
			get
			{
				return this.m_evaluator;
			}
			set
			{
				this.m_evaluator = value;
			}
		}
		public ITriggeringEventEvaluator LossyEvaluator
		{
			get
			{
				return this.m_lossyEvaluator;
			}
			set
			{
				this.m_lossyEvaluator = value;
			}
		}
		[Obsolete("Use Fix property")]
		public virtual bool OnlyFixPartialEventData
		{
			get
			{
				return this.Fix == FixFlags.Partial;
			}
			set
			{
				if (value)
				{
					this.Fix = FixFlags.Partial;
				}
				else
				{
					this.Fix = FixFlags.All;
				}
			}
		}
		public virtual FixFlags Fix
		{
			get
			{
				return this.m_fixFlags;
			}
			set
			{
				this.m_fixFlags = value;
			}
		}
		protected BufferingAppenderSkeleton() : this(true)
		{
		}
		protected BufferingAppenderSkeleton(bool eventMustBeFixed)
		{
			this.m_eventMustBeFixed = eventMustBeFixed;
		}
		public virtual void Flush()
		{
			this.Flush(false);
		}
		public virtual void Flush(bool flushLossyBuffer)
		{
			Monitor.Enter(this);
			try
			{
				if (this.m_cb != null && this.m_cb.Length > 0)
				{
					if (this.m_lossy)
					{
						if (flushLossyBuffer)
						{
							if (this.m_lossyEvaluator != null)
							{
								LoggingEvent[] array = this.m_cb.PopAll();
								ArrayList arrayList = new ArrayList(array.Length);
								LoggingEvent[] array2 = array;
								for (int i = 0; i < array2.Length; i++)
								{
									LoggingEvent loggingEvent = array2[i];
									if (this.m_lossyEvaluator.IsTriggeringEvent(loggingEvent))
									{
										arrayList.Add(loggingEvent);
									}
								}
								if (arrayList.Count > 0)
								{
									this.SendBuffer((LoggingEvent[])arrayList.ToArray(typeof(LoggingEvent)));
								}
							}
							else
							{
								this.m_cb.Clear();
							}
						}
					}
					else
					{
						this.SendFromBuffer(null, this.m_cb);
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			if (this.m_lossy && this.m_evaluator == null)
			{
				this.ErrorHandler.Error("Appender [" + base.Name + "] is Lossy but has no Evaluator. The buffer will never be sent!");
			}
			if (this.m_bufferSize > 1)
			{
				this.m_cb = new CyclicBuffer(this.m_bufferSize);
			}
			else
			{
				this.m_cb = null;
			}
		}
		protected override void OnClose()
		{
			this.Flush(true);
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (this.m_cb == null || this.m_bufferSize <= 1)
			{
				if (!this.m_lossy || (this.m_evaluator != null && this.m_evaluator.IsTriggeringEvent(loggingEvent)) || (this.m_lossyEvaluator != null && this.m_lossyEvaluator.IsTriggeringEvent(loggingEvent)))
				{
					if (this.m_eventMustBeFixed)
					{
						loggingEvent.Fix = this.Fix;
					}
					this.SendBuffer(new LoggingEvent[]
					{
						loggingEvent
					});
				}
			}
			else
			{
				loggingEvent.Fix = this.Fix;
				LoggingEvent loggingEvent2 = this.m_cb.Append(loggingEvent);
				if (loggingEvent2 != null)
				{
					if (!this.m_lossy)
					{
						this.SendFromBuffer(loggingEvent2, this.m_cb);
					}
					else
					{
						if (this.m_lossyEvaluator == null || !this.m_lossyEvaluator.IsTriggeringEvent(loggingEvent2))
						{
							loggingEvent2 = null;
						}
						if (this.m_evaluator != null && this.m_evaluator.IsTriggeringEvent(loggingEvent))
						{
							this.SendFromBuffer(loggingEvent2, this.m_cb);
						}
						else
						{
							if (loggingEvent2 != null)
							{
								this.SendBuffer(new LoggingEvent[]
								{
									loggingEvent2
								});
							}
						}
					}
				}
				else
				{
					if (this.m_evaluator != null && this.m_evaluator.IsTriggeringEvent(loggingEvent))
					{
						this.SendFromBuffer(null, this.m_cb);
					}
				}
			}
		}
		protected virtual void SendFromBuffer(LoggingEvent firstLoggingEvent, CyclicBuffer buffer)
		{
			LoggingEvent[] array = buffer.PopAll();
			if (firstLoggingEvent == null)
			{
				this.SendBuffer(array);
			}
			else
			{
				if (array.Length == 0)
				{
					this.SendBuffer(new LoggingEvent[]
					{
						firstLoggingEvent
					});
				}
				else
				{
					LoggingEvent[] array2 = new LoggingEvent[array.Length + 1];
					Array.Copy(array, 0, array2, 1, array.Length);
					array2[0] = firstLoggingEvent;
					this.SendBuffer(array2);
				}
			}
		}
		protected abstract void SendBuffer(LoggingEvent[] events);
	}
}
