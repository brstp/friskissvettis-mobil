using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using ImageResizer.Resizing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class SizeLimiting : BuilderExtension, IPlugin, IIssueProvider
	{
		private SizeLimits limits;
		public SizeLimits Limits
		{
			get
			{
				return this.limits;
			}
			set
			{
				this.limits = value;
			}
		}
		public IPlugin Install(Config c)
		{
			this.limits = new SizeLimits(c);
			c.Plugins.AllPlugins.Add(this);
			c.Plugins.ImageBuilderExtensions.Add(this);
			return this;
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			return true;
		}
		protected override RequestedAction PostLayoutImage(ImageState s)
		{
			base.PostLayoutImage(s);
			if (!this.limits.HasImageSize)
			{
				return RequestedAction.None;
			}
			SizeF size = s.layout.GetBoundingBox().Size;
			double num = (double)(size.Width / (float)this.limits.ImageSize.Width);
			double num2 = (double)(size.Height / (float)this.limits.ImageSize.Height);
			double num3 = (num > num2) ? num : num2;
			if (num3 > 1.0)
			{
				s.layout.Scale(1.0 / num3, new PointF(0f, 0f));
			}
			return RequestedAction.None;
		}
		protected override RequestedAction PrepareDestinationBitmap(ImageState s)
		{
			this.limits.ValidateTotalSize(s.destSize);
			return RequestedAction.None;
		}
		public IEnumerable<IIssue> GetIssues()
		{
			if (this.limits != null)
			{
				return this.limits.GetIssues();
			}
			return null;
		}
	}
}
