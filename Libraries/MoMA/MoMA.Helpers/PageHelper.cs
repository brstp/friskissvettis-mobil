using System;
using System.Web.UI;
namespace MoMA.Helpers
{
	public class PageHelper
	{
		public static void AddCSSHeadControl(Page page, string content)
		{
			string content2 = "<style type=\"text/css\">" + content + "</style>";
			PageHelper.AddHeadControl(page, 0, content2);
		}
		public static void AddHeadControl(Page page, string content)
		{
			PageHelper.AddHeadControl(page, -1, content);
		}
		public static void AddHeadControl(Page page, int index, string content)
		{
			if (index >= 0)
			{
				page.Header.Controls.AddAt(index, new LiteralControl(content));
				return;
			}
			page.Header.Controls.Add(new LiteralControl(content));
		}
	}
}
