using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class NoViableAltException : RecognitionException
	{
		public string grammarDecisionDescription;
		public int decisionNumber;
		public int stateNumber;
		public NoViableAltException()
		{
		}
		public NoViableAltException(string grammarDecisionDescription, int decisionNumber, int stateNumber, IIntStream input) : base(input)
		{
			this.grammarDecisionDescription = grammarDecisionDescription;
			this.decisionNumber = decisionNumber;
			this.stateNumber = stateNumber;
		}
		public override string ToString()
		{
			if (this.input is ICharStream)
			{
				return string.Concat(new object[]
				{
					"NoViableAltException('",
					(char)this.UnexpectedType,
					"'@[",
					this.grammarDecisionDescription,
					"])"
				});
			}
			return string.Concat(new object[]
			{
				"NoViableAltException(",
				this.UnexpectedType,
				"@[",
				this.grammarDecisionDescription,
				"])"
			});
		}
	}
}
