using System;
namespace WURFL.Matchers
{
	internal class IndexedString : IIndexedString
	{
		private const int First = 1;
		private readonly string needle;
		public IndexedString(string needle)
		{
			this.needle = needle;
		}
		public IToleranceCalculator After(string startingNeedle)
		{
			return new ToleranceCalculatorAfterString(this.needle, 1, startingNeedle);
		}
	}
}
