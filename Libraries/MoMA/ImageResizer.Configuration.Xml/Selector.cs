using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
namespace ImageResizer.Configuration.Xml
{
	[ComVisible(true)]
	public class Selector : List<string>
	{
		public string Last
		{
			get
			{
				if (base.Count > 0)
				{
					return base[base.Count - 1];
				}
				return null;
			}
		}
		public Selector(List<string> items)
		{
			base.AddRange(items);
		}
		public Selector(string selector) : base(selector.Trim(new char[]
		{
			'.'
		}).Split(new char[]
		{
			'.'
		}, StringSplitOptions.RemoveEmptyEntries))
		{
		}
		public Selector GetSublist(int startAt)
		{
			return new Selector(base.GetRange(startAt, base.Count - startAt));
		}
		public Selector GetRemainder()
		{
			if (base.Count < 2)
			{
				return null;
			}
			return this.GetSublist(1);
		}
		public Selector GetAllExceptLast()
		{
			if (base.Count < 2)
			{
				return null;
			}
			return new Selector(base.GetRange(0, base.Count - 1));
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < base.Count; i++)
			{
				stringBuilder.Append(base[i]);
				if (i < base.Count - 1)
				{
					stringBuilder.Append('.');
				}
			}
			return stringBuilder.ToString();
		}
	}
}
