using HtmlAgilityPack;
using MoMA.Mobile.Html;
using MoMA.Mobile.Pages;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.Test
{
	internal class Mockup
	{
		public HtmlDocumentWrapper HtmlDocumentWrapperMockup
		{
			get;
			set;
		}
		public MobilePage mobilePage
		{
			get;
			set;
		}
		public HtmlNode HtmlNodeMockup
		{
			get;
			set;
		}
		public string HtmlMarkup
		{
			get;
			private set;
		}
		public Mockup(Dictionary<string, string> cssProperties)
		{
			string text = Environment.NewLine;
			foreach (KeyValuePair<string, string> current in cssProperties)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					current.Key,
					": ",
					current.Value,
					";",
					Environment.NewLine
				});
			}
			this.HtmlMarkup = "\r\n                <html>\r\n                    <head>\r\n                        <style type=\"text/css\">\r\n                            #item {" + text + "}\r\n                        </style>\r\n                    </head>\r\n                    <body>\r\n                        <div id=\"item\" />\r\n                    </body>\r\n                </html>";
			this.mobilePage = new MobilePage();
			this.HtmlDocumentWrapperMockup = new HtmlDocumentWrapper(this.HtmlMarkup, this.mobilePage);
			this.HtmlNodeMockup = this.HtmlDocumentWrapperMockup.GetNodeByCssSelector("#item");
			this.HtmlDocumentWrapperMockup.SetupCSS();
		}
	}
}
