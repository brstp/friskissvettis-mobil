using System;
using System.Collections.Generic;
using System.Linq;
namespace MoMA.Helpers.Url
{
	public class Querystring
	{
		private Dictionary<string, List<string>> _items = new Dictionary<string, List<string>>();
		public List<string> this[string key]
		{
			get
			{
				if (this._items.ContainsKey(key))
				{
					return this._items[key];
				}
				return null;
			}
			set
			{
				if (this._items.ContainsKey(key))
				{
					this._items[key] = value;
					return;
				}
				this._items.Add(key, value);
			}
		}
		public Querystring(string url)
		{
			if (url.Contains('?'))
			{
				string text = url.Split(new char[]
				{
					'?'
				}).LastOrDefault<string>();
				List<string> list = text.Split(new char[]
				{
					'&'
				}).ToList<string>();
				foreach (string current in list)
				{
					List<string> list2 = current.Split(new char[]
					{
						'='
					}).ToList<string>();
					if (list2.Count == 2)
					{
						string key = list2[0];
						string value = list2[1];
						this.Add(key, value);
					}
				}
			}
		}
		public void Add(string key, string value)
		{
			if (!this._items.ContainsKey(key))
			{
				this._items.Add(key, new List<string>());
			}
			this._items[key].Add(value);
		}
		public void Update(string key, string value)
		{
			if (!this._items.ContainsKey(key))
			{
				this._items.Add(key, new List<string>());
			}
			this._items[key].Clear();
			this._items[key].Add(value);
		}
		public void Update(string key, List<string> values)
		{
			if (!this._items.ContainsKey(key))
			{
				this._items.Add(key, new List<string>());
			}
			this._items[key] = values;
		}
		public void Remove(string key)
		{
			if (this._items.ContainsKey(key))
			{
				this._items.Remove(key);
			}
		}
		public void Remove(string key, string value)
		{
			if (this._items.ContainsKey(key))
			{
				if (this._items[key].Contains(value))
				{
					this._items[key].Remove(value);
				}
				if (this._items[key].Count == 0)
				{
					this.Remove(key);
				}
			}
		}
		public override string ToString()
		{
			if (this._items.Count == 0)
			{
				return "";
			}
			string text = "?";
			foreach (KeyValuePair<string, List<string>> current in this._items)
			{
				if (current.Value != null && current.Value.Count > 0)
				{
					foreach (string current2 in current.Value)
					{
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							current.Key,
							"=",
							current2,
							"&"
						});
					}
				}
			}
			return text.TrimEnd(new char[]
			{
				'&'
			});
		}
	}
}
