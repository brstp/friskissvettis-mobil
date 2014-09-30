using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface ICallable
	{
		object Call(Context cx, IScriptable scope, IScriptable thisObj, object[] args);
	}
}
