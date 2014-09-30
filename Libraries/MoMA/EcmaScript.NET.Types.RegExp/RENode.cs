using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class RENode
	{
		internal sbyte op;
		internal RENode next;
		internal RENode kid;
		internal RENode kid2;
		internal int parenIndex;
		internal int min;
		internal int max;
		internal int parenCount;
		internal bool greedy;
		internal int startIndex;
		internal int kidlen;
		internal int bmsize;
		internal int index;
		internal char chr;
		internal int length;
		internal int flatIndex;
		internal RENode(sbyte op)
		{
			this.op = op;
		}
		public override string ToString()
		{
			return "[RENode: Op: " + this.op + "]";
		}
	}
}
