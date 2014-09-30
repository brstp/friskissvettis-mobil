using System;
namespace MoMA.Mobile.Device
{
	public interface IDeviceInfo
	{
		bool IsDesktopBrowser
		{
			get;
		}
		bool IsMobileIE
		{
			get;
		}
		int DisplayWidth
		{
			get;
		}
		int DisplayHeight
		{
			get;
		}
		bool Gif87
		{
			get;
		}
		bool GIF89A
		{
			get;
		}
		bool JPG
		{
			get;
		}
		bool PNG
		{
			get;
		}
		bool UriSchemeTel
		{
			get;
		}
		bool UriSchemeSmsTo
		{
			get;
		}
		bool UriSchemeSms
		{
			get;
		}
		string DeviceModel
		{
			get;
		}
		string DeviceVendor
		{
			get;
		}
		string DeviceYearReleased
		{
			get;
		}
		bool IsJQM
		{
			get;
		}
		bool ScriptSupport
		{
			get;
		}
		bool IsAdvancedDevice
		{
			get;
		}
		bool IsAndroid
		{
			get;
		}
		bool IsIphone
		{
			get;
		}
		bool IsSymbian
		{
			get;
		}
		bool IsWindows
		{
			get;
		}
		bool IsWindowsPhone
		{
			get;
		}
		bool IsBlackberry
		{
			get;
		}
		bool IsBada
		{
			get;
		}
		bool IsProprietary
		{
			get;
		}
		float OSVersion
		{
			get;
		}
		bool IsRobot
		{
			get;
		}
		bool IsTablet
		{
			get;
		}
		bool IsMobilePhone
		{
			get;
		}
		bool OsVersion(float minVersion, float maxVersion);
		bool IsAndroidVersion(float minVersion);
		bool IsAndroidVersion(float minVersion, float maxVersion);
		bool IsIphoneVersion(float minVersion);
		bool IsIphoneVersion(float minVersion, float maxVersion);
		bool IsBlackberryVersion(float minVersion, float maxVersion);
		bool IsBadaVersion(float minVersion, float maxVersion);
		bool IsWindowsVersion(float minVersion, float maxVersion);
	}
}
