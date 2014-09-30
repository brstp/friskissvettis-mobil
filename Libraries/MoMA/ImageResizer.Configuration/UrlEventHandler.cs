using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public delegate void UrlEventHandler(IHttpModule sender, HttpContext context, IUrlEventArgs e);
}
