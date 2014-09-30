using log4net.Core;
using System;
using System.Threading;
namespace log4net.Util
{
	public class CyclicBuffer
	{
		private LoggingEvent[] m_events;
		private int m_first;
		private int m_last;
		private int m_numElems;
		private int m_maxSize;
		public LoggingEvent this[int i]
		{
			get
			{
				Monitor.Enter(this);
				LoggingEvent result;
				try
				{
					if (i < 0 || i >= this.m_numElems)
					{
						result = null;
					}
					else
					{
						result = this.m_events[(this.m_first + i) % this.m_maxSize];
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public int MaxSize
		{
			get
			{
				Monitor.Enter(this);
				int maxSize;
				try
				{
					maxSize = this.m_maxSize;
				}
				finally
				{
					Monitor.Exit(this);
				}
				return maxSize;
			}
		}
		public int Length
		{
			get
			{
				Monitor.Enter(this);
				int numElems;
				try
				{
					numElems = this.m_numElems;
				}
				finally
				{
					Monitor.Exit(this);
				}
				return numElems;
			}
		}
		public CyclicBuffer(int maxSize)
		{
			if (maxSize < 1)
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("maxSize", maxSize, "Parameter: maxSize, Value: [" + maxSize + "] out of range. Non zero positive integer required");
			}
			this.m_maxSize = maxSize;
			this.m_events = new LoggingEvent[maxSize];
			this.m_first = 0;
			this.m_last = 0;
			this.m_numElems = 0;
		}
		public LoggingEvent Append(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			Monitor.Enter(this);
			LoggingEvent result;
			try
			{
				LoggingEvent loggingEvent2 = this.m_events[this.m_last];
				this.m_events[this.m_last] = loggingEvent;
				if (++this.m_last == this.m_maxSize)
				{
					this.m_last = 0;
				}
				if (this.m_numElems < this.m_maxSize)
				{
					this.m_numElems++;
				}
				else
				{
					if (++this.m_first == this.m_maxSize)
					{
						this.m_first = 0;
					}
				}
				if (this.m_numElems < this.m_maxSize)
				{
					result = null;
				}
				else
				{
					result = loggingEvent2;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public LoggingEvent PopOldest()
		{
			Monitor.Enter(this);
			LoggingEvent result;
			try
			{
				LoggingEvent loggingEvent = null;
				if (this.m_numElems > 0)
				{
					this.m_numElems--;
					loggingEvent = this.m_events[this.m_first];
					this.m_events[this.m_first] = null;
					if (++this.m_first == this.m_maxSize)
					{
						this.m_first = 0;
					}
				}
				result = loggingEvent;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public LoggingEvent[] PopAll()
		{
			Monitor.Enter(this);
			LoggingEvent[] result;
			try
			{
				LoggingEvent[] array = new LoggingEvent[this.m_numElems];
				if (this.m_numElems > 0)
				{
					if (this.m_first < this.m_last)
					{
						Array.Copy(this.m_events, this.m_first, array, 0, this.m_numElems);
					}
					else
					{
						Array.Copy(this.m_events, this.m_first, array, 0, this.m_maxSize - this.m_first);
						Array.Copy(this.m_events, 0, array, this.m_maxSize - this.m_first, this.m_last);
					}
				}
				this.Clear();
				result = array;
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
		public void Clear()
		{
			Monitor.Enter(this);
			try
			{
				Array.Clear(this.m_events, 0, this.m_events.Length);
				this.m_first = 0;
				this.m_last = 0;
				this.m_numElems = 0;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
	}
}
