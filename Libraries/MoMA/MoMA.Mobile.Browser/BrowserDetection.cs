using MoMA.Mobile.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace MoMA.Mobile.Browser
{
	internal class BrowserDetection
	{
		private static Dictionary<BrowserName, Regex> regexs = new Dictionary<BrowserName, Regex>
		{

			{
				BrowserName.Opera,
				new Regex("((?<Name>Opera).+Version/(?<Version>[\\d]+(\\.\\d+)+)|(?<Name>Opera)/(?<Version>[\\d]+(\\.\\d+)+)|(?<Name>Opera)\\s(?<Version>[\\d]+(\\.\\d+)+))")
			},

			{
				BrowserName.IE,
				new Regex("(?<Name>MSIE)\\s(?<Version>[\\d]+(\\.\\d+)+)")
			},

			{
				BrowserName.FF,
				new Regex("(?<Name>Firefox)/(?<Version>[\\d]+(\\.\\d+)+)")
			},

			{
				BrowserName.Chrome,
				new Regex("(?<Name>Chrome)/(?<Version>[\\d]+(\\.\\d+)+)")
			},

			{
				BrowserName.Safari,
				new Regex("(Version/(?<Version>[\\d]+(\\.\\d+)+).+(?<Name>Safari)|Version/(?<Version>[[\\d]+(\\.\\d+)+)\\.+(?<Name>Safari))")
			}
		};
		public static BrowserInfo Detect()
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent);
		}
		public static bool Detect(BrowserName name, float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent, name, minVersion, maxVersion);
		}
		public static bool Detect(string UA, BrowserName name, float minVersion, float maxVersion)
		{
			BrowserInfo browser = BrowserDetection.Detect(UA);
			return BrowserDetection.Detect(browser, name, minVersion, maxVersion);
		}
		public static bool Detect(BrowserInfo browser, BrowserName name, float minVersion, float maxVersion)
		{
			return browser != null && browser.Name == name && browser.Version >= (double)minVersion && browser.Version <= (double)maxVersion;
		}
		public static BrowserInfo Detect(string UA)
		{
			foreach (KeyValuePair<BrowserName, Regex> current in BrowserDetection.regexs)
			{
				BrowserInfo browserInfo = BrowserDetection.Detect(UA, current.Value);
				if (browserInfo != null)
				{
					return browserInfo;
				}
			}
			return null;
		}
		public static BrowserInfo Detect(string UA, Regex regex)
		{
			BrowserInfo browserInfo = null;
			DeviceInfo deviceInfo = new DeviceInfo();
			UA = ((UA == null) ? "" : UA);
			Match match = regex.Match(UA);
			Group group = match.Groups["Name"];
			Group group2 = match.Groups["Version"];
			if (group.Success && group.Success)
			{
				browserInfo = new BrowserInfo();
				int num = (
					from c in group2.Value
					where c.Equals('.')
					select c).Count<char>();
				string text = group2.Value;
				if (num > 1)
				{
					int num2 = text.IndexOf(".");
					int startIndex = text.IndexOf(".", num2 + 1);
					text = text.Remove(startIndex);
				}
				text = text.Replace(".", ",");
				double version = 0.0;
				double.TryParse(text, out version);
				browserInfo.Version = version;
				string value;
				if ((value = group.Value) != null)
				{
					if (!(value == "MSIE"))
					{
						if (!(value == "Firefox"))
						{
							if (!(value == "Chrome"))
							{
								if (!(value == "Opera"))
								{
									if (value == "Safari")
									{
										if (!deviceInfo.IsIphone)
										{
											browserInfo.Name = BrowserName.Safari;
										}
									}
								}
								else
								{
									browserInfo.Name = BrowserName.Opera;
								}
							}
							else
							{
								browserInfo.Name = BrowserName.Chrome;
							}
						}
						else
						{
							browserInfo.Name = BrowserName.FF;
						}
					}
					else
					{
						browserInfo.Name = BrowserName.IE;
					}
				}
			}
			return browserInfo;
		}
		public static bool IsIE(BrowserInfo browser, float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(browser, BrowserName.IE, minVersion, maxVersion);
		}
		public static bool IsIE(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent, BrowserName.IE, minVersion, maxVersion);
		}
		public static bool IsFF(BrowserInfo browser, float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(browser, BrowserName.FF, minVersion, maxVersion);
		}
		public static bool IsFF(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent, BrowserName.FF, minVersion, maxVersion);
		}
		public static bool IsChrome(BrowserInfo browser, float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(browser, BrowserName.Chrome, minVersion, maxVersion);
		}
		public static bool IsChrome(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent, BrowserName.Chrome, minVersion, maxVersion);
		}
		internal static bool IsOpera(BrowserInfo browser, float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(browser, BrowserName.Opera, minVersion, maxVersion);
		}
		public static bool IsOpera(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent, BrowserName.Opera, minVersion, maxVersion);
		}
		public static bool IsSafari(BrowserInfo browser, float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(browser, BrowserName.Safari, minVersion, maxVersion);
		}
		public static bool IsSafari(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(HttpContext.Current.Request.UserAgent, BrowserName.Safari, minVersion, maxVersion);
		}
	}
}
