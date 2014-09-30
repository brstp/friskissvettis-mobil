using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace MoMA.Helpers
{
	public static class ContextHelper
	{
		public static void OutputImage(Image image)
		{
			ImageCodecInfo encoder = (
				from i in ImageCodecInfo.GetImageDecoders()
				where i.MimeType.Equals("image/jpeg")
				select i).FirstOrDefault<ImageCodecInfo>();
			using (EncoderParameters encoderParameters = new EncoderParameters())
			{
				encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					image.Save(memoryStream, encoder, encoderParameters);
					HttpContext.Current.Response.Clear();
					HttpContext.Current.Response.ContentType = "image/png";
					memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
				}
			}
		}
		public static T GetValueIfContains<T>(string Name, T DefaultValue)
		{
			string text = "";
			string[] allKeys = HttpContext.Current.Request.Form.AllKeys;
			for (int i = 0; i < allKeys.Length; i++)
			{
				string text2 = allKeys[i];
				if (text2.Contains(Name))
				{
					text = text2;
				}
			}
			string[] allKeys2 = HttpContext.Current.Request.QueryString.AllKeys;
			for (int j = 0; j < allKeys2.Length; j++)
			{
				string text3 = allKeys2[j];
				if (text3.Contains(Name))
				{
					text = text3;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				return ContextHelper.GetValue<T>(HttpContext.Current, text, DefaultValue);
			}
			return DefaultValue;
		}
		public static T GetValue<T>(string Name, T DefaultValue)
		{
			return ContextHelper.GetValue<T>(HttpContext.Current, Name, DefaultValue);
		}
		public static T GetValue<T>(HttpContext context, string Name, T DefaultValue)
		{
			T result = DefaultValue;
			if (context.Request.Form[Name] != null)
			{
				result = ConversionHelper.Convert<T>(context.Request.Form[Name], DefaultValue);
			}
			else
			{
				if (context.Request.QueryString[Name] != null)
				{
					result = ConversionHelper.Convert<T>(context.Request.QueryString[Name], DefaultValue);
				}
			}
			return result;
		}
		public static Guid GetGuid(string Name, Guid DefaultValue)
		{
			string g = "";
			Guid result = DefaultValue;
			if (HttpContext.Current.Request.Form[Name] != null)
			{
				g = HttpContext.Current.Request.Form[Name].ToString();
			}
			else
			{
				if (HttpContext.Current.Request.QueryString[Name] != null)
				{
					g = HttpContext.Current.Request.QueryString[Name].ToString();
				}
			}
			try
			{
				result = new Guid(g);
			}
			catch
			{
			}
			return result;
		}
		public static List<T> GetList<T>(string Name, char[] Separators)
		{
			string value = ContextHelper.GetValue<string>(Name, "");
			string[] array = value.Split(Separators);
			if (string.IsNullOrEmpty(value))
			{
				return new List<T>();
			}
			List<T> list = new List<T>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string value2 = array2[i];
				try
				{
					list.Add((T)((object)Convert.ChangeType(value2, typeof(T))));
				}
				catch
				{
				}
			}
			return list;
		}
		public static List<T> GetList<T>(string Name, string Separator)
		{
			string value = ContextHelper.GetValue<string>(Name, "");
			List<string> list = Regex.Split(value, Separator).ToList<string>();
			if (string.IsNullOrEmpty(value))
			{
				return new List<T>();
			}
			List<T> list2 = new List<T>();
			foreach (string current in list)
			{
				try
				{
					list2.Add((T)((object)Convert.ChangeType(current, typeof(T))));
				}
				catch
				{
				}
			}
			return list2;
		}
	}
}
