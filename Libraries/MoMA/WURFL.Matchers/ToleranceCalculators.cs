using System;
namespace WURFL.Matchers
{
	public static class ToleranceCalculators
	{
		public static readonly IToleranceCalculator FirstSlash = new ToleranceCalculator("/", 1);
		public static readonly IToleranceCalculator ThirdSlash = new ToleranceCalculator("/", 3);
		public static readonly IToleranceCalculator FirstSpace = new ToleranceCalculator(" ", 1);
		public static IIndexedString First(string needle)
		{
			return new IndexedString(needle);
		}
	}
}
