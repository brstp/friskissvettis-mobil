using System;
using System.Collections.Generic;
using System.Configuration;
namespace WURFL.Config
{
	public class WURFLConfigurationSection : ConfigurationSection
	{
		private const string MainFileElementName = "mainFile";
		private const string PatchesElementName = "patches";
		[ConfigurationProperty("mainFile")]
		public FileLocationConfigElement MainFileConfigurationElement
		{
			get
			{
				return (FileLocationConfigElement)base["mainFile"];
			}
		}
		[ConfigurationCollection(typeof(PatchesConfigElementCollection), AddItemName = "patch"), ConfigurationProperty("patches")]
		public PatchesConfigElementCollection PatchesLocation
		{
			get
			{
				return (PatchesConfigElementCollection)base["patches"];
			}
		}
		public string MainFile
		{
			get
			{
				return this.MainFileConfigurationElement.Path;
			}
		}
		public string[] Patches
		{
			get
			{
				return WURFLConfigurationSection.PatchesPath(this.PatchesLocation);
			}
		}
		private static string[] PatchesPath(PatchesConfigElementCollection patchesLocation)
		{
			List<string> list = new List<string>(patchesLocation.Count);
			foreach (PatchConfigurationElement patchConfigurationElement in patchesLocation)
			{
				list.Add(patchConfigurationElement.Path);
			}
			return list.ToArray();
		}
	}
}
