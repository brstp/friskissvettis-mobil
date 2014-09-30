using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public delegate void ApplyResponseHeadersDelegate(IResponseHeaders headers, HttpContext context);
}
