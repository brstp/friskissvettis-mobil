using log4net.Core;
using log4net.Plugin;
using log4net.Util;
using System;
namespace log4net.Config
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	[Serializable]
	public sealed class PluginAttribute : Attribute, IPluginFactory
	{
		private string m_typeName = null;
		private Type m_type = null;
		public Type Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}
		public string TypeName
		{
			get
			{
				return this.m_typeName;
			}
			set
			{
				this.m_typeName = value;
			}
		}
		public PluginAttribute(string typeName)
		{
			this.m_typeName = typeName;
		}
		public PluginAttribute(Type type)
		{
			this.m_type = type;
		}
		public IPlugin CreatePlugin()
		{
			Type type = this.m_type;
			if (this.m_type == null)
			{
				type = SystemInfo.GetTypeFromString(this.m_typeName, true, true);
			}
			if (!typeof(IPlugin).IsAssignableFrom(type))
			{
				throw new LogException("Plugin type [" + type.FullName + "] does not implement the log4net.IPlugin interface");
			}
			return (IPlugin)Activator.CreateInstance(type);
		}
		public override string ToString()
		{
			string result;
			if (this.m_type != null)
			{
				result = "PluginAttribute[Type=" + this.m_type.FullName + "]";
			}
			else
			{
				result = "PluginAttribute[Type=" + this.m_typeName + "]";
			}
			return result;
		}
	}
}
