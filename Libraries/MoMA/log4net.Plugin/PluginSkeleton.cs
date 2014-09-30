using log4net.Repository;
using System;
namespace log4net.Plugin
{
	public abstract class PluginSkeleton : IPlugin
	{
		private string m_name;
		private ILoggerRepository m_repository;
		public virtual string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}
		protected virtual ILoggerRepository LoggerRepository
		{
			get
			{
				return this.m_repository;
			}
			set
			{
				this.m_repository = value;
			}
		}
		protected PluginSkeleton(string name)
		{
			this.m_name = name;
		}
		public virtual void Attach(ILoggerRepository repository)
		{
			this.m_repository = repository;
		}
		public virtual void Shutdown()
		{
		}
	}
}
