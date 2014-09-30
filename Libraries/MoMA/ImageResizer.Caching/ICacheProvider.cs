using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public interface ICacheProvider
	{
		ICache GetCachingSystem(HttpContext context, IResponseArgs responseArgs);
	}
}
