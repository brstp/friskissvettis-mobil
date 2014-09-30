using log4net.Util;
using System;
using System.Collections;
using System.Threading;
namespace log4net.Core
{
	public sealed class LevelMap
	{
		private Hashtable m_mapName2Level = SystemInfo.CreateCaseInsensitiveHashtable();
		public Level this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				Monitor.Enter(this);
				Level result;
				try
				{
					result = (Level)this.m_mapName2Level[name];
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public LevelCollection AllLevels
		{
			get
			{
				Monitor.Enter(this);
				LevelCollection result;
				try
				{
					result = new LevelCollection(this.m_mapName2Level.Values);
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public void Clear()
		{
			this.m_mapName2Level.Clear();
		}
		public void Add(string name, int value)
		{
			this.Add(name, value, null);
		}
		public void Add(string name, int value, string displayName)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("name", name, "Parameter: name, Value: [" + name + "] out of range. Level name must not be empty");
			}
			if (displayName == null || displayName.Length == 0)
			{
				displayName = name;
			}
			this.Add(new Level(value, name, displayName));
		}
		public void Add(Level level)
		{
			if (level == null)
			{
				throw new ArgumentNullException("level");
			}
			Monitor.Enter(this);
			try
			{
				this.m_mapName2Level[level.Name] = level;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public Level LookupWithDefault(Level defaultLevel)
		{
			if (defaultLevel == null)
			{
				throw new ArgumentNullException("defaultLevel");
			}
			Monitor.Enter(this);
			Level result;
			try
			{
				Level level = (Level)this.m_mapName2Level[defaultLevel.Name];
				if (level == null)
				{
					this.m_mapName2Level[defaultLevel.Name] = defaultLevel;
					result = defaultLevel;
				}
				else
				{
					result = level;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			return result;
		}
	}
}
