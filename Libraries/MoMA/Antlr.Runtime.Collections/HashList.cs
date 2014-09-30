using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Antlr.Runtime.Collections
{
	public sealed class HashList : IDictionary, ICollection, IEnumerable
	{
		private sealed class HashListEnumerator : IDictionaryEnumerator, IEnumerator
		{
			internal enum EnumerationMode
			{
				Key,
				Value,
				Entry
			}
			private HashList _hashList;
			private List<object> _orderList;
			private HashList.HashListEnumerator.EnumerationMode _mode;
			private int _index;
			private int _version;
			private object _key;
			private object _value;
			public object Key
			{
				get
				{
					if (this._key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._key;
				}
			}
			public object Value
			{
				get
				{
					if (this._key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._value;
				}
			}
			public DictionaryEntry Entry
			{
				get
				{
					if (this._key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return new DictionaryEntry(this._key, this._value);
				}
			}
			public object Current
			{
				get
				{
					if (this._key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					if (this._mode == HashList.HashListEnumerator.EnumerationMode.Key)
					{
						return this._key;
					}
					if (this._mode == HashList.HashListEnumerator.EnumerationMode.Value)
					{
						return this._value;
					}
					return new DictionaryEntry(this._key, this._value);
				}
			}
			internal HashListEnumerator()
			{
				this._index = 0;
				this._key = null;
				this._value = null;
			}
			internal HashListEnumerator(HashList hashList, HashList.HashListEnumerator.EnumerationMode mode)
			{
				this._hashList = hashList;
				this._mode = mode;
				this._version = hashList._version;
				this._orderList = hashList._insertionOrderList;
				this._index = 0;
				this._key = null;
				this._value = null;
			}
			public void Reset()
			{
				if (this._version != this._hashList._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._index = 0;
				this._key = null;
				this._value = null;
			}
			public bool MoveNext()
			{
				if (this._version != this._hashList._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (this._index < this._orderList.Count)
				{
					this._key = this._orderList[this._index];
					this._value = this._hashList[this._key];
					this._index++;
					return true;
				}
				this._key = null;
				return false;
			}
		}
		private sealed class KeyCollection : ICollection, IEnumerable
		{
			private HashList _hashList;
			public bool IsSynchronized
			{
				get
				{
					return this._hashList.IsSynchronized;
				}
			}
			public int Count
			{
				get
				{
					return this._hashList.Count;
				}
			}
			public object SyncRoot
			{
				get
				{
					return this._hashList.SyncRoot;
				}
			}
			internal KeyCollection()
			{
			}
			internal KeyCollection(HashList hashList)
			{
				this._hashList = hashList;
			}
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				List<object> insertionOrderList = this._hashList._insertionOrderList;
				for (int i = 0; i < insertionOrderList.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(insertionOrderList[i]);
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
			public override bool Equals(object o)
			{
				if (o is HashList.KeyCollection)
				{
					HashList.KeyCollection keyCollection = (HashList.KeyCollection)o;
					if (this.Count == 0 && keyCollection.Count == 0)
					{
						return true;
					}
					if (this.Count == keyCollection.Count)
					{
						for (int i = 0; i < this.Count; i++)
						{
							if (this._hashList._insertionOrderList[i] == keyCollection._hashList._insertionOrderList[i] || this._hashList._insertionOrderList[i].Equals(keyCollection._hashList._insertionOrderList[i]))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			public override int GetHashCode()
			{
				return this._hashList._insertionOrderList.GetHashCode();
			}
			public void CopyTo(Array array, int index)
			{
				this._hashList.CopyKeysTo(array, index);
			}
			public IEnumerator GetEnumerator()
			{
				return new HashList.HashListEnumerator(this._hashList, HashList.HashListEnumerator.EnumerationMode.Key);
			}
		}
		private sealed class ValueCollection : ICollection, IEnumerable
		{
			private HashList _hashList;
			public bool IsSynchronized
			{
				get
				{
					return this._hashList.IsSynchronized;
				}
			}
			public int Count
			{
				get
				{
					return this._hashList.Count;
				}
			}
			public object SyncRoot
			{
				get
				{
					return this._hashList.SyncRoot;
				}
			}
			internal ValueCollection()
			{
			}
			internal ValueCollection(HashList hashList)
			{
				this._hashList = hashList;
			}
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				IEnumerator enumerator = new HashList.HashListEnumerator(this._hashList, HashList.HashListEnumerator.EnumerationMode.Value);
				if (enumerator.MoveNext())
				{
					stringBuilder.Append((enumerator.Current == null) ? "null" : enumerator.Current);
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(", ");
						stringBuilder.Append((enumerator.Current == null) ? "null" : enumerator.Current);
					}
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
			public void CopyTo(Array array, int index)
			{
				this._hashList.CopyValuesTo(array, index);
			}
			public IEnumerator GetEnumerator()
			{
				return new HashList.HashListEnumerator(this._hashList, HashList.HashListEnumerator.EnumerationMode.Value);
			}
		}
		private Hashtable _dictionary = new Hashtable();
		private List<object> _insertionOrderList = new List<object>();
		private int _version;
		public bool IsReadOnly
		{
			get
			{
				return this._dictionary.IsReadOnly;
			}
		}
		public object this[object key]
		{
			get
			{
				return this._dictionary[key];
			}
			set
			{
				bool flag = !this._dictionary.Contains(key);
				this._dictionary[key] = value;
				if (flag)
				{
					this._insertionOrderList.Add(key);
				}
				this._version++;
			}
		}
		public ICollection Values
		{
			get
			{
				return new HashList.ValueCollection(this);
			}
		}
		public ICollection Keys
		{
			get
			{
				return new HashList.KeyCollection(this);
			}
		}
		public bool IsFixedSize
		{
			get
			{
				return this._dictionary.IsFixedSize;
			}
		}
		public bool IsSynchronized
		{
			get
			{
				return this._dictionary.IsSynchronized;
			}
		}
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}
		public object SyncRoot
		{
			get
			{
				return this._dictionary.SyncRoot;
			}
		}
		public HashList() : this(-1)
		{
		}
		public HashList(int capacity)
		{
			if (capacity < 0)
			{
				this._dictionary = new Hashtable();
				this._insertionOrderList = new List<object>();
			}
			else
			{
				this._dictionary = new Hashtable(capacity);
				this._insertionOrderList = new List<object>(capacity);
			}
			this._version = 0;
		}
		public IDictionaryEnumerator GetEnumerator()
		{
			return new HashList.HashListEnumerator(this, HashList.HashListEnumerator.EnumerationMode.Entry);
		}
		public void Remove(object key)
		{
			this._dictionary.Remove(key);
			this._insertionOrderList.Remove(key);
			this._version++;
		}
		public bool Contains(object key)
		{
			return this._dictionary.Contains(key);
		}
		public void Clear()
		{
			this._dictionary.Clear();
			this._insertionOrderList.Clear();
			this._version++;
		}
		public void Add(object key, object value)
		{
			this._dictionary.Add(key, value);
			this._insertionOrderList.Add(key);
			this._version++;
		}
		public void CopyTo(Array array, int index)
		{
			int count = this._insertionOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				DictionaryEntry dictionaryEntry = new DictionaryEntry(this._insertionOrderList[i], this._dictionary[this._insertionOrderList[i]]);
				array.SetValue(dictionaryEntry, index++);
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashList.HashListEnumerator(this, HashList.HashListEnumerator.EnumerationMode.Entry);
		}
		private void CopyKeysTo(Array array, int index)
		{
			int count = this._insertionOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				array.SetValue(this._insertionOrderList[i], index++);
			}
		}
		private void CopyValuesTo(Array array, int index)
		{
			int count = this._insertionOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				array.SetValue(this._dictionary[this._insertionOrderList[i]], index++);
			}
		}
	}
}
