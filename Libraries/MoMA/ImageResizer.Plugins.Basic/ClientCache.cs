using ImageResizer.Caching;
using ImageResizer.Configuration;
using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class ClientCache : IPlugin
	{
		private Config c;
		public IPlugin Install(Config c)
		{
			this.c = c;
			c.Plugins.add_plugin(this);
			c.Pipeline.PreHandleImage += new PreHandleImageEventHandler(this.Pipeline_PreHandleImage);
			return this;
		}
		private void Pipeline_PreHandleImage(IHttpModule sender, HttpContext context, IResponseArgs e)
		{
			int num = this.c.get("clientcache.minutes", -1);
			if (num > 0)
			{
				e.ResponseHeaders.Expires = DateTime.UtcNow.AddMinutes((double)num);
			}
			DateTime dateTime = e.GetModifiedDateUTC();
			if (dateTime != DateTime.MinValue)
			{
				e.ResponseHeaders.LastModified = dateTime;
			}
			if (context.Request.IsAuthenticated)
			{
				e.ResponseHeaders.CacheControl = HttpCacheability.Private;
				return;
			}
			e.ResponseHeaders.CacheControl = HttpCacheability.Public;
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			c.Pipeline.PreHandleImage -= new PreHandleImageEventHandler(this.Pipeline_PreHandleImage);
			return true;
		}
	}
}
