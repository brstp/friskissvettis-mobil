using HtmlAgilityPack;
using MoMA.Mobile.Browser;
using MoMA.Mobile.Device;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSBoxShadow : ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "box-shadow";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			bool flag = true;
			HtmlDocumentWrapper arg_08_0 = templateProperty.HtmlWrapper;
			string text = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			DeviceInfo deviceInfo = new DeviceInfo();
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
			if (flag || browserInfo.IsSafari(3f) || browserInfo.IsChrome(0f, 9.99f) || deviceInfo.IsIphoneVersion(3.2f, 5f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "-webkit-box-shadow",
					Value = text
				});
			}
			if (flag || browserInfo.IsFF(3.5f, 3.6f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "-moz-box-shadow",
					Value = text
				});
			}
			if (flag || browserInfo.IsOpera() || browserInfo.IsIE(9f) || browserInfo.IsFF(4f) || browserInfo.IsChrome(10f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "box-shadow",
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
			CSSProperty cSSProperty = new CSSProperty(templateProperty.Parent);
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "-webkit-box-shadow",
				Value = text
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "-moz-box-shadow",
				Value = text
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "box-shadow",
				Value = text
			});
			return list;
		}
	}
}
