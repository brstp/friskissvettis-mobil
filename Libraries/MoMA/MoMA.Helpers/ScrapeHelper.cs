using MoMA.Helpers.Url;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
namespace MoMA.Helpers
{
	public class ScrapeHelper
	{
		public static string Get(Uri uri)
		{
			HttpWebRequest httpWebRequest;
			if (uri.IsAbsoluteUri)
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			}
			else
			{
				string completeUrl = ExtendedUrl.GetCompleteUrl(uri);
				httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(completeUrl));
			}
			httpWebRequest.ContentType = "utf-8";
			httpWebRequest.Method = "GET";
			httpWebRequest.UserAgent = HttpContext.Current.Request.UserAgent;
			WebResponse webResponse = null;
			try
			{
				webResponse = httpWebRequest.GetResponse();
			}
			catch
			{
				return "";
			}
			string result = uri.ToString();
			try
			{
				StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
				result = streamReader.ReadToEnd();
				streamReader.Close();
				webResponse.Close();
			}
			catch
			{
			}
			return result;
		}
	}
}
