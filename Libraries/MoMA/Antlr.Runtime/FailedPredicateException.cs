using System;
namespace Antlr.Runtime
{
	[Serializable]
	public class FailedPredicateException : RecognitionException
	{
		public string ruleName;
		public string predicateText;
		public FailedPredicateException()
		{
		}
		public FailedPredicateException(IIntStream input, string ruleName, string predicateText) : base(input)
		{
			this.ruleName = ruleName;
			this.predicateText = predicateText;
		}
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"FailedPredicateException(",
				this.ruleName,
				",{",
				this.predicateText,
				"}?)"
			});
		}
	}
}
