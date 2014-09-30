using System;
using System.Collections.Generic;
using System.Web;
using WURFL.Request;
namespace WURFL
{
	public interface IWURFLManager
	{
		IDevice GetDeviceForRequest(WURFLRequest wurflRequest);
		IDevice GetDeviceForRequest(string userAgent);
		IDevice GetDeviceForRequest(HttpRequest httpRequest);
		IDevice GetDeviceById(string deviceId);
		ICollection<IDevice> GetAllDevices();
	}
}
