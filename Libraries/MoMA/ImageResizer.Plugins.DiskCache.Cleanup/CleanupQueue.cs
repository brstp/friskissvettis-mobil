using ImageResizer.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Plugins.DiskCache.Cleanup
{
	[ComVisible(true)]
	public class CleanupQueue
	{
		private LinkedList<CleanupWorkItem> queue;
		protected readonly object _sync = new object();
		public bool IsEmpty
		{
			get
			{
				object sync;
				Monitor.Enter(sync = this._sync);
				bool result;
				try
				{
					result = (this.queue.Count <= 0);
				}
				finally
				{
					Monitor.Exit(sync);
				}
				return result;
			}
		}
		public int Count
		{
			get
			{
				object sync;
				Monitor.Enter(sync = this._sync);
				int count;
				try
				{
					count = this.queue.Count;
				}
				finally
				{
					Monitor.Exit(sync);
				}
				return count;
			}
		}
		public CleanupQueue()
		{
			this.queue = new LinkedList<CleanupWorkItem>();
		}
		public void Queue(CleanupWorkItem item)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				this.queue.AddLast(item);
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public bool QueueIfUnique(CleanupWorkItem item)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			bool result;
			try
			{
				bool flag = !this.queue.Contains(item);
				if (flag)
				{
					this.queue.AddLast(item);
				}
				result = flag;
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public bool Exists(CleanupWorkItem item)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			bool result;
			try
			{
				result = this.queue.Contains(item);
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public void Insert(CleanupWorkItem item)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				this.queue.AddFirst(item);
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public void QueueRange(IEnumerable<CleanupWorkItem> items)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				foreach (CleanupWorkItem current in items)
				{
					this.queue.AddLast(current);
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public void InsertRange(IList<CleanupWorkItem> items)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				ReverseEnumerable<CleanupWorkItem> reverseEnumerable = new ReverseEnumerable<CleanupWorkItem>(new ReadOnlyCollection<CleanupWorkItem>(items));
				foreach (CleanupWorkItem current in reverseEnumerable)
				{
					this.queue.AddFirst(current);
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public CleanupWorkItem Pop()
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			CleanupWorkItem result;
			try
			{
				CleanupWorkItem cleanupWorkItem = (this.queue.Count > 0) ? this.queue.First.Value : null;
				if (cleanupWorkItem != null)
				{
					this.queue.RemoveFirst();
				}
				result = cleanupWorkItem;
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public void Clear()
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				this.queue.Clear();
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public void ReplaceWith(CleanupWorkItem item)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				this.queue.Clear();
				this.queue.AddLast(item);
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
	}
}
