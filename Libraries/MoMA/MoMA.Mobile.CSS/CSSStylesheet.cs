using HtmlAgilityPack;
using MoMA.Helpers;
using MoMA.Mobile.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace MoMA.Mobile.CSS
{
	internal class CSSStylesheet : List<CSSSection>
	{
		public string OriginalCSS = "";
		public CSSVariableCollection Variables = new CSSVariableCollection();
		public HtmlNode StylesheetHtmlNode
		{
			get;
			set;
		}
		public HtmlDocumentWrapper Parent
		{
			get;
			private set;
		}
		public bool Empty
		{
			get
			{
				return !this.HasSections();
			}
		}
		public CSSStylesheet(HtmlDocumentWrapper parent)
		{
			this.Parent = parent;
		}
		public CSSStylesheet(HtmlDocumentWrapper parent, HtmlNode stylesheetNode)
		{
			this.Parent = parent;
			this.Load(stylesheetNode);
		}
		public void Load(HtmlNode stylesheetNode)
		{
			string css = "";
			this.StylesheetHtmlNode = stylesheetNode;
			if (stylesheetNode.Attributes["href"] != null)
			{
				string value = stylesheetNode.Attributes["href"].Value;
				Uri uri = null;
				if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
				{
					return;
				}
				css = (this.OriginalCSS = ScrapeHelper.Get(uri));
			}
			else
			{
				css = (this.OriginalCSS = stylesheetNode.InnerHtml);
			}
			css = this.ClearComments(css);
			this.Variables.AddRange(ref css);
			this.Variables.ReplaceVariables(ref css);
			this.AddSectionsFromCSS(css);
		}
		public CSSStylesheet UpdateAgainstNodeAndDevice()
		{
			CSSStylesheet cSSStylesheet = new CSSStylesheet(this.Parent);
			foreach (CSSSection current in this)
			{
				if (current.IsValidSection())
				{
					HtmlNodeCollection nodesByCssSelector = this.Parent.GetNodesByCssSelector(current.Selector);
					if (nodesByCssSelector != null)
					{
						foreach (HtmlNode current2 in (IEnumerable<HtmlNode>)nodesByCssSelector)
						{
							CSSSection cSSSection = current.UpdateAgainstNodeAndDevice(current2, current);
							if (cSSSection != null)
							{
								cSSStylesheet.Add(cSSSection);
							}
						}
					}
				}
			}
			return cSSStylesheet;
		}
		public string ClearComments(string css)
		{
			return Regex.Replace(css, "(?<match>/\\*[^/]*\\*/)", "").Trim();
		}
		public void MergeEqualSections()
		{
			List<CSSSection> list = new List<CSSSection>();
			foreach (CSSSection current in this)
			{
				if (!list.Contains(current))
				{
					foreach (CSSSection current2 in this)
					{
						if (current != current2 && current.ToString(true).Equals(current2.ToString(true)))
						{
							CSSSection expr_5D = current;
							expr_5D.Selector = expr_5D.Selector + ", " + current2.Selector;
							list.Add(current2);
						}
					}
				}
			}
			foreach (CSSSection current3 in list)
			{
				base.Remove(current3);
			}
		}
		public bool HasSection(string selector)
		{
			return this.GetSections(selector).Count > 0;
		}
		public bool HasSections()
		{
			return base.Count > 0;
		}
		public List<CSSSection> GetSections(string selector)
		{
			return (
				from s in this
				where s.Selector.Trim().Equals(selector.Trim())
				select s).ToList<CSSSection>();
		}
		public void AddSectionsFromCSS(string css)
		{
			Regex regex = new Regex("(?<selector>(\\#|\\.){0,1}[^{]+){(?<body>[^}]*)}");
			Match match = regex.Match(css);
			while (match.Success)
			{
				string selector = match.Groups["selector"].ToString().Trim();
				string text = match.Groups["body"].ToString().Trim();
				CSSSection cSSSection = new CSSSection(this);
				cSSSection.Selector = selector;
				base.Add(cSSSection);
				cSSSection.Properties.AddRange(text.Trim());
				if (!cSSSection.IsValidSection())
				{
					base.Remove(cSSSection);
				}
				match = match.NextMatch();
			}
		}
		public void RemoveEmptySections()
		{
			List<CSSSection> list = new List<CSSSection>();
			foreach (CSSSection current in this)
			{
				if (current.Properties.Count == 0)
				{
					list.Add(current);
				}
			}
			foreach (CSSSection current2 in list)
			{
				base.Remove(current2);
			}
		}
		public CSSProperty GetProperty(List<string> selectors, string propertyName)
		{
			return this.GetAllProperties(selectors, propertyName).LastOrDefault<CSSProperty>();
		}
		public List<CSSProperty> GetAllProperties(List<string> selectors, string propertyName)
		{
			List<CSSProperty> list = new List<CSSProperty>();
			foreach (string current in selectors)
			{
				List<CSSSection> sections = this.GetSections(current);
				foreach (CSSSection current2 in sections)
				{
					CSSProperty property = current2.GetProperty(propertyName);
					if (property != null && !string.IsNullOrEmpty(property.Value))
					{
						list.Add(property);
					}
				}
			}
			return list;
		}
		public T GetPropertyValue<T>(List<string> selectors, string propertyName, T defaultValue)
		{
			CSSProperty property = this.GetProperty(selectors, propertyName);
			T result = defaultValue;
			if (property != null)
			{
				result = ConversionHelper.Convert<T>(property.Value, defaultValue);
			}
			return result;
		}
		public override string ToString()
		{
			string text = Environment.NewLine;
			foreach (CSSSection current in this)
			{
				text += current.ToString();
			}
			return text;
		}
	}
}
