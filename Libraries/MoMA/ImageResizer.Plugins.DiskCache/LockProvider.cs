using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class LockProvider
	{
		protected Dictionary<string, object> locks = new Dictionary<string, object>(StringComparer.Ordinal);
		protected object createLock = new object();
		public bool TryExecute(string key, int timeoutMs, LockCallback success)
		{
			DateTime utcNow = DateTime.UtcNow;
			bool flag = true;
			object obj = null;
			while (true)
			{
				object obj2;
				Monitor.Enter(obj2 = this.createLock);
				try
				{
					if (!this.locks.TryGetValue(key, out obj))
					{
						obj = (this.locks[key] = new object());
					}
				}
				finally
				{
					Monitor.Exit(obj2);
				}
				if (!Monitor.TryEnter(obj, timeoutMs))
				{
					break;
				}
				try
				{
					object obj3;
					Monitor.Enter(obj3 = this.createLock);
					try
					{
						object obj4 = null;
						flag = this.locks.TryGetValue(key, out obj4);
						flag = (flag && obj4 == obj);
					}
					finally
					{
						Monitor.Exit(obj3);
					}
					if (flag)
					{
						success();
					}
					goto IL_AE;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				break;
				IL_AE:
				if (!flag && Math.Abs(DateTime.UtcNow.Subtract(utcNow).TotalMilliseconds) > (double)timeoutMs)
				{
					return false;
				}
				if (flag)
				{
					goto Block_5;
				}
			}
			return false;
			Block_5:
			object obj5;
			Monitor.Enter(obj5 = this.createLock);
			try
			{
				if (Monitor.TryEnter(obj))
				{
					try
					{
						object obj6 = null;
						if (this.locks.TryGetValue(key, out obj6) && obj6 == obj)
						{
							this.locks.Remove(key);
						}
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
			}
			finally
			{
				Monitor.Exit(obj5);
			}
			return true;
		}
	}
}
