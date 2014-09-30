using System;
using System.Configuration;
namespace MoMA.Mobile.Configuration
{
	internal class MinimizeConfiguration : ConfigurationSection
	{
		public const string SECTION_LOCATION = "moma/minimize";
		[ConfigurationProperty("html", DefaultValue = "true", IsRequired = false)]
		public bool HTML
		{
			get
			{
				return (bool)base["html"];
			}
			set
			{
				base["html"] = value;
			}
		}
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
		public static MinimizeConfiguration GetSection()
		{
			MinimizeConfiguration minimizeConfiguration = (MinimizeConfiguration)ConfigurationManager.GetSection("moma/minimize");
			if (minimizeConfiguration == null)
			{
				minimizeConfiguration = new MinimizeConfiguration();
				minimizeConfiguration.HTML = true;
				minimizeConfiguration.CSS = true;
				minimizeConfiguration.JS = false;
			}
			return minimizeConfiguration;
		}
	}
}
