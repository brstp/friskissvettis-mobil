using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface IFunction : IScriptable, ICallable
	{
		IScriptable Construct(Context cx, IScriptable scope, object[] args);
	}
}
