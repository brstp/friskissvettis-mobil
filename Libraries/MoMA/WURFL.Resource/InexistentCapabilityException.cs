using System;
namespace WURFL.Resource
{
	public class InexistentCapabilityException : Exception
	{
		public InexistentCapabilityException(IModelDevice device, string capabilityName) : base(string.Format("Device with id {0} defines unknow capability {1}", device.Id, capabilityName))
		{
		}
	}
}
