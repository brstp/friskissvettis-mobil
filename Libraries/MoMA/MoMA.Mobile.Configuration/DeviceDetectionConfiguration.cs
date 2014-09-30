using MoMA.Mobile.Device;
using System;
using System.Configuration;
namespace MoMA.Mobile.Configuration
{
	internal class DeviceDetectionConfiguration : ConfigurationSection
	{
		public const string SECTION_LOCATION = "moma/deviceDetection";
		[ConfigurationProperty("deviceDetectionType", DefaultValue = DeviceInfoType.Wurfl, IsRequired = false)]
		public DeviceInfoType DeviceDetectionType
		{
			get
			{
				return (DeviceInfoType)base["deviceDetectionType"];
			}
			set
			{
				base["deviceDetectionType"] = value;
			}
		}
		public static DeviceDetectionConfiguration GetSection()
		{
			DeviceDetectionConfiguration deviceDetectionConfiguration = (DeviceDetectionConfiguration)ConfigurationManager.GetSection("moma/deviceDetection");
			if (deviceDetectionConfiguration == null)
			{
				deviceDetectionConfiguration = new DeviceDetectionConfiguration();
				deviceDetectionConfiguration.DeviceDetectionType = DeviceInfoType.Wurfl;
			}
			return deviceDetectionConfiguration;
		}
	}
}
