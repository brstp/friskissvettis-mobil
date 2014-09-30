using System;
namespace MoMA.Mobile.Browser
{
	internal class BrowserInfo
	{
		public BrowserName Name
		{
			get;
			set;
		}
		public double Version
		{
			get;
			set;
		}
		public bool IsIE()
		{
			return BrowserDetection.Detect(this, BrowserName.IE, 0f, 9999f);
		}
		public bool IsIE(float minVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.IE, minVersion, 9999f);
		}
		public bool IsIE(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.IE, minVersion, maxVersion);
		}
		internal bool IsFF()
		{
			return BrowserDetection.Detect(this, BrowserName.FF, 0f, 9999f);
		}
		public bool IsFF(float minVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.FF, minVersion, 9999f);
		}
		internal bool IsFF(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.FF, minVersion, maxVersion);
		}
		public bool IsChrome()
		{
			return BrowserDetection.Detect(this, BrowserName.Chrome, 0f, 9999f);
		}
		public bool IsChrome(float minVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.Chrome, minVersion, 9999f);
		}
		public bool IsChrome(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.Chrome, minVersion, maxVersion);
		}
		public bool IsOpera()
		{
			return BrowserDetection.Detect(this, BrowserName.Opera, 0f, 9999f);
		}
		public bool IsOpera(float minVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.Opera, minVersion, 9999f);
		}
		public bool IsOpera(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.Opera, minVersion, maxVersion);
		}
		public bool IsSafari()
		{
			return BrowserDetection.Detect(this, BrowserName.Safari, 0f, 9999f);
		}
		public bool IsSafari(float minVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.Safari, minVersion, 9999f);
		}
		public bool IsSafari(float minVersion, float maxVersion)
		{
			return BrowserDetection.Detect(this, BrowserName.Safari, minVersion, maxVersion);
		}
	}
}
