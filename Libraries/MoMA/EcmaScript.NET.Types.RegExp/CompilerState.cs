using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class CompilerState
	{
		internal char[] cpbegin;
		internal int cpend;
		internal int cp;
		internal int flags;
		internal int parenCount;
		internal int parenNesting;
		internal int classCount;
		internal int progLength;
		internal RENode result;
		internal CompilerState(char[] source, int length, int flags)
		{
			this.cpbegin = source;
			this.cp = 0;
			this.cpend = length;
			this.flags = flags;
			this.parenCount = 0;
			this.classCount = 0;
			this.progLength = 0;
		}
	}
}
