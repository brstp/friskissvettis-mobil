using System;
using System.Runtime.InteropServices;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[ComVisible(true)]
	[Serializable]
	public class InvalidVersionInfoException : VersionInformationException
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public InvalidVersionInfoException(string message)
		{
			this.message = message;
		}
	}
}
