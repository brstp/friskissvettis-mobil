using System;
using System.Collections.Generic;
namespace MoMA.Helpers
{
	public class JavascriptHelper
	{
		public static string CreateScript(string script, Dictionary<string, string> values, bool tags)
		{
			script = TextHelper.ReplaceTemplateValues(script, values);
			if (tags)
			{
				return JavascriptHelper.AddScriptTags(script);
			}
			return script;
		}
		public static string AddScriptTags(string script)
		{
			return "<script type=\"text/javascript\">" + script + "</script>";
		}
		public static string GetValue(object value)
		{
			string result = "";
			if (value == null)
			{
				return result;
			}
			if (value.GetType() == typeof(double))
			{
				result = value.ToString().Replace(",", ".");
			}
			if (value.GetType() == typeof(string))
			{
				result = "'" + value.ToString() + "'";
			}
			return result;
		}
		public static string GetProperty(string name, object value)
		{
			string value2 = JavascriptHelper.GetValue(value);
			return string.IsNullOrEmpty(value2) ? "" : (",'" + name + "': " + value2);
		}
	}
}
