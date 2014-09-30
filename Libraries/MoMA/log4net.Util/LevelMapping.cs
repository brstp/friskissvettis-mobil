using log4net.Core;
using System;
using System.Collections;
namespace log4net.Util
{
	public sealed class LevelMapping : IOptionHandler
	{
		private Hashtable m_entriesMap = new Hashtable();
		private LevelMappingEntry[] m_entries = null;
		public void Add(LevelMappingEntry entry)
		{
			if (this.m_entriesMap.ContainsKey(entry.Level))
			{
				this.m_entriesMap.Remove(entry.Level);
			}
			this.m_entriesMap.Add(entry.Level, entry);
		}
		public LevelMappingEntry Lookup(Level level)
		{
			LevelMappingEntry result;
			if (this.m_entries != null)
			{
				LevelMappingEntry[] entries = this.m_entries;
				for (int i = 0; i < entries.Length; i++)
				{
					LevelMappingEntry levelMappingEntry = entries[i];
					if (level >= levelMappingEntry.Level)
					{
						result = levelMappingEntry;
						return result;
					}
				}
			}
			result = null;
			return result;
		}
		public void ActivateOptions()
		{
			Level[] array = new Level[this.m_entriesMap.Count];
			LevelMappingEntry[] array2 = new LevelMappingEntry[this.m_entriesMap.Count];
			this.m_entriesMap.Keys.CopyTo(array, 0);
			this.m_entriesMap.Values.CopyTo(array2, 0);
			Array.Sort<Level, LevelMappingEntry>(array, array2, 0, array.Length, null);
			Array.Reverse(array2, 0, array2.Length);
			LevelMappingEntry[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				LevelMappingEntry levelMappingEntry = array3[i];
				levelMappingEntry.ActivateOptions();
			}
			this.m_entries = array2;
		}
	}
}
