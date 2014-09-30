using HtmlAgilityPack;
using MoMA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MoMA.Mobile.CSS
{
	internal class CSSSection
	{
		public bool Failed;
		public string Selector
		{
			get;
			set;
		}
		public CSSPropertyCollection Properties
		{
			get;
			set;
		}
		public CSSStylesheet Parent
		{
			get;
			private set;
		}
		public CSSSection(CSSStylesheet Parent)
		{
			this.Parent = Parent;
			this.Properties = new CSSPropertyCollection(this);
		}
		public void AddProperty(string name, string value)
		{
			this.Properties.Add(new CSSProperty(this)
			{
				Name = name,
				Value = value
			});
		}
		public void AddPropertyRange(IEnumerable<CSSProperty> properties)
		{
			this.Properties.AddRange(properties);
		}
		public void AddProperty(CSSProperty property)
		{
			this.Properties.Add(property);
		}
		public void UpdateProperty(string name, string value)
		{
			this.Properties.UpdateProperty(name, value);
		}
		public void UpdateProperty(CSSProperty property)
		{
			this.Properties.UpdateProperty(property.Name, property.Value);
		}
		public bool HasProperty(string name)
		{
			return (
				from p in this.Properties
				where p.Name == name
				select p).Count<CSSProperty>() > 0;
		}
		public CSSProperty GetProperty(string name)
		{
			return (
				from p in this.Properties
				where p.Name == name
				select p).FirstOrDefault<CSSProperty>();
		}
		public T GetPropertyValue<T>(string name, T defaultValue)
		{
			CSSProperty cSSProperty = (
				from p in this.Properties
				where p.Name == name
				select p).FirstOrDefault<CSSProperty>();
			if (cSSProperty != null)
			{
				return ConversionHelper.Convert<T>(cSSProperty.Value, defaultValue);
			}
			return defaultValue;
		}
		public int GetPropertyIndex(string propertyName)
		{
			for (int i = 0; i < this.Properties.Count; i++)
			{
				if (this.Properties[i].Name.ToLower().Equals(propertyName.ToLower()))
				{
					return i;
				}
			}
			return -1;
		}
		public CSSSection UpdateAgainstNodeAndDevice(HtmlNode node, CSSSection templateSection)
		{
			if (templateSection.IsValidSection())
			{
				CSSSection cSSSection = new CSSSection(null);
				foreach (CSSProperty current in templateSection.Properties)
				{
					List<CSSProperty> properties = current.UpdateAgainstNodeAndDevice(node, false);
					cSSSection.AddPropertyRange(properties);
				}
				cSSSection.Selector = "#" + node.Id;
				return cSSSection;
			}
			return null;
		}
		public bool IsValidSection()
		{
			if (this.HasProperty("debug"))
			{
				if (this.GetPropertyValue<bool>("debug", false) && !DebugHelper.IsWebConfigCompilationDebugMode)
				{
					return false;
				}
				if (!this.GetPropertyValue<bool>("debug", false) && DebugHelper.IsWebConfigCompilationDebugMode)
				{
					return false;
				}
			}
			return true;
		}
		public override string ToString()
		{
			return this.ToString(false);
		}
		public string ToString(bool onlyProperties)
		{
			string text = "";
			string text2 = "  ";
			if (string.IsNullOrEmpty(this.Selector) || this.Properties.Count == 0)
			{
				return "";
			}
			if (!onlyProperties)
			{
				string text3 = text;
				text = string.Concat(new string[]
				{
					text3,
					Environment.NewLine,
					text2,
					this.Selector,
					" {",
					Environment.NewLine
				});
			}
			foreach (CSSProperty current in this.Properties)
			{
				if (!string.IsNullOrWhiteSpace(current.ToString()))
				{
					string text4 = text;
					text = string.Concat(new string[]
					{
						text4,
						text2,
						text2,
						current.ToString(),
						Environment.NewLine
					});
				}
			}
			if (!onlyProperties)
			{
				string text5 = text;
				text = string.Concat(new string[]
				{
					text5,
					text2,
					"}",
					Environment.NewLine,
					Environment.NewLine
				});
			}
			return text;
		}
	}
}
