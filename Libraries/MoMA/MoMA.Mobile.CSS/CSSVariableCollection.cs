using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace MoMA.Mobile.CSS
{
	internal class CSSVariableCollection : List<CSSVariable>
	{
		public void AddRange(ref string css)
		{
			int num = css.IndexOf('{');
			if (num < 0)
			{
				return;
			}
			css.Remove(num);
			Regex regex = new Regex("(?<name>[a-zA-Z-]+)[ ]*=[ ]*(?<value>.+)\\n");
			Match match = regex.Match(css);
			while (match.Success)
			{
				string text = match.Groups["name"].ToString().Trim();
				string value = match.Groups["value"].ToString().Trim();
				css = css.Replace(match.ToString(), "");
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value))
				{
					this.UpdateVariable(text, value);
				}
				match = match.NextMatch();
			}
		}
		public CSSVariable UpdateVariable(string name, string value)
		{
			CSSVariable cSSVariable = this.GetVariable(name);
			if (cSSVariable == null)
			{
				cSSVariable = new CSSVariable();
				cSSVariable.Name = name;
			}
			cSSVariable.Value = value;
			base.Add(cSSVariable);
			return cSSVariable;
		}
		public CSSVariable GetVariable(string name)
		{
			return (
				from v in this
				where v.Name.Equals(name)
				select v).FirstOrDefault<CSSVariable>();
		}
		public void ReplaceVariables(ref string css)
		{
			foreach (CSSVariable current in this)
			{
				css = css.Replace("[" + current.Name + "]", current.Value);
			}
		}
	}
}
