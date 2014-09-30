using ImageResizer.Configuration;
using ImageResizer.Resizing;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class DropShadow : BuilderExtension, IPlugin, IQuerystringPlugin
	{
		public IPlugin Install(Config c)
		{
			c.Plugins.add_plugin(this);
			return this;
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			return true;
		}
		public IEnumerable<string> GetSupportedQuerystringKeys()
		{
			return new string[]
			{
				"shadowColor",
				"shadowOffset",
				"shadowWidth"
			};
		}
		protected override RequestedAction LayoutEffects(ImageState s)
		{
			if (base.LayoutEffects(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			if (s.settings["shadowWidth"] != null)
			{
				float @float = Utils.getFloat(s.settings, "shadowWidth", 0f);
				PointF offset = Utils.parsePointF(s.settings["shadowOffset"], new PointF(0f, 0f));
				s.layout.AddInvisiblePolygon("shadowInner", PolygonMath.MovePoly(s.layout.LastRing.points, offset));
				s.layout.AddRing("shadow", PolygonMath.InflatePoly(s.layout.LastRing.points, new float[]
				{
					Math.Max(0f, @float - offset.Y),
					Math.Max(0f, @float + offset.X),
					Math.Max(0f, @float + offset.Y),
					Math.Max(0f, @float - offset.X)
				}));
			}
			return RequestedAction.None;
		}
		protected override RequestedAction RenderEffects(ImageState s)
		{
			if (base.RenderEffects(s) == RequestedAction.Cancel)
			{
				return RequestedAction.Cancel;
			}
			Color color = Utils.parseColor(s.settings["shadowColor"], Color.Transparent);
			int @int = Utils.getInt(s.settings, "shadowWidth", -1);
			if (color == Color.Transparent || @int <= 0)
			{
				return RequestedAction.None;
			}
			s.destGraphics.FillPolygon(new SolidBrush(color), PolygonMath.InflatePoly(s.layout["shadowInner"], 1f));
			Utils.DrawOuterGradient(s.destGraphics, s.layout["shadowInner"], color, Color.Transparent, (float)@int);
			return RequestedAction.None;
		}
	}
}
