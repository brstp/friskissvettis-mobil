using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal sealed class RECharSet
	{
		internal int length;
		internal int startIndex;
		internal int strlength;
		internal volatile bool converted;
		internal volatile bool sense;
		internal volatile sbyte[] bits;
		internal RECharSet(int length, int startIndex, int strlength)
		{
			this.length = length;
			this.startIndex = startIndex;
			this.strlength = strlength;
		}
	}
}
