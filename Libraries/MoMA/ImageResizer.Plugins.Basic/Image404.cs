using ImageResizer.Configuration;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class Image404 : IQuerystringPlugin, IPlugin
	{
		private Config c;
		public IPlugin Install(Config c)
		{
			this.c = c;
			if (c.Plugins.Has<Image404>())
			{
				throw new InvalidOperationException();
			}
			c.Pipeline.ImageMissing += new UrlEventHandler(this.Pipeline_ImageMissing);
			c.Plugins.add_plugin(this);
			return this;
		}
		private void Pipeline_ImageMissing(IHttpModule sender, HttpContext context, IUrlEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.QueryString["404"]))
			{
				string text = this.resolve404Path(e.QueryString["404"]);
				text = PathUtils.ResolveAppRelative(text);
				e.QueryString.Remove("404");
				text = PathUtils.MergeQueryString(text, e.QueryString);
				context.Response.Redirect(text, true);
			}
		}
		protected string resolve404Path(string path)
		{
			if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
			{
				throw new ImageProcessingException("Image 404 redirects must be server-local. Received " + path);
			}
			if (path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				return path;
			}
			if (path.StartsWith("~", StringComparison.OrdinalIgnoreCase))
			{
				return path;
			}
			if (new Regex("^[a-zA-Z][a-zA-Z0-9]*$").IsMatch(path))
			{
				string text = this.c.get("image404." + path, null);
				if (text != null)
				{
					return text;
				}
			}
			string text2 = this.c.get("image404.basedir", "~/");
			path = text2.TrimEnd(new char[]
			{
				'/'
			}) + '/' + path.TrimStart(new char[]
			{
				'/'
			});
			return path;
		}
		public bool Uninstall(Config c)
		{
			c.Pipeline.ImageMissing -= new UrlEventHandler(this.Pipeline_ImageMissing);
			c.Plugins.remove_plugin(this);
			return true;
		}
		public IEnumerable<string> GetSupportedQuerystringKeys()
		{
			return new string[]
			{
				"404"
			};
		}
	}
}
