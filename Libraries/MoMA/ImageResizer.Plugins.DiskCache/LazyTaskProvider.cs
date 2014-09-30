using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public delegate CleanupWorkItem LazyTaskProvider();
}
