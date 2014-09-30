using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using WURFL.Cache;
using WURFL.Request;
using WURFL.Resource;
namespace WURFL
{
	public class WURFLManager : IWURFLManager
	{
		private readonly ICache<object, IDevice> cache;
		private readonly IDeviceRepository deviceRepository;
		private readonly IWURFLRequestFactory wurflRequestFactory;
		public WURFLManager(IDeviceRepository deviceRepository, IWURFLRequestFactory wurflRequestFactory) : this(deviceRepository, wurflRequestFactory, new LRUCache<object, IDevice>())
		{
		}
		public WURFLManager(IDeviceRepository deviceRepository, IWURFLRequestFactory wurflRequestFactory, ICache<object, IDevice> cache)
		{
			this.deviceRepository = deviceRepository;
			this.wurflRequestFactory = wurflRequestFactory;
			this.cache = cache;
		}
		public IDevice GetDeviceForRequest(WURFLRequest wurflRequest)
		{
			object key = WURFLManager.KeyFor(wurflRequest);
			IDevice device = this.cache.Get(key);
			if (device == null)
			{
				IModelDevice deviceByRequest = this.deviceRepository.GetDeviceByRequest(wurflRequest);
				device = new Device(deviceByRequest);
				this.cache.Put(key, device);
			}
			return device;
		}
		public IDevice GetDeviceForRequest(string userAgent)
		{
			WURFLRequest wurflRequest = this.wurflRequestFactory.CreateRequest(userAgent);
			return this.GetDeviceForRequest(wurflRequest);
		}
		public IDevice GetDeviceForRequest(HttpRequest httpRequest)
		{
			WURFLRequest wurflRequest = this.wurflRequestFactory.CreateRequest(httpRequest);
			return this.GetDeviceForRequest(wurflRequest);
		}
		public IDevice GetDeviceById(string deviceId)
		{
			if (string.IsNullOrEmpty(deviceId))
			{
				throw new ArgumentException("deviceId cannot be null or empty");
			}
			return new Device(this.deviceRepository.GetDeviceById(deviceId));
		}
		public ICollection<IDevice> GetAllDevices()
		{
			ICollection<IModelDevice> allDevices = this.deviceRepository.GetAllDevices();
			ICollection<IDevice> collection = new List<IDevice>(allDevices.Count);
			foreach (IModelDevice current in allDevices)
			{
				collection.Add(new Device(current));
			}
			return collection;
		}
		private static object KeyFor(WURFLRequest wurflRequest)
		{
			return new StringBuilder().Append(wurflRequest.UserAgent).Append(wurflRequest.UserAgentProfile).ToString();
		}
	}
}
