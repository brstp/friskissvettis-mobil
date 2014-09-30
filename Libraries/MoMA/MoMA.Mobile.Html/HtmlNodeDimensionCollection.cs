using HtmlAgilityPack;
using MoMA.Mobile.CSS;
using System;
using System.Collections.Generic;
namespace MoMA.Mobile.Html
{
	internal class HtmlNodeDimensionCollection : Dictionary<string, HtmlNodeDimension>
	{
		public void Update(string id, HtmlNodeDimension dimension)
		{
			if (base.ContainsKey(id))
			{
				base[id] = dimension;
				return;
			}
			base.Add(id, dimension);
		}
		public void Update(string id, int width, int height, CSSMeasure margin, CSSMeasure padding, CSSMeasure border)
		{
			HtmlNodeDimension value = new HtmlNodeDimension(width, height, margin, padding, border);
			if (base.ContainsKey(id))
			{
				base[id] = value;
				return;
			}
			base.Add(id, value);
		}
		public HtmlNodeDimension GetDimension(HtmlNode node)
		{
			if (base.ContainsKey(node.Id))
			{
				return base[node.Id];
			}
			return null;
		}
	}
}
