using HtmlAgilityPack;
using MoMA.Mobile.CSS.Properties.Helpers;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSBackgroundCrop : CSSImageBackgroundHelper, ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "background-crop";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			return this.GetAllProperties(templateProperty, node);
		}
		public List<CSSProperty> GetAllProperties(CSSProperty templateProperty, HtmlNode node)
		{
			HtmlDocumentWrapper htmlWrapper = templateProperty.HtmlWrapper;
			string text = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			CSSMeasure.GetIntValue(text, containerWidth, containerHeight);
			text = CSSMeasure.GetValue(text, containerWidth, containerHeight).Replace("px", "");
			new List<CSSProperty>();
			Dictionary<string, string> values = new Dictionary<string, string>
			{

				{
					"crop",
					text
				}
			};
			return base.UpdateImageUrlQuerystring(values, node, htmlWrapper);
		}
	}
}
