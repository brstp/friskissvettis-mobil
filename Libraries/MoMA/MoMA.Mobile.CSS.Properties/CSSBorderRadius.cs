using HtmlAgilityPack;
using MoMA.Mobile.Browser;
using MoMA.Mobile.Device;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSBorderRadius : ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "border-radius";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			bool flag = true;
			HtmlDocumentWrapper arg_08_0 = templateProperty.HtmlWrapper;
			string value = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			DeviceInfo deviceInfo = new DeviceInfo();
			List<CSSProperty> list = new List<CSSProperty>();
			CSSMeasure cSSMeasure = new CSSMeasure(value, containerWidth, containerHeight);
			BrowserInfo browserInfo = BrowserDetection.Detect();
			if (browserInfo == null)
			{
				browserInfo = new BrowserInfo
				{
					Name = BrowserName.None,
					Version = 0.0
				};
			}
			if (flag || browserInfo.IsFF(1f, 3.6f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "-moz-border-radius",
					Value = cSSMeasure.ToString()
				});
			}
			if (flag || browserInfo.IsSafari(3f, 4f) || deviceInfo.IsIphoneVersion(1f, 3.2f) || deviceInfo.IsAndroidVersion(1f, 1.6f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "-webkit-border-radius",
					Value = cSSMeasure.ToString()
				});
			}
			if (flag || browserInfo.IsOpera() || browserInfo.IsChrome() || browserInfo.IsIE(9f) || browserInfo.IsSafari(5f) || browserInfo.IsFF(4f) || deviceInfo.IsIphoneVersion(4f) || deviceInfo.IsAndroidVersion(2.1f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "border-radius",
					Value = cSSMeasure.ToString()
				});
			}
			return list;
		}
		public List<CSSProperty> GetAllProperties(CSSProperty templateProperty, HtmlNode node)
		{
			HtmlDocumentWrapper arg_06_0 = templateProperty.HtmlWrapper;
			string value = templateProperty.Value;
			int containerWidth = 0;
			int containerHeight = 0;
			List<CSSProperty> list = new List<CSSProperty>();
			CSSMeasure cSSMeasure = new CSSMeasure(value, containerWidth, containerHeight);
			CSSProperty cSSProperty = new CSSProperty(templateProperty.Parent);
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "-moz-border-radius",
				Value = cSSMeasure.ToString()
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "-webkit-border-radius",
				Value = cSSMeasure.ToString()
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "border-radius",
				Value = cSSMeasure.ToString()
			});
			return list;
		}
	}
}
