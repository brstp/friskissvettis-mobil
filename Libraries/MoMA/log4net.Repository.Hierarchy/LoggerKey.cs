using System;
namespace log4net.Repository.Hierarchy
{
	internal sealed class LoggerKey
	{
		private readonly string m_name;
		private readonly int m_hashCache;
		internal LoggerKey(string name)
		{
			this.m_name = string.Intern(name);
			this.m_hashCache = name.GetHashCode();
		}
		public override int GetHashCode()
		{
			return this.m_hashCache;
		}
		public override bool Equals(object obj)
		{
			bool result;
			if (this == obj)
			{
				result = true;
			}
			else
			{
				LoggerKey loggerKey = obj as LoggerKey;
				result = (loggerKey != null && this.m_name == loggerKey.m_name);
			}
			return result;
		}
	}
}
