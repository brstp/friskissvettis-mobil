using System;
using System.Text;
namespace EcmaScript.NET.Types.RegExp
{
	internal sealed class GlobData
	{
		internal RegExpActions mode = RegExpActions.None;
		internal int optarg;
		internal bool global;
		internal string str;
		internal BuiltinRegExp regexp;
		internal IScriptable arrayobj;
		internal IFunction lambda;
		internal string repstr;
		internal int dollar = -1;
		internal StringBuilder charBuf;
		internal int leftIndex;
	}
}
