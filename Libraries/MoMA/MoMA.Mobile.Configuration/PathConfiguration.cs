using System;
using System.Configuration;
using System.IO;
using System.Web;
namespace MoMA.Mobile.Configuration
{
	internal class PathConfiguration : ConfigurationSection
	{
		public const string SECTION_LOCATION = "moma/location";
		[ConfigurationProperty("baseDir", DefaultValue = "~/App_Data/moma/", IsRequired = false)]
		public string BaseDir
		{
			get
			{
				return base["baseDir"].ToString();
			}
			set
			{
				base["baseDir"] = value;
			}
		}
		[ConfigurationProperty("licenseFile", DefaultValue = "~/App_Data/moma/license/license.xml", IsRequired = false)]
		public string LicenseFile
		{
			get
			{
				return base["licenseFile"].ToString();
			}
			set
			{
				base["licenseFile"] = value;
			}
		}
		[ConfigurationProperty("externalImageCache", DefaultValue = "~/App_Data/moma/cache/external", IsRequired = false)]
		public string ExternalImageCache
		{
			get
			{
				string text = base["externalImageCache"].ToString();
				string path = HttpContext.Current.Server.MapPath(text);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return text;
			}
			set
			{
				base["externalImageCache"] = value;
			}
		}
		public static PathConfiguration GetSection()
		{
			PathConfiguration pathConfiguration = (PathConfiguration)ConfigurationManager.GetSection("moma/location");
			if (pathConfiguration == null)
			{
				pathConfiguration = new PathConfiguration();
				pathConfiguration.BaseDir = "~/App_Data/moma/";
				pathConfiguration.LicenseFile = "~/App_Data/moma/license/license.xml";
				pathConfiguration.ExternalImageCache = "~/App_Data/moma/cache/external";
			}
			return pathConfiguration;
		}
	}
}
