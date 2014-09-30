using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class EcmaScriptRuntimeException : EcmaScriptException
	{
		public EcmaScriptRuntimeException(Exception innerException) : base(innerException.Message, innerException)
		{
			int[] array = new int[1];
			int[] array2 = array;
			string sourcePositionFromStack = Context.GetSourcePositionFromStack(array2);
			int num = array2[0];
			if (sourcePositionFromStack != null)
			{
				base.InitSourceName(sourcePositionFromStack);
			}
			if (num != 0)
			{
				base.InitLineNumber(num);
			}
		}
		public EcmaScriptRuntimeException(string detail) : base(detail)
		{
		}
		public EcmaScriptRuntimeException(string detail, string sourceName, int lineNumber) : this(detail, sourceName, lineNumber, null, 0)
		{
		}
		public EcmaScriptRuntimeException(string detail, string sourceName, int lineNumber, string lineSource, int columnNumber) : base(detail)
		{
			base.RecordErrorOrigin(sourceName, lineNumber, lineSource, columnNumber);
		}
	}
}
