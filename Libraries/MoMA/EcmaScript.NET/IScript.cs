using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface IScript
	{
		object Exec(Context cx, IScriptable scope);
	}
}
