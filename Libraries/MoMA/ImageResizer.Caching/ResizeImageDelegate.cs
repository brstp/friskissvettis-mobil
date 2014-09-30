using System;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public delegate void ResizeImageDelegate(Stream s);
}
