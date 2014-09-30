using System;
using System.Configuration;
namespace MoMA.Mobile.Configuration
{
	internal class ExternalConfiguration : ConfigurationSection
	{
		public const string SECTION_LOCATION = "moma/external";
		[ConfigurationProperty("css", DefaultValue = "true", IsRequired = false)]
		public bool CSS
		{
			get
			{
				return (bool)base["css"];
			}
			set
			{
				base["css"] = value;
			}
		}
		[ConfigurationProperty("js", DefaultValue = "true", IsRequired = false)]
		public bool JS
		{
			get
			{
				return (bool)base["js"];
			}
			set
			{
				base["js"] = value;
			}
		}
		public static ExternalConfiguration GetSection()
		{
			ExternalConfiguration externalConfiguration = (ExternalConfiguration)ConfigurationManager.GetSection("moma/external");
			if (externalConfiguration == null)
			{
				externalConfiguration = new ExternalConfiguration();
				externalConfiguration.CSS = true;
				externalConfiguration.JS = false;
			}
			return externalConfiguration;
		}
	}
}
