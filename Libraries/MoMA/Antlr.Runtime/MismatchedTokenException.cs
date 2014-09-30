using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class MismatchedTokenException : RecognitionException
	{
		private int expecting;
		public int Expecting
		{
			get
			{
				return this.expecting;
			}
			set
			{
				this.expecting = value;
			}
		}
		public MismatchedTokenException()
		{
		}
		public MismatchedTokenException(int expecting, IIntStream input) : base(input)
		{
			this.expecting = expecting;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"MismatchedTokenException(",
				this.UnexpectedType,
				"!=",
				this.expecting,
				")"
			});
		}
	}
}
