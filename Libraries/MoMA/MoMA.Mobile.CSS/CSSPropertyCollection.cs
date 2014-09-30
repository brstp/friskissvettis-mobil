using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace MoMA.Mobile.CSS
{
	internal class CSSPropertyCollection : List<CSSProperty>
	{
		public CSSSection Parent
		{
			get;
			private set;
		}
		public CSSPropertyCollection(CSSSection Parent)
		{
			this.Parent = Parent;
		}
		public new void Add(CSSProperty property)
		{
			base.Add(property);
		}
		public bool HasProperty(string name)
		{
			return this.GetProperty(name) != null;
		}
		public List<CSSProperty> GetProperties(string name)
		{
			return (
				from s in this
				where s.Name == name
				select s).ToList<CSSProperty>();
		}
		public CSSProperty GetProperty(string name)
		{
			return (
				from s in this
				where s.Name == name
				select s).FirstOrDefault<CSSProperty>();
		}
		public void UpdateProperty(string name, string value)
		{
			CSSProperty cSSProperty = this.GetProperty(name);
			if (cSSProperty == null)
			{
				cSSProperty = new CSSProperty(this.Parent);
				cSSProperty.Name = name;
				this.Add(cSSProperty);
			}
			cSSProperty.Value = value;
		}
		public void AddRange(string css)
		{
			Regex regex = new Regex("(?<name>[a-zA-Z-]+)[ ]*:[ ]*(?<value>[^;]+);");
			Match match = regex.Match(css);
			while (match.Success)
			{
				string text = match.Groups["name"].ToString();
				string value = match.Groups["value"].ToString();
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value))
				{
					this.UpdateProperty(text, value);
				}
				match = match.NextMatch();
			}
		}
	}
}
