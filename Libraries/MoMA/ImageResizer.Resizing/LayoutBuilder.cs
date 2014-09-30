using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ImageResizer.Resizing
{
	[ComVisible(true)]
	public class LayoutBuilder
	{
		[Flags]
		public enum PointFlags
		{
			Ring = 1,
			Invisible = 2,
			Ignored = 4
		}
		public enum PointTranslationBehavior
		{
			Exact,
			ClosestVisiblePoint,
			ClosestImagePoint,
			Empty
		}
		public class PointSet
		{
			public PointF[] points;
			public LayoutBuilder.PointFlags flags = LayoutBuilder.PointFlags.Ring;
			protected LayoutBuilder.PointTranslationBehavior pointBehavior = LayoutBuilder.PointTranslationBehavior.ClosestImagePoint;
			public LayoutBuilder.PointTranslationBehavior PointBehavior
			{
				get
				{
					return this.pointBehavior;
				}
			}
			public PointSet(PointF[] pts, LayoutBuilder.PointFlags settings)
			{
				this.points = pts;
				this.flags = settings;
			}
			public PointSet(PointF[] pts)
			{
				this.points = pts;
			}
		}
		protected Dictionary<string, LayoutBuilder.PointSet> ring = new Dictionary<string, LayoutBuilder.PointSet>(10, StringComparer.OrdinalIgnoreCase);
		protected List<LayoutBuilder.PointSet> ringList = new List<LayoutBuilder.PointSet>(10);
		public PointF[] this[string key]
		{
			get
			{
				return this.ring[key].points;
			}
			set
			{
				this.ring[key].points = value;
			}
		}
		public LayoutBuilder.PointSet LastRing
		{
			get
			{
				for (int i = this.ringList.Count - 1; i >= 0; i--)
				{
					if (this.ringList[i].flags == LayoutBuilder.PointFlags.Ring)
					{
						return this.ringList[i];
					}
				}
				return null;
			}
		}
		public LayoutBuilder.PointSet AddRing(string name, PointF[] points)
		{
			LayoutBuilder.PointSet pointSet = new LayoutBuilder.PointSet(points);
			this.ring.Add(name, pointSet);
			this.ringList.Add(pointSet);
			return pointSet;
		}
		public LayoutBuilder.PointSet AddRing(string name, BoxPadding padding)
		{
			return this.AddRing(name, PolygonMath.InflatePoly(this.LastRing.points, padding.GetEdgeOffsets()));
		}
		public LayoutBuilder.PointSet AddInvisiblePolygon(string name, PointF[] points)
		{
			LayoutBuilder.PointSet pointSet = new LayoutBuilder.PointSet(points);
			pointSet.flags = LayoutBuilder.PointFlags.Invisible;
			this.ring.Add(name, pointSet);
			this.ringList.Add(pointSet);
			return pointSet;
		}
		public LayoutBuilder.PointSet AddIgnoredPoints(string name, PointF[] points)
		{
			LayoutBuilder.PointSet pointSet = new LayoutBuilder.PointSet(points);
			pointSet.flags = LayoutBuilder.PointFlags.Ignored;
			this.ring.Add(name, pointSet);
			this.ringList.Add(pointSet);
			return pointSet;
		}
		public RectangleF GetBoundingBox()
		{
			List<PointF> list = new List<PointF>(this.ring.Count * 5);
			foreach (LayoutBuilder.PointSet current in this.ringList)
			{
				if (current.flags == LayoutBuilder.PointFlags.Ring)
				{
					list.AddRange(current.points);
				}
			}
			return PolygonMath.GetBoundingBox(list.ToArray());
		}
		public void Rotate(double degrees, PointF origin)
		{
			foreach (LayoutBuilder.PointSet current in this.ringList)
			{
				if (current.flags != LayoutBuilder.PointFlags.Ignored)
				{
					current.points = PolygonMath.RotatePoly(current.points, degrees, origin);
				}
			}
		}
		public void Normalize(PointF origin)
		{
			PointF location = this.GetBoundingBox().Location;
			location.X *= -1f;
			location.Y *= -1f;
			foreach (LayoutBuilder.PointSet current in this.ringList)
			{
				if (current.flags != LayoutBuilder.PointFlags.Ignored)
				{
					current.points = PolygonMath.MovePoly(current.points, location);
				}
			}
		}
		public void Round()
		{
			foreach (LayoutBuilder.PointSet current in this.ringList)
			{
				if (current.flags != LayoutBuilder.PointFlags.Ignored)
				{
					current.points = PolygonMath.RoundPoints(current.points);
				}
			}
		}
		public void Scale(double factor, PointF origin)
		{
			foreach (LayoutBuilder.PointSet current in this.ringList)
			{
				if (current.flags != LayoutBuilder.PointFlags.Ignored)
				{
					current.points = PolygonMath.ScalePoints(current.points, factor, factor, origin);
				}
			}
		}
		public void Shift(RectangleF from, RectangleF to)
		{
			PointF pointF = new PointF(from.X + from.Width / 2f, from.Y + from.Height / 2f);
			PointF origin = new PointF(to.X + to.Width / 2f, to.Y + to.Height / 2f);
			PointF offset = new PointF(origin.X - pointF.X, origin.Y - pointF.Y);
			double xfactor = (double)(to.Width / from.Width);
			double yfactor = (double)(to.Height / from.Height);
			foreach (LayoutBuilder.PointSet current in this.ringList)
			{
				if (current.flags != LayoutBuilder.PointFlags.Ignored)
				{
					current.points = PolygonMath.MovePoly(current.points, offset);
				}
			}
			foreach (LayoutBuilder.PointSet current2 in this.ringList)
			{
				if (current2.flags != LayoutBuilder.PointFlags.Ignored)
				{
					current2.points = PolygonMath.ScalePoints(current2.points, xfactor, yfactor, origin);
				}
			}
		}
	}
}
