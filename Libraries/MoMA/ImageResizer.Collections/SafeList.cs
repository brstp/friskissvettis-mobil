using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Collections
{
	[ComVisible(true)]
	public class SafeList<T> : IEnumerable<T>, IEnumerable
	{
		public delegate void ChangedHandler(SafeList<T> sender);
		public delegate IEnumerable<T> ListEditor(IList<T> items);
		protected volatile ReadOnlyCollection<T> items;
		protected object writeLock = new object();
		public event SafeList<T>.ChangedHandler Changed;
		public T First
		{
			get
			{
				ReadOnlyCollection<T> readOnlyCollection = this.items;
				if (readOnlyCollection.Count > 0)
				{
					return readOnlyCollection[0];
				}
				return default(T);
			}
		}
		public T Last
		{
			get
			{
				ReadOnlyCollection<T> readOnlyCollection = this.items;
				if (readOnlyCollection.Count > 0)
				{
					return readOnlyCollection[readOnlyCollection.Count - 1];
				}
				return default(T);
			}
		}
		public IEnumerable<T> Reversed
		{
			get
			{
				return new ReverseEnumerable<T>(this.items);
			}
		}
		public SafeList()
		{
			this.items = new ReadOnlyCollection<T>(new List<T>());
		}
		public SafeList(IEnumerable<T> items)
		{
			items = new ReadOnlyCollection<T>(new List<T>(items));
		}
		protected void FireChanged()
		{
			if (this.Changed != null)
			{
				this.Changed(this);
			}
		}
		public ReadOnlyCollection<T> GetCollection()
		{
			return this.items;
		}
		public IList<T> GetList()
		{
			return new List<T>(this.items);
		}
		public void SetList(IEnumerable<T> list)
		{
			object obj;
			Monitor.Enter(obj = this.writeLock);
			try
			{
				this.items = new ReadOnlyCollection<T>(new List<T>(list));
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this.FireChanged();
		}
		public void Add(T item)
		{
			object obj;
			Monitor.Enter(obj = this.writeLock);
			try
			{
				IList<T> list = this.GetList();
				list.Add(item);
				this.items = new ReadOnlyCollection<T>(list);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this.FireChanged();
		}
		public bool Remove(T item)
		{
			object obj;
			Monitor.Enter(obj = this.writeLock);
			try
			{
				IList<T> list = this.GetList();
				if (!list.Remove(item))
				{
					return false;
				}
				this.items = new ReadOnlyCollection<T>(list);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this.FireChanged();
			return true;
		}
		public void AddFirst(T item)
		{
			object obj;
			Monitor.Enter(obj = this.writeLock);
			try
			{
				IList<T> list = this.GetList();
				list.Insert(0, item);
				this.items = new ReadOnlyCollection<T>(list);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this.FireChanged();
		}
		public void ModifyList(SafeList<T>.ListEditor callback)
		{
			object obj;
			Monitor.Enter(obj = this.writeLock);
			try
			{
				this.items = new ReadOnlyCollection<T>(new List<T>(callback(this.GetList())));
			}
			finally
			{
				Monitor.Exit(obj);
			}
			this.FireChanged();
		}
		public bool Contains(T item)
		{
			return this.items.Contains(item);
		}
		public IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.items).GetEnumerator();
		}
	}
}
