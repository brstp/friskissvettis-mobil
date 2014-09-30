using System;
using WURFL.Commons;
namespace WURFL.Matchers
{
	public class ToleranceCalculatorAfterString : IToleranceCalculator
	{
		private readonly string needle;
		private readonly int ordinalIndex;
		private readonly string startingNeedle;
		public ToleranceCalculatorAfterString(string needle, int ordinalIndex, string startingNeedle)
		{
			this.needle = needle;
			this.ordinalIndex = ordinalIndex;
			this.startingNeedle = startingNeedle;
		}
		public int Tolerance(string userAgent)
		{
			return StringUtils.OrdinalIndexOfOrLength(userAgent, this.needle, this.ordinalIndex, userAgent.IndexOf(this.startingNeedle));
		}
	}
}
