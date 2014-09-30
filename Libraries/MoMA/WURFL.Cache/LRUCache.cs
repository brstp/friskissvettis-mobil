using System;
using System.Collections.Generic;
using System.Threading;
namespace WURFL.Cache
{
	public class LRUCache<TK, TV> : ICache<TK, TV>
	{
		private class CacheEntry
		{
			internal readonly TK CacheKey;
			internal readonly TV CacheValue;
			internal CacheEntry(TK k, TV v)
			{
				this.CacheKey = k;
				this.CacheValue = v;
			}
		}
		private const int MaxSize = 3000;
		private readonly LinkedList<LRUCache<TK, TV>.CacheEntry> cacheEntries;
		private readonly IDictionary<TK, LinkedListNode<LRUCache<TK, TV>.CacheEntry>> keyToCacheEntry;
		private readonly int maxSize;
		private readonly object mutex;
		public int Size
		{
			get
			{
				object obj;
				Monitor.Enter(obj = this.mutex);
				int count;
				try
				{
					count = this.cacheEntries.Count;
				}
				finally
				{
					Monitor.Exit(obj);
				}
				return count;
			}
		}
		public LRUCache() : this(3000)
		{
		}
		public LRUCache(int maxSize)
		{
			this.maxSize = maxSize;
			this.cacheEntries = new LinkedList<LRUCache<TK, TV>.CacheEntry>();
			this.keyToCacheEntry = new Dictionary<TK, LinkedListNode<LRUCache<TK, TV>.CacheEntry>>();
			this.mutex = this;
		}
		public TV Get(TK key)
		{
			object obj;
			Monitor.Enter(obj = this.mutex);
			TV result;
			try
			{
				LinkedListNode<LRUCache<TK, TV>.CacheEntry> linkedListNode = null;
				if (!this.keyToCacheEntry.TryGetValue(key, out linkedListNode))
				{
					result = default(TV);
				}
				else
				{
					this.cacheEntries.Remove(linkedListNode);
					this.cacheEntries.AddFirst(linkedListNode);
					result = linkedListNode.Value.CacheValue;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public void Put(TK key, TV value)
		{
			object obj;
			Monitor.Enter(obj = this.mutex);
			try
			{
				if (object.Equals(this.Get(key), default(TV)))
				{
					LinkedListNode<LRUCache<TK, TV>.CacheEntry> linkedListNode = new LinkedListNode<LRUCache<TK, TV>.CacheEntry>(new LRUCache<TK, TV>.CacheEntry(key, value));
					this.cacheEntries.AddFirst(linkedListNode);
					this.keyToCacheEntry.Add(key, linkedListNode);
					if (this.cacheEntries.Count > this.maxSize)
					{
						this.keyToCacheEntry.Remove(this.cacheEntries.Last.Value.CacheKey);
						this.cacheEntries.RemoveLast();
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void Remove(TK key)
		{
			object obj;
			Monitor.Enter(obj = this.mutex);
			try
			{
				LinkedListNode<LRUCache<TK, TV>.CacheEntry> node = null;
				if (this.keyToCacheEntry.TryGetValue(key, out node))
				{
					this.cacheEntries.Remove(node);
					this.keyToCacheEntry.Remove(key);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void Clear()
		{
			object obj;
			Monitor.Enter(obj = this.mutex);
			try
			{
				this.keyToCacheEntry.Clear();
				this.cacheEntries.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
	}
}
