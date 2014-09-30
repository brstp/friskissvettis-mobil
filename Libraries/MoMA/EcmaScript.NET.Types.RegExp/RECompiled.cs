using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class RECompiled
	{
		internal char[] source;
		internal int parenCount;
		internal int flags;
		internal sbyte[] program;
		internal int classCount;
		internal RECharSet[] classList;
		internal int anchorCh = -1;
	}
}
