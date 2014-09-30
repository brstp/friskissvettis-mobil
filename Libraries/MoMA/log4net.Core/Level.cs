using System;
namespace log4net.Core
{
	[Serializable]
	public sealed class Level : IComparable
	{
		public static readonly Level Off = new Level(2147483647, "OFF");
		public static readonly Level Emergency = new Level(120000, "EMERGENCY");
		public static readonly Level Fatal = new Level(110000, "FATAL");
		public static readonly Level Alert = new Level(100000, "ALERT");
		public static readonly Level Critical = new Level(90000, "CRITICAL");
		public static readonly Level Severe = new Level(80000, "SEVERE");
		public static readonly Level Error = new Level(70000, "ERROR");
		public static readonly Level Warn = new Level(60000, "WARN");
		public static readonly Level Notice = new Level(50000, "NOTICE");
		public static readonly Level Info = new Level(40000, "INFO");
		public static readonly Level Debug = new Level(30000, "DEBUG");
		public static readonly Level Fine = new Level(30000, "FINE");
		public static readonly Level Trace = new Level(20000, "TRACE");
		public static readonly Level Finer = new Level(20000, "FINER");
		public static readonly Level Verbose = new Level(10000, "VERBOSE");
		public static readonly Level Finest = new Level(10000, "FINEST");
		public static readonly Level All = new Level(-2147483648, "ALL");
		private readonly int m_levelValue;
		private readonly string m_levelName;
		private readonly string m_levelDisplayName;
		public string Name
		{
			get
			{
				return this.m_levelName;
			}
		}
		public int Value
		{
			get
			{
				return this.m_levelValue;
			}
		}
		public string DisplayName
		{
			get
			{
				return this.m_levelDisplayName;
			}
		}
		public Level(int level, string levelName, string displayName)
		{
			if (levelName == null)
			{
				throw new ArgumentNullException("levelName");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			this.m_levelValue = level;
			this.m_levelName = string.Intern(levelName);
			this.m_levelDisplayName = displayName;
		}
		public Level(int level, string levelName) : this(level, levelName, levelName)
		{
		}
		public override string ToString()
		{
			return this.m_levelName;
		}
		public override bool Equals(object o)
		{
			Level level = o as Level;
			bool result;
			if (level != null)
			{
				result = (this.m_levelValue == level.m_levelValue);
			}
			else
			{
				result = base.Equals(o);
			}
			return result;
		}
		public override int GetHashCode()
		{
			return this.m_levelValue;
		}
		public int CompareTo(object r)
		{
			Level level = r as Level;
			if (level != null)
			{
				return Level.Compare(this, level);
			}
			throw new ArgumentException("Parameter: r, Value: [" + r + "] is not an instance of Level");
		}
		public static bool operator >(Level l, Level r)
		{
			return l.m_levelValue > r.m_levelValue;
		}
		public static bool operator <(Level l, Level r)
		{
			return l.m_levelValue < r.m_levelValue;
		}
		public static bool operator >=(Level l, Level r)
		{
			return l.m_levelValue >= r.m_levelValue;
		}
		public static bool operator <=(Level l, Level r)
		{
			return l.m_levelValue <= r.m_levelValue;
		}
		public static bool operator ==(Level l, Level r)
		{
			bool result;
			if (l != null && r != null)
			{
				result = (l.m_levelValue == r.m_levelValue);
			}
			else
			{
				result = (l == r);
			}
			return result;
		}
		public static bool operator !=(Level l, Level r)
		{
			return !(l == r);
		}
		public static int Compare(Level l, Level r)
		{
			int result;
			if (l == r)
			{
				result = 0;
			}
			else
			{
				if (l == null && r == null)
				{
					result = 0;
				}
				else
				{
					if (l == null)
					{
						result = -1;
					}
					else
					{
						if (r == null)
						{
							result = 1;
						}
						else
						{
							result = l.m_levelValue - r.m_levelValue;
						}
					}
				}
			}
			return result;
		}
	}
}
