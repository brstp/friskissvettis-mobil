using System;
using System.Runtime.InteropServices;
namespace ImageResizer
{
	[ComVisible(true)]
	public class ImageCorruptedException : ImageProcessingException
	{
		public ImageCorruptedException(string message, Exception innerException) : base(500, message, message, innerException)
		{
		}
	}
}
