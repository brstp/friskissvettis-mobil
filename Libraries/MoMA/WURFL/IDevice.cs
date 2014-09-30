using System;
using System.Collections.Generic;
namespace WURFL
{
	public interface IDevice
	{
		string Id
		{
			get;
		}
		string UserAgent
		{
			get;
		}
		string GetCapability(string name);
		IDictionary<string, string> GetCapabilities();
	}
}
