using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Debugging
{
	[ComVisible(true)]
	public interface DebugFrame
	{
		void OnEnter(Context cx, IScriptable activation, IScriptable thisObj, object[] args);
		void OnLineChange(Context cx, int lineNumber);
		void OnExceptionThrown(Context cx, Exception ex);
		void OnExit(Context cx, bool byThrow, object resultOrException);
		void OnDebuggerStatement(Context cx);
	}
}
