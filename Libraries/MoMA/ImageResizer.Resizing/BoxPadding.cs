using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Resizing
{
	[ComVisible(true)]
	public class BoxPadding
	{
		protected double top;
		protected double left;
		protected double bottom;
		protected double right;
		public double Top
		{
			get
			{
				return this.top;
			}
		}
		public double Left
		{
			get
			{
				return this.left;
			}
		}
		public double Right
		{
			get
			{
				return this.right;
			}
		}
		public double Bottom
		{
			get
			{
				return this.bottom;
			}
		}
		public double All
		{
			get
			{
				return this.all;
			}
		}
		protected double all
		{
			get
			{
				if (this.top == this.left && this.left == this.bottom && this.bottom == this.right)
				{
					return this.top;
				}
				return -1.0;
			}
			set
			{
				this.right = value;
				this.bottom = value;
				this.left = value;
				this.top = value;
			}
		}
		public static BoxPadding Empty
		{
			get
			{
				return new BoxPadding(0.0);
			}
		}
		public bool IsEmpty
		{
			get
			{
				return this.all == 0.0;
			}
		}
		public BoxPadding(double all)
		{
			this.all = all;
		}
		public BoxPadding(double left, double top, double right, double bottom)
		{
			this.top = top;
			this.left = left;
			this.bottom = bottom;
			this.right = right;
		}
		public BoxPadding(BoxPadding original)
		{
			this.top = original.top;
			this.bottom = original.bottom;
			this.right = original.right;
			this.left = original.left;
		}
		public BoxPadding SetAll(double all)
		{
			return new BoxPadding(this)
			{
				all = all
			};
		}
		public BoxPadding SetTop(double top)
		{
			return new BoxPadding(this)
			{
				top = top
			};
		}
		public BoxPadding SetLeft(double left)
		{
			return new BoxPadding(this)
			{
				left = left
			};
		}
		public BoxPadding SetRight(double right)
		{
			return new BoxPadding(this)
			{
				right = right
			};
		}
		public BoxPadding SetBottom(double bottom)
		{
			return new BoxPadding(this)
			{
				bottom = bottom
			};
		}
		public float[] GetEdgeOffsets()
		{
			return new float[]
			{
				(float)this.top,
				(float)this.right,
				(float)this.bottom,
				(float)this.left
			};
		}
	}
}
