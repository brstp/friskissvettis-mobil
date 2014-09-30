using HtmlAgilityPack;
using MoMA.Helpers;
using MoMA.Helpers.Url;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS.Properties.Helpers
{
	internal class CSSImageBackgroundHelper
	{
		protected List<CSSProperty> GetUsedProperties(List<string> propertyNames, string contains, HtmlNode node, HtmlDocumentWrapper wrapper)
		{
			List<CSSProperty> list = new List<CSSProperty>();
			foreach (string current in propertyNames)
			{
				CSSProperty cSSProperty = wrapper.GetCSSProperty(node, current, "");
				if (cSSProperty != null && !string.IsNullOrEmpty(cSSProperty.Value) && cSSProperty.Value.Contains(contains))
				{
					list.Add(cSSProperty);
				}
			}
			return list;
		}
		protected List<CSSProperty> UpdateImageUrlQuerystring(Dictionary<string, string> values, HtmlNode node, HtmlDocumentWrapper wrapper)
		{
			List<CSSProperty> list = new List<CSSProperty>();
			List<CSSProperty> usedProperties = this.GetUsedProperties(new List<string>
			{
				"background",
				"background-image"
			}, "url(", node, wrapper);
			foreach (CSSProperty current in usedProperties)
			{
				string text = CssHelper.ExtractUrl(current.Value);
				ExtendedUrl extendedUrl = new ExtendedUrl(text);
				if (extendedUrl.Valid && extendedUrl.Localhost)
				{
					foreach (KeyValuePair<string, string> current2 in values)
					{
						extendedUrl.Querystring.Add(current2.Key, current2.Value);
					}
					current.Value = current.Value.Replace("url(" + text + ")", "url('" + extendedUrl + "')");
					list.Add(current);
				}
			}
			return list;
		}
	}
}
