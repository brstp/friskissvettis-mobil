using System;
using System.Collections.Generic;
using WURFL.Request;
namespace WURFL.Matchers
{
	internal class MatchersChain : IMatchersChain
	{
		private readonly ICollection<IMatcher> _matchers;
		public MatchersChain(IEnumerable<IMatcher> matchers)
		{
			this._matchers = new List<IMatcher>(matchers);
		}
		public string Match(WURFLRequest wurflRequest)
		{
			string result = null;
			foreach (IMatcher current in this._matchers)
			{
				if (current.CanMatch(wurflRequest))
				{
					result = current.Match(wurflRequest);
					break;
				}
			}
			return result;
		}
	}
}
