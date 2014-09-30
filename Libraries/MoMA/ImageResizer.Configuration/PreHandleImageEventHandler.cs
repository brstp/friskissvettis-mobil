using ImageResizer.Caching;
using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public delegate void PreHandleImageEventHandler(IHttpModule sender, HttpContext context, IResponseArgs e);
}
