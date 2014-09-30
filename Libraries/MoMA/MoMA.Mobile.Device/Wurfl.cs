using MoMA.Mobile.Configuration;
using System;
using System.Linq;
using System.Web;
using WURFL;
using WURFL.Config;
namespace MoMA.Mobile.Device
{
	public class Wurfl : IDeviceInfo
	{
		public const int DEFAULT_DISPLAY_WIDTH = 320;
		public const int DEFAULT_DISPLAY_HEIGHT = 480;
		public const string DEFAULT_USER_AGENT = "";
		public const string WurflManagerCacheKey = "__WurflManager";
		public string UserAgent = "";
		private static string _wurflDataPath;
		private static string _wurflPatchFilePath;
		public static DeviceInfo CurrentDevice
		{
			get
			{
				return new DeviceInfo();
			}
		}
		public bool IsDesktopBrowser
		{
			get
			{
				return !this.IsMobilePhone && !this.IsTablet;
			}
		}
		public bool IsMobileIE
		{
			get
			{
				return this.UserAgent.Contains("MSIE") && this.IsMobilePhone;
			}
		}
		public int DisplayWidth
		{
			get
			{
				if (this.UserAgent.Contains("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; XBLWP7; ZuneWP7)"))
				{
					return 480;
				}
				return this.GetIntProperty("resolution_width", 320);
			}
		}
		public int DisplayHeight
		{
			get
			{
				return this.GetIntProperty("resolution_height", 480);
			}
		}
		public bool Gif87
		{
			get
			{
				return this.GetBoolProperty("gif");
			}
		}
		public bool GIF89A
		{
			get
			{
				return this.GetBoolProperty("gif_animated");
			}
		}
		public bool JPG
		{
			get
			{
				return this.GetBoolProperty("jpg");
			}
		}
		public bool PNG
		{
			get
			{
				return this.GetBoolProperty("png");
			}
		}
		public bool UriSchemeTel
		{
			get
			{
				return this.GetProperty("xhtml_make_phone_call_string").Contains("tel");
			}
		}
		public bool UriSchemeSmsTo
		{
			get
			{
				return this.GetProperty("xhtml_send_sms_string").Contains("smsto");
			}
		}
		public bool UriSchemeSms
		{
			get
			{
				return this.GetProperty("xhtml_send_sms_string").Contains("sms");
			}
		}
		public string DeviceModel
		{
			get
			{
				return this.GetProperty("model_name");
			}
		}
		public string DeviceVendor
		{
			get
			{
				return this.GetProperty("brand_name");
			}
		}
		public string DeviceYearReleased
		{
			get
			{
				return null;
			}
		}
		public bool IsJQM
		{
			get
			{
				return true;
			}
		}
		public bool ScriptSupport
		{
			get
			{
				return this.IsJQM || this.IsDesktopBrowser;
			}
		}
		public bool IsAdvancedDevice
		{
			get
			{
				return this.IsJQM;
			}
		}
		public bool IsAndroid
		{
			get
			{
				return this.GetProperty("device_os").Equals("Android");
			}
		}
		public bool IsIphone
		{
			get
			{
				return this.GetProperty("device_os").Equals("iPhone OS");
			}
		}
		public bool IsSymbian
		{
			get
			{
				return this.GetProperty("device_os").Equals("Symbian OS");
			}
		}
		public bool IsWindows
		{
			get
			{
				return this.GetProperty("device_os").Equals("Windows Mobile OS") || this.GetProperty("device_os").Equals("Windows Phone OS");
			}
		}
		public bool IsWindowsPhone
		{
			get
			{
				return this.GetProperty("device_os").Equals("Windows Mobile OS") || this.GetProperty("device_os").Equals("Windows Phone OS");
			}
		}
		public bool IsBlackberry
		{
			get
			{
				return this.GetProperty("device_os").Equals("RIM OS");
			}
		}
		public bool IsBada
		{
			get
			{
				return this.GetProperty("device_os").Equals("Bada OS");
			}
		}
		public bool IsProprietary
		{
			get
			{
				return string.IsNullOrWhiteSpace(this.GetProperty("device_os"));
			}
		}
		public float OSVersion
		{
			get
			{
				return this.GetFloatProperty("device_os_version", 0f);
			}
		}
		public bool IsRobot
		{
			get
			{
				return false;
			}
		}
		public bool IsTablet
		{
			get
			{
				return this.GetBoolProperty("is_tablet");
			}
		}
		public bool IsMobilePhone
		{
			get
			{
				return this.GetBoolProperty("can_assign_phone_number");
			}
		}
		public Wurfl()
		{
			try
			{
				this.UserAgent = HttpContext.Current.Request.UserAgent;
			}
			catch
			{
				this.UserAgent = "";
			}
		}
		static Wurfl()
		{
			Wurfl._wurflDataPath = PathConfiguration.GetSection().BaseDir + "wurfl/wurfl-latest.zip";
			Wurfl._wurflPatchFilePath = PathConfiguration.GetSection().BaseDir + "wurfl/web_browsers_patch.xml";
		}
		public void SetDataPath(string wurflDataPath, string wurflPatchFilePath)
		{
			if (!string.IsNullOrEmpty(wurflDataPath))
			{
				Wurfl._wurflDataPath = wurflDataPath;
			}
			if (!string.IsNullOrEmpty(wurflPatchFilePath))
			{
				Wurfl._wurflPatchFilePath = wurflPatchFilePath;
			}
		}
		public static void Start()
		{
			Wurfl.Start(HttpContext.Current);
		}
		public static void Start(HttpContext context)
		{
			Wurfl.InitializeWurflManager(context, Wurfl._wurflDataPath, Wurfl._wurflPatchFilePath);
		}
		public static IWURFLManager GetManager()
		{
			return Wurfl.GetManager(HttpContext.Current);
		}
		public static IWURFLManager GetManager(HttpContext context)
		{
			IWURFLManager iWURFLManager = context.Cache["__WurflManager"] as IWURFLManager;
			if (iWURFLManager == null)
			{
				return Wurfl.InitializeWurflManager(context, Wurfl._wurflDataPath, Wurfl._wurflPatchFilePath);
			}
			return iWURFLManager;
		}
		private static IWURFLManager InitializeWurflManager(HttpContext context, string wurflDataPath, string wurflPatchFilePath)
		{
			string mainFile = context.Server.MapPath(wurflDataPath);
			string patchFile = context.Server.MapPath(wurflPatchFilePath);
			InMemoryConfigurer configurer = new InMemoryConfigurer().MainFile(mainFile).PatchFile(patchFile);
			IWURFLManager iWURFLManager = WURFLManagerBuilder.Build(configurer);
			context.Cache["__WurflManager"] = iWURFLManager;
			return iWURFLManager;
		}
		private string GetProperty(string PropertyKey)
		{
			IWURFLManager manager = Wurfl.GetManager(HttpContext.Current);
			IDevice deviceForRequest = manager.GetDeviceForRequest(HttpContext.Current.Request);
			return deviceForRequest.GetCapability(PropertyKey).ToString();
		}
		private bool GetBoolProperty(string PropertyKey)
		{
			IWURFLManager manager = Wurfl.GetManager(HttpContext.Current);
			IDevice deviceForRequest = manager.GetDeviceForRequest(HttpContext.Current.Request);
			return deviceForRequest.GetCapability(PropertyKey).ToString().Equals("true");
		}
		private int GetIntProperty(string PropertyKey, int DefaultValue)
		{
			int result = DefaultValue;
			try
			{
				IWURFLManager manager = Wurfl.GetManager(HttpContext.Current);
				IDevice deviceForRequest = manager.GetDeviceForRequest(HttpContext.Current.Request);
				string s = deviceForRequest.GetCapability(PropertyKey).ToString();
				int.TryParse(s, out result);
			}
			catch
			{
			}
			return result;
		}
		private float GetFloatProperty(string PropertyKey, float DefaultValue)
		{
			IWURFLManager manager = Wurfl.GetManager(HttpContext.Current);
			IDevice deviceForRequest = manager.GetDeviceForRequest(HttpContext.Current.Request);
			float result = DefaultValue;
			string text = deviceForRequest.GetCapability(PropertyKey).ToString();
			if (text.Count((char c) => c.Equals('.')) > 1)
			{
				text = text.Remove(text.IndexOf('.', text.IndexOf('.') + 1));
			}
			float.TryParse(text.Replace(".", ","), out result);
			return result;
		}
		public bool OsVersion(float minVersion, float maxVersion)
		{
			return this.OSVersion >= minVersion && this.OSVersion <= maxVersion;
		}
		public bool IsAndroidVersion(float minVersion)
		{
			return this.IsAndroid && this.OsVersion(minVersion, 9999f);
		}
		public bool IsAndroidVersion(float minVersion, float maxVersion)
		{
			return this.IsAndroid && this.OsVersion(minVersion, maxVersion);
		}
		public bool IsIphoneVersion(float minVersion)
		{
			return this.IsIphone && this.OsVersion(minVersion, 9999f);
		}
		public bool IsIphoneVersion(float minVersion, float maxVersion)
		{
			return this.IsIphone && this.OsVersion(minVersion, maxVersion);
		}
		public bool IsBlackberryVersion(float minVersion, float maxVersion)
		{
			return this.IsBlackberry && this.OsVersion(minVersion, maxVersion);
		}
		public bool IsBadaVersion(float minVersion, float maxVersion)
		{
			return this.IsBada && this.OsVersion(minVersion, maxVersion);
		}
		public bool IsWindowsVersion(float minVersion, float maxVersion)
		{
			return this.IsWindows && this.OsVersion(minVersion, maxVersion);
		}
	}
}
