using HtmlAgilityPack;
using MoMA.Mobile.CSS.Properties.Helpers;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSBackgroundWidth : CSSImageBackgroundHelper, ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "background-width";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			return this.GetAllProperties(templateProperty, node);
		}
		public List<CSSProperty> GetAllProperties(CSSProperty templateProperty, HtmlNode node)
		{
			HtmlDocumentWrapper htmlWrapper = templateProperty.HtmlWrapper;
			string value = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			int intValue = CSSMeasure.GetIntValue(value, containerWidth, containerHeight);
			new List<CSSProperty>();
			Dictionary<string, string> values = new Dictionary<string, string>
			{

				{
					"width",
					intValue.ToString()
				},

				{
					"scale",
					"both"
				}
			};
			return base.UpdateImageUrlQuerystring(values, node, htmlWrapper);
		}
	}
}
