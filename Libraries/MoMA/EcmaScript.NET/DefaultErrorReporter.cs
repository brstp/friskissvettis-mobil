using System;
namespace EcmaScript.NET
{
	internal class DefaultErrorReporter : ErrorReporter
	{
		internal static readonly DefaultErrorReporter instance = new DefaultErrorReporter();
		private bool forEval;
		private ErrorReporter chainedReporter;
		private DefaultErrorReporter()
		{
		}
		internal static ErrorReporter ForEval(ErrorReporter reporter)
		{
			return new DefaultErrorReporter
			{
				forEval = true,
				chainedReporter = reporter
			};
		}
		public virtual void Warning(string message, string sourceURI, int line, string lineText, int lineOffset)
		{
			if (this.chainedReporter != null)
			{
				this.chainedReporter.Warning(message, sourceURI, line, lineText, lineOffset);
			}
			else
			{
				Console.Error.WriteLine("strict warnung: " + message);
			}
		}
		public virtual void Error(string message, string sourceURI, int line, string lineText, int lineOffset)
		{
			if (this.forEval)
			{
				throw ScriptRuntime.ConstructError("SyntaxError", message, sourceURI, line, lineText, lineOffset);
			}
			if (this.chainedReporter != null)
			{
				this.chainedReporter.Error(message, sourceURI, line, lineText, lineOffset);
				return;
			}
			throw this.RuntimeError(message, sourceURI, line, lineText, lineOffset);
		}
		public virtual EcmaScriptRuntimeException RuntimeError(string message, string sourceURI, int line, string lineText, int lineOffset)
		{
			EcmaScriptRuntimeException result;
			if (this.chainedReporter != null)
			{
				result = this.chainedReporter.RuntimeError(message, sourceURI, line, lineText, lineOffset);
			}
			else
			{
				result = new EcmaScriptRuntimeException(message, sourceURI, line, lineText, lineOffset);
			}
			return result;
		}
	}
}
