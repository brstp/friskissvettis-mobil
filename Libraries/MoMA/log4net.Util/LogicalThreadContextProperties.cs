using System;
using System.Runtime.Remoting.Messaging;
namespace log4net.Util
{
	public sealed class LogicalThreadContextProperties : ContextPropertiesBase
	{
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
		internal LogicalThreadContextProperties()
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
			PropertiesDictionary propertiesDictionary = (PropertiesDictionary)CallContext.GetData("log4net.Util.LogicalThreadContextProperties");
			if (propertiesDictionary == null && create)
			{
				propertiesDictionary = new PropertiesDictionary();
				CallContext.SetData("log4net.Util.LogicalThreadContextProperties", propertiesDictionary);
			}
			return propertiesDictionary;
		}
	}
}
