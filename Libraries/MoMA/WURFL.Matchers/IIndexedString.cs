using System;
namespace WURFL.Matchers
{
	public interface IIndexedString
	{
		IToleranceCalculator After(string startingNeedle);
	}
}
