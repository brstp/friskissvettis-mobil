using ImageResizer.Configuration;
using ImageResizer.Encoding;
using ImageResizer.Plugins;
using ImageResizer.Resizing;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Hosting;
namespace ImageResizer
{
	[ComVisible(true)]
	public class ImageBuilder : AbstractImageProcessor, IQuerystringPlugin, IFileExtensionPlugin
	{
		protected class BitmapHolder
		{
			public Bitmap bitmap;
		}
		protected IEncoderProvider _encoderProvider;
		private readonly string[] _supportedFileExtensions = new string[]
		{
			"bmp",
			"gif",
			"exif",
			"png",
			"tif",
			"tiff",
			"tff",
			"jpg",
			"jpeg",
			"jpe",
			"jif",
			"jfif",
			"jfi"
		};
		private readonly string[] _supportedQuerystringKeys = new string[]
		{
			"format",
			"thumbnail",
			"maxwidth",
			"maxheight",
			"width",
			"height",
			"scale",
			"stretch",
			"crop",
			"page",
			"bgcolor",
			"rotate",
			"flip",
			"sourceFlip",
			"borderWidth",
			"borderColor",
			"paddingWidth",
			"paddingColor",
			"ignoreicc",
			"frame",
			"useresizingpipeline",
			"cache",
			"process"
		};
		public IEncoderProvider EncoderProvider
		{
			get
			{
				return this._encoderProvider;
			}
		}
		public static ImageBuilder Current
		{
			get
			{
				return Config.Current.CurrentImageBuilder;
			}
		}
		public ImageBuilder(IEncoderProvider encoderProvider)
		{
			this._encoderProvider = encoderProvider;
		}
		public ImageBuilder(IEnumerable<BuilderExtension> extensions, IEncoderProvider encoderProvider) : base(extensions)
		{
			this._encoderProvider = encoderProvider;
		}
		public virtual ImageBuilder Create(IEnumerable<BuilderExtension> extensions, IEncoderProvider writer)
		{
			return new ImageBuilder(extensions, writer);
		}
		public virtual ImageBuilder Copy()
		{
			return new ImageBuilder(this.exts, this._encoderProvider);
		}
		public virtual Bitmap LoadImage(object source, ResizeSettings settings)
		{
			bool flag = !(source is Stream) && !(source is HttpPostedFile);
			this.PreLoadImage(ref source, settings);
			Bitmap bitmap = null;
			string message = "File may be corrupted, empty, or may contain a PNG image with a single dimension greater than 65,535 pixels.";
			string text = null;
			if (source is string)
			{
				text = (source as string);
				if (text.StartsWith("~", StringComparison.OrdinalIgnoreCase))
				{
					source = HostingEnvironment.VirtualPathProvider.GetFile(PathUtils.ResolveAppRelative(text));
				}
			}
			if (source is Bitmap)
			{
				return source as Bitmap;
			}
			if (source is Image)
			{
				return new Bitmap((Image)source);
			}
			if (source is IVirtualBitmapFile)
			{
				bitmap = ((IVirtualBitmapFile)source).GetBitmap();
				if (bitmap.Tag == null)
				{
					bitmap.Tag = new BitmapTag(((IVirtualBitmapFile)source).VirtualPath);
				}
				return bitmap;
			}
			Stream stream = null;
			text = null;
			if (source is Stream)
			{
				stream = (Stream)source;
			}
			else
			{
				if (source is HttpPostedFile)
				{
					text = ((HttpPostedFile)source).FileName;
					stream = ((HttpPostedFile)source).InputStream;
				}
				else
				{
					if (source is VirtualFile)
					{
						text = ((VirtualFile)source).VirtualPath;
						stream = ((VirtualFile)source).Open();
					}
					else
					{
						if (source is IVirtualFile)
						{
							text = ((IVirtualFile)source).VirtualPath;
							stream = ((IVirtualFile)source).Open();
						}
						else
						{
							if (!(source is string))
							{
								throw new ArgumentException("Paramater source may only be an instance of string, VirtualFile, IVirtualBitmapFile, HttpPostedFile, Bitmap, Image, or Stream.", "source");
							}
							text = (string)source;
							stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read);
						}
					}
				}
			}
			long num = (source is HttpPostedFile) ? stream.Position : -1L;
			try
			{
				try
				{
					bitmap = this.DecodeStream(stream, settings, text);
					if (bitmap == null)
					{
						throw new ImageCorruptedException("Failed to decode image. Plugin made DecodeStream return null.", null);
					}
				}
				catch (Exception ex)
				{
					if (stream.CanSeek && stream.Position != 0L)
					{
						stream.Seek(0L, SeekOrigin.Begin);
					}
					else
					{
						if (!stream.CanSeek)
						{
							throw new ImageCorruptedException("Cannot attempt fallback decoding path on a non-seekable stream", ex);
						}
					}
					bitmap = this.DecodeStreamFailed(stream, settings, text);
					if (bitmap == null)
					{
						throw ex;
					}
				}
			}
			catch (ArgumentException ex2)
			{
				ex2.Data.Add("path", text);
				throw new ImageCorruptedException(message, ex2);
			}
			catch (ExternalException ex3)
			{
				ex3.Data.Add("path", text);
				throw new ImageCorruptedException(message, ex3);
			}
			finally
			{
				if (bitmap != null && bitmap.Tag != null && bitmap.Tag is BitmapTag && ((BitmapTag)bitmap.Tag).Source == stream)
				{
					stream = null;
				}
				if (stream != null && flag)
				{
					stream.Dispose();
					stream = null;
				}
				if (num > -1L && stream != null && stream.CanSeek)
				{
					stream.Position = num;
				}
				if (bitmap != null && bitmap.Tag == null && text != null)
				{
					bitmap.Tag = new BitmapTag(text);
				}
			}
			return bitmap;
		}
		public override Bitmap DecodeStream(Stream s, ResizeSettings settings, string optionalPath)
		{
			Bitmap bitmap = base.DecodeStream(s, settings, optionalPath);
			if (bitmap != null && bitmap.Tag == null)
			{
				bitmap.Tag = new BitmapTag(optionalPath);
			}
			if (bitmap != null)
			{
				return bitmap;
			}
			bool useIcm = true;
			if (settings != null && "true".Equals(settings["ignoreicc"], StringComparison.OrdinalIgnoreCase))
			{
				useIcm = false;
			}
			MemoryStream memoryStream = StreamUtils.CopyStream(s);
			return new Bitmap(memoryStream, useIcm)
			{
				Tag = new BitmapTag(optionalPath, memoryStream)
			};
		}
		public virtual Bitmap Build(object source, ResizeSettings settings)
		{
			return this.Build(source, settings, true);
		}
		public virtual Bitmap Build(object source, ResizeSettings settings, bool disposeSource)
		{
			ImageBuilder.BitmapHolder bitmapHolder = new ImageBuilder.BitmapHolder();
			this.Build(source, bitmapHolder, settings, disposeSource);
			return bitmapHolder.bitmap;
		}
		public virtual void Build(object source, object dest, ResizeSettings settings)
		{
			this.Build(source, dest, settings, true);
		}
		public virtual void Build(object source, object dest, ResizeSettings settings, bool disposeSource)
		{
			this.Build(source, dest, settings, disposeSource, false);
		}
		public virtual string Build(object source, object dest, ResizeSettings settings, bool disposeSource, bool addFileExtension)
		{
			ResizeSettings settings2 = new ResizeSettings(settings);
			Bitmap bitmap = null;
			try
			{
				bitmap = this.LoadImage(source, settings);
				this.PreAcquireStream(ref dest, settings);
				if (dest is string)
				{
					string text = dest as string;
					if (text.StartsWith("~", StringComparison.OrdinalIgnoreCase))
					{
						text = HostingEnvironment.MapPath(text);
					}
					if (addFileExtension)
					{
						IEncoder encoder = this.EncoderProvider.GetEncoder(settings, bitmap);
						if (encoder != null)
						{
							text = text + "." + encoder.Extension;
						}
					}
					FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write);
					using (fileStream)
					{
						this.buildToStream(bitmap, fileStream, settings2);
					}
					return text;
				}
				if (dest is Stream)
				{
					this.buildToStream(bitmap, (Stream)dest, settings2);
				}
				else
				{
					if (!(dest is ImageBuilder.BitmapHolder))
					{
						throw new ArgumentException("Paramater dest may only be a string, Stream, or BitmapHolder", "dest");
					}
					((ImageBuilder.BitmapHolder)dest).bitmap = this.buildToBitmap(bitmap, settings2, true);
				}
			}
			finally
			{
				Stream stream = null;
				if (bitmap != null && bitmap.Tag != null && bitmap.Tag is BitmapTag)
				{
					stream = ((BitmapTag)bitmap.Tag).Source;
				}
				if (bitmap != source && stream != source && stream != null)
				{
					stream.Dispose();
				}
				if (bitmap != null && bitmap != source)
				{
					bitmap.Dispose();
				}
				if (disposeSource && source is IDisposable)
				{
					((IDisposable)source).Dispose();
				}
			}
			return null;
		}
		protected override RequestedAction buildToStream(Bitmap source, Stream dest, ResizeSettings settings)
		{
			if (base.buildToStream(source, dest, settings) == RequestedAction.Cancel)
			{
				return RequestedAction.None;
			}
			IEncoder encoder = this.EncoderProvider.GetEncoder(settings, source);
			if (encoder == null)
			{
				throw new ImageProcessingException("No image encoder was found for this request.");
			}
			using (Bitmap bitmap = this.buildToBitmap(source, settings, encoder.SupportsTransparency))
			{
				encoder.Write(bitmap, dest);
			}
			return RequestedAction.None;
		}
		protected override Bitmap buildToBitmap(Bitmap source, ResizeSettings settings, bool transparencySupported)
		{
			Bitmap bitmap = base.buildToBitmap(source, settings, transparencySupported);
			if (bitmap != null)
			{
				return bitmap;
			}
			using (ImageState imageState = new ImageState(settings, source.Size, transparencySupported))
			{
				imageState.sourceBitmap = source;
				this.Process(imageState);
				bitmap = imageState.destBitmap;
				imageState.destBitmap = null;
				imageState.sourceBitmap = null;
			}
			return bitmap;
		}
		public virtual void Process(ImageState s)
		{
			if (this.OnProcess(s) == RequestedAction.Cancel)
			{
				return;
			}
			this.PrepareSourceBitmap(s);
			this.PostPrepareSourceBitmap(s);
			this.Layout(s);
			this.PrepareDestinationBitmap(s);
			this.Render(s);
			this.ProcessFinalBitmap(s);
			this.EndProcess(s);
		}
		protected override RequestedAction Layout(ImageState s)
		{
			if (base.Layout(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			this.FlipExistingPoints(s);
			this.LayoutImage(s);
			this.PostLayoutImage(s);
			this.LayoutPadding(s);
			this.PostLayoutPadding(s);
			this.LayoutBorder(s);
			this.PostLayoutBorder(s);
			this.LayoutEffects(s);
			this.PostLayoutEffects(s);
			this.LayoutMargin(s);
			this.PostLayoutMargin(s);
			this.LayoutRotate(s);
			this.PostLayoutRotate(s);
			this.LayoutNormalize(s);
			this.PostLayoutNormalize(s);
			this.LayoutRound(s);
			this.PostLayoutRound(s);
			this.EndLayout(s);
			return RequestedAction.None;
		}
		protected override RequestedAction Render(ImageState s)
		{
			if (base.Render(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			this.RenderBackground(s);
			this.PostRenderBackground(s);
			this.RenderEffects(s);
			this.PostRenderEffects(s);
			this.RenderPadding(s);
			this.PostRenderPadding(s);
			this.CreateImageAttribues(s);
			this.PostCreateImageAttributes(s);
			this.RenderImage(s);
			this.PostRenderImage(s);
			this.RenderBorder(s);
			this.PostRenderBorder(s);
			this.PreRenderOverlays(s);
			this.RenderOverlays(s);
			this.PreFlushChanges(s);
			this.FlushChanges(s);
			this.PostFlushChanges(s);
			return RequestedAction.None;
		}
		protected override RequestedAction PrepareSourceBitmap(ImageState s)
		{
			if (base.PrepareSourceBitmap(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.sourceBitmap == null)
			{
				return RequestedAction.None;
			}
			Bitmap sourceBitmap = s.sourceBitmap;
			ResizeSettings settings = s.settings;
			int num = 0;
			if (!string.IsNullOrEmpty(settings["page"]) && !int.TryParse(settings["page"], out num))
			{
				num = 0;
			}
			int num2 = 0;
			if (!string.IsNullOrEmpty(settings["frame"]) && !int.TryParse(settings["frame"], out num2))
			{
				num2 = 0;
			}
			num--;
			num2--;
			try
			{
				if (num > 0 && num >= sourceBitmap.GetFrameCount(FrameDimension.Page))
				{
					num = sourceBitmap.GetFrameCount(FrameDimension.Page) - 1;
				}
				if (num2 > 0 && num2 >= sourceBitmap.GetFrameCount(FrameDimension.Time))
				{
					num2 = sourceBitmap.GetFrameCount(FrameDimension.Time) - 1;
				}
				if (num > 0)
				{
					sourceBitmap.SelectActiveFrame(FrameDimension.Page, num);
				}
				if (num2 > 0)
				{
					sourceBitmap.SelectActiveFrame(FrameDimension.Time, num2);
				}
			}
			catch (ExternalException)
			{
			}
			if (s.sourceBitmap != null && s.settings.SourceFlip != RotateFlipType.RotateNoneFlipNone)
			{
				s.sourceBitmap.RotateFlip(s.settings.SourceFlip);
				s.originalSize = s.sourceBitmap.Size;
			}
			return RequestedAction.None;
		}
		protected override RequestedAction LayoutPadding(ImageState s)
		{
			if (base.LayoutPadding(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (!s.settings.Padding.IsEmpty)
			{
				s.layout.AddRing("padding", s.settings.Padding);
			}
			return RequestedAction.None;
		}
		protected override RequestedAction LayoutMargin(ImageState s)
		{
			if (base.LayoutMargin(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (!s.settings.Margin.IsEmpty)
			{
				s.layout.AddRing("margin", s.settings.Margin);
			}
			return RequestedAction.None;
		}
		protected override RequestedAction LayoutBorder(ImageState s)
		{
			if (base.LayoutBorder(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (!s.settings.Border.IsEmpty)
			{
				s.layout.AddRing("border", s.settings.Border);
			}
			return RequestedAction.None;
		}
		protected override RequestedAction LayoutRound(ImageState s)
		{
			if (base.LayoutRound(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			return RequestedAction.None;
		}
		protected override RequestedAction LayoutRotate(ImageState s)
		{
			if (base.LayoutRotate(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			s.layout.Rotate(s.settings.Rotate, new PointF(0f, 0f));
			return RequestedAction.None;
		}
		protected override RequestedAction LayoutNormalize(ImageState s)
		{
			if (base.LayoutNormalize(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			s.layout.Normalize(new PointF(0f, 0f));
			return RequestedAction.None;
		}
		protected override RequestedAction EndLayout(ImageState s)
		{
			if (base.EndLayout(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			s.destSize = PolygonMath.RoundPoints(s.layout.GetBoundingBox().Size);
			s.destSize = new Size(Math.Max(1, s.destSize.Width), Math.Max(1, s.destSize.Height));
			return RequestedAction.None;
		}
		protected override RequestedAction PrepareDestinationBitmap(ImageState s)
		{
			if (base.PrepareDestinationBitmap(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.sourceBitmap == null)
			{
				return RequestedAction.None;
			}
			s.destBitmap = new Bitmap(s.destSize.Width, s.destSize.Height, PixelFormat.Format32bppArgb);
			Graphics graphics = s.destGraphics = Graphics.FromImage(s.destBitmap);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.CompositingMode = CompositingMode.SourceOver;
			return RequestedAction.None;
		}
		protected override RequestedAction RenderBackground(ImageState s)
		{
			if (base.RenderBackground(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.destGraphics == null)
			{
				return RequestedAction.None;
			}
			Graphics destGraphics = s.destGraphics;
			Color color = s.settings.BackgroundColor;
			if (color == Color.Transparent && !s.supportsTransparency && !PolygonMath.GetBoundingBox(s.layout["image"]).Equals(s.layout.GetBoundingBox()))
			{
				color = Color.White;
			}
			if (color != Color.Transparent)
			{
				destGraphics.Clear(color);
			}
			return RequestedAction.None;
		}
		protected override RequestedAction RenderPadding(ImageState s)
		{
			if (base.RenderPadding(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.destGraphics == null)
			{
				return RequestedAction.None;
			}
			Color color = s.settings.PaddingColor;
			if (color.Equals(Color.Transparent))
			{
				color = s.settings.BackgroundColor;
			}
			if (!color.Equals(s.settings.BackgroundColor) && color != Color.Transparent)
			{
				s.destGraphics.FillPolygon(new SolidBrush(color), s.layout["padding"]);
			}
			return RequestedAction.None;
		}
		protected override RequestedAction CreateImageAttribues(ImageState s)
		{
			if (base.CreateImageAttribues(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.copyAttibutes == null)
			{
				s.copyAttibutes = new ImageAttributes();
			}
			return RequestedAction.None;
		}
		protected override RequestedAction RenderImage(ImageState s)
		{
			if (base.RenderImage(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.destGraphics == null)
			{
				return RequestedAction.None;
			}
			s.copyAttibutes.SetWrapMode(WrapMode.TileFlipXY);
			s.destGraphics.DrawImage(s.sourceBitmap, PolygonMath.getParallelogram(s.layout["image"]), s.copyRect, GraphicsUnit.Pixel, s.copyAttibutes);
			return RequestedAction.None;
		}
		protected override RequestedAction RenderBorder(ImageState s)
		{
			if (base.RenderBorder(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.destGraphics == null)
			{
				return RequestedAction.None;
			}
			if (s.settings.Border.IsEmpty)
			{
				return RequestedAction.None;
			}
			if (s.settings.Border.All <= 0.0)
			{
				throw new NotImplementedException("Separate border widths have not yet been implemented");
			}
			Pen pen = new Pen(s.settings.BorderColor, (float)s.settings.Border.All);
			pen.Alignment = PenAlignment.Center;
			pen.LineJoin = LineJoin.Miter;
			s.destGraphics.DrawPolygon(pen, PolygonMath.InflatePoly(s.layout["border"], (float)(s.settings.Border.All / -2.0)));
			return RequestedAction.None;
		}
		protected override RequestedAction FlushChanges(ImageState s)
		{
			if (base.FlushChanges(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.destGraphics == null)
			{
				return RequestedAction.None;
			}
			s.destGraphics.Flush(FlushIntention.Flush);
			s.destGraphics.Dispose();
			s.destGraphics = null;
			return RequestedAction.None;
		}
		protected override RequestedAction ProcessFinalBitmap(ImageState s)
		{
			if (base.ProcessFinalBitmap(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			s.finalSize = s.destSize;
			if (s.destBitmap == null)
			{
				return RequestedAction.None;
			}
			if (s.settings.Flip != RotateFlipType.RotateNoneFlipNone)
			{
				s.destBitmap.RotateFlip(s.settings.Flip);
			}
			s.finalSize = s.destBitmap.Size;
			return RequestedAction.None;
		}
		public virtual PointF[] TranslatePoints(PointF[] sourcePoints, Size originalSize, ResizeSettings q)
		{
			ImageState imageState = new ImageState(q, originalSize, true);
			imageState.layout.AddInvisiblePolygon("points", sourcePoints);
			this.Process(imageState);
			return imageState.layout["points"];
		}
		public virtual Size GetFinalSize(Size originalSize, ResizeSettings q)
		{
			ImageState imageState = new ImageState(q, originalSize, true);
			this.Process(imageState);
			return imageState.finalSize;
		}
		protected override RequestedAction LayoutImage(ImageState s)
		{
			if (base.LayoutImage(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			s.copyRect = new RectangleF(new PointF(0f, 0f), s.originalSize);
			if (s.settings.CropMode == CropMode.Custom)
			{
				s.copyRect = s.settings.getCustomCropSourceRect(s.originalSize);
				if (s.copyRect.Size.IsEmpty)
				{
					throw new Exception("You must specify a custom crop rectange if crop=custom");
				}
			}
			double num = (double)(s.copySize.Width / s.copySize.Height);
			bool flag = s.settings.Width != -1 || s.settings.Height != -1 || s.settings.MaxHeight != -1 || s.settings.MaxWidth != -1;
			SizeF sizeF = new SizeF(-1f, -1f);
			SizeF sizeF2 = new SizeF(-1f, -1f);
			if (!flag)
			{
				sizeF = (sizeF2 = s.copySize);
			}
			else
			{
				double num2 = (double)s.settings.Width;
				double num3 = (double)s.settings.Height;
				double num4 = (double)s.settings.MaxWidth;
				double num5 = (double)s.settings.MaxHeight;
				if (num4 > 0.0 && num2 > 0.0)
				{
					num2 = Math.Min(num4, num2);
					num4 = -1.0;
				}
				if (num5 > 0.0 && num3 > 0.0)
				{
					num3 = Math.Min(num5, num3);
					num5 = -1.0;
				}
				if (num2 > 0.0 || num3 > 0.0)
				{
					if (num2 > 0.0)
					{
						if (num3 < 0.0)
						{
							num3 = num2 / num;
						}
						if (num5 > 0.0 && num3 > num5)
						{
							num3 = num5;
						}
					}
					else
					{
						if (num3 > 0.0)
						{
							if (num2 < 0.0)
							{
								num2 = num3 * num;
							}
							if (num5 > 0.0 && num3 > num5)
							{
								num3 = num5;
							}
						}
					}
					sizeF = new SizeF((float)num2, (float)num3);
				}
				else
				{
					if (num5 > 0.0 && num4 <= 0.0)
					{
						num4 = num5 * num;
					}
					else
					{
						if (num4 > 0.0 && num5 <= 0.0)
						{
							num5 = num4 / num;
						}
					}
					sizeF = PolygonMath.ScaleInside(s.copySize, new SizeF((float)num4, (float)num5));
				}
				sizeF2 = sizeF;
				if (s.settings.Stretch == StretchMode.Proportionally)
				{
					sizeF = PolygonMath.ScaleInside(s.copySize, sizeF2);
				}
				if (s.settings.Scale == ScaleMode.DownscaleOnly)
				{
					if (PolygonMath.FitsInside(s.copySize, sizeF))
					{
						sizeF = (sizeF2 = s.copySize);
					}
				}
				else
				{
					if (s.settings.Scale == ScaleMode.UpscaleOnly)
					{
						if (!PolygonMath.FitsInside(s.copySize, sizeF))
						{
							sizeF = (sizeF2 = s.copySize);
						}
					}
					else
					{
						if (s.settings.Scale == ScaleMode.UpscaleCanvas && PolygonMath.FitsInside(s.copySize, sizeF))
						{
							sizeF = s.copySize;
						}
					}
				}
			}
			sizeF2.Width = Math.Max(1f, sizeF2.Width);
			sizeF2.Height = Math.Max(1f, sizeF2.Height);
			if (s.settings.CropMode == CropMode.Auto && s.settings.Stretch == StretchMode.Proportionally)
			{
				SizeF sizeF3 = PolygonMath.ScaleInside(sizeF2, s.originalSize);
				s.copyRect = new RectangleF(((float)s.originalSize.Width - sizeF3.Width) / 2f, ((float)s.originalSize.Height - sizeF3.Height) / 2f, sizeF3.Width, sizeF3.Height);
				sizeF = sizeF2;
			}
			sizeF2.Width = Math.Max(1f, (float)Math.Round((double)sizeF2.Width));
			sizeF2.Height = Math.Max(1f, (float)Math.Round((double)sizeF2.Height));
			sizeF.Width = Math.Max(1f, (float)Math.Round((double)sizeF.Width));
			sizeF.Height = Math.Max(1f, (float)Math.Round((double)sizeF.Height));
			s.layout.Shift(new RectangleF(0f, 0f, (float)s.originalSize.Width, (float)s.originalSize.Height), new RectangleF(new Point(0, 0), sizeF));
			s.layout.AddRing("image", PolygonMath.ToPoly(new RectangleF(new PointF(0f, 0f), sizeF)));
			s.layout.AddRing("imageArea", PolygonMath.ToPoly(new RectangleF(new PointF(0f, 0f), sizeF2)));
			s.layout["imageArea"] = PolygonMath.CenterInside(s.layout["imageArea"], s.layout["image"]);
			return RequestedAction.None;
		}
		public virtual IEnumerable<string> GetSupportedFileExtensions()
		{
			return this._supportedFileExtensions;
		}
		public virtual IEnumerable<string> GetSupportedQuerystringKeys()
		{
			return this._supportedQuerystringKeys;
		}
	}
}
