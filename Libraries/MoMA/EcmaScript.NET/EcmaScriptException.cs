using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public abstract class EcmaScriptException : ApplicationException
	{
		private string m_SourceName;
		private int m_LineNumber;
		private string m_LineSource;
		private int m_ColumnNumber;
		internal object m_InterpreterStackInfo;
		internal int[] m_InterpreterLineData;
		private string m_ScriptStackTrace = null;
		public override string Message
		{
			get
			{
				string message = base.Message;
				string result;
				if (this.m_SourceName == null || this.m_LineNumber <= 0)
				{
					result = message;
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder(message);
					stringBuilder.Append(" (");
					if (this.m_SourceName != null)
					{
						stringBuilder.Append(this.m_SourceName);
					}
					if (this.m_LineNumber > 0)
					{
						stringBuilder.Append('#');
						stringBuilder.Append(this.m_LineNumber);
					}
					stringBuilder.Append(')');
					result = stringBuilder.ToString();
				}
				return result;
			}
		}
		public virtual string SourceName
		{
			get
			{
				return this.m_SourceName;
			}
		}
		public int LineNumber
		{
			get
			{
				return this.m_LineNumber;
			}
		}
		public int ColumnNumber
		{
			get
			{
				return this.m_ColumnNumber;
			}
		}
		public string LineSource
		{
			get
			{
				return this.m_LineSource;
			}
		}
		public string ScriptStackTrace
		{
			get
			{
				return this.m_ScriptStackTrace;
			}
		}
		internal EcmaScriptException()
		{
			Interpreter.captureInterpreterStackInfo(this);
		}
		internal EcmaScriptException(string details) : base(details)
		{
			Interpreter.captureInterpreterStackInfo(this);
		}
		internal EcmaScriptException(string details, Exception innerException) : base(details, innerException)
		{
			Interpreter.captureInterpreterStackInfo(this);
		}
		public void InitSourceName(string sourceName)
		{
			if (sourceName == null)
			{
				throw new ArgumentException();
			}
			if (this.m_SourceName != null)
			{
				throw new ApplicationException();
			}
			this.m_SourceName = sourceName;
		}
		public void InitLineNumber(int lineNumber)
		{
			if (lineNumber <= 0)
			{
				throw new ArgumentException(Convert.ToString(lineNumber));
			}
			if (this.m_LineNumber > 0)
			{
				throw new ApplicationException();
			}
			this.m_LineNumber = lineNumber;
		}
		public void InitColumnNumber(int columnNumber)
		{
			if (columnNumber <= 0)
			{
				throw new ArgumentException(Convert.ToString(columnNumber));
			}
			if (this.m_ColumnNumber > 0)
			{
				throw new ApplicationException();
			}
			this.m_ColumnNumber = columnNumber;
		}
		public void InitLineSource(string lineSource)
		{
			if (lineSource == null)
			{
				throw new ArgumentException();
			}
			if (this.m_LineSource != null)
			{
				throw new ApplicationException();
			}
			this.m_LineSource = lineSource;
		}
		internal void RecordErrorOrigin(string sourceName, int lineNumber, string lineSource, int columnNumber)
		{
			if (sourceName != null)
			{
				this.InitSourceName(sourceName);
			}
			if (lineNumber != 0)
			{
				this.InitLineNumber(lineNumber);
			}
			if (lineSource != null)
			{
				this.InitLineSource(lineSource);
			}
			if (columnNumber != 0)
			{
				this.InitColumnNumber(columnNumber);
			}
			this.InitScriptStackTrace();
		}
		private void InitScriptStackTrace()
		{
			this.m_ScriptStackTrace = Interpreter.GetStack(this);
		}
		public override string ToString()
		{
			string result;
			if (this.StackTrace != null)
			{
				result = Interpreter.getPatchedStack(this, this.StackTrace.ToString());
			}
			else
			{
				result = this.ScriptStackTrace;
			}
			return result;
		}
	}
}
