using System;
using WURFL.Commons;
namespace WURFL.Matchers
{
	public class ToleranceCalculator : IToleranceCalculator
	{
		private readonly string needle;
		private readonly int ordinalIndex;
		public ToleranceCalculator(string needle, int ordinalIndex)
		{
			this.needle = needle;
			this.ordinalIndex = ordinalIndex;
		}
		public int Tolerance(string userAgent)
		{
			return StringUtils.OrdinalIndexOfOrLength(userAgent, this.needle, this.ordinalIndex);
		}
	}
}
