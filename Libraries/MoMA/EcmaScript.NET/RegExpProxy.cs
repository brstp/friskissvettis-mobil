using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public interface RegExpProxy
	{
		bool IsRegExp(IScriptable obj);
		object Compile(Context cx, string source, string flags);
		IScriptable Wrap(Context cx, IScriptable scope, object compiled);
		object Perform(Context cx, IScriptable scope, IScriptable thisObj, object[] args, RegExpActions actionType);
		int FindSplit(Context cx, IScriptable scope, string target, string separator, IScriptable re, int[] ip, int[] matchlen, bool[] matched, string[][] parensp);
	}
}
