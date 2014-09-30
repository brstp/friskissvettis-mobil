using System;
namespace WURFL.Resource
{
	public class BadCapabilityGroupException : Exception
	{
		public BadCapabilityGroupException(IModelDevice device, string capabilityName, string deviceGroup, string genericGroup) : base(string.Format("Device {0} defines the capability {1} in group {2} instead of {3}", new object[]
		{
			device.Id,
			capabilityName,
			deviceGroup,
			genericGroup
		}))
		{
		}
	}
}
