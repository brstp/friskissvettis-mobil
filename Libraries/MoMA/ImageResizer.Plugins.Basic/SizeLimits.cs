using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class SizeLimits : IssueSink
	{
		public class SizeLimitException : ImageProcessingException
		{
			public SizeLimitException(string message) : base(message)
			{
			}
		}
		public enum TotalSizeBehavior
		{
			ThrowException,
			IgnoreLimits
		}
		protected Size totalSize = new Size(3200, 3200);
		protected SizeLimits.TotalSizeBehavior totalBehavior;
		protected Size imageSize = new Size(0, 0);
		public Size TotalSize
		{
			get
			{
				return this.totalSize;
			}
			set
			{
				this.totalSize = value;
			}
		}
		public SizeLimits.TotalSizeBehavior TotalBehavior
		{
			get
			{
				return this.totalBehavior;
			}
			set
			{
				this.totalBehavior = value;
			}
		}
		public bool HasImageSize
		{
			get
			{
				return this.ImageSize.Width > 0 && this.ImageSize.Height > 0;
			}
		}
		public Size ImageSize
		{
			get
			{
				return this.imageSize;
			}
			set
			{
				this.imageSize = value;
			}
		}
		public SizeLimits() : base("SizeLimits")
		{
		}
		public SizeLimits(Config c) : base("SizeLimits")
		{
			int width = c.get("sizelimits.imageWidth", this.imageSize.Width);
			int height = c.get("sizelimits.imageHeight", this.imageSize.Height);
			this.imageSize = new Size(width, height);
			int num = Math.Max(1, c.get("sizelimits.totalWidth", this.totalSize.Width));
			int num2 = Math.Max(1, c.get("sizelimits.totaleHeight", this.totalSize.Height));
			if (num < 1 || num2 < 1)
			{
				this.AcceptIssue(new Issue("sizelimits.totalWidth and sizelimits.totalHeight must both be greater than 0. Reverting to defaults.", IssueSeverity.ConfigurationError));
			}
			else
			{
				this.totalSize = new Size(num, num2);
			}
			this.totalBehavior = c.get<SizeLimits.TotalSizeBehavior>("sizelimits.totalbehavior", SizeLimits.TotalSizeBehavior.ThrowException);
		}
		public void ValidateTotalSize(Size total)
		{
			if (this.TotalBehavior == SizeLimits.TotalSizeBehavior.ThrowException && !this.FitsInsideTotalSize(total))
			{
				throw new SizeLimits.SizeLimitException(string.Concat(new object[]
				{
					"The dimensions of the output image (",
					total.Width,
					"x",
					total.Height,
					") exceed the maximum permitted dimensions of ",
					this.TotalSize.Width,
					"x",
					this.TotalSize.Height,
					"."
				}));
			}
		}
		public bool FitsInsideTotalSize(Size s)
		{
			return s.Width < this.totalSize.Width && s.Height < this.totalSize.Height;
		}
	}
}
