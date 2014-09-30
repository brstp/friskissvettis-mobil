using System;
using WURFL.Request;
namespace WURFL
{
	public interface IMatchersChain
	{
		string Match(WURFLRequest wurflRequest);
	}
}
