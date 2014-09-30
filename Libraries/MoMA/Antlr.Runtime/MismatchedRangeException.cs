using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class MismatchedRangeException : RecognitionException
	{
		private int a;
		private int b;
		public int A
		{
			get
			{
				return this.a;
			}
			set
			{
				this.a = value;
			}
		}
		public int B
		{
			get
			{
				return this.b;
			}
			set
			{
				this.b = value;
			}
		}
		public MismatchedRangeException()
		{
		}
		public MismatchedRangeException(int a, int b, IIntStream input) : base(input)
		{
			this.a = a;
			this.b = b;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"MismatchedNotSetException(",
				this.UnexpectedType,
				" not in [",
				this.a,
				",",
				this.b,
				"])"
			});
		}
	}
}
