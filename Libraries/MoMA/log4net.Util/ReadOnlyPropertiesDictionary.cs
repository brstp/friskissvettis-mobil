using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
namespace log4net.Util
{
	[Serializable]
	public class ReadOnlyPropertiesDictionary : ISerializable, IDictionary, ICollection, IEnumerable
	{
		private Hashtable m_hashtable = new Hashtable();
		public virtual object this[string key]
		{
			get
			{
				return this.InnerHashtable[key];
			}
			set
			{
				throw new NotSupportedException("This is a Read Only Dictionary and can not be modified");
			}
		}
		protected Hashtable InnerHashtable
		{
			get
			{
				return this.m_hashtable;
			}
		}
		bool IDictionary.IsReadOnly
		{
			get
			{
				return true;
			}
		}
		object IDictionary.this[object key]
		{
			get
			{
				if (!(key is string))
				{
					throw new ArgumentException("key must be a string");
				}
				return this.InnerHashtable[key];
			}
			set
			{
				throw new NotSupportedException("This is a Read Only Dictionary and can not be modified");
			}
		}
		ICollection IDictionary.Values
		{
			get
			{
				return this.InnerHashtable.Values;
			}
		}
		ICollection IDictionary.Keys
		{
			get
			{
				return this.InnerHashtable.Keys;
			}
		}
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.InnerHashtable.IsFixedSize;
			}
		}
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.InnerHashtable.IsSynchronized;
			}
		}
		public int Count
		{
			get
			{
				return this.InnerHashtable.Count;
			}
		}
		object ICollection.SyncRoot
		{
			get
			{
				return this.InnerHashtable.SyncRoot;
			}
		}
		public ReadOnlyPropertiesDictionary()
		{
		}
		public ReadOnlyPropertiesDictionary(ReadOnlyPropertiesDictionary propertiesDictionary)
		{
			foreach (DictionaryEntry dictionaryEntry in (IEnumerable)propertiesDictionary)
			{
				this.InnerHashtable.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
		}
		protected ReadOnlyPropertiesDictionary(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry current = enumerator.Current;
				this.InnerHashtable[XmlConvert.DecodeName(current.Name)] = current.Value;
			}
		}
		public string[] GetKeys()
		{
			string[] array = new string[this.InnerHashtable.Count];
			this.InnerHashtable.Keys.CopyTo(array, 0);
			return array;
		}
		public bool Contains(string key)
		{
			return this.InnerHashtable.Contains(key);
		}
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (DictionaryEntry dictionaryEntry in this.InnerHashtable)
			{
				string text = dictionaryEntry.Key as string;
				object value = dictionaryEntry.Value;
				if (text != null && value != null && value.GetType().IsSerializable)
				{
					info.AddValue(XmlConvert.EncodeLocalName(text), value);
				}
			}
		}
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return this.InnerHashtable.GetEnumerator();
		}
		void IDictionary.Remove(object key)
		{
			throw new NotSupportedException("This is a Read Only Dictionary and can not be modified");
		}
		bool IDictionary.Contains(object key)
		{
			return this.InnerHashtable.Contains(key);
		}
		public virtual void Clear()
		{
			throw new NotSupportedException("This is a Read Only Dictionary and can not be modified");
		}
		void IDictionary.Add(object key, object value)
		{
			throw new NotSupportedException("This is a Read Only Dictionary and can not be modified");
		}
		void ICollection.CopyTo(Array array, int index)
		{
			this.InnerHashtable.CopyTo(array, index);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.InnerHashtable).GetEnumerator();
		}
	}
}
