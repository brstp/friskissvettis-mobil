using System;
using System.Collections;
namespace log4net.Util
{
	[Serializable]
	public sealed class EmptyDictionary : IDictionary, ICollection, IEnumerable
	{
		private static readonly EmptyDictionary s_instance = new EmptyDictionary();
		public static EmptyDictionary Instance
		{
			get
			{
				return EmptyDictionary.s_instance;
			}
		}
		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}
		public int Count
		{
			get
			{
				return 0;
			}
		}
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}
		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}
		public ICollection Keys
		{
			get
			{
				return EmptyCollection.Instance;
			}
		}
		public ICollection Values
		{
			get
			{
				return EmptyCollection.Instance;
			}
		}
		public object this[object key]
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}
		private EmptyDictionary()
		{
		}
		public void CopyTo(Array array, int index)
		{
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return NullEnumerator.Instance;
		}
		public void Add(object key, object value)
		{
			throw new InvalidOperationException();
		}
		public void Clear()
		{
			throw new InvalidOperationException();
		}
		public bool Contains(object key)
		{
			return false;
		}
		public IDictionaryEnumerator GetEnumerator()
		{
			return NullDictionaryEnumerator.Instance;
		}
		public void Remove(object key)
		{
			throw new InvalidOperationException();
		}
	}
}
