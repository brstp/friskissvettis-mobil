using log4net.Repository;
using System;
using System.Collections;
using System.Threading;
namespace log4net.Plugin
{
	public sealed class PluginMap
	{
		private readonly Hashtable m_mapName2Plugin = new Hashtable();
		private readonly ILoggerRepository m_repository;
		public IPlugin this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				Monitor.Enter(this);
				IPlugin result;
				try
				{
					result = (IPlugin)this.m_mapName2Plugin[name];
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public PluginCollection AllPlugins
		{
			get
			{
				Monitor.Enter(this);
				PluginCollection result;
				try
				{
					result = new PluginCollection(this.m_mapName2Plugin.Values);
				}
				finally
				{
					Monitor.Exit(this);
				}
				return result;
			}
		}
		public PluginMap(ILoggerRepository repository)
		{
			this.m_repository = repository;
		}
		public void Add(IPlugin plugin)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException("plugin");
			}
			IPlugin plugin2 = null;
			Monitor.Enter(this);
			try
			{
				plugin2 = (this.m_mapName2Plugin[plugin.Name] as IPlugin);
				this.m_mapName2Plugin[plugin.Name] = plugin;
			}
			finally
			{
				Monitor.Exit(this);
			}
			if (plugin2 != null)
			{
				plugin2.Shutdown();
			}
			plugin.Attach(this.m_repository);
		}
		public void Remove(IPlugin plugin)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException("plugin");
			}
			Monitor.Enter(this);
			try
			{
				this.m_mapName2Plugin.Remove(plugin.Name);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
	}
}
