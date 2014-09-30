using HtmlAgilityPack;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS.Properties
{
	internal interface ICSSPropertyHelper
	{
		string PropertyName
		{
			get;
		}
		List<CSSProperty> GetProperties(CSSProperty property, HtmlNode node);
		List<CSSProperty> GetAllProperties(CSSProperty property, HtmlNode node);
	}
}
