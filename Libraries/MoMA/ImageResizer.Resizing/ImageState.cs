using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Resizing
{
	[ComVisible(true)]
	public class ImageState : IDisposable
	{
		public ResizeSettings settings;
		public Size originalSize;
		public bool supportsTransparency = true;
		public LayoutBuilder layout = new LayoutBuilder();
		public Size destSize;
		public Size finalSize;
		public RectangleF copyRect;
		public Bitmap sourceBitmap;
		public Bitmap destBitmap;
		public Graphics destGraphics;
		public ImageAttributes copyAttibutes;
		private Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		public SizeF copySize
		{
			get
			{
				return this.copyRect.Size;
			}
		}
		public Dictionary<string, object> Data
		{
			get
			{
				return this.data;
			}
		}
		public ImageState(ResizeSettings settings, Size originalSize, bool transparencySupported)
		{
			this.settings = settings;
			this.originalSize = originalSize;
			this.supportsTransparency = transparencySupported;
		}
		public void Dispose()
		{
			try
			{
				if (this.sourceBitmap != null)
				{
					if (this.sourceBitmap.Tag != null && this.sourceBitmap.Tag is BitmapTag)
					{
						Stream source = ((BitmapTag)this.sourceBitmap.Tag).Source;
						if (source != null)
						{
							source.Dispose();
						}
					}
					this.sourceBitmap.Dispose();
				}
			}
			finally
			{
				try
				{
					if (this.destGraphics != null)
					{
						this.destGraphics.Dispose();
					}
				}
				finally
				{
					try
					{
						if (this.destBitmap != null)
						{
							this.destBitmap.Dispose();
						}
					}
					finally
					{
						if (this.copyAttibutes != null)
						{
							this.copyAttibutes.Dispose();
						}
					}
				}
			}
		}
	}
}
