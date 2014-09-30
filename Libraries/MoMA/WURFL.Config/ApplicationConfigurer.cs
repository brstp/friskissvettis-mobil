using System;
using System.Configuration;
namespace WURFL.Config
{
	public class ApplicationConfigurer : IWURFLConfigurer
	{
		private const string WURFLConfigurationSectionName = "wurfl";
		public Configuration Build()
		{
			WURFLConfigurationSection wURFLConfigurationSection = (WURFLConfigurationSection)ConfigurationManager.GetSection("wurfl");
			return new Configuration(wURFLConfigurationSection.MainFile, wURFLConfigurationSection.Patches);
		}
	}
}
