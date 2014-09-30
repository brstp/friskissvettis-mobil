using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace WURFL.Resource
{
	public class WURFLParser : IWURFLParser
	{
		public class ModelDevice : IModelDevice
		{
			private readonly bool actualDeviceRoot;
			private readonly string fallBackId;
			private readonly string id;
			private readonly string userAgent;
			private IDictionary<string, string> capabilities;
			private IDictionary<string, IDictionary<string, string>> capabilitiesByGroupId = new Dictionary<string, IDictionary<string, string>>();
			private IModelDevice fallBack;
			public string Id
			{
				get
				{
					return this.id;
				}
			}
			public string UserAgent
			{
				get
				{
					return this.userAgent;
				}
			}
			public string FallBackId
			{
				get
				{
					return this.fallBackId;
				}
			}
			public bool ActualDeviceRoot
			{
				get
				{
					return this.actualDeviceRoot;
				}
			}
			public IDictionary<string, IDictionary<string, string>> CapabilitiesByGroupId
			{
				get
				{
					return this.capabilitiesByGroupId;
				}
				set
				{
					this.capabilitiesByGroupId = value;
					this.SetCapabilities();
				}
			}
			public IModelDevice FallBack
			{
				get
				{
					return this.fallBack;
				}
				set
				{
					this.fallBack = value;
				}
			}
			public ModelDevice(string id, string userAgent, string fallBackId, bool actualDeviceRoot)
			{
				this.id = id;
				this.userAgent = userAgent;
				this.fallBackId = fallBackId;
				this.actualDeviceRoot = actualDeviceRoot;
			}
			public ICollection<string> Groups()
			{
				return this.capabilitiesByGroupId.Keys;
			}
			public bool IsCapabilityDefined(string capabilityName)
			{
				return this.capabilities.ContainsKey(capabilityName);
			}
			public string GetCapability(string capabilityName)
			{
				string result = null;
				this.capabilities.TryGetValue(capabilityName, out result);
				return result;
			}
			public IDictionary<string, string> GetCapabilities()
			{
				return new Dictionary<string, string>(this.capabilities);
			}
			public IModelDevice PatchWith(IModelDevice patchingDevice)
			{
				if (!this.id.Equals(patchingDevice.Id))
				{
					throw new ArgumentException("Can't patch device: ids mismatch");
				}
				return this.CopyGroups(patchingDevice);
			}
			private void SetCapabilities()
			{
				this.capabilities = new Dictionary<string, string>();
				foreach (KeyValuePair<string, IDictionary<string, string>> current in this.CapabilitiesByGroupId)
				{
					foreach (KeyValuePair<string, string> current2 in current.Value)
					{
						this.capabilities.Add(current2);
					}
				}
			}
			private IModelDevice CopyGroups(IModelDevice patchDevice)
			{
				IDictionary<string, IDictionary<string, string>> dictionary = new Dictionary<string, IDictionary<string, string>>(this.capabilitiesByGroupId);
				foreach (KeyValuePair<string, IDictionary<string, string>> current in patchDevice.CapabilitiesByGroupId)
				{
					if (dictionary.ContainsKey(current.Key))
					{
						this.UpdateCapabilities(dictionary[current.Key], current.Value);
					}
					else
					{
						dictionary.Add(current);
					}
				}
				return new WURFLParser.ModelDevice(this.id, this.userAgent, patchDevice.FallBackId, patchDevice.ActualDeviceRoot)
				{
					CapabilitiesByGroupId = dictionary
				};
			}
			private void UpdateCapabilities(IDictionary<string, string> deviceCapabilities, IDictionary<string, string> patchCapabilities)
			{
				foreach (KeyValuePair<string, string> current in patchCapabilities)
				{
					deviceCapabilities[current.Key] = current.Value;
				}
			}
		}
		private static class XmlConstants
		{
			public const string Id = "id";
			public const string UserAgent = "user_agent";
			public const string FallBack = "fall_back";
			public const string ActualDeviceRoot = "actual_device_root";
			public const string Group = "group";
			public const string Capability = "capability";
		}
		public ResourceData Parse(IWURFLResource wurflResource)
		{
			return this.Parse(wurflResource, new IWURFLResource[0]);
		}
		public ResourceData Parse(IWURFLResource wurflResource, IWURFLResource[] patchResources)
		{
			IDictionary<string, IModelDevice> dictionary = new Dictionary<string, IModelDevice>();
			IDictionary<string, WURFLParser.ModelDevice> patches = this.ParsePatches(patchResources);
			WURFLInfo wurflInfo;
			using (Stream stream = wurflResource.GetStream())
			{
				using (XmlReader xmlReader = XmlReader.Create(stream, null))
				{
					wurflInfo = (xmlReader.ReadToFollowing("version") ? WURFLParser.WURFLInfoFrom(xmlReader.ReadSubtree()) : new WURFLInfo());
					while (xmlReader.ReadToFollowing("device"))
					{
						WURFLParser.ModelDevice modelDevice = this.DeviceFrom(xmlReader);
						this.PatchAndAddModelDevice(patches, modelDevice, dictionary);
					}
					this.AddRemainingPatchDevices(patches, dictionary);
				}
			}
			this.UpdateModelDevicesWithFallBack(dictionary);
			return new ResourceData(wurflInfo, dictionary);
		}
		private void UpdateModelDevicesWithFallBack(IDictionary<string, IModelDevice> devicesById)
		{
			foreach (KeyValuePair<string, IModelDevice> current in devicesById)
			{
				IModelDevice fallBack;
				if (devicesById.TryGetValue(current.Value.FallBackId, out fallBack))
				{
					((WURFLParser.ModelDevice)current.Value).FallBack = fallBack;
				}
			}
		}
		private void AddRemainingPatchDevices(IDictionary<string, WURFLParser.ModelDevice> patches, IDictionary<string, IModelDevice> devicesById)
		{
			foreach (KeyValuePair<string, WURFLParser.ModelDevice> current in patches)
			{
				devicesById.Add(current.Key, current.Value);
			}
		}
		private void PatchAndAddModelDevice(IDictionary<string, WURFLParser.ModelDevice> patches, WURFLParser.ModelDevice modelDevice, IDictionary<string, IModelDevice> devicesById)
		{
			IModelDevice value = modelDevice;
			if (patches.ContainsKey(modelDevice.Id))
			{
				value = modelDevice.PatchWith(patches[modelDevice.Id]);
				patches.Remove(modelDevice.Id);
			}
			devicesById[modelDevice.Id] = value;
		}
		private IDictionary<string, WURFLParser.ModelDevice> ParsePatches(IWURFLResource[] patchesPath)
		{
			IDictionary<string, WURFLParser.ModelDevice> dictionary = new Dictionary<string, WURFLParser.ModelDevice>();
			for (int i = 0; i < patchesPath.Length; i++)
			{
				IWURFLResource iWURFLResource = patchesPath[i];
				using (Stream stream = iWURFLResource.GetStream())
				{
					using (XmlReader xmlReader = XmlReader.Create(stream, null))
					{
						while (xmlReader.ReadToFollowing("device"))
						{
							WURFLParser.ModelDevice modelDevice = this.DeviceFrom(xmlReader);
							if (dictionary.ContainsKey(modelDevice.Id))
							{
								WURFLParser.ModelDevice modelDevice2 = dictionary[modelDevice.Id];
								modelDevice = (WURFLParser.ModelDevice)modelDevice2.PatchWith(modelDevice);
							}
							dictionary.Add(modelDevice.Id, modelDevice);
						}
					}
				}
			}
			return dictionary;
		}
		private static WURFLInfo WURFLInfoFrom(XmlReader reader)
		{
			string version = "";
			string lastUpdated = "";
			while (reader.Read())
			{
				string name;
				if (reader.IsStartElement() && (name = reader.Name) != null)
				{
					if (!(name == "ver"))
					{
						if (name == "last_updated")
						{
							lastUpdated = reader.ReadString();
						}
					}
					else
					{
						version = reader.ReadString();
					}
				}
			}
			return new WURFLInfo(version, lastUpdated);
		}
		private WURFLParser.ModelDevice DeviceFrom(XmlReader xmlReader)
		{
			WURFLParser.ModelDevice modelDevice = this.EmptyDeviceFrom(xmlReader);
			IDictionary<string, IDictionary<string, string>> capabilitiesByGroupId = new Dictionary<string, IDictionary<string, string>>();
			if (!xmlReader.IsEmptyElement)
			{
				capabilitiesByGroupId = this.ReadGroups(xmlReader);
			}
			modelDevice.CapabilitiesByGroupId = capabilitiesByGroupId;
			return modelDevice;
		}
		private WURFLParser.ModelDevice EmptyDeviceFrom(XmlReader xmlReader)
		{
			WURFLParser.ModelDevice result = null;
			string name;
			if ((name = xmlReader.Name) != null && name == "device")
			{
				string attribute = xmlReader.GetAttribute("id");
				string attribute2 = xmlReader.GetAttribute("user_agent");
				string attribute3 = xmlReader.GetAttribute("fall_back");
				string attribute4 = xmlReader.GetAttribute("actual_device_root");
				bool actualDeviceRoot = false;
				bool.TryParse(attribute4, out actualDeviceRoot);
				result = new WURFLParser.ModelDevice(attribute, attribute2, attribute3, actualDeviceRoot);
			}
			return result;
		}
		private IDictionary<string, IDictionary<string, string>> ReadGroups(XmlReader xmlReader)
		{
			IDictionary<string, IDictionary<string, string>> dictionary = new Dictionary<string, IDictionary<string, string>>();
			if (xmlReader.ReadToDescendant("group"))
			{
				do
				{
					string attribute = xmlReader.GetAttribute("id");
					if (xmlReader.ReadToDescendant("capability"))
					{
						IDictionary<string, string> dictionary2 = new Dictionary<string, string>();
						do
						{
							string attribute2 = xmlReader.GetAttribute("name");
							string attribute3 = xmlReader.GetAttribute("value");
							dictionary2.Add(string.Intern(attribute2), attribute3);
						}
						while (xmlReader.ReadToNextSibling("capability"));
						if (dictionary2.Count > 0)
						{
							dictionary[string.Intern(attribute)] = dictionary2;
						}
					}
				}
				while (xmlReader.ReadToNextSibling("group"));
			}
			return dictionary;
		}
	}
}
