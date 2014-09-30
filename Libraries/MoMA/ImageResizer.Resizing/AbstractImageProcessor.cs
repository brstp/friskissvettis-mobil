using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Resizing
{
	[ComVisible(true)]
	public class AbstractImageProcessor
	{
		protected volatile IEnumerable<BuilderExtension> exts;
		public AbstractImageProcessor()
		{
			this.exts = null;
		}
		public AbstractImageProcessor(IEnumerable<BuilderExtension> extensions)
		{
			this.exts = new List<BuilderExtension>((extensions != null) ? extensions : new BuilderExtension[0]);
		}
		protected virtual void PreLoadImage(ref object source, ResizeSettings settings)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					current.PreLoadImage(ref source, settings);
				}
			}
		}
		public virtual Bitmap DecodeStreamFailed(Stream s, ResizeSettings settings, string optionalPath)
		{
			if (this.exts == null)
			{
				return null;
			}
			foreach (AbstractImageProcessor current in this.exts)
			{
				Bitmap bitmap = current.DecodeStreamFailed(s, settings, optionalPath);
				if (bitmap != null)
				{
					return bitmap;
				}
			}
			return null;
		}
		public virtual Bitmap DecodeStream(Stream s, ResizeSettings settings, string optionalPath)
		{
			if (this.exts == null)
			{
				return null;
			}
			foreach (AbstractImageProcessor current in this.exts)
			{
				Bitmap bitmap = current.DecodeStream(s, settings, optionalPath);
				if (bitmap != null)
				{
					return bitmap;
				}
			}
			return null;
		}
		protected virtual void PreAcquireStream(ref object dest, ResizeSettings settings)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					current.PreAcquireStream(ref dest, settings);
				}
			}
		}
		protected virtual RequestedAction buildToStream(Bitmap source, Stream dest, ResizeSettings settings)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.buildToStream(source, dest, settings) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual Bitmap buildToBitmap(Bitmap source, ResizeSettings settings, bool transparencySupported)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					Bitmap bitmap = current.buildToBitmap(source, settings, transparencySupported);
					if (bitmap != null)
					{
						return bitmap;
					}
				}
			}
			return null;
		}
		protected virtual RequestedAction OnProcess(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.OnProcess(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PrepareSourceBitmap(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PrepareSourceBitmap(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostPrepareSourceBitmap(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostPrepareSourceBitmap(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction Layout(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.Layout(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction FlipExistingPoints(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.FlipExistingPoints(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutImage(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutImage(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutImage(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutImage(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutPadding(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutPadding(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutPadding(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutPadding(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutBorder(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutBorder(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutBorder(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutBorder(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutEffects(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutEffects(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutEffects(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutEffects(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutMargin(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutMargin(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutMargin(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutMargin(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutRotate(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutRotate(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutRotate(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutRotate(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutNormalize(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutNormalize(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutNormalize(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutNormalize(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction LayoutRound(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.LayoutRound(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostLayoutRound(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostLayoutRound(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction EndLayout(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.EndLayout(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PrepareDestinationBitmap(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PrepareDestinationBitmap(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction Render(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.Render(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction RenderBackground(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.RenderBackground(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostRenderBackground(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostRenderBackground(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction RenderEffects(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.RenderEffects(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostRenderEffects(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostRenderEffects(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction RenderPadding(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.RenderPadding(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostRenderPadding(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostRenderPadding(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction CreateImageAttribues(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.CreateImageAttribues(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostCreateImageAttributes(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostCreateImageAttributes(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction RenderImage(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.RenderImage(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostRenderImage(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostRenderImage(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction RenderBorder(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.RenderBorder(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostRenderBorder(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostRenderBorder(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PreRenderOverlays(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PreRenderOverlays(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction RenderOverlays(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.RenderOverlays(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PreFlushChanges(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PreFlushChanges(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction FlushChanges(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.FlushChanges(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction PostFlushChanges(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.PostFlushChanges(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction ProcessFinalBitmap(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.ProcessFinalBitmap(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
		protected virtual RequestedAction EndProcess(ImageState s)
		{
			if (this.exts != null)
			{
				foreach (AbstractImageProcessor current in this.exts)
				{
					if (current.EndProcess(s) == RequestedAction.Cancel)
					{
						return RequestedAction.Cancel;
					}
				}
				return RequestedAction.None;
			}
			return RequestedAction.None;
		}
	}
}
