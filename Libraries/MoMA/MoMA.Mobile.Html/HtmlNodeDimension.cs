using MoMA.Mobile.CSS;
using System;
namespace MoMA.Mobile.Html
{
	internal class HtmlNodeDimension
	{
		public int CalculatedWidth
		{
			get;
			set;
		}
		public int CalculatedHeight
		{
			get;
			set;
		}
		public CSSMeasure CalculatedMargin
		{
			get;
			set;
		}
		public CSSMeasure CalculatedPadding
		{
			get;
			set;
		}
		public CSSMeasure CalculatedBorder
		{
			get;
			set;
		}
		public int FullWidth
		{
			get
			{
				return this.CalculatedWidth + this.CalculatedMargin.Left + this.CalculatedMargin.Right + this.CalculatedPadding.Left + this.CalculatedPadding.Right + this.CalculatedBorder.Left + this.CalculatedBorder.Right;
			}
		}
		public HtmlNodeDimension()
		{
		}
		public HtmlNodeDimension(int width, int height, CSSMeasure margin, CSSMeasure padding, CSSMeasure border)
		{
			this.CalculatedWidth = width;
			this.CalculatedHeight = height;
			this.CalculatedMargin = margin;
			this.CalculatedPadding = padding;
			this.CalculatedBorder = border;
		}
	}
}
