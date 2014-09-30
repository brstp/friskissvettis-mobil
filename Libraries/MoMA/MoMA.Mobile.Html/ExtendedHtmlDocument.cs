using HtmlAgilityPack;
using System;
namespace MoMA.Mobile.Html
{
	internal class ExtendedHtmlDocument : HtmlDocument
	{
		public HtmlDocumentWrapper HtmlWrapper
		{
			get;
			set;
		}
		public ExtendedHtmlDocument(HtmlDocumentWrapper HtmlWrapper)
		{
			this.HtmlWrapper = HtmlWrapper;
		}
	}
}
