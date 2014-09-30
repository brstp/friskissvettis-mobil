using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public delegate void FileDisappearedHandler(string relativePath, string physicalPath);
}
