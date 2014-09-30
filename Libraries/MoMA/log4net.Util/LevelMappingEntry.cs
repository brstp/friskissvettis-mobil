using log4net.Core;
using System;
namespace log4net.Util
{
	public abstract class LevelMappingEntry : IOptionHandler
	{
		private Level m_level;
		public Level Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}
		public virtual void ActivateOptions()
		{
		}
	}
}
