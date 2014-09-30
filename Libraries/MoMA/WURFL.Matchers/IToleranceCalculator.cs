using System;
namespace WURFL.Matchers
{
	public interface IToleranceCalculator
	{
		int Tolerance(string userAgent);
	}
}
