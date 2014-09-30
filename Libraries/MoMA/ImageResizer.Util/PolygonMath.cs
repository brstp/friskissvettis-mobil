using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
namespace ImageResizer.Util
{
	[ComVisible(true)]
	public class PolygonMath
	{
		public delegate object ForEachFunction(object o);
		public static PointF[] RoundPoints(PointF[] a)
		{
			PolygonMath.ForEach(a, delegate(object o)
			{
				PointF pointF = (PointF)o;
				pointF.X = (float)Math.Round((double)pointF.X);
				pointF.Y = (float)Math.Round((double)pointF.Y);
				return pointF;
			});
			return a;
		}
		public static PointF[,] RoundPoints(PointF[,] a)
		{
			PolygonMath.ForEach(a, delegate(object o)
			{
				PointF pointF = (PointF)o;
				pointF.X = (float)Math.Round((double)pointF.X);
				pointF.Y = (float)Math.Round((double)pointF.Y);
				return pointF;
			});
			return a;
		}
		public static void ForEach(Array a, PolygonMath.ForEachFunction func)
		{
			long[] array = new long[a.Rank];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (long)a.GetLowerBound(i);
			}
			for (long num = 0L; num < a.LongLength; num += 1L)
			{
				a.SetValue(func(a.GetValue(array)), array);
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] < (long)a.GetUpperBound(j))
					{
						array[j] += 1L;
						break;
					}
					array[j] = (long)a.GetLowerBound(j);
					if (j == array.Length - 1 && num < a.LongLength - 1L)
					{
						throw new Exception();
					}
				}
			}
		}
		public static PointF[] RotatePoly(PointF[] poly, double degrees)
		{
			PointF[] array = new PointF[poly.Length];
			for (int i = 0; i < poly.Length; i++)
			{
				array[i] = PolygonMath.RotateVector(poly[i], degrees * 3.1415926535897931 / 180.0);
			}
			return array;
		}
		public static PointF[] RotatePoly(PointF[] poly, double degrees, PointF origin)
		{
			PointF[] array = new PointF[poly.Length];
			for (int i = 0; i < poly.Length; i++)
			{
				array[i] = PolygonMath.RotateVector(poly[i], degrees * 3.1415926535897931 / 180.0, origin);
			}
			return array;
		}
		public static PointF[] ScalePoints(PointF[] poly, double xfactor, double yfactor, PointF origin)
		{
			PointF[] array = new PointF[poly.Length];
			for (int i = 0; i < poly.Length; i++)
			{
				array[i] = PolygonMath.ScalePoint(poly[i], xfactor, yfactor, origin);
			}
			return array;
		}
		public static PointF ScalePoint(PointF point, double xfactor, double yfactor, PointF origin)
		{
			return new PointF((float)((double)(point.X - origin.X) * xfactor + (double)origin.X), (float)((double)(point.Y - origin.Y) * yfactor + (double)origin.Y));
		}
		public static PointF[] ToPoly(RectangleF rect)
		{
			return new PointF[]
			{
				rect.Location,
				new PointF(rect.Right, rect.Top),
				new PointF(rect.Right, rect.Bottom),
				new PointF(rect.Left, rect.Bottom)
			};
		}
		public static PointF[] NormalizePoly(PointF[] poly)
		{
			RectangleF boundingBox = PolygonMath.GetBoundingBox(poly);
			return PolygonMath.MovePoly(poly, new PointF(-boundingBox.X, -boundingBox.Y));
		}
		public static PointF RotateVector(PointF v, double radians)
		{
			return new PointF((float)(Math.Cos(radians) * (double)v.X - Math.Sin(radians) * (double)v.Y), (float)(Math.Sin(radians) * (double)v.X + Math.Cos(radians) * (double)v.Y));
		}
		public static PointF RotateVector(PointF v, double radians, PointF origin)
		{
			return new PointF((float)(Math.Cos(radians) * (double)(v.X - origin.X) - Math.Sin(radians) * (double)(v.Y - origin.Y)) + origin.X, (float)(Math.Sin(radians) * (double)(v.X - origin.X) + Math.Cos(radians) * (double)(v.Y - origin.Y)) + origin.Y);
		}
		public static PointF ChangeMagnitude(PointF v, float length)
		{
			double num = Math.Sqrt((double)(v.X * v.X + v.Y * v.Y));
			float num2 = (float)((double)length / num);
			return new PointF(v.X * num2, v.Y * num2);
		}
		public static RectangleF GetBoundingBox(PointF[] points)
		{
			float num = 3.40282347E+38f;
			float num2 = 3.40282347E+38f;
			float num3 = -3.40282347E+38f;
			float num4 = -3.40282347E+38f;
			for (int i = 0; i < points.Length; i++)
			{
				PointF pointF = points[i];
				if (pointF.X < num)
				{
					num = pointF.X;
				}
				if (pointF.X > num3)
				{
					num3 = pointF.X;
				}
				if (pointF.Y < num2)
				{
					num2 = pointF.Y;
				}
				if (pointF.Y > num4)
				{
					num4 = pointF.Y;
				}
			}
			return new RectangleF(num, num2, num3 - num, num4 - num2);
		}
		public static PointF[] MovePoly(PointF[] points, PointF offset)
		{
			PointF[] array = new PointF[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				array[i].X = points[i].X + offset.X;
				array[i].Y = points[i].Y + offset.Y;
			}
			return array;
		}
		public static bool ArraysEqual(PointF[] a1, PointF[] a2)
		{
			if (a1.Length != a2.Length)
			{
				return false;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					return false;
				}
			}
			return true;
		}
		public static PointF[] getParallelogram(PointF[] quad)
		{
			return new PointF[]
			{
				quad[0],
				quad[1],
				quad[3]
			};
		}
		public static PointF[] GetSubArray(PointF[,] array, int index)
		{
			PointF[] array2 = new PointF[array.GetUpperBound(1) + 1];
			for (int i = 0; i < array.GetUpperBound(1) + 1; i++)
			{
				array2[i] = array[index, i];
			}
			return array2;
		}
		public static Brush GenerateRadialBrush(Color inner, Color outer, PointF pt, float width)
		{
			PointF[] array = new PointF[(int)Math.Round((double)(width * 2f) * 3.1415926535897931) + 1];
			for (int i = 0; i < array.Length - 1; i++)
			{
				double num = ((double)i - (double)width) / (double)width;
				array[i] = new PointF((float)(Math.Sin(num) * (double)width + (double)pt.X), (float)(Math.Cos(num) * (double)width + (double)pt.Y));
			}
			array[array.Length - 1] = array[0];
			PathGradientBrush pathGradientBrush = new PathGradientBrush(array);
			pathGradientBrush.CenterColor = inner;
			pathGradientBrush.CenterPoint = pt;
			pathGradientBrush.WrapMode = WrapMode.Clamp;
			Color[] array2 = new Color[array.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = outer;
			}
			pathGradientBrush.SurroundColors = array2;
			pathGradientBrush.SetSigmaBellShape(1f);
			return pathGradientBrush;
		}
		public static SizeF ScaleInside(SizeF inner, SizeF bounding)
		{
			double num = (double)(inner.Width / inner.Height);
			double num2 = (double)(bounding.Width / bounding.Height);
			if (num2 > num)
			{
				return new SizeF((float)(num * (double)bounding.Height), bounding.Height);
			}
			return new SizeF(bounding.Width, (float)((double)bounding.Width / num));
		}
		public static SizeF DownScaleInside(SizeF inner, SizeF bounding)
		{
			SizeF result = PolygonMath.ScaleInside(inner, bounding);
			if (result.Width > inner.Width || result.Height > inner.Height)
			{
				return inner;
			}
			return result;
		}
		public static bool FitsInside(SizeF inner, SizeF outer)
		{
			return inner.Width <= outer.Width && inner.Height <= outer.Height;
		}
		public static PointF[,] GetCorners(PointF[] poly, float width)
		{
			float[] array = new float[poly.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = width;
			}
			return PolygonMath.GetCorners(poly, array);
		}
		public static PointF[,] GetCorners(PointF[] poly, float[] widths)
		{
			if (poly.Length != widths.Length)
			{
				throw new ArgumentException("Arrays 'poly' and 'widths' must have the same number of elements");
			}
			PointF[,] array = new PointF[poly.Length, 4];
			int num = poly.Length - 1;
			for (int i = 0; i < poly.Length; i++)
			{
				PointF pointF = (i < num) ? poly[i + 1] : poly[i - num];
				PointF pointF2 = (i > 0) ? poly[i - 1] : poly[i + num];
				PointF pointF3 = poly[i];
				float length = (i > 0) ? widths[i - 1] : widths[i + num];
				float length2 = widths[i];
				PointF pointF4 = PolygonMath.ChangeMagnitude(PolygonMath.RotateVector(new PointF(pointF2.X - pointF3.X, pointF2.Y - pointF3.Y), 1.5707963267948966), length);
				PointF pointF5 = PolygonMath.ChangeMagnitude(PolygonMath.RotateVector(new PointF(pointF.X - pointF3.X, pointF.Y - pointF3.Y), -1.5707963267948966), length2);
				array[i, 0] = pointF3;
				array[i, 1] = new PointF(pointF3.X + pointF4.X, pointF3.Y + pointF4.Y);
				array[i, 2] = new PointF(pointF3.X + pointF4.X + pointF5.X, pointF3.Y + pointF4.Y + pointF5.Y);
				array[i, 3] = new PointF(pointF3.X + pointF5.X, pointF3.Y + pointF5.Y);
			}
			return array;
		}
		public static PointF[,] GetSides(PointF[] poly, float width)
		{
			PointF[,] corners = PolygonMath.GetCorners(poly, width);
			PointF[,] array = new PointF[corners.GetUpperBound(0) + 1, 4];
			for (int i = 0; i <= corners.GetUpperBound(0); i++)
			{
				int num = (i < corners.GetUpperBound(0)) ? (i + 1) : (i - corners.GetUpperBound(0));
				array[i, 0] = corners[i, 3];
				array[i, 3] = corners[i, 0];
				array[i, 1] = corners[num, 1];
				array[i, 2] = corners[num, 0];
			}
			return array;
		}
		public static PointF[] InflatePoly(PointF[] poly, float offset)
		{
			PointF[,] corners = PolygonMath.GetCorners(poly, offset);
			PointF[] array = new PointF[poly.Length];
			for (int i = 0; i <= corners.GetUpperBound(0); i++)
			{
				array[i] = corners[i, 2];
			}
			return array;
		}
		public static PointF[] InflatePoly(PointF[] poly, float[] offsets)
		{
			PointF[,] corners = PolygonMath.GetCorners(poly, offsets);
			PointF[] array = new PointF[poly.Length];
			for (int i = 0; i <= corners.GetUpperBound(0); i++)
			{
				array[i] = corners[i, 2];
			}
			return array;
		}
		public static PointF[] CenterInside(PointF[] inner, PointF[] outer)
		{
			RectangleF boundingBox = PolygonMath.GetBoundingBox(inner);
			RectangleF boundingBox2 = PolygonMath.GetBoundingBox(outer);
			return PolygonMath.MovePoly(PolygonMath.NormalizePoly(inner), new PointF((boundingBox2.Width - boundingBox.Width) / 2f + boundingBox2.X, (boundingBox2.Height - boundingBox.Height) / 2f + boundingBox2.Y));
		}
		public static Rectangle ToRectangle(RectangleF r)
		{
			return new Rectangle((int)Math.Round((double)r.X), (int)Math.Round((double)r.Y), (int)Math.Round((double)r.Width), (int)Math.Round((double)r.Height));
		}
		public static Size RoundPoints(SizeF sizeF)
		{
			return new Size((int)Math.Round((double)sizeF.Width), (int)Math.Round((double)sizeF.Height));
		}
	}
}
