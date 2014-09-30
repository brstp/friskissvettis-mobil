using System;
using System.Threading;
namespace log4net.Util
{
	public sealed class GlobalContextProperties : ContextPropertiesBase
	{
		private volatile ReadOnlyPropertiesDictionary m_readOnlyProperties = new ReadOnlyPropertiesDictionary();
		private readonly object m_syncRoot = new object();
		public override object this[string key]
		{
			get
			{
				return this.m_readOnlyProperties[key];
			}
			set
			{
				object syncRoot;
				Monitor.Enter(syncRoot = this.m_syncRoot);
				try
				{
					PropertiesDictionary propertiesDictionary = new PropertiesDictionary(this.m_readOnlyProperties);
					propertiesDictionary[key] = value;
					this.m_readOnlyProperties = new ReadOnlyPropertiesDictionary(propertiesDictionary);
				}
				finally
				{
					Monitor.Exit(syncRoot);
				}
			}
		}
		internal GlobalContextProperties()
		{
		}
		public void Remove(string key)
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this.m_syncRoot);
			try
			{
				if (this.m_readOnlyProperties.Contains(key))
				{
					PropertiesDictionary propertiesDictionary = new PropertiesDictionary(this.m_readOnlyProperties);
					propertiesDictionary.Remove(key);
					this.m_readOnlyProperties = new ReadOnlyPropertiesDictionary(propertiesDictionary);
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}
		public void Clear()
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this.m_syncRoot);
			try
			{
				this.m_readOnlyProperties = new ReadOnlyPropertiesDictionary();
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}
		internal ReadOnlyPropertiesDictionary GetReadOnlyProperties()
		{
			return this.m_readOnlyProperties;
		}
	}
}
