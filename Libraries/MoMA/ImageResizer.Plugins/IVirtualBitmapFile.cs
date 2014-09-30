using System;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IVirtualBitmapFile : IVirtualFile
	{
		Bitmap GetBitmap();
	}
}
