using System;
using System.Runtime.InteropServices;
namespace ImageResizer
{
	[ComVisible(true)]
	public enum ScaleMode
	{
		DownscaleOnly,
		UpscaleOnly,
		Both,
		UpscaleCanvas
	}
}
