using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface ErrorReporter
	{
		void Warning(string message, string sourceName, int line, string lineSource, int lineOffset);
		void Error(string message, string sourceName, int line, string lineSource, int lineOffset);
		EcmaScriptRuntimeException RuntimeError(string message, string sourceName, int line, string lineSource, int lineOffset);
	}
}
