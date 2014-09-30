using System;
using System.Collections.Generic;
namespace WURFL.Resource
{
	public interface IModelDevice
	{
		string Id
		{
			get;
		}
		string UserAgent
		{
			get;
		}
		string FallBackId
		{
			get;
		}
		bool ActualDeviceRoot
		{
			get;
		}
		IDictionary<string, IDictionary<string, string>> CapabilitiesByGroupId
		{
			get;
		}
		IModelDevice FallBack
		{
			get;
		}
		ICollection<string> Groups();
		bool IsCapabilityDefined(string capabilityName);
		string GetCapability(string capabilityName);
		IDictionary<string, string> GetCapabilities();
		IModelDevice PatchWith(IModelDevice patchingDevice);
	}
}
