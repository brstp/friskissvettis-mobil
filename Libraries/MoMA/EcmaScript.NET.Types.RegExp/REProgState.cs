using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class REProgState
	{
		internal REProgState previous;
		internal int min;
		internal int max;
		internal int index;
		internal int continuation_op;
		internal int continuation_pc;
		internal REBackTrackData backTrack;
		internal REProgState(REProgState previous, int min, int max, int index, REBackTrackData backTrack, int continuation_pc, int continuation_op)
		{
			this.previous = previous;
			this.min = min;
			this.max = max;
			this.index = index;
			this.continuation_op = continuation_op;
			this.continuation_pc = continuation_pc;
			this.backTrack = backTrack;
		}
	}
}
