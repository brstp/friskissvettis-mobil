using System;
using System.Collections.Generic;
using WURFL.Resource;
namespace WURFL
{
	public class Device : IDevice
	{
		private readonly IModelDevice modelDevice;
		public string Id
		{
			get
			{
				return this.modelDevice.Id;
			}
		}
		public string UserAgent
		{
			get
			{
				return this.modelDevice.UserAgent;
			}
		}
		public Device(IModelDevice modelDevice)
		{
			this.modelDevice = modelDevice;
		}
		public string GetCapability(string name)
		{
			IModelDevice fallBack = this.modelDevice;
			string capability;
			do
			{
				capability = fallBack.GetCapability(name);
				fallBack = fallBack.FallBack;
			}
			while (fallBack != null && capability == null);
			if (capability == null)
			{
				throw new ArgumentException(string.Format("Capability name [{0}] is not found in wurfl.", name));
			}
			return capability;
		}
		public IDictionary<string, string> GetCapabilities()
		{
			IEnumerable<IModelDevice> enumerable = this.Hierarchy();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (IModelDevice current in enumerable)
			{
				Device.AddAll(dictionary, current.GetCapabilities());
			}
			return dictionary;
		}
		private IEnumerable<IModelDevice> Hierarchy()
		{
			Stack<IModelDevice> stack = new Stack<IModelDevice>();
			for (IModelDevice fallBack = this.modelDevice; fallBack != null; fallBack = fallBack.FallBack)
			{
				stack.Push(fallBack);
			}
			return stack;
		}
		private static void AddAll(Dictionary<string, string> first, IDictionary<string, string> second)
		{
			foreach (KeyValuePair<string, string> current in second)
			{
				first[current.Key] = current.Value;
			}
		}
	}
}
