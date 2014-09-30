using HtmlAgilityPack;
using MoMA.Mobile.Browser;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSTextShadow : ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "text-shadow";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			bool flag = true;
			HtmlDocumentWrapper arg_08_0 = templateProperty.HtmlWrapper;
			string text = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			List<CSSProperty> list = new List<CSSProperty>();
			List<string> list2 = text.Split(new char[]
			{
				' '
			}).ToList<string>();
			BrowserInfo browserInfo = BrowserDetection.Detect();
			if (browserInfo == null)
			{
				browserInfo = new BrowserInfo
				{
					Name = BrowserName.None,
					Version = 0.0
				};
			}
			if (list2.Count != 4)
			{
				return new List<CSSProperty>();
			}
			int intValue = CSSMeasure.GetIntValue(list2[0], containerWidth, containerHeight);
			int intValue2 = CSSMeasure.GetIntValue(list2[1], containerWidth, containerHeight);
			int intValue3 = CSSMeasure.GetIntValue(list2[2], containerWidth, containerHeight);
			text = string.Concat(new object[]
			{
				intValue,
				"px ",
				intValue2,
				"px ",
				intValue3,
				"px ",
				list2[3]
			});
			if (flag || browserInfo.IsOpera(9f) || browserInfo.IsChrome() || browserInfo.IsSafari(1f) || browserInfo.IsFF(3.5f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "text-shadow",
					Value = text
				});
			}
			return list;
		}
		public List<CSSProperty> GetAllProperties(CSSProperty templateProperty, HtmlNode node)
		{
			HtmlDocumentWrapper arg_06_0 = templateProperty.HtmlWrapper;
			string text = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			List<CSSProperty> list = new List<CSSProperty>();
			List<string> list2 = text.Split(new char[]
			{
				' '
			}).ToList<string>();
			if (list2.Count != 4)
			{
				return new List<CSSProperty>();
			}
			int intValue = CSSMeasure.GetIntValue(list2[0], containerWidth, containerHeight);
			int intValue2 = CSSMeasure.GetIntValue(list2[1], containerWidth, containerHeight);
			text = string.Concat(new object[]
			{
				intValue,
				"px ",
				intValue2,
				"px ",
				list2[2],
				" ",
				list2[3]
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "text-shadow",
				Value = text
			});
			return list;
		}
	}
}
