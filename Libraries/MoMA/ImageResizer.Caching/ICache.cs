using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public interface ICache
	{
		bool CanProcess(HttpContext current, IResponseArgs e);
		void Process(HttpContext current, IResponseArgs e);
	}
}
