using System;
using System.Web;
namespace MoMA.Helpers
{
	public class BrowserHelper
	{
		private const string IE = "IE";
		private const string MSIE = "MSIE";
		private const string FIREFOX = "Firefox";
		private const string OPERA = "Opera";
		private const string SAFARI = "Safari";
		private const string MOBILE_IE = "Microsoft Mobile Explorer";
		private const string ERICSSON = "Ericsson";
		private const string SONY_ERICSSON = "Sony Ericsson";
		private const string NOKIA = "Nokia";
		public static bool IsIE
		{
			get
			{
				return BrowserHelper.GetName().Equals("IE") || BrowserHelper.GetName().Equals("MSIE");
			}
		}
		public static bool IsOpera
		{
			get
			{
				return HttpContext.Current.Request.UserAgent.Contains("Opera");
			}
		}
		public static bool IsFirefox
		{
			get
			{
				return BrowserHelper.GetName().Equals("Firefox");
			}
		}
		public static bool IsSafari
		{
			get
			{
				return BrowserHelper.GetName().Equals("Safari");
			}
		}
		public static bool IsChrome
		{
			get
			{
				return HttpContext.Current.Request.UserAgent.Contains("Chrome");
			}
		}
		public static bool IsMobileIE
		{
			get
			{
				return BrowserHelper.GetName().Equals("IE") || BrowserHelper.GetName().Equals("MSIE");
			}
		}
		public static bool IsSonyEricsson
		{
			get
			{
				return BrowserHelper.GetName().Equals("Ericsson") || BrowserHelper.GetName().Equals("Sony Ericsson");
			}
		}
		public static bool INokia
		{
			get
			{
				return BrowserHelper.GetName().Equals("Nokia");
			}
		}
		public static float GetVersion()
		{
			HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
			return (float)((double)browser.MajorVersion + browser.MinorVersion);
		}
		public static string GetName()
		{
			HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
			return browser.Browser;
		}
	}
}
