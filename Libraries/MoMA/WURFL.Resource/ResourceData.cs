using System;
using System.Collections.Generic;
using WURFL.Commons;
namespace WURFL.Resource
{
	public class ResourceData
	{
		private readonly IDictionary<string, IModelDevice> idToDevice;
		private readonly WURFLInfo wurflInfo;
		public ICollection<IModelDevice> ModelDevices
		{
			get
			{
				return this.idToDevice.Values;
			}
		}
		public WURFLInfo WURFLInfo
		{
			get
			{
				return this.wurflInfo;
			}
		}
		public ResourceData(WURFLInfo wurflInfo, IDictionary<string, IModelDevice> idToDevice)
		{
			this.wurflInfo = wurflInfo;
			this.idToDevice = idToDevice;
			this.VerifyConsistency();
		}
		private void VerifyConsistency()
		{
			List<string> list = new List<string>();
			this.VerifyGeneric();
			foreach (IModelDevice current in this.idToDevice.Values)
			{
				this.VerifyHierarchy(current, list);
				list.Add(current.Id);
				this.VerifyGroups(current);
				this.VerifyCapabilities(current);
			}
		}
		private void VerifyGeneric()
		{
			if (!this.idToDevice.ContainsKey(Constants.Generic))
			{
				throw new Exception("Can't find generic device in wurfl");
			}
		}
		private void VerifyHierarchy(IModelDevice device, List<string> hierarchyVerifiedDevicesId)
		{
			List<string> list = new List<string>(10);
			string text = device.Id;
			list.Add(text);
			while (!string.Equals(Constants.Generic, text))
			{
				IModelDevice modelDevice = this.idToDevice[text];
				string fallBackId = modelDevice.FallBackId;
				if (hierarchyVerifiedDevicesId.Contains(fallBackId))
				{
					return;
				}
				if (!this.idToDevice.ContainsKey(fallBackId))
				{
					throw new OrphanHierarchyException(list);
				}
				int num = list.IndexOf(fallBackId);
				if (num != -1)
				{
					throw new CircularHierarchyException(list.GetRange(num, list.Count));
				}
				list.Add(fallBackId);
				text = fallBackId;
			}
		}
		private void VerifyGroups(IModelDevice device)
		{
			IModelDevice modelDevice = this.GenericDevice();
			ICollection<string> collection = device.Groups();
			ICollection<string> collection2 = modelDevice.Groups();
			foreach (string current in collection)
			{
				if (!collection2.Contains(current))
				{
					throw new InexistentGroupException(device, current);
				}
			}
		}
		private IModelDevice GenericDevice()
		{
			return this.idToDevice[Constants.Generic];
		}
		private void VerifyCapabilities(IModelDevice device)
		{
			IModelDevice modelDevice = this.GenericDevice();
			IDictionary<string, string> capabilities = modelDevice.GetCapabilities();
			IDictionary<string, string> capabilities2 = device.GetCapabilities();
			foreach (KeyValuePair<string, string> current in capabilities2)
			{
				string key = current.Key;
				if (!capabilities.ContainsKey(key))
				{
					throw new InexistentCapabilityException(device, key);
				}
				string groupForCapability = ResourceData.GetGroupForCapability(device, key);
				string groupForCapability2 = ResourceData.GetGroupForCapability(modelDevice, key);
				if (!string.Equals(groupForCapability, groupForCapability2))
				{
					throw new BadCapabilityGroupException(device, key, groupForCapability, groupForCapability2);
				}
			}
		}
		private static string GetGroupForCapability(IModelDevice device, string capabilityName)
		{
			IDictionary<string, IDictionary<string, string>> capabilitiesByGroupId = device.CapabilitiesByGroupId;
			foreach (KeyValuePair<string, IDictionary<string, string>> current in capabilitiesByGroupId)
			{
				if (current.Value.ContainsKey(capabilityName))
				{
					return current.Key;
				}
			}
			throw new Exception(string.Format("capability {0} is not defined in for device {1}", capabilityName, device.Id));
		}
	}
}
