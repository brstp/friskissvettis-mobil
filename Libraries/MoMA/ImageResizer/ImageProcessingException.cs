using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer
{
	[ComVisible(true)]
	public class ImageProcessingException : HttpException
	{
		private string publicSafeMessage;
		protected string PublicSafeMessage
		{
			get
			{
				return this.publicSafeMessage;
			}
			set
			{
				this.publicSafeMessage = value;
			}
		}
		public ImageProcessingException(string message) : base(500, message)
		{
		}
		public ImageProcessingException(int httpCode, string message) : base(httpCode, message)
		{
		}
		public ImageProcessingException(int httpCode, string message, string safeMessage) : base(httpCode, message)
		{
			this.publicSafeMessage = safeMessage;
		}
		public ImageProcessingException(int httpCode, string message, string safeMessage, Exception innerException) : base(httpCode, message, innerException)
		{
			this.publicSafeMessage = safeMessage;
		}
	}
}
