using System;
namespace NCalc
{
	[Flags]
	public enum EvaluateOptions
	{
		None = 1,
		IgnoreCase = 2,
		NoCache = 4,
		IterateParameters = 8,
		RoundAwayFromZero = 16
	}
}
