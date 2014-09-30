using ImageResizer.Caching;
using ImageResizer.Configuration;
using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class NoCache : ICache, IPlugin
	{
		public IPlugin Install(Config c)
		{
			c.Plugins.add_plugin(this);
			return this;
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			return true;
		}
		public void Process(HttpContext context, IResponseArgs e)
		{
			context.RemapHandler(new NoCacheHandler(e));
		}
		public bool CanProcess(HttpContext current, IResponseArgs e)
		{
			return true;
		}
	}
}
