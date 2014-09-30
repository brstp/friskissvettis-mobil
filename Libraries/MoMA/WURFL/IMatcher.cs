using System;
using WURFL.Request;
namespace WURFL
{
	public interface IMatcher
	{
		void Add(string userAgent, string deviceId);
		bool CanMatch(WURFLRequest wurflRequest);
		string Match(WURFLRequest wurflRequest);
	}
}
