using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public delegate void CacheResultHandler(CustomDiskCache sender, CacheResult r);
}
