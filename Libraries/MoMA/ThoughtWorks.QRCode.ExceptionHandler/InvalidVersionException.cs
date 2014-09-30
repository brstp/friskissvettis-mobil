using System;
using System.Runtime.InteropServices;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[ComVisible(true)]
	[Serializable]
	public class InvalidVersionException : VersionInformationException
	{
		internal string message;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public InvalidVersionException(string message)
		{
			this.message = message;
		}
	}
}
