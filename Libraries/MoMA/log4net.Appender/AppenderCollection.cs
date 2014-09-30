using log4net.Util;
using System;
using System.Collections;
namespace log4net.Appender
{
	public class AppenderCollection : IList, ICollection, IEnumerable, ICloneable
	{
		public interface IAppenderCollectionEnumerator
		{
			IAppender Current
			{
				get;
			}
			bool MoveNext();
			void Reset();
		}
		protected internal enum Tag
		{
			Default
		}
		private sealed class Enumerator : IEnumerator, AppenderCollection.IAppenderCollectionEnumerator
		{
			private readonly AppenderCollection m_collection;
			private int m_index;
			private int m_version;
			public IAppender Current
			{
				get
				{
					return this.m_collection[this.m_index];
				}
			}
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}
			internal Enumerator(AppenderCollection tc)
			{
				this.m_collection = tc;
				this.m_index = -1;
				this.m_version = tc.m_version;
			}
			public bool MoveNext()
			{
				if (this.m_version != this.m_collection.m_version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this.m_index++;
				return this.m_index < this.m_collection.Count;
			}
			public void Reset()
			{
				this.m_index = -1;
			}
		}
		private sealed class ReadOnlyAppenderCollection : AppenderCollection, ICollection, IEnumerable
		{
			private readonly AppenderCollection m_collection;
			public override int Count
			{
				get
				{
					return this.m_collection.Count;
				}
			}
			public override bool IsSynchronized
			{
				get
				{
					return this.m_collection.IsSynchronized;
				}
			}
			public override object SyncRoot
			{
				get
				{
					return this.m_collection.SyncRoot;
				}
			}
			public override IAppender this[int i]
			{
				get
				{
					return this.m_collection[i];
				}
				set
				{
					throw new NotSupportedException("This is a Read Only Collection and can not be modified");
				}
			}
			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}
			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}
			public override int Capacity
			{
				get
				{
					return this.m_collection.Capacity;
				}
				set
				{
					throw new NotSupportedException("This is a Read Only Collection and can not be modified");
				}
			}
			internal ReadOnlyAppenderCollection(AppenderCollection list) : base(AppenderCollection.Tag.Default)
			{
				this.m_collection = list;
			}
			public override void CopyTo(IAppender[] array)
			{
				this.m_collection.CopyTo(array);
			}
			public override void CopyTo(IAppender[] array, int start)
			{
				this.m_collection.CopyTo(array, start);
			}
			void ICollection.CopyTo(Array array, int start)
			{
				((ICollection)this.m_collection).CopyTo(array, start);
			}
			public override int Add(IAppender x)
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
			public override void Clear()
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
			public override bool Contains(IAppender x)
			{
				return this.m_collection.Contains(x);
			}
			public override int IndexOf(IAppender x)
			{
				return this.m_collection.IndexOf(x);
			}
			public override void Insert(int pos, IAppender x)
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
			public override void Remove(IAppender x)
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
			public override void RemoveAt(int pos)
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
			public override AppenderCollection.IAppenderCollectionEnumerator GetEnumerator()
			{
				return this.m_collection.GetEnumerator();
			}
			public override int AddRange(AppenderCollection x)
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
			public override int AddRange(IAppender[] x)
			{
				throw new NotSupportedException("This is a Read Only Collection and can not be modified");
			}
		}
		private const int DEFAULT_CAPACITY = 16;
		private IAppender[] m_array;
		private int m_count = 0;
		private int m_version = 0;
		public static readonly AppenderCollection EmptyCollection = AppenderCollection.ReadOnly(new AppenderCollection(0));
		public virtual int Count
		{
			get
			{
				return this.m_count;
			}
		}
		public virtual bool IsSynchronized
		{
			get
			{
				return this.m_array.IsSynchronized;
			}
		}
		public virtual object SyncRoot
		{
			get
			{
				return this.m_array.SyncRoot;
			}
		}
		public virtual IAppender this[int index]
		{
			get
			{
				this.ValidateIndex(index);
				return this.m_array[index];
			}
			set
			{
				this.ValidateIndex(index);
				this.m_version++;
				this.m_array[index] = value;
			}
		}
		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		public virtual int Capacity
		{
			get
			{
				return this.m_array.Length;
			}
			set
			{
				if (value < this.m_count)
				{
					value = this.m_count;
				}
				if (value != this.m_array.Length)
				{
					if (value > 0)
					{
						IAppender[] array = new IAppender[value];
						Array.Copy(this.m_array, 0, array, 0, this.m_count);
						this.m_array = array;
					}
					else
					{
						this.m_array = new IAppender[16];
					}
				}
			}
		}
		object IList.this[int i]
		{
			get
			{
				return this[i];
			}
			set
			{
				this[i] = (IAppender)value;
			}
		}
		public static AppenderCollection ReadOnly(AppenderCollection list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			return new AppenderCollection.ReadOnlyAppenderCollection(list);
		}
		public AppenderCollection()
		{
			this.m_array = new IAppender[16];
		}
		public AppenderCollection(int capacity)
		{
			this.m_array = new IAppender[capacity];
		}
		public AppenderCollection(AppenderCollection c)
		{
			this.m_array = new IAppender[c.Count];
			this.AddRange(c);
		}
		public AppenderCollection(IAppender[] a)
		{
			this.m_array = new IAppender[a.Length];
			this.AddRange(a);
		}
		public AppenderCollection(ICollection col)
		{
			this.m_array = new IAppender[col.Count];
			this.AddRange(col);
		}
		protected internal AppenderCollection(AppenderCollection.Tag tag)
		{
			this.m_array = null;
		}
		public virtual void CopyTo(IAppender[] array)
		{
			this.CopyTo(array, 0);
		}
		public virtual void CopyTo(IAppender[] array, int start)
		{
			if (this.m_count > array.GetUpperBound(0) + 1 - start)
			{
				throw new ArgumentException("Destination array was not long enough.");
			}
			Array.Copy(this.m_array, 0, array, start, this.m_count);
		}
		public virtual int Add(IAppender item)
		{
			if (this.m_count == this.m_array.Length)
			{
				this.EnsureCapacity(this.m_count + 1);
			}
			this.m_array[this.m_count] = item;
			this.m_version++;
			return this.m_count++;
		}
		public virtual void Clear()
		{
			this.m_version++;
			this.m_array = new IAppender[16];
			this.m_count = 0;
		}
		public virtual object Clone()
		{
			AppenderCollection appenderCollection = new AppenderCollection(this.m_count);
			Array.Copy(this.m_array, 0, appenderCollection.m_array, 0, this.m_count);
			appenderCollection.m_count = this.m_count;
			appenderCollection.m_version = this.m_version;
			return appenderCollection;
		}
		public virtual bool Contains(IAppender item)
		{
			bool result;
			for (int num = 0; num != this.m_count; num++)
			{
				if (this.m_array[num].Equals(item))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public virtual int IndexOf(IAppender item)
		{
			int result;
			for (int num = 0; num != this.m_count; num++)
			{
				if (this.m_array[num].Equals(item))
				{
					result = num;
					return result;
				}
			}
			result = -1;
			return result;
		}
		public virtual void Insert(int index, IAppender item)
		{
			this.ValidateIndex(index, true);
			if (this.m_count == this.m_array.Length)
			{
				this.EnsureCapacity(this.m_count + 1);
			}
			if (index < this.m_count)
			{
				Array.Copy(this.m_array, index, this.m_array, index + 1, this.m_count - index);
			}
			this.m_array[index] = item;
			this.m_count++;
			this.m_version++;
		}
		public virtual void Remove(IAppender item)
		{
			int num = this.IndexOf(item);
			if (num < 0)
			{
				throw new ArgumentException("Cannot remove the specified item because it was not found in the specified Collection.");
			}
			this.m_version++;
			this.RemoveAt(num);
		}
		public virtual void RemoveAt(int index)
		{
			this.ValidateIndex(index);
			this.m_count--;
			if (index < this.m_count)
			{
				Array.Copy(this.m_array, index + 1, this.m_array, index, this.m_count - index);
			}
			IAppender[] sourceArray = new IAppender[1];
			Array.Copy(sourceArray, 0, this.m_array, this.m_count, 1);
			this.m_version++;
		}
		public virtual AppenderCollection.IAppenderCollectionEnumerator GetEnumerator()
		{
			return new AppenderCollection.Enumerator(this);
		}
		public virtual int AddRange(AppenderCollection x)
		{
			if (this.m_count + x.Count >= this.m_array.Length)
			{
				this.EnsureCapacity(this.m_count + x.Count);
			}
			Array.Copy(x.m_array, 0, this.m_array, this.m_count, x.Count);
			this.m_count += x.Count;
			this.m_version++;
			return this.m_count;
		}
		public virtual int AddRange(IAppender[] x)
		{
			if (this.m_count + x.Length >= this.m_array.Length)
			{
				this.EnsureCapacity(this.m_count + x.Length);
			}
			Array.Copy(x, 0, this.m_array, this.m_count, x.Length);
			this.m_count += x.Length;
			this.m_version++;
			return this.m_count;
		}
		public virtual int AddRange(ICollection col)
		{
			if (this.m_count + col.Count >= this.m_array.Length)
			{
				this.EnsureCapacity(this.m_count + col.Count);
			}
			foreach (object current in col)
			{
				this.Add((IAppender)current);
			}
			return this.m_count;
		}
		public virtual void TrimToSize()
		{
			this.Capacity = this.m_count;
		}
		public virtual IAppender[] ToArray()
		{
			IAppender[] array = new IAppender[this.m_count];
			if (this.m_count > 0)
			{
				Array.Copy(this.m_array, 0, array, 0, this.m_count);
			}
			return array;
		}
		private void ValidateIndex(int i)
		{
			this.ValidateIndex(i, false);
		}
		private void ValidateIndex(int i, bool allowEqualEnd)
		{
			int num = allowEqualEnd ? this.m_count : (this.m_count - 1);
			if (i < 0 || i > num)
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("i", i, "Index was out of range. Must be non-negative and less than the size of the collection. [" + i + "] Specified argument was out of the range of valid values.");
			}
		}
		private void EnsureCapacity(int min)
		{
			int num = (this.m_array.Length == 0) ? 16 : (this.m_array.Length * 2);
			if (num < min)
			{
				num = min;
			}
			this.Capacity = num;
		}
		void ICollection.CopyTo(Array array, int start)
		{
			if (this.m_count > 0)
			{
				Array.Copy(this.m_array, 0, array, start, this.m_count);
			}
		}
		int IList.Add(object x)
		{
			return this.Add((IAppender)x);
		}
		bool IList.Contains(object x)
		{
			return this.Contains((IAppender)x);
		}
		int IList.IndexOf(object x)
		{
			return this.IndexOf((IAppender)x);
		}
		void IList.Insert(int pos, object x)
		{
			this.Insert(pos, (IAppender)x);
		}
		void IList.Remove(object x)
		{
			this.Remove((IAppender)x);
		}
		void IList.RemoveAt(int pos)
		{
			this.RemoveAt(pos);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)this.GetEnumerator();
		}
	}
}
