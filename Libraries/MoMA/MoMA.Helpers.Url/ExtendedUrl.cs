using System;
using System.Linq;
using System.Web;
namespace MoMA.Helpers.Url
{
	public class ExtendedUrl
	{
		public const string DEFAULT_URL = "http://localhost:80/";
		public string Url
		{
			get;
			set;
		}
		public Querystring Querystring
		{
			get;
			private set;
		}
		public bool Localhost
		{
			get
			{
				return ExtendedUrl.IsLocalhost(this.Url);
			}
		}
		public bool Valid
		{
			get
			{
				return ExtendedUrl.IsValid(this.Url);
			}
		}
		public bool External
		{
			get
			{
				return ExtendedUrl.IsExternal(this.Url);
			}
		}
		public string Extension
		{
			get
			{
				return ExtendedUrl.GetExtension(this.Url);
			}
		}
		public ExtendedUrl(string url)
		{
			if (url.Contains('?'))
			{
				this.Url = url.Split(new char[]
				{
					'?'
				}).FirstOrDefault<string>();
			}
			else
			{
				this.Url = url;
			}
			this.Querystring = new Querystring(url);
		}
		public static bool IsLocalhost(string url)
		{
			Uri uri = null;
			return !Uri.TryCreate(url, UriKind.Absolute, out uri) || uri.Host.Equals("localhost");
		}
		public static bool IsLocalhost()
		{
			return ExtendedUrl.IsLocalhost(HttpContext.Current.Request.Url.ToString());
		}
		public static bool IsExternal(string url)
		{
			return !ExtendedUrl.IsLocalhost(url);
		}
		public static bool IsExternal()
		{
			return ExtendedUrl.IsExternal(HttpContext.Current.Request.Url.ToString());
		}
		public static bool IsValid(string url)
		{
			Uri uri = null;
			return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
		}
		public static bool IsValid()
		{
			return ExtendedUrl.IsValid(HttpContext.Current.Request.Url.ToString());
		}
		public static string GetExtension(string url)
		{
			int num = url.LastIndexOf('.');
			return url.Remove(0, num + 1);
		}
		public static string GetCurrentDomain()
		{
			string text = "http://" + ExtendedUrl.GetCurrentUri().Host;
			if (ExtendedUrl.GetCurrentUri().Port != 80)
			{
				text = text + ":" + ExtendedUrl.GetCurrentUri().Port.ToString();
			}
			return text;
		}
		public static Uri GetCurrentUri()
		{
			try
			{
				return HttpContext.Current.Request.Url;
			}
			catch
			{
			}
			return new Uri("http://localhost:80/");
		}
		public static string GetCompleteUrl(Uri uri)
		{
			string text = uri.ToString();
			if (text.StartsWith("/"))
			{
				return ExtendedUrl.GetCurrentDomain() + text;
			}
			Uri currentUri = ExtendedUrl.GetCurrentUri();
			string[] segments = currentUri.Segments;
			return ExtendedUrl.GetCurrentDomain() + string.Join("", segments, 0, segments.Count<string>() - 1) + uri.ToString();
		}
		public override string ToString()
		{
			return this.Url + this.Querystring.ToString();
		}
	}
}
