using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Encoding
{
	[ComVisible(true)]
	public interface IEncoder
	{
		bool SupportsTransparency
		{
			get;
		}
		string MimeType
		{
			get;
		}
		string Extension
		{
			get;
		}
		IEncoder CreateIfSuitable(ResizeSettings settings, object original);
		void Write(Image i, Stream s);
	}
}
