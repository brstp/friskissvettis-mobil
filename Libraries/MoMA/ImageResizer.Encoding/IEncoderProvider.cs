using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Encoding
{
	[ComVisible(true)]
	public interface IEncoderProvider
	{
		IEncoder GetEncoder(ResizeSettings settings, object original);
	}
}
