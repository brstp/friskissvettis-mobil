using ImageResizer.Configuration;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class Gradient : IPlugin, IQuerystringPlugin, IVirtualImageProvider
	{
		public class GradientVirtualFile : IVirtualBitmapFile, IVirtualFile
		{
			protected ResizeSettings query;
			public string VirtualPath
			{
				get
				{
					return "gradient.png";
				}
			}
			public GradientVirtualFile(NameValueCollection query)
			{
				this.query = new ResizeSettings(query);
			}
			public Stream Open()
			{
				throw new NotImplementedException();
			}
			public Bitmap GetBitmap()
			{
				Bitmap bitmap = null;
				try
				{
					int width = (this.query.Width > 0) ? this.query.Width : 8;
					int height = (this.query.Height > 0) ? this.query.Height : 8;
					float @float = Utils.getFloat(this.query, "angle", 0f);
					Color color = Utils.parseColor(this.query["color1"], Color.White);
					Color color2 = Utils.parseColor(this.query["color2"], Color.Black);
					bitmap = new Bitmap(width, height);
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						using (Brush brush = new LinearGradientBrush(new Rectangle(0, 0, width, height), color, color2, @float))
						{
							graphics.FillRectangle(brush, 0, 0, width, height);
						}
					}
				}
				catch
				{
					if (bitmap != null)
					{
						bitmap.Dispose();
					}
					throw;
				}
				return bitmap;
			}
		}
		public bool FileExists(string virtualPath, NameValueCollection queryString)
		{
			return virtualPath.EndsWith("/gradient.png", StringComparison.OrdinalIgnoreCase);
		}
		public IVirtualFile GetFile(string virtualPath, NameValueCollection queryString)
		{
			return new Gradient.GradientVirtualFile(queryString);
		}
		public IEnumerable<string> GetSupportedQuerystringKeys()
		{
			return new string[]
			{
				"color1",
				"color2",
				"angle",
				"width",
				"height"
			};
		}
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
	}
}
