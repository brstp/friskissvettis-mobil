using System;
using System.Runtime.InteropServices;
namespace ImageResizer
{
	[ComVisible(true)]
	public class ImageMissingException : ImageProcessingException
	{
		public ImageMissingException(string message) : base(404, message)
		{
		}
		public ImageMissingException(string message, string safeMessage) : base(404, message, safeMessage)
		{
		}
		public ImageMissingException(string message, string safeMessage, Exception innerException) : base(404, message, safeMessage, innerException)
		{
		}
	}
}
