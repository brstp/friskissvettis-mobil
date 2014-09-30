using System;
using System.Runtime.InteropServices;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[ComVisible(true)]
	[Serializable]
	public class FinderPatternNotFoundException : Exception
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public FinderPatternNotFoundException(string message)
		{
			this.message = message;
		}
	}
}
