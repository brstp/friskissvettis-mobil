using System;
using System.Linq;
namespace MoMA.Mobile.CSS
{
	internal class CSSSimpleMeasure
	{
		public string Top
		{
			get;
			set;
		}
		public string Right
		{
			get;
			set;
		}
		public string Bottom
		{
			get;
			set;
		}
		public string Left
		{
			get;
			set;
		}
		public CSSSimpleMeasure(string value)
		{
			this.Top = "0px";
			this.Right = "0px";
			this.Bottom = "0px";
			this.Left = "0px";
			string[] array = value.Trim().Split(new char[]
			{
				' '
			});
			if (array.Count<string>() <= 0 && array.Count<string>() >= 5)
			{
				return;
			}
			switch (array.Length)
			{
			case 1:
				this.Right = (this.Left = (this.Top = (this.Bottom = array[0])));
				return;
			case 2:
				this.Top = (this.Bottom = array[0]);
				this.Right = (this.Left = array[1]);
				return;
			case 3:
				this.Top = array[0];
				this.Right = array[1];
				this.Left = this.Right;
				this.Bottom = array[2];
				return;
			case 4:
				this.Top = array[0];
				this.Right = array[1];
				this.Bottom = array[2];
				this.Left = array[3];
				return;
			default:
				return;
			}
		}
		public override string ToString()
		{
			this.Top = (string.IsNullOrWhiteSpace(this.Top) ? "0px" : this.Top);
			this.Right = (string.IsNullOrWhiteSpace(this.Right) ? "0px" : this.Right);
			this.Bottom = (string.IsNullOrWhiteSpace(this.Bottom) ? "0px" : this.Bottom);
			this.Left = (string.IsNullOrWhiteSpace(this.Left) ? "0px" : this.Left);
			if ((this.Top.Equals("0px") && this.Right.Equals("0px") && this.Bottom.Equals("0px") && this.Left.Equals("0px")) || (this.Top.Equals("0") && this.Right.Equals("0") && this.Bottom.Equals("0") && this.Left.Equals("0")) || (this.Top.Equals("") && this.Right.Equals("") && this.Bottom.Equals("") && this.Left.Equals("")))
			{
				return "";
			}
			return string.Concat(new string[]
			{
				this.Top,
				" ",
				this.Right,
				" ",
				this.Bottom,
				" ",
				this.Left
			}).Trim();
		}
	}
}
