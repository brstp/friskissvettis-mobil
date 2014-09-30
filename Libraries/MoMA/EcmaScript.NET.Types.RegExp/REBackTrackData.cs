using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class REBackTrackData
	{
		internal REBackTrackData previous;
		internal int continuation_op;
		internal int continuation_pc;
		internal int lastParen;
		internal long[] parens;
		internal int cp;
		internal REProgState stateStackTop;
		internal REBackTrackData(REGlobalData gData, int op, int pc)
		{
			this.previous = gData.backTrackStackTop;
			this.continuation_op = op;
			this.continuation_pc = pc;
			this.lastParen = gData.lastParen;
			if (gData.parens != null)
			{
				this.parens = new long[gData.parens.Length];
				gData.parens.CopyTo(this.parens, 0);
			}
			this.cp = gData.cp;
			this.stateStackTop = gData.stateStackTop;
		}
	}
}
