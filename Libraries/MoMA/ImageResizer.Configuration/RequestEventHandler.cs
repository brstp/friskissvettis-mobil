using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public delegate void RequestEventHandler(IHttpModule sender, HttpContext context);
}
