using MoMA.Mobile.Configuration;
using System;
namespace MoMA.Mobile.Device
{
	public class DeviceInfo : IDeviceInfo
	{
		public const string DEFAULT_USER_AGENT = "";
		private IDeviceInfo currentDevice;
		public static IDeviceInfo CurrentDevice
		{
			get
			{
				IDeviceInfo result = null;
				switch (DeviceInfo.GetCurrentType())
				{
				case DeviceInfoType.DeviceAtlas:
					result = new DeviceAtlas();
					break;
				case DeviceInfoType.Wurfl:
					result = new Wurfl();
					break;
				}
				return result;
			}
		}
		public bool IsDesktopBrowser
		{
			get
			{
				return this.currentDevice.IsDesktopBrowser;
			}
		}
		public bool IsMobileIE
		{
			get
			{
				return this.currentDevice.IsMobileIE;
			}
		}
		public int DisplayWidth
		{
			get
			{
				return this.currentDevice.DisplayWidth;
			}
		}
		public int DisplayHeight
		{
			get
			{
				return this.currentDevice.DisplayHeight;
			}
		}
		public bool Gif87
		{
			get
			{
				return this.currentDevice.Gif87;
			}
		}
		public bool GIF89A
		{
			get
			{
				return this.currentDevice.GIF89A;
			}
		}
		public bool JPG
		{
			get
			{
				return this.currentDevice.JPG;
			}
		}
		public bool PNG
		{
			get
			{
				return this.currentDevice.PNG;
			}
		}
		public bool UriSchemeTel
		{
			get
			{
				return this.currentDevice.UriSchemeTel;
			}
		}
		public bool UriSchemeSmsTo
		{
			get
			{
				return this.currentDevice.UriSchemeSmsTo;
			}
		}
		public bool UriSchemeSms
		{
			get
			{
				return this.currentDevice.UriSchemeSms;
			}
		}
		public string DeviceModel
		{
			get
			{
				return this.currentDevice.DeviceModel;
			}
		}
		public string DeviceVendor
		{
			get
			{
				return this.currentDevice.DeviceVendor;
			}
		}
		public string DeviceYearReleased
		{
			get
			{
				return this.currentDevice.DeviceYearReleased;
			}
		}
		public bool IsJQM
		{
			get
			{
				return this.currentDevice.IsJQM;
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
				return this.currentDevice.IsAndroid;
			}
		}
		public bool IsIphone
		{
			get
			{
				return this.currentDevice.IsIphone;
			}
		}
		public bool IsSymbian
		{
			get
			{
				return this.currentDevice.IsSymbian;
			}
		}
		public bool IsWindows
		{
			get
			{
				return this.currentDevice.IsWindows;
			}
		}
		public bool IsWindowsPhone
		{
			get
			{
				return this.currentDevice.IsWindowsPhone;
			}
		}
		public bool IsBlackberry
		{
			get
			{
				return this.currentDevice.IsBlackberry;
			}
		}
		public bool IsBada
		{
			get
			{
				return this.currentDevice.IsBada;
			}
		}
		public bool IsProprietary
		{
			get
			{
				return this.currentDevice.IsProprietary;
			}
		}
		public float OSVersion
		{
			get
			{
				return this.currentDevice.OSVersion;
			}
		}
		public bool IsRobot
		{
			get
			{
				return this.currentDevice.IsRobot;
			}
		}
		public bool IsTablet
		{
			get
			{
				return this.currentDevice.IsTablet;
			}
		}
		public bool IsMobilePhone
		{
			get
			{
				return this.currentDevice.IsMobilePhone;
			}
		}
		public static DeviceInfoType GetCurrentType()
		{
			return DeviceDetectionConfiguration.GetSection().DeviceDetectionType;
		}
		public DeviceInfo()
		{
			this.currentDevice = DeviceInfo.CurrentDevice;
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
