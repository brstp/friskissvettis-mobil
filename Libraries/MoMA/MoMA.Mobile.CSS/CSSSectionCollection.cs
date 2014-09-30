using System;
using System.Collections.Generic;
namespace MoMA.Mobile.CSS
{
	internal class CSSSectionCollection : List<CSSSection>
	{
		public override string ToString()
		{
			string text = "";
			foreach (CSSSection current in this)
			{
				text = text + current.ToString() + " ";
			}
			return text;
		}
	}
}
