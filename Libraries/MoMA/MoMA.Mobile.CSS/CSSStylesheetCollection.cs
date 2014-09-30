using HtmlAgilityPack;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS
{
	internal class CSSStylesheetCollection : List<CSSStylesheet>
	{
		public HtmlDocumentWrapper Parent
		{
			get;
			private set;
		}
		public CSSStylesheetCollection(HtmlDocumentWrapper Parent)
		{
			this.Parent = Parent;
		}
		public void AddRange(HtmlDocumentWrapper htmlWrapper)
		{
			HtmlNodeCollection allStylingNodes = htmlWrapper.GetAllStylingNodes();
			foreach (HtmlNode current in (IEnumerable<HtmlNode>)allStylingNodes)
			{
				CSSStylesheet item = new CSSStylesheet(htmlWrapper, current);
				base.Add(item);
			}
		}
		public CSSProperty GetProperty(HtmlNode node, List<string> propertyNames)
		{
			for (int i = base.Count - 1; i >= 0; i--)
			{
				CSSStylesheet cSSStylesheet = base[i];
				for (int j = cSSStylesheet.Count - 1; j >= 0; j--)
				{
					CSSSection cSSSection = cSSStylesheet[j];
					if (this.Parent.GetNodesByCssSelector(cSSSection.Selector).Contains(node))
					{
						for (int k = cSSSection.Properties.Count - 1; k >= 0; k--)
						{
							CSSProperty cSSProperty = cSSSection.Properties[k];
							if (propertyNames.Contains(cSSProperty.Name.ToLower()))
							{
								return cSSProperty;
							}
						}
					}
				}
			}
			return null;
		}
	}
}
