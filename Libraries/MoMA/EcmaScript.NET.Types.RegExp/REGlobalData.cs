using System;
namespace EcmaScript.NET.Types.RegExp
{
	internal class REGlobalData
	{
		internal bool multiline;
		internal RECompiled regexp;
		internal int lastParen;
		internal int skipped;
		internal int cp;
		internal long[] parens;
		internal REProgState stateStackTop;
		internal REBackTrackData backTrackStackTop;
		internal virtual int parens_index(int i)
		{
			return (int)this.parens[i];
		}
		internal virtual int parens_length(int i)
		{
			return (int)((ulong)this.parens[i] >> 32);
		}
		internal virtual void set_parens(int i, int index, int length)
		{
			this.parens[i] = (((long)index & -1L) | (long)length << 32);
		}
	}
}
