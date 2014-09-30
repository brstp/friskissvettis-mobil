using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Hosting;
namespace ImageResizer.Util
{
	[ComVisible(true)]
	public class PathUtils
	{
		public static string AppVirtualPath
		{
			get
			{
				if (HostingEnvironment.ApplicationVirtualPath == null)
				{
					return "/";
				}
				return HostingEnvironment.ApplicationVirtualPath;
			}
		}
		public static string AppPhysicalPath
		{
			get
			{
				if (HostingEnvironment.ApplicationPhysicalPath == null)
				{
					return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
				}
				return HostingEnvironment.ApplicationPhysicalPath;
			}
		}
		public static string SetExtension(string path, string newExtension)
		{
			int num = path.IndexOf('?');
			if (num < 0)
			{
				num = path.Length;
			}
			int num2 = path.LastIndexOfAny(new char[]
			{
				' ',
				'/',
				'\\'
			}, num - 1) + 1;
			int num3 = path.IndexOf('.', num2, num - num2);
			if (num3 < 0)
			{
				num3 = num;
			}
			return path.Substring(0, num3) + "." + newExtension.TrimStart(new char[]
			{
				'.'
			}) + path.Substring(num);
		}
		public static string RemoveFullExtension(string path)
		{
			int num = path.IndexOf('?');
			if (num < 0)
			{
				num = path.Length;
			}
			int num2 = path.LastIndexOfAny(new char[]
			{
				' ',
				'/',
				'\\'
			}, num - 1) + 1;
			int num3 = path.IndexOf('.', num2, num - num2);
			if (num3 < 0)
			{
				num3 = num;
			}
			return path.Substring(0, num3) + path.Substring(num);
		}
		public static string RemoveExtension(string path)
		{
			int num = path.IndexOf('?');
			if (num < 0)
			{
				num = path.Length;
			}
			int num2 = path.LastIndexOfAny(new char[]
			{
				' ',
				'/',
				'\\'
			}, num - 1) + 1;
			int num3 = path.LastIndexOf('.', num - 1, num - num2);
			if (num3 < 0)
			{
				num3 = num;
			}
			return path.Substring(0, num3) + path.Substring(num);
		}
		public static string AddExtension(string path, string newExtension)
		{
			int num = path.IndexOf('?');
			if (num < 0)
			{
				num = path.Length;
			}
			return path.Substring(0, num) + "." + newExtension.TrimStart(new char[]
			{
				'.'
			}) + path.Substring(num);
		}
		public static string GetFullExtension(string path)
		{
			int num = path.IndexOf('?');
			if (num < 0)
			{
				num = path.Length;
			}
			int num2 = path.LastIndexOfAny(new char[]
			{
				' ',
				'/',
				'\\'
			}, num - 1) + 1;
			int num3 = path.IndexOf('.', num2, num - num2);
			if (num3 < 0)
			{
				num3 = num;
			}
			return path.Substring(num3, num - num3);
		}
		public static string GetExtension(string path)
		{
			int num = path.IndexOf('?');
			if (num < 0)
			{
				num = path.Length;
			}
			int num2 = path.LastIndexOfAny(new char[]
			{
				' ',
				'/',
				'\\'
			}, num - 1) + 1;
			int num3 = path.LastIndexOf('.', num - 1, num - num2);
			if (num3 < 0)
			{
				num3 = num;
			}
			return path.Substring(num3, num - num3);
		}
		public static string ResolveAppRelative(string virtualPath)
		{
			if (virtualPath.StartsWith("~", StringComparison.OrdinalIgnoreCase))
			{
				return HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
				{
					'/'
				}) + '/' + virtualPath.TrimStart(new char[]
				{
					'~',
					'/'
				});
			}
			return virtualPath;
		}
		public static string AddQueryString(string virtualPath, string querystring)
		{
			if (virtualPath.IndexOf('?') > -1)
			{
				virtualPath = virtualPath.TrimEnd(new char[]
				{
					'&'
				}) + '&';
			}
			else
			{
				virtualPath += '?';
			}
			return virtualPath + querystring.TrimStart(new char[]
			{
				'&',
				'?'
			});
		}
		public static string MergeOverwriteQueryString(string path, NameValueCollection newQuerystring)
		{
			NameValueCollection nameValueCollection = PathUtils.ParseQueryString(path);
			foreach (string text in newQuerystring.Keys)
			{
				if (text != null)
				{
					nameValueCollection[text] = newQuerystring[text];
				}
			}
			return PathUtils.AddQueryString(PathUtils.RemoveQueryString(path), PathUtils.BuildQueryString(nameValueCollection));
		}
		public static string MergeQueryString(string path, NameValueCollection newQuerystring)
		{
			NameValueCollection nameValueCollection = PathUtils.ParseQueryString(path);
			foreach (string name in nameValueCollection.Keys)
			{
				newQuerystring[name] = nameValueCollection[name];
			}
			return PathUtils.AddQueryString(PathUtils.RemoveQueryString(path), PathUtils.BuildQueryString(newQuerystring));
		}
		public static string BuildQueryString(NameValueCollection QueryString)
		{
			return PathUtils.BuildQueryString(QueryString, true);
		}
		public static string BuildQueryString(NameValueCollection QueryString, bool urlEncode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (QueryString.Count > 0)
			{
				stringBuilder.Append('?');
				foreach (string text in QueryString.Keys)
				{
					if (text != null)
					{
						string text2 = QueryString[text];
						stringBuilder.Append(urlEncode ? HttpUtility.UrlEncode(text) : text);
						stringBuilder.Append('=');
						stringBuilder.Append(urlEncode ? HttpUtility.UrlEncode(text2) : text2);
						stringBuilder.Append('&');
					}
				}
				if (stringBuilder[stringBuilder.Length - 1] == '&')
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}
			return stringBuilder.ToString();
		}
		public static string RemoveQueryString(string path)
		{
			int num = path.IndexOf('?');
			if (num <= -1)
			{
				return path;
			}
			return path.Substring(0, num);
		}
		public static NameValueCollection ParseQueryStringFriendly(string path)
		{
			if (path.IndexOf('?') < 0)
			{
				path = '?' + path;
			}
			return PathUtils.ParseQueryString(path);
		}
		public static NameValueCollection ParseQueryString(string path)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			int num = path.IndexOf('?');
			if (num < 0)
			{
				return nameValueCollection;
			}
			string text = "";
			if (num < path.Length)
			{
				text = path.Substring(num, path.Length - num);
			}
			if (text.Length > 0)
			{
				string[] array = text.Split(new char[]
				{
					'?',
					'&'
				}, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					string[] array3 = text2.Split(new char[]
					{
						'='
					}, StringSplitOptions.RemoveEmptyEntries);
					if (array3.Length == 2)
					{
						nameValueCollection[HttpUtility.UrlDecode(array3[0])] = HttpUtility.UrlDecode(array3[1]);
					}
					else
					{
						nameValueCollection[HttpUtility.UrlDecode(array3[0])] = "";
					}
				}
			}
			return nameValueCollection;
		}
	}
}
