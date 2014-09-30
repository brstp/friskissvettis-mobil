using System;
using System.Runtime.InteropServices;
namespace ThoughtWorks.QRCode.Codec.Data
{
	[ComVisible(true)]
	public interface QRCodeImage
	{
		int Width
		{
			get;
		}
		int Height
		{
			get;
		}
		int getPixel(int x, int y);
	}
}
