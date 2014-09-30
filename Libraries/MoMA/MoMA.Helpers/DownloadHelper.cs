using System;
using System.Net;
namespace MoMA.Helpers
{
	public class DownloadHelper
	{
		public static bool Download(string url, string savePath)
		{
			try
			{
				WebClient webClient = new WebClient();
				webClient.DownloadFile(new Uri(url), savePath);
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
