using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace MostThingsWeb
{
	public static class css2xpath
	{
		private static List<Regex> patterns;
		private static List<object> replacements;
		static css2xpath()
		{
			css2xpath.patterns = new List<Regex>();
			css2xpath.replacements = new List<object>();
			css2xpath.AddRule(new Regex("\\[([^\\]~\\$\\*\\^\\|\\!]+)(=[^\\]]+)?\\]"), "[@$1$2]");
			css2xpath.AddRule(new Regex("\\s*,\\s*"), "|");
			css2xpath.AddRule(new Regex("\\s*(\\+|~|>)\\s*"), "$1");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*])~([a-zA-Z0-9_\\-\\*])"), "$1/following-sibling::$2");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*])\\+([a-zA-Z0-9_\\-\\*])"), "$1/following-sibling::*[1]/self::$2");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*])>([a-zA-Z0-9_\\-\\*])"), "$1/$2");
			css2xpath.AddRule(new Regex("\\[([^=]+)=([^'|\"][^\\]]*)\\]"), "[$1='$2']");
			css2xpath.AddRule(new Regex("(^|[^a-zA-Z0-9_\\-\\*])(#|\\.)([a-zA-Z0-9_\\-]+)"), "$1*$2$3");
			css2xpath.AddRule(new Regex("([\\>\\+\\|\\~\\,\\s])([a-zA-Z\\*]+)"), "$1//$2");
			css2xpath.AddRule(new Regex("\\s+\\/\\/"), "//");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*]+):first-child"), "*[1]/self::$1");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*]+):last-child"), "$1[not(following-sibling::*)]");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*]+):only-child"), "*[last()=1]/self::$1");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*]+):empty"), "$1[not(*) and not(normalize-space())]");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*]+):not\\(([^\\)]*)\\)"), (Match m) => m.Groups[1].Value + "[not(" + new Regex("^[^\\[]+\\[([^\\]]*)\\].*$").Replace(css2xpath.Transform(m.Groups[2].Value), "$1") + ")]");
			css2xpath.AddRule(new Regex("([a-zA-Z0-9_\\-\\*]+):nth-child\\(([^\\)]*)\\)"), delegate(Match m)
			{
				string text = m.Groups[2].Value;
				string value = m.Groups[1].Value;
				string a;
				if ((a = text) != null)
				{
					if (a == "n")
					{
						return value;
					}
					if (a == "even")
					{
						return "*[position() mod 2=0 and position()>=0]/self::" + value;
					}
					if (a == "odd")
					{
						return value + "[(count(preceding-sibling::*) + 1) mod 2=1]";
					}
				}
				text = new Regex("^([0-9])*n.*?([0-9])*$").Replace(text, "$1+$2");
				string[] array = new string[2];
				string[] array2 = text.Split(new char[]
				{
					'+'
				});
				array[0] = array2[0];
				int num = 0;
				if (array2.Length == 2 && !int.TryParse(array2[1], out num))
				{
					num = 0;
				}
				array[1] = num.ToString();
				return string.Concat(new string[]
				{
					"*[(position()-",
					array[1],
					") mod ",
					array[0],
					"=0 and position()>=",
					array[1],
					"]/self::",
					value
				});
			});
			css2xpath.AddRule(new Regex(":contains\\(([^\\)]*)\\)"), (Match m) => "[contains(string(.),'" + m.Groups[1].Value + "')]");
			css2xpath.AddRule(new Regex("\\[([a-zA-Z0-9_\\-]+)\\|=([^\\]]+)\\]"), "[@$1=$2 or starts-with(@$1,concat($2,'-'))]");
			css2xpath.AddRule(new Regex("\\[([a-zA-Z0-9_\\-]+)\\*=([^\\]]+)\\]"), "[contains(@$1,$2)]");
			css2xpath.AddRule(new Regex("\\[([a-zA-Z0-9_\\-]+)~=([^\\]]+)\\]"), "[contains(concat(' ',normalize-space(@$1),' '),concat(' ',$2,' '))]");
			css2xpath.AddRule(new Regex("\\[([a-zA-Z0-9_\\-]+)\\^=([^\\]]+)\\]"), "[starts-with(@$1,$2)]");
			css2xpath.AddRule(new Regex("\\[([a-zA-Z0-9_\\-]+)\\$=([^\\]]+)\\]"), delegate(Match m)
			{
				string value = m.Groups[1].Value;
				string value2 = m.Groups[2].Value;
				return string.Concat(new object[]
				{
					"[substring(@",
					value,
					",string-length(@",
					value,
					")-",
					value2.Length - 3,
					")=",
					value2,
					"]"
				});
			});
			css2xpath.AddRule(new Regex("\\[([a-zA-Z0-9_\\-]+)\\!=([^\\]]+)\\]"), "[not(@$1) or @$1!=$2]");
			css2xpath.AddRule(new Regex("#([a-zA-Z0-9_\\-]+)"), "[@id='$1']");
			css2xpath.AddRule(new Regex("\\.([a-zA-Z0-9_\\-]+)"), "[contains(concat(' ',normalize-space(@class),' '),' $1 ')]");
			css2xpath.AddRule(new Regex("\\]\\[([^\\]]+)"), " and ($1)");
		}
		public static void AddRule(Regex regex, MatchEvaluator replacement)
		{
			css2xpath._AddRule(regex, replacement);
		}
		public static void AddRule(Regex regex, string replacement)
		{
			css2xpath._AddRule(regex, replacement);
		}
		private static void _AddRule(Regex regex, object replacement)
		{
			if (regex == null)
			{
				throw new ArgumentException("Must supply non-null Regex.", "regex");
			}
			if (replacement == null || (!(replacement is string) && !(replacement is MatchEvaluator)))
			{
				throw new ArgumentException("Must supply non-null replacement (either String or MatchEvaluator).", "replacement");
			}
			css2xpath.patterns.Add(regex);
			css2xpath.replacements.Add(replacement);
		}
		public static string Transform(string css)
		{
			int count = css2xpath.patterns.Count;
			for (int i = 0; i < count; i++)
			{
				Regex regex = css2xpath.patterns[i];
				object obj = css2xpath.replacements[i];
				if (obj is string)
				{
					css = regex.Replace(css, (string)obj);
				}
				else
				{
					css = regex.Replace(css, (MatchEvaluator)obj);
				}
			}
			return "//" + css;
		}
		public static void PreloadRules()
		{
		}
	}
}
