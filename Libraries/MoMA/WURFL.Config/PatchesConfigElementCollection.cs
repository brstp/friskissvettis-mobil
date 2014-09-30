using System;
using System.Configuration;
namespace WURFL.Config
{
	public class PatchesConfigElementCollection : ConfigurationElementCollection
	{
		public PatchConfigurationElement this[int index]
		{
			get
			{
				return (PatchConfigurationElement)base.BaseGet(index);
			}
		}
		protected override string ElementName
		{
			get
			{
				return "patch";
			}
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new PatchConfigurationElement();
		}
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PatchConfigurationElement)element).Path;
		}
	}
}
