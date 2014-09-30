using System;
using System.Collections.Generic;
namespace WURFL.Matchers.StringMatcher
{
	public interface IStringMatcher
	{
		string Match(ICollection<string> candidates, string needle, int tolerance);
	}
}
