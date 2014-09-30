using HtmlAgilityPack;
using MoMA.Mobile.Browser;
using MoMA.Mobile.Device;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MoMA.Mobile.CSS.Properties
{
	internal class CSSBackgroundLinearGradient : ICSSPropertyHelper
	{
		public string PropertyName
		{
			get
			{
				return "background-linear-gradient";
			}
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node)
		{
			BrowserInfo browser = BrowserDetection.Detect();
			DeviceInfo deviceInfo = new DeviceInfo();
			return this.GetProperties(templateProperty, node, browser, deviceInfo);
		}
		public List<CSSProperty> GetProperties(CSSProperty templateProperty, HtmlNode node, BrowserInfo browser, DeviceInfo deviceInfo)
		{
			bool flag = true;
			HtmlDocumentWrapper arg_08_0 = templateProperty.HtmlWrapper;
			string value = templateProperty.Value;
			List<CSSProperty> list = new List<CSSProperty>();
			List<string> list2 = value.Trim().Split(new char[]
			{
				' '
			}).ToList<string>();
			if (browser == null)
			{
				browser = new BrowserInfo
				{
					Name = BrowserName.None,
					Version = 0.0
				};
			}
			if (flag || browser.IsFF(3.6f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "background",
					Value = string.Concat(new string[]
					{
						"-moz-linear-gradient(center top, ",
						list2[0],
						", ",
						list2[1],
						") repeat scroll 0 0 ",
						list2[0]
					})
				});
			}
			if (flag || browser.IsSafari(4f, 5f) || browser.IsChrome(0f, 9.9f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "background-image",
					Value = string.Concat(new string[]
					{
						"-webkit-gradient(linear, left top, left bottom, from(",
						list2[0],
						"), to(",
						list2[1],
						"))"
					})
				});
			}
			if (flag || browser.IsChrome(10f) || browser.IsSafari(5.1f) || deviceInfo.IsBlackberry || deviceInfo.IsAndroid)
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "background-image",
					Value = string.Concat(new string[]
					{
						"-webkit-linear-gradient(top, ",
						list2[0],
						", ",
						list2[1],
						")"
					})
				});
			}
			if (flag || browser.IsIE(10f))
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "background-image",
					Value = string.Concat(new string[]
					{
						"-ms-linear-gradient(top, ",
						list2[0],
						", ",
						list2[1],
						")"
					})
				});
			}
			if (flag || browser.IsOpera())
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "background-image",
					Value = string.Concat(new string[]
					{
						"-o-linear-gradient(top, ",
						list2[0],
						", ",
						list2[1],
						")"
					})
				});
			}
			if (flag || browser.IsIE(6f, 9f) || deviceInfo.IsMobileIE)
			{
				list.Add(new CSSProperty(templateProperty.Parent)
				{
					Name = "filter",
					Value = string.Concat(new string[]
					{
						"progid:DXImageTransform.Microsoft.gradient(startColorStr='",
						list2[0],
						"', EndColorStr='",
						list2[1],
						"')"
					})
				});
			}
			return list;
		}
		public List<CSSProperty> GetAllProperties(CSSProperty templateProperty, HtmlNode node)
		{
			string value = templateProperty.Value;
			List<CSSProperty> list = new List<CSSProperty>();
			List<string> list2 = value.Trim().Split(new char[]
			{
				' '
			}).ToList<string>();
			CSSProperty cSSProperty = new CSSProperty(templateProperty.Parent);
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "background",
				Value = string.Concat(new string[]
				{
					"-moz-linear-gradient(center top, ",
					list2[0],
					", ",
					list2[1],
					") repeat scroll 0 0 ",
					list2[0]
				})
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "background",
				Value = string.Concat(new string[]
				{
					"-webkit-gradient(linear, left top, left bottom, from(",
					list2[0],
					"), to(",
					list2[1],
					"))",
					list2[0]
				})
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "background",
				Value = string.Concat(new string[]
				{
					"-webkit-linear-gradient(top, ",
					list2[0],
					", ",
					list2[1],
					")",
					list2[0]
				})
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "background",
				Value = string.Concat(new string[]
				{
					"-ms-linear-gradient(top, ",
					list2[0],
					", ",
					list2[1],
					")",
					list2[0]
				})
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "background",
				Value = string.Concat(new string[]
				{
					"-o-linear-gradient(top, ",
					list2[0],
					", ",
					list2[1],
					")",
					list2[0]
				})
			});
			list.Add(new CSSProperty(templateProperty.Parent)
			{
				Name = "filter",
				Value = string.Concat(new string[]
				{
					"progid:DXImageTransform.Microsoft.gradient(startColorStr='",
					list2[0],
					"', EndColorStr='",
					list2[1],
					"')"
				})
			});
			return list;
		}
	}
}
