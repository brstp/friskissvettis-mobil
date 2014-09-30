using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class EcmaScriptError : EcmaScriptException
	{
		private string m_ErrorName;
		private string m_ErrorMessage;
		public virtual string Name
		{
			get
			{
				return this.m_ErrorName;
			}
		}
		public override string Message
		{
			get
			{
				return string.Format("\"{0}\", {1} at line {2}: {3}", new object[]
				{
					base.SourceName,
					this.Name,
					base.LineNumber,
					this.ErrorMessage
				});
			}
		}
		public virtual string ErrorMessage
		{
			get
			{
				return this.m_ErrorMessage;
			}
		}
		internal EcmaScriptError(string errorName, string errorMessage, string sourceName, int lineNumber, string lineSource, int columnNumber)
		{
			base.RecordErrorOrigin(sourceName, lineNumber, lineSource, columnNumber);
			this.m_ErrorName = errorName;
			this.m_ErrorMessage = errorMessage;
		}
	}
}
