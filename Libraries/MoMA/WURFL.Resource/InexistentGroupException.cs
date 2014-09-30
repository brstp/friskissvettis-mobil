using System;
namespace WURFL.Resource
{
	public class InexistentGroupException : Exception
	{
		public InexistentGroupException(IModelDevice device, string deviceGroup) : base(string.Format("Device with id {0} defines unknow group {1}", device.Id, deviceGroup))
		{
		}
	}
}
