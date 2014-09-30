using System;
using System.Threading;
namespace log4net.Util
{
	public sealed class ThreadContextProperties : ContextPropertiesBase
	{
		private static readonly LocalDataStoreSlot s_threadLocalSlot = Thread.AllocateDataSlot();
		public override object this[string key]
		{
			get
			{
				PropertiesDictionary properties = this.GetProperties(false);
				object result;
				if (properties != null)
				{
					result = properties[key];
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.GetProperties(true)[key] = value;
			}
		}
		internal ThreadContextProperties()
		{
		}
		public void Remove(string key)
		{
			PropertiesDictionary properties = this.GetProperties(false);
			if (properties != null)
			{
				properties.Remove(key);
			}
		}
		public void Clear()
		{
			PropertiesDictionary properties = this.GetProperties(false);
			if (properties != null)
			{
				properties.Clear();
			}
		}
		internal PropertiesDictionary GetProperties(bool create)
		{
			PropertiesDictionary propertiesDictionary = (PropertiesDictionary)Thread.GetData(ThreadContextProperties.s_threadLocalSlot);
			if (propertiesDictionary == null && create)
			{
				propertiesDictionary = new PropertiesDictionary();
				Thread.SetData(ThreadContextProperties.s_threadLocalSlot, propertiesDictionary);
			}
			return propertiesDictionary;
		}
	}
}
