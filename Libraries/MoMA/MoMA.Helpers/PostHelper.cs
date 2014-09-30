using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
namespace MoMA.Helpers
{
	public class PostHelper
	{
		public static string Post(string url, Dictionary<string, string> data)
		{
			string text = "";
			foreach (KeyValuePair<string, string> current in data)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					current.Key,
					"=",
					current,
					"&"
				});
			}
			return PostHelper.Post(url, HttpContext.Current.Server.UrlEncode(text.TrimEnd(new char[]
			{
				'&'
			})));
		}
		public static string Post(string url, string data)
		{
			string text = null;
			try
			{
				byte[] bytes = Encoding.ASCII.GetBytes(data);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = (long)bytes.Length;
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream responseStream = httpWebResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream);
				text = streamReader.ReadToEnd();
			}
			catch
			{
				return "";
			}
			return text.Trim() + "\n";
		}
	}
}
