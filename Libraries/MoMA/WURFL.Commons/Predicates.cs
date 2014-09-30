using System;
namespace WURFL.Commons
{
	public static class Predicates
	{
		public static Predicate<string> StartsWith(string prefix)
		{
			return (string argument) => argument.StartsWith(prefix);
		}
		public static Predicate<string> ContainedIn(string target)
		{
			return (string argument) => target.Contains(argument);
		}
		public static Predicate<string> ContainedInCaseInsensitive(string target)
		{
			return (string argument) => target.ToLower().Contains(argument.ToLower());
		}
		public static Predicate<string> PrefixOf(string input)
		{
			return (string argument) => input.StartsWith(argument);
		}
	}
}
