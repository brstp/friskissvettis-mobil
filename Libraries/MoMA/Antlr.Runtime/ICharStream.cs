using System;
namespace Antlr.Runtime
{
	public interface ICharStream : IIntStream
	{
		int Line
		{
			get;
			set;
		}
		int CharPositionInLine
		{
			get;
			set;
		}
		int LT(int i);
		string Substring(int start, int stop);
	}
}
