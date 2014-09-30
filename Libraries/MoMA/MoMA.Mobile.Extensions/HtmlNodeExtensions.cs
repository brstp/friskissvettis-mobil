using HtmlAgilityPack;
using MoMA.Mobile.CSS;
using MoMA.Mobile.Device;
using MoMA.Mobile.Html;
using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace MoMA.Mobile.Extensions
{
	internal static class HtmlNodeExtensions
	{
		public static int GetMarginPaddingBorder(this HtmlNode htmlNode, bool width)
		{
			HtmlDocumentWrapper htmlWrapper = htmlNode.GetHtmlWrapper();
			HtmlNodeDimension dimension = htmlWrapper.CachedNodeDimensions.GetDimension(htmlNode);
			if (dimension == null)
			{
				return 0;
			}
			CSSMeasure calculatedMargin = dimension.CalculatedMargin;
			CSSMeasure calculatedPadding = dimension.CalculatedPadding;
			CSSMeasure calculatedBorder = dimension.CalculatedBorder;
			if (width)
			{
				return calculatedMargin.Left + calculatedMargin.Right + calculatedPadding.Left + calculatedPadding.Right + calculatedBorder.Left + calculatedBorder.Right;
			}
			return calculatedMargin.Top + calculatedMargin.Bottom + calculatedPadding.Top + calculatedPadding.Bottom + calculatedBorder.Top + calculatedBorder.Bottom;
		}
		private static int GetCalculated(this HtmlNode htmlNode, bool width)
		{
			HtmlDocumentWrapper htmlWrapper = htmlNode.GetHtmlWrapper();
			HtmlNodeDimension dimension = htmlWrapper.CachedNodeDimensions.GetDimension(htmlNode);
			if (dimension != null)
			{
				if (width)
				{
					return dimension.CalculatedWidth;
				}
				return dimension.CalculatedHeight;
			}
			else
			{
				int containerWidth;
				int containerHeight;
				if (htmlNode.IsBody() || htmlNode.IsHtml())
				{
					containerWidth = DeviceInfo.CurrentDevice.DisplayWidth;
					containerHeight = DeviceInfo.CurrentDevice.DisplayHeight;
				}
				else
				{
					containerWidth = htmlNode.ParentNode.GetCalculatedWidth();
					containerHeight = htmlNode.ParentNode.GetCalculatedHeight();
				}
				CSSMeasure cSSMeasure;
				CSSMeasure cSSMeasure2;
				CSSMeasure cSSMeasure3;
				if (dimension == null)
				{
					CSSProperty cSSProperty = htmlWrapper.GetCSSProperty(htmlNode, "margin", "0px");
					cSSMeasure = new CSSMeasure(cSSProperty.Value, containerWidth, containerHeight);
					CSSProperty cSSProperty2 = htmlWrapper.GetCSSProperty(htmlNode, "padding", "0px");
					cSSMeasure2 = new CSSMeasure(cSSProperty2.Value, containerWidth, containerHeight);
					cSSMeasure3 = new CSSMeasure();
					CSSProperty property = htmlWrapper.CSSStylesheets.GetProperty(htmlNode, new List<string>
					{
						"border",
						"border-width",
						"border-top",
						"border-top-width"
					});
					CSSProperty property2 = htmlWrapper.CSSStylesheets.GetProperty(htmlNode, new List<string>
					{
						"border",
						"border-width",
						"border-right",
						"border-right-width"
					});
					CSSProperty property3 = htmlWrapper.CSSStylesheets.GetProperty(htmlNode, new List<string>
					{
						"border",
						"border-width",
						"border-bottom",
						"border-bottom-width"
					});
					CSSProperty property4 = htmlWrapper.CSSStylesheets.GetProperty(htmlNode, new List<string>
					{
						"border",
						"border-width",
						"border-left",
						"border-left-width"
					});
					cSSMeasure3.Top = ((property != null) ? CSSMeasure.GetIntValue(property.Value, containerWidth, containerHeight) : 0);
					cSSMeasure3.Right = ((property2 != null) ? CSSMeasure.GetIntValue(property2.Value, containerWidth, containerHeight) : 0);
					cSSMeasure3.Bottom = ((property3 != null) ? CSSMeasure.GetIntValue(property3.Value, containerWidth, containerHeight) : 0);
					cSSMeasure3.Left = ((property4 != null) ? CSSMeasure.GetIntValue(property4.Value, containerWidth, containerHeight) : 0);
				}
				else
				{
					cSSMeasure = dimension.CalculatedMargin;
					cSSMeasure2 = dimension.CalculatedPadding;
					cSSMeasure3 = dimension.CalculatedBorder;
				}
				int offsetWidth = cSSMeasure.Left + cSSMeasure.Right + cSSMeasure2.Left + cSSMeasure2.Right + cSSMeasure3.Left + cSSMeasure3.Right;
				int offsetHeight = cSSMeasure.Top + cSSMeasure.Bottom + cSSMeasure2.Top + cSSMeasure2.Bottom + cSSMeasure3.Top + cSSMeasure3.Bottom;
				CSSProperty cSSProperty3 = htmlWrapper.GetCSSProperty(htmlNode, "width", "100CW%");
				CSSProperty cSSProperty4 = htmlWrapper.GetCSSProperty(htmlNode, "height", "100CH%");
				int intValue = CSSMeasure.GetIntValue(cSSProperty3.Value, containerWidth, containerHeight, offsetWidth, offsetHeight);
				int intValue2 = CSSMeasure.GetIntValue(cSSProperty4.Value, containerWidth, containerHeight, offsetWidth, offsetHeight);
				htmlWrapper.CachedNodeDimensions.Update(htmlNode.Id, intValue, intValue2, cSSMeasure, cSSMeasure2, cSSMeasure3);
				if (width)
				{
					return intValue;
				}
				return intValue2;
			}
		}
		public static int GetCalculatedWidth(this HtmlNode htmlNode)
		{
			return htmlNode.GetCalculated(true);
		}
		public static int GetCalculatedHeight(this HtmlNode htmlNode)
		{
			return htmlNode.GetCalculated(false);
		}
		public static HtmlDocumentWrapper GetHtmlWrapper(this HtmlNode htmlNode)
		{
			return (htmlNode.OwnerDocument as ExtendedHtmlDocument).HtmlWrapper;
		}
		public static string GetAttribute(this HtmlNode htmlNode, string name, string defaultValue)
		{
			if (htmlNode.Attributes.Contains(name))
			{
				return htmlNode.Attributes[name].Value;
			}
			return defaultValue;
		}
		public static void Map(this HtmlNode htmlNode, HtmlDocumentWrapper wrapper)
		{
			string pattern = "{[^}]+}";
			MatchEvaluator @object = delegate(Match m)
			{
				string text = m.Value.Replace("{", "").Replace("}", "");
				text = text.Replace("width", htmlNode.GetCalculatedWidth().ToString());
				text = text.Replace("height", htmlNode.GetCalculatedHeight().ToString());
				Expression expression = new Expression(text);
				return expression.Evaluate().ToString();
			};
			MatchEvaluator evaluator = new MatchEvaluator(@object.Invoke);
			foreach (HtmlAttribute current in (IEnumerable<HtmlAttribute>)htmlNode.Attributes)
			{
				current.Value = Regex.Replace(current.Value, pattern, evaluator);
			}
			htmlNode.InnerHtml = Regex.Replace(htmlNode.InnerHtml, pattern, evaluator);
		}
		public static bool IsBody(this HtmlNode htmlNode)
		{
			return htmlNode.Name.ToLower().Equals("body");
		}
		public static bool IsHtml(this HtmlNode htmlNode)
		{
			return htmlNode.Name.ToLower().Equals("html");
		}
		public static bool IsViewstate(this HtmlNode htmlNode)
		{
			return htmlNode.Id != null && htmlNode.Id.Equals("__VIEWSTATE");
		}
		public static bool HasAspNetHiddenClass(this HtmlNode htmlNode)
		{
			return htmlNode.Attributes.Contains("class") && htmlNode.Attributes["class"].Value.Contains("aspNetHidden");
		}
		public static bool IsLink(this HtmlNode htmlNode)
		{
			return htmlNode.Name.ToLower().Equals("a");
		}
		public static bool IsCenter(this HtmlNode htmlNode)
		{
			return htmlNode.Name.ToLower().Equals("center");
		}
		public static bool IsCheckbox(this HtmlNode htmlNode)
		{
			return htmlNode.IsInput("checkbox");
		}
		public static bool IsRadio(this HtmlNode htmlNode)
		{
			return htmlNode.IsInput("radio");
		}
		public static bool IsInput(this HtmlNode htmlNode, string type)
		{
			return htmlNode.Name.ToLower().Equals("input") && htmlNode.Attributes["type"] != null && htmlNode.Attributes["type"].Value.ToLower().Equals(type);
		}
	}
}
