using ImageResizer.Caching;
using ImageResizer.Configuration;
using ImageResizer.Resizing;
using ImageResizer.Util;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class Trial : BuilderExtension, IPlugin
	{
		public enum TrialWatermarkMode
		{
			After500,
			Always,
			Randomly
		}
		private Config c;
		private static int requestCount;
		public static void InstallPermanent()
		{
			Config.Current.Pipeline.PreHandleImage -= new PreHandleImageEventHandler(Trial.Pipeline_PreHandleImage);
			Config.Current.Pipeline.PreHandleImage += new PreHandleImageEventHandler(Trial.Pipeline_PreHandleImage);
			if (!Config.Current.Plugins.Has<Trial>())
			{
				new Trial().Install(Config.Current);
			}
		}
		private static void Pipeline_PreHandleImage(IHttpModule sender, HttpContext context, IResponseArgs e)
		{
			if (!Config.Current.Plugins.Has<Trial>())
			{
				new Trial().Install(Config.Current);
			}
		}
		public IPlugin Install(Config c)
		{
			c.Plugins.add_plugin(this);
			this.c = c;
			return this;
		}
		public bool Uninstall(Config c)
		{
			return false;
		}
		protected override RequestedAction PreFlushChanges(ImageState s)
		{
			if (s.destGraphics == null)
			{
				return RequestedAction.None;
			}
			Interlocked.Increment(ref Trial.requestCount);
			string value = this.c.get("trial.watermarkMode", "After500");
			Trial.TrialWatermarkMode trialWatermarkMode = Trial.TrialWatermarkMode.After500;
			if ("always".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				trialWatermarkMode = Trial.TrialWatermarkMode.Always;
			}
			if ("randomly".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				trialWatermarkMode = Trial.TrialWatermarkMode.Randomly;
			}
			bool flag = trialWatermarkMode == Trial.TrialWatermarkMode.Always;
			if (trialWatermarkMode == Trial.TrialWatermarkMode.After500 && Trial.requestCount > 500)
			{
				flag = true;
			}
			if (trialWatermarkMode == Trial.TrialWatermarkMode.Randomly)
			{
				flag = (new Random(Trial.requestCount).Next(0, 41) < 10);
			}
			if (!flag)
			{
				return RequestedAction.None;
			}
			this.DrawString(PolygonMath.GetBoundingBox(s.layout["image"]), s.destGraphics, "Unlicensed", FontFamily.GenericSansSerif, Color.FromArgb(70, Color.White));
			return RequestedAction.None;
		}
		public virtual void DrawString(RectangleF area, Graphics g, string text, FontFamily ff, Color c)
		{
			SizeF sizeF = g.MeasureString(text, new Font(ff, 32f));
			double val = (double)((sizeF.Width - area.Width) / -(double)sizeF.Width);
			double val2 = (double)((sizeF.Height - area.Height) / -(double)sizeF.Height);
			float emSize = 32f + (float)(32.0 * Math.Min(val, val2));
			SizeF sizeF2 = g.MeasureString(text, new Font(ff, emSize));
			g.DrawString(text, new Font(ff, emSize), new SolidBrush(c), new PointF((area.Width - sizeF2.Width) / 2f + area.Left, (area.Height - sizeF2.Height) / 2f + area.Height));
			g.Flush();
		}
	}
}
