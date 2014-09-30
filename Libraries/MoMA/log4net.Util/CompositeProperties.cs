using System;
using System.Collections;
namespace log4net.Util
{
	public sealed class CompositeProperties
	{
		private PropertiesDictionary m_flattened = null;
		private ArrayList m_nestedProperties = new ArrayList();
		public object this[string key]
		{
			get
			{
				object result;
				if (this.m_flattened != null)
				{
					result = this.m_flattened[key];
				}
				else
				{
					foreach (ReadOnlyPropertiesDictionary readOnlyPropertiesDictionary in this.m_nestedProperties)
					{
						if (readOnlyPropertiesDictionary.Contains(key))
						{
							result = readOnlyPropertiesDictionary[key];
							return result;
						}
					}
					result = null;
				}
				return result;
			}
		}
		internal CompositeProperties()
		{
		}
		public void Add(ReadOnlyPropertiesDictionary properties)
		{
			this.m_flattened = null;
			this.m_nestedProperties.Add(properties);
		}
		public PropertiesDictionary Flatten()
		{
			if (this.m_flattened == null)
			{
				this.m_flattened = new PropertiesDictionary();
				int num = this.m_nestedProperties.Count;
				while (--num >= 0)
				{
					ReadOnlyPropertiesDictionary readOnlyPropertiesDictionary = (ReadOnlyPropertiesDictionary)this.m_nestedProperties[num];
					foreach (DictionaryEntry dictionaryEntry in (IEnumerable)readOnlyPropertiesDictionary)
					{
						this.m_flattened[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
					}
				}
			}
			return this.m_flattened;
		}
	}
}
