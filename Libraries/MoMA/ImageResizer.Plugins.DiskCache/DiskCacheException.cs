using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class DiskCacheException : Exception
	{
		public DiskCacheException(string message) : base(message)
		{
		}
		public DiskCacheException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
