using System;
using System.Runtime.InteropServices;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[ComVisible(true)]
	[Serializable]
	public class DecodingFailedException : ArgumentException
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public DecodingFailedException(string message)
		{
			this.message = message;
		}
	}
}
