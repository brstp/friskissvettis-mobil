using MoMA.Mobile.CSS;
using System;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
namespace MoMA.Mobile.UI.Controls
{
	public class GoogleAnalytics : MobileControl
	{
		private const string Version = "4.4sa";
		private const string CookieName = "__utmmobile";
		private const string CookiePath = "/";
		private readonly TimeSpan CookieUserPersistence = TimeSpan.FromSeconds(63072000.0);
		private static readonly Regex IpAddressMatcher = new Regex("^([^.]+\\.[^.]+\\.[^.]+\\.).*");
		[Category("Appearance"), DefaultValue(false), Description("")]
		public string AnalyticsAccountID
		{
			get;
			set;
		}
		private static bool IsEmpty(string input)
		{
			return input == null || "-" == input || "" == input;
		}
		private static string GetIP(string remoteAddress)
		{
			if (GoogleAnalytics.IsEmpty(remoteAddress))
			{
				return "";
			}
			Match match = GoogleAnalytics.IpAddressMatcher.Match(remoteAddress);
			if (match.Success)
			{
				return match.Groups[1] + "0";
			}
			return "";
		}
		private static string GetVisitorId(string guid, string account, string userAgent, HttpCookie cookie)
		{
			if (cookie != null && cookie.Value != null)
			{
				return cookie.Value;
			}
			string s;
			if (!GoogleAnalytics.IsEmpty(guid))
			{
				s = guid + account;
			}
			else
			{
				s = userAgent + GoogleAnalytics.GetRandomNumber() + Guid.NewGuid().ToString();
			}
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			byte[] value = mD5CryptoServiceProvider.ComputeHash(bytes);
			string text = BitConverter.ToString(value);
			text = text.Replace("-", "");
			text = text.PadLeft(32, '0');
			return "0x" + text.Substring(0, 16);
		}
		private static string GetRandomNumber()
		{
			Random random = new Random();
			return random.Next(2147483647).ToString();
		}
		private void SendRequestToGoogleAnalytics(string utmUrl)
		{
			try
			{
				WebRequest webRequest = WebRequest.Create(utmUrl);
				((HttpWebRequest)webRequest).UserAgent = HttpContext.Current.Request.UserAgent;
				webRequest.Headers.Add("Accepts-Language", HttpContext.Current.Request.Headers.Get("Accepts-Language"));
				using (webRequest.GetResponse())
				{
				}
			}
			catch (Exception innerException)
			{
				if (HttpContext.Current.Request.QueryString.Get("utmdebug") != null)
				{
					throw new Exception("Error contacting Google Analytics", innerException);
				}
			}
		}
		private void TrackPageView(string utmr, string utmp, string utmdt, string utmac)
		{
			(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds.ToString();
			string text = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
			if (GoogleAnalytics.IsEmpty(text))
			{
				text = "";
			}
			string text2 = HttpContext.Current.Request.UserAgent;
			if (GoogleAnalytics.IsEmpty(text2))
			{
				text2 = "";
			}
			HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("__utmmobile");
			string visitorId = GoogleAnalytics.GetVisitorId(HttpContext.Current.Request.Headers.Get("X-DCMGUID"), utmac, text2, cookie);
			HttpCookie httpCookie = new HttpCookie("__utmmobile");
			httpCookie.Value = visitorId;
			httpCookie.Expires = DateTime.Now + this.CookieUserPersistence;
			httpCookie.Path = "/";
			HttpContext.Current.Response.Cookies.Add(httpCookie);
			string text3 = "http://www.google-analytics.com/__utm.gif";
			string utmUrl = string.Concat(new string[]
			{
				text3,
				"?utmwv=4.4sa&utmn=",
				GoogleAnalytics.GetRandomNumber(),
				"&utmhn=",
				HttpUtility.UrlEncode(text),
				"&utmr=",
				HttpUtility.UrlEncode(utmr),
				"&utmp=",
				HttpUtility.UrlEncode(utmp),
				"&utmdt=",
				HttpUtility.UrlEncode(utmdt),
				"&utmac=",
				utmac,
				"&utmcc=__utma%3D999.999.999.999.999.1%3B&utmvid=",
				visitorId,
				"&utmip=",
				GoogleAnalytics.GetIP(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"])
			});
			this.SendRequestToGoogleAnalytics(utmUrl);
		}
		internal override void RegisterJsIncludes()
		{
		}
		internal override void RegisterJsScripts()
		{
		}
		internal override void RegisterStartupJsScripts()
		{
		}
		internal override string BuildCSS(CSSStylesheet stylesheet)
		{
			return "";
		}
		internal override string BuildHtml()
		{
			return "";
		}
		protected override void Render(HtmlTextWriter writer)
		{
			string analyticsAccountID = this.AnalyticsAccountID;
			Random random = new Random();
			random.Next(2147483647).ToString();
			string text = "-";
			if (HttpContext.Current.Request.UrlReferrer != null && "" != HttpContext.Current.Request.UrlReferrer.ToString())
			{
				text = HttpContext.Current.Request.UrlReferrer.ToString();
			}
			string utmr = text;
			string utmp = "";
			if (HttpContext.Current.Request.Url != null)
			{
				utmp = HttpContext.Current.Request.Url.PathAndQuery;
			}
			string title = this.Page.Title;
			this.TrackPageView(utmr, utmp, title, analyticsAccountID);
		}
	}
}
