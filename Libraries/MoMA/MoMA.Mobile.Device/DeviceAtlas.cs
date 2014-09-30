using Mobi.Mtld.DA;
using System;
using System.Collections;
using System.Linq;
using System.Web;
namespace MoMA.Mobile.Device
{
	public class DeviceAtlas : IDeviceInfo
	{
		public const int DEFAULT_DISPLAY_WIDTH = 320;
		public const int DEFAULT_DISPLAY_HEIGHT = 480;
		public const string DEFAULT_USER_AGENT = "";
		public string UserAgent = "";
		public static DeviceInfo CurrentDevice
		{
			get
			{
				return new DeviceInfo();
			}
		}
		private Hashtable DeviceAtlasHashtable
		{
			get
			{
				Hashtable result;
				try
				{
					string filename = HttpContext.Current.Server.MapPath("~/App_Data/moma/da/latest.json");
					if (HttpContext.Current.Application["DeviceAtlasHashtable"] == null)
					{
						try
						{
							HttpContext.Current.Application["DeviceAtlasHashtable"] = Api.GetTreeFromFile(filename);
						}
						catch
						{
						}
					}
					Hashtable tree = HttpContext.Current.Application["DeviceAtlasHashtable"] as Hashtable;
					result = Api.GetProperties(tree, this.UserAgent);
				}
				catch
				{
					result = new Hashtable();
				}
				return result;
			}
		}
		public bool IsDesktopBrowser
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("isBrowser");
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
				return this.GetIntDeviceAtlasProperty("displayWidth", 320);
			}
		}
		public int DisplayHeight
		{
			get
			{
				return this.GetIntDeviceAtlasProperty("displayHeight", 480);
			}
		}
		public bool Gif87
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("image.Gif87");
			}
		}
		public bool GIF89A
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("image.Gif89a");
			}
		}
		public bool JPG
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("image.Jpg");
			}
		}
		public bool PNG
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("image.Png");
			}
		}
		public bool UriSchemeTel
		{
			get
			{
				return !this.DeviceAtlasHashtable.ContainsKey("uriSchemeTel") || this.DeviceAtlasHashtable["uriSchemeTel"].ToString().Equals("1");
			}
		}
		public bool UriSchemeSmsTo
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("uriSchemeSmsTo");
			}
		}
		public bool UriSchemeSms
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("uriSchemeSms");
			}
		}
		public string DeviceModel
		{
			get
			{
				return this.GetDeviceAtlasProperty("model");
			}
		}
		public string DeviceVendor
		{
			get
			{
				return this.GetDeviceAtlasProperty("vendor");
			}
		}
		public string DeviceYearReleased
		{
			get
			{
				return this.GetDeviceAtlasProperty("yearReleased");
			}
		}
		public bool IsJQM
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("jqm");
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
				return this.GetBoolDeviceAtlasProperty("osAndroid");
			}
		}
		public bool IsIphone
		{
			get
			{
				return this.GetBoolDeviceAtlasProperty("osOsx");
			}
		}
		public bool IsSymbian
		{
			get
			{
				return this.GetDeviceAtlasProperty("osSymbian").ToString() == "1";
			}
		}
		public bool IsWindows
		{
			get
			{
				return this.GetDeviceAtlasProperty("osWindows").ToString() == "1";
			}
		}
		public bool IsWindowsPhone
		{
			get
			{
				return this.GetDeviceAtlasProperty("osWindowsPhone").ToString() == "1";
			}
		}
		public bool IsBlackberry
		{
			get
			{
				return this.GetDeviceAtlasProperty("osRim").ToString() == "1";
			}
		}
		public bool IsBada
		{
			get
			{
				return this.GetDeviceAtlasProperty("osBada").ToString() == "1";
			}
		}
		public bool IsProprietary
		{
			get
			{
				return this.GetDeviceAtlasProperty("osProprietary").ToString() == "1";
			}
		}
		public float OSVersion
		{
			get
			{
				return this.GetFloatProperty("osVersion", 0f);
			}
		}
		public bool IsRobot
		{
			get
			{
				return this.GetDeviceAtlasProperty("isRobot").ToString() == "1";
			}
		}
		public bool IsTablet
		{
			get
			{
				return this.GetDeviceAtlasProperty("isTablet").ToString() == "1";
			}
		}
		public bool IsMobilePhone
		{
			get
			{
				return this.GetDeviceAtlasProperty("isMobilePhone").ToString() == "1";
			}
		}
		public DeviceAtlas()
		{
			try
			{
				this.UserAgent = HttpContext.Current.Request.Headers["User-Agent"];
			}
			catch
			{
				this.UserAgent = "";
			}
		}
		private string GetDeviceAtlasProperty(string PropertyKey)
		{
			if (this.DeviceAtlasHashtable.ContainsKey(PropertyKey))
			{
				return this.DeviceAtlasHashtable[PropertyKey].ToString();
			}
			return "";
		}
		private bool GetBoolDeviceAtlasProperty(string PropertyKey)
		{
			return this.DeviceAtlasHashtable.ContainsKey(PropertyKey) && this.DeviceAtlasHashtable[PropertyKey].ToString().Equals("1");
		}
		private int GetIntDeviceAtlasProperty(string PropertyKey, int DefaultValue)
		{
			if (this.DeviceAtlasHashtable.ContainsKey(PropertyKey))
			{
				int result = DefaultValue;
				string s = this.DeviceAtlasHashtable[PropertyKey].ToString();
				int.TryParse(s, out result);
				return result;
			}
			return DefaultValue;
		}
		private float GetFloatProperty(string PropertyKey, float DefaultValue)
		{
			float result = DefaultValue;
			string text = this.DeviceAtlasHashtable[PropertyKey].ToString();
			if (text.Count((char c) => c.Equals('.')) > 1)
			{
				text = text.Remove(text.IndexOf('.', text.IndexOf('.')));
			}
			float.TryParse(text, out result);
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
