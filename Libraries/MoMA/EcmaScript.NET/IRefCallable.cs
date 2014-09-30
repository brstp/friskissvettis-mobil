using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface IRefCallable : ICallable
	{
		IRef RefCall(Context cx, IScriptable thisObj, object[] args);
	}
}
