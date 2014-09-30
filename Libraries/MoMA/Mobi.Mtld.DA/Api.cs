using Mobi.Mtld.DA.Exception;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace Mobi.Mtld.DA
{
	public class Api
	{
		private static string API_RULE_ID = "3";
		private static Hashtable trees = new Hashtable();
		public static Hashtable GetTreeFromString(string json)
		{
			Hashtable hashtable = new Json(json).GetHashtable();
			if (!hashtable.ContainsKey("$"))
			{
				throw new JsonException("Bad data loaded into the tree.", JsonException.ErrorCode.BAD_DATA);
			}
			if (float.Parse(Api.GetString(Api.GetHashtable(hashtable, "$"), "Ver"), NumberFormatInfo.InvariantInfo) < 0.7f)
			{
				throw new JsonException("This version of the API requires a newer version of the JSON data.", JsonException.ErrorCode.JSON_VERSION);
			}
			Hashtable hashtable2 = new Hashtable();
			Hashtable hashtable3 = new Hashtable();
			foreach (DictionaryEntry entry in Api.GetHashtable(hashtable, "p"))
			{
				hashtable2[Api.GetValue(entry)] = Api.GetKey(entry);
				hashtable3[Api.GetValue(entry).Substring(1)] = Api.GetKey(entry);
			}
			hashtable["pr"] = hashtable2;
			hashtable["pn"] = hashtable3;
			Hashtable hashtable4 = Api.GetHashtable(hashtable, "r");
			if (hashtable4 != null)
			{
				Hashtable hashtable5 = (Hashtable)hashtable4[Api.API_RULE_ID];
				Hashtable hashtable6 = new Hashtable();
				foreach (DictionaryEntry entry2 in hashtable5)
				{
					hashtable6[Api.GetKey(entry2)] = new Regex(Api.GetValue(entry2));
				}
				hashtable["regex"] = hashtable6;
			}
			return hashtable;
		}
		public static Hashtable GetTreeFromFile(string filename)
		{
			return Api.GetTreeFromFile(filename, false);
		}
		public static Hashtable GetTreeFromFile(string filename, bool reload)
		{
			if (reload || !Api.trees.ContainsKey(filename))
			{
				string json;
				try
				{
					json = File.ReadAllText(filename);
				}
				catch (FileNotFoundException ex)
				{
					throw new JsonException("Unable to load JSON file: " + ex.Message, JsonException.ErrorCode.FILE_NOT_FOUND_ERROR);
				}
				Api.trees[filename] = Api.GetTreeFromString(json);
			}
			return Api.GetHashtable(Api.trees, filename);
		}
		public static int GetApiRevision()
		{
			return Api.RevisionFromKeyword("$Rev: 13768 $");
		}
		public static int GetTreeRevision(Hashtable tree)
		{
			return Api.RevisionFromKeyword(Api.GetString(Api.GetHashtable(tree, "$"), "Rev"));
		}
		private static int RevisionFromKeyword(string keyword)
		{
			int result;
			try
			{
				result = int.Parse(keyword.Substring(6).Replace("$", "").Trim());
			}
			catch
			{
				result = 0;
			}
			return result;
		}
		public static Hashtable ListProperties(Hashtable tree)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["s"] = "string";
			hashtable["b"] = "boolean";
			hashtable["i"] = "integer";
			hashtable["d"] = "date";
			hashtable["u"] = "unknown";
			Hashtable hashtable2 = new Hashtable();
			foreach (DictionaryEntry entry in Api.GetHashtable(tree, "p"))
			{
				string value = Api.GetValue(entry);
				hashtable2[value.Substring(1)] = hashtable[value.Substring(0, 1)];
			}
			return hashtable2;
		}
		public static Hashtable GetProperties(Hashtable tree, string userAgent)
		{
			return Api.GetProperties(tree, userAgent, false);
		}
		public static Hashtable GetPropertiesAsTyped(Hashtable tree, string userAgent)
		{
			return Api.GetProperties(tree, userAgent, true);
		}
		private static Hashtable GetProperties(Hashtable tree, string userAgent, bool typedValues)
		{
			Hashtable hashtable = new Hashtable();
			StringBuilder stringBuilder = new StringBuilder("");
			Hashtable hashtable2 = Api.GetHashtable(tree, "regex");
			Api.SeekProperties(Api.GetHashtable(tree, "t"), userAgent.Trim(), hashtable, null, stringBuilder, hashtable2);
			Hashtable hashtable3 = new Hashtable();
			foreach (DictionaryEntry entry in hashtable)
			{
				if (typedValues)
				{
					hashtable3[Api.PropertyFromId(tree, Api.GetKey(entry))] = Api.ValueAsTypedFromId(tree, Api.GetValue(entry), Api.GetKey(entry));
				}
				else
				{
					hashtable3[Api.PropertyFromId(tree, Api.GetKey(entry))] = Api.ValueFromId(tree, Api.GetValue(entry));
				}
			}
			string text = stringBuilder.ToString();
			hashtable3["_matched"] = text;
			hashtable3["_unmatched"] = userAgent.Substring(text.Length);
			return hashtable3;
		}
		public static string GetProperty(Hashtable tree, string userAgent, string property)
		{
			string text = Api.IdFromProperty(tree, property);
			Hashtable hashtable = new Hashtable();
			StringBuilder matched = new StringBuilder("");
			Hashtable hashtable2 = new Hashtable();
			hashtable2[text] = 1;
			Hashtable hashtable3 = Api.GetHashtable(tree, "regex");
			Api.SeekProperties(Api.GetHashtable(tree, "t"), userAgent.Trim(), hashtable, hashtable2, matched, hashtable3);
			if (hashtable.Count == 0)
			{
				throw new InvalidPropertyException(string.Concat(new string[]
				{
					"The property \"",
					property,
					"\" is invalid for the User Agent:\"",
					userAgent,
					"\""
				}));
			}
			return Api.ValueFromId(tree, Api.GetString(hashtable, text));
		}
		public static string GetPropertyAsString(Hashtable tree, string userAgent, string property)
		{
			Api.PropertyTypeCheck(tree, property, "s", "string");
			return Api.GetProperty(tree, userAgent, property);
		}
		public static bool GetPropertyAsBoolean(Hashtable tree, string userAgent, string property)
		{
			Api.PropertyTypeCheck(tree, property, "b", "boolean");
			string property2 = Api.GetProperty(tree, userAgent, property);
			return property2.Equals("1") || property2.Equals("true") || property2.Equals("True");
		}
		public static int GetPropertyAsInteger(Hashtable tree, string userAgent, string property)
		{
			Api.PropertyTypeCheck(tree, property, "i", "integer");
			return int.Parse(Api.GetProperty(tree, userAgent, property));
		}
		public static string GetPropertyAsDate(Hashtable tree, string userAgent, string property)
		{
			Api.PropertyTypeCheck(tree, property, "d", "string");
			return Api.GetProperty(tree, userAgent, property);
		}
		private static void PropertyTypeCheck(Hashtable tree, string property, string prefix, string typeName)
		{
			if (!Api.GetHashtable(tree, "pr").ContainsKey(prefix + property))
			{
				throw new IncorrectPropertyTypeException(property + " is not of type " + typeName);
			}
		}
		private static string PropertyFromId(Hashtable tree, string id)
		{
			return Api.GetString(Api.GetHashtable(tree, "p"), id).Substring(1);
		}
		private static string IdFromProperty(Hashtable tree, string property)
		{
			string @string = Api.GetString(Api.GetHashtable(tree, "pn"), property);
			if (@string == null)
			{
				throw new UnknownPropertyException("The property \"" + property + "\" is not known in this tree.");
			}
			return @string;
		}
		private static string ValueFromId(Hashtable tree, string id)
		{
			return Api.GetString(Api.GetHashtable(tree, "v"), id);
		}
		private static object ValueAsTypedFromId(Hashtable tree, string id, string propertyId)
		{
			string @string = Api.GetString(Api.GetHashtable(tree, "v"), id);
			char c = Api.GetString(Api.GetHashtable(tree, "p"), propertyId)[0];
			object result;
			switch (c)
			{
			case 'b':
				result = (@string.Equals("1") || @string.Equals("true") || @string.Equals("True"));
				return result;
			case 'c':
				break;
			case 'd':
				result = @string;
				return result;
			default:
				if (c == 'i')
				{
					result = int.Parse(@string);
					return result;
				}
				if (c == 's')
				{
					result = @string;
					return result;
				}
				break;
			}
			result = @string;
			return result;
		}
		private static void SeekProperties(Hashtable node, string str, Hashtable properties, Hashtable sought, StringBuilder matched, Hashtable rules)
		{
			if (node.ContainsKey("d"))
			{
				if (sought != null && sought.Count == 0)
				{
					return;
				}
				foreach (DictionaryEntry entry in Api.GetHashtable(node, "d"))
				{
					string key = Api.GetKey(entry);
					if (sought == null || sought.ContainsKey(key))
					{
						properties[key] = Api.GetValue(entry);
					}
					if (sought != null && (!node.ContainsKey("m") || (node.ContainsKey("m") && !Api.GetHashtable(node, "m").ContainsKey(key))))
					{
						sought.Remove(key);
					}
				}
			}
			if (node.ContainsKey("c"))
			{
				if (node.ContainsKey("r"))
				{
					Hashtable hashtable = Api.GetHashtable(node, "r");
					foreach (string key2 in hashtable.Values)
					{
						Regex regex = (Regex)rules[key2];
						str = regex.Replace(str, "", 1);
					}
				}
				for (int i = 1; i < str.Length + 1; i++)
				{
					string text = str.Substring(0, i);
					if (Api.GetHashtable(node, "c").ContainsKey(text))
					{
						matched.Append(text);
						Api.SeekProperties(Api.GetHashtable(Api.GetHashtable(node, "c"), text), str.Substring(i), properties, sought, matched, rules);
						break;
					}
				}
			}
		}
		private static string GetString(Hashtable hashtable, string str)
		{
			return string.Concat(hashtable[str]);
		}
		private static Hashtable GetHashtable(Hashtable hashtable, string str)
		{
			return (Hashtable)hashtable[str];
		}
		private static string GetKey(DictionaryEntry entry)
		{
			return string.Concat(entry.Key);
		}
		private static string GetValue(DictionaryEntry entry)
		{
			return string.Concat(entry.Value);
		}
	}
}
