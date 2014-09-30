using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET.Debugging
{
	[ComVisible(true)]
	public interface Debugger
	{
		void HandleCompilationDone(Context cx, DebuggableScript fnOrScript, string source);
		DebugFrame GetFrame(Context cx, DebuggableScript fnOrScript);
	}
}
