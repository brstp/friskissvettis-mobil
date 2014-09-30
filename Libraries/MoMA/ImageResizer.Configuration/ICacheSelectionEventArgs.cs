using ImageResizer.Caching;
using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public interface ICacheSelectionEventArgs
	{
		HttpContext Context
		{
			get;
		}
		IResponseArgs ResponseArgs
		{
			get;
		}
		ICache SelectedCache
		{
			get;
			set;
		}
	}
}
