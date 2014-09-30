using MostThingsWeb;
using System;
namespace MoMA.Helpers
{
	public class CssHelper
	{
		public static string SelectorToXPath(string selector)
		{
			return css2xpath.Transform(selector);
		}
		public static string ExtractUrl(string value)
		{
			int num = value.ToLower().IndexOf("url(");
			int num2 = value.ToLower().IndexOf(")", num);
			int startIndex = num2 - num;
			if (num >= 0 && num2 > num)
			{
				return value.Remove(0, num).Remove(startIndex).Remove(0, 4);
			}
			return "";
		}
	}
}
