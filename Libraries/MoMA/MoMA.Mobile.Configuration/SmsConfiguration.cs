using System;
using System.Configuration;
namespace MoMA.Mobile.Configuration
{
	internal class SmsConfiguration : ConfigurationSection
	{
		public const string SECTION_LOCATION = "moma/sms";
		[ConfigurationProperty("enabled", DefaultValue = false, IsRequired = false)]
		public bool Enabled
		{
			get
			{
				return bool.Parse(base["enabled"].ToString());
			}
			set
			{
				base["enabled"] = value;
			}
		}
		public static SmsConfiguration GetSection()
		{
			SmsConfiguration smsConfiguration = (SmsConfiguration)ConfigurationManager.GetSection("moma/sms");
			if (smsConfiguration == null)
			{
				smsConfiguration = new SmsConfiguration();
				smsConfiguration.Enabled = false;
			}
			return smsConfiguration;
		}
	}
}
