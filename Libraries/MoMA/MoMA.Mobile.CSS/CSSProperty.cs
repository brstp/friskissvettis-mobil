using HtmlAgilityPack;
using MoMA.Mobile.CSS.Properties;
using MoMA.Mobile.Device;
using MoMA.Mobile.Extensions;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS
{
	internal class CSSProperty
	{
		public string Name
		{
			get;
			set;
		}
		public string Value
		{
			get;
			set;
		}
		public CSSSection Parent
		{
			get;
			private set;
		}
		public CSSStylesheet Stylesheet
		{
			get
			{
				if (this.Parent != null)
				{
					return this.Parent.Parent;
				}
				return null;
			}
		}
		public HtmlDocumentWrapper HtmlWrapper
		{
			get
			{
				if (this.Parent != null && this.Parent.Parent != null)
				{
					return this.Parent.Parent.Parent;
				}
				return null;
			}
		}
		public static List<ICSSPropertyHelper> PropertyHelpers
		{
			get;
			set;
		}
		public CSSProperty(CSSSection Parent)
		{
			this.Name = "";
			this.Value = "";
			this.Parent = Parent;
			if (CSSProperty.PropertyHelpers == null)
			{
				CSSProperty.PropertyHelpers = new List<ICSSPropertyHelper>();
				CSSProperty.PropertyHelpers.Add(new CSSBackgroundCrop());
				CSSProperty.PropertyHelpers.Add(new CSSBackgroundHeight());
				CSSProperty.PropertyHelpers.Add(new CSSBackgroundLinearGradient());
				CSSProperty.PropertyHelpers.Add(new CSSBackgroundWidth());
				CSSProperty.PropertyHelpers.Add(new CSSBorderRadius());
				CSSProperty.PropertyHelpers.Add(new CSSBoxShadow());
				CSSProperty.PropertyHelpers.Add(new CSSBoxSizing());
				CSSProperty.PropertyHelpers.Add(new CSSTextShadow());
			}
		}
		public string IsPropertyPart()
		{
			if (this.Name.ToLower().EndsWith("-top"))
			{
				return "top";
			}
			if (this.Name.ToLower().EndsWith("-right"))
			{
				return "right";
			}
			if (this.Name.ToLower().EndsWith("-bottom"))
			{
				return "bottom";
			}
			if (this.Name.ToLower().EndsWith("-left"))
			{
				return "left";
			}
			return null;
		}
		public bool ContainUrl()
		{
			return this.Value.ToLower().Contains("url(");
		}
		public static void RegisterCSSPropertyHelper<T>() where T : ICSSPropertyHelper, new()
		{
			foreach (ICSSPropertyHelper current in CSSProperty.PropertyHelpers)
			{
				if (current is T)
				{
					return;
				}
			}
			ICSSPropertyHelper item = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			CSSProperty.PropertyHelpers.Add(item);
		}
		public List<CSSProperty> UpdateAgainstNodeAndDevice(HtmlNode node, bool getAll)
		{
			HtmlDocumentWrapper arg_06_0 = this.HtmlWrapper;
			List<CSSProperty> list = new List<CSSProperty>();
			bool flag = false;
			foreach (ICSSPropertyHelper current in CSSProperty.PropertyHelpers)
			{
				if (this.Name.Equals(current.PropertyName))
				{
					flag = true;
					if (getAll)
					{
						list.AddRange(current.GetAllProperties(this, node));
					}
					else
					{
						list.AddRange(current.GetProperties(this, node));
					}
					int arg_6C_0 = list.Count;
				}
			}
			new DeviceInfo();
			int containerWidth = 0;
			int containerHeight = 0;
			if (!flag)
			{
				string arg_A8_0 = this.Name;
				string text = this.Value;
				text = CSSMeasure.GetValue(text, containerWidth, containerHeight);
				int num = 0;
				int.TryParse(text.Replace("px", ""), out num);
				if (this.Name.ToLower().Equals("width"))
				{
					num -= node.GetMarginPaddingBorder(true);
					text = num.ToString() + "px";
				}
				else
				{
					if (this.Name.ToLower().Equals("height"))
					{
						num -= node.GetMarginPaddingBorder(false);
						text = num.ToString() + "px";
					}
				}
				list.Add(new CSSProperty(null)
				{
					Name = this.Name,
					Value = text
				});
			}
			return list;
		}
		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(this.Value))
			{
				return "";
			}
			return this.Name + ": " + this.Value + ";";
		}
	}
}
