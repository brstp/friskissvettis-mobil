using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace ImageResizer.Collections
{
	[ComVisible(true)]
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private IDictionary<TKey, TValue> _dictionary;
		public ICollection<TKey> Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}
		public ICollection<TValue> Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}
		public TValue this[TKey key]
		{
			get
			{
				return this._dictionary[key];
			}
			set
			{
				throw new NotSupportedException("This dictionary is read-only");
			}
		}
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}
		public ReadOnlyDictionary()
		{
			this._dictionary = new Dictionary<TKey, TValue>();
		}
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			this._dictionary = dictionary;
		}
		public void Add(TKey key, TValue value)
		{
			throw new NotSupportedException("This dictionary is read-only");
		}
		public bool ContainsKey(TKey key)
		{
			return this._dictionary.ContainsKey(key);
		}
		public bool Remove(TKey key)
		{
			throw new NotSupportedException("This dictionary is read-only");
		}
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException("This dictionary is read-only");
		}
		public void Clear()
		{
			throw new NotSupportedException("This dictionary is read-only");
		}
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this._dictionary.Contains(item);
		}
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this._dictionary.CopyTo(array, arrayIndex);
		}
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException("This dictionary is read-only");
		}
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}
	}
}
