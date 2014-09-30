using System;
using System.Collections.Generic;
using WURFL.Commons;
using WURFL.Request;
using WURFL.Resource;
namespace WURFL
{
	public class DeviceRepository : IDeviceRepository
	{
		private readonly IDictionary<string, IModelDevice> devicesById;
		private readonly IMatchersChain matchersChain;
		private readonly WURFLInfo wurflInfo;
		private readonly IModelDevice genericDevice;
		public WURFLInfo WURFLInfo
		{
			get
			{
				return this.wurflInfo;
			}
		}
		public DeviceRepository(WURFLInfo wurflInfo, IDictionary<string, IModelDevice> devicesById, IMatchersChain matchersChain)
		{
			this.wurflInfo = wurflInfo;
			this.devicesById = devicesById;
			this.genericDevice = devicesById[Constants.Generic];
			this.matchersChain = matchersChain;
		}
		public DeviceRepository(ResourceData resourceData, IMatchersChain matchersChain) : this(resourceData.WURFLInfo, resourceData.ModelDevices, matchersChain)
		{
		}
		public DeviceRepository(WURFLInfo wurflInfo, ICollection<IModelDevice> devices, IMatchersChain matchersChain) : this(wurflInfo, DeviceRepository.ToDevicesById(devices), matchersChain)
		{
		}
		public IModelDevice GetDeviceByRequest(WURFLRequest request)
		{
			string id = this.matchersChain.Match(request);
			return this.GetDeviceById(id);
		}
		public ICollection<IModelDevice> GetAllDevices()
		{
			return this.devicesById.Values;
		}
		public IModelDevice GetDeviceById(string id)
		{
			IModelDevice modelDevice;
			this.devicesById.TryGetValue(id, out modelDevice);
			if (modelDevice == null)
			{
				throw new Exception(string.Format("There is no device in wurfl with id [{0}]", id));
			}
			return modelDevice;
		}
		public int TotalNumberOfDevices()
		{
			return this.devicesById.Count;
		}
		public bool IsDeviceDefined(string deviceId)
		{
			return this.devicesById.ContainsKey(deviceId);
		}
		public bool IsGroupDefined(string groupId)
		{
			return this.genericDevice.Groups().Contains(groupId);
		}
		public bool IsCapabilityDefined(string capabilityName)
		{
			return this.genericDevice.IsCapabilityDefined(capabilityName);
		}
		private static IDictionary<string, IModelDevice> ToDevicesById(ICollection<IModelDevice> devices)
		{
			IDictionary<string, IModelDevice> dictionary = new Dictionary<string, IModelDevice>();
			foreach (IModelDevice current in devices)
			{
				dictionary.Add(current.Id, current);
			}
			return dictionary;
		}
	}
}
