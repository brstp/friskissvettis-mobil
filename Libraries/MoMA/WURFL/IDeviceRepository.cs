using System;
using System.Collections.Generic;
using WURFL.Request;
using WURFL.Resource;
namespace WURFL
{
	public interface IDeviceRepository
	{
		WURFLInfo WURFLInfo
		{
			get;
		}
		IModelDevice GetDeviceById(string id);
		IModelDevice GetDeviceByRequest(WURFLRequest wurflRequest);
		ICollection<IModelDevice> GetAllDevices();
		int TotalNumberOfDevices();
		bool IsDeviceDefined(string deviceId);
		bool IsCapabilityDefined(string capabilityName);
		bool IsGroupDefined(string groupId);
	}
}
