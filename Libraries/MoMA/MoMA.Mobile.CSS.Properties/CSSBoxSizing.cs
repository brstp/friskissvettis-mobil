using HtmlAgilityPack;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSBoxSizing : ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "box-sizing";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			return new List<CSSProperty>();
		}
		public List<CSSProperty> GetAllProperties(CSSProperty templateProperty, HtmlNode node)
		{
			return new List<CSSProperty>
			{
				new CSSProperty(templateProperty.Parent)
				{
					Name = "box-sizing",
					Value = templateProperty.Value
				}
			};
		}
	}
}
