using log4net.Repository;
using System;
using System.Reflection;
namespace log4net.Config
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public abstract class ConfiguratorAttribute : Attribute, IComparable
	{
		private int m_priority = 0;
		protected ConfiguratorAttribute(int priority)
		{
			this.m_priority = priority;
		}
		public abstract void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository);
		public int CompareTo(object obj)
		{
			int result;
			if (this == obj)
			{
				result = 0;
			}
			else
			{
				int num = -1;
				ConfiguratorAttribute configuratorAttribute = obj as ConfiguratorAttribute;
				if (configuratorAttribute != null)
				{
					num = configuratorAttribute.m_priority.CompareTo(this.m_priority);
					if (num == 0)
					{
						num = -1;
					}
				}
				result = num;
			}
			return result;
		}
	}
}
