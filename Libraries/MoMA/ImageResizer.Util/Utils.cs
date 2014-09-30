using ImageResizer.Resizing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
namespace ImageResizer.Util
{
	[ComVisible(true)]
	public class Utils
	{
		public static Color parseColor(string value, Color defaultValue)
		{
			if (!string.IsNullOrEmpty(value))
			{
				value = value.TrimStart(new char[]
				{
					'#'
				});
				int num;
				if (int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out num))
				{
					int num2 = 255;
					if (value.Length == 4 || value.Length == 8)
					{
						int num3 = value.Length - value.Length / 4;
						num2 = int.Parse(value.Substring(num3), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture);
						if (num3 == 3)
						{
							num2 *= 16;
						}
						value = value.Substring(0, num3);
					}
					return Color.FromArgb(num2, ColorTranslator.FromHtml("#" + value));
				}
				try
				{
					Color color = ColorTranslator.FromHtml(value);
					Color result = color.IsEmpty ? defaultValue : color;
					return result;
				}
				catch
				{
					Color result = defaultValue;
					return result;
				}
				return defaultValue;
			}
			return defaultValue;
		}
		public static string writeColor(Color value)
		{
			string text = ColorTranslator.ToHtml(value);
			if (text.StartsWith("#"))
			{
				text = text.TrimStart(new char[]
				{
					'#'
				});
				if (value.A != 255)
				{
					text += value.A.ToString("X2", null);
				}
			}
			return text;
		}
		public static double[] parseList(string text, double defaultValue)
		{
			text = text.Trim(new char[]
			{
				' ',
				'(',
				')'
			});
			string[] array = text.Split(new char[]
			{
				','
			}, StringSplitOptions.None);
			double[] array2 = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!double.TryParse(array[i], out array2[i]))
				{
					array2[i] = defaultValue;
				}
			}
			return array2;
		}
		public static int getInt(NameValueCollection q, string name, int defaultValue)
		{
			int result = defaultValue;
			if (!string.IsNullOrEmpty(q[name]) && !int.TryParse(q[name], out result))
			{
				return defaultValue;
			}
			return result;
		}
		public static float getFloat(NameValueCollection q, string name, float defaultValue)
		{
			float result = defaultValue;
			if (!string.IsNullOrEmpty(q[name]) && !float.TryParse(q[name], out result))
			{
				return defaultValue;
			}
			return result;
		}
		public static double getDouble(NameValueCollection q, string name, double defaultValue)
		{
			double result = defaultValue;
			if (!string.IsNullOrEmpty(q[name]) && !double.TryParse(q[name], out result))
			{
				return defaultValue;
			}
			return result;
		}
		public static bool getBool(NameValueCollection q, string name, bool defaultValue)
		{
			if (!string.IsNullOrEmpty(q[name]))
			{
				string value = q[name];
				if ("true".Equals(value, StringComparison.OrdinalIgnoreCase) || "1".Equals(value, StringComparison.OrdinalIgnoreCase) || "yes".Equals(value, StringComparison.OrdinalIgnoreCase) || "on".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("false".Equals(value, StringComparison.OrdinalIgnoreCase) || "0".Equals(value, StringComparison.OrdinalIgnoreCase) || "no".Equals(value, StringComparison.OrdinalIgnoreCase) || "off".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return defaultValue;
		}
		public static T parseEnum<T>(string value, T defaultValue) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			if (value == null)
			{
				return defaultValue;
			}
			value = value.Trim();
			T result;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), value, true));
			}
			catch (ArgumentException)
			{
				result = defaultValue;
			}
			return result;
		}
		public static void copyStream(Stream source, Stream dest)
		{
			byte[] array = new byte[32768];
			while (true)
			{
				int num = source.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				dest.Write(array, 0, num);
			}
		}
		public static RotateFlipType parseFlip(string sFlip)
		{
			if (!string.IsNullOrEmpty(sFlip))
			{
				if ("none".Equals(sFlip, StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.RotateNoneFlipNone;
				}
				if (sFlip.Equals("h", StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.RotateNoneFlipX;
				}
				if (sFlip.Equals("x", StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.RotateNoneFlipX;
				}
				if (sFlip.Equals("v", StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.Rotate180FlipX;
				}
				if (sFlip.Equals("y", StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.Rotate180FlipX;
				}
				if (sFlip.Equals("both", StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.Rotate180FlipNone;
				}
				if (sFlip.Equals("xy", StringComparison.OrdinalIgnoreCase))
				{
					return RotateFlipType.Rotate180FlipNone;
				}
			}
			return RotateFlipType.RotateNoneFlipNone;
		}
		public static string writeFlip(RotateFlipType flip)
		{
			if (flip == RotateFlipType.RotateNoneFlipNone)
			{
				return "none";
			}
			if (flip == RotateFlipType.RotateNoneFlipX)
			{
				return "x";
			}
			if (flip == RotateFlipType.Rotate180FlipX)
			{
				return "y";
			}
			if (flip == RotateFlipType.Rotate180FlipNone)
			{
				return "xy";
			}
			throw new ArgumentException("Valid flip values are RotateNoneFlipNone, RotateNoneFlipX, RotateNoneFlipY, and RotateNoneFlipXY. Rotation must be specified with Rotate instead. Received: " + flip.ToString());
		}
		public static StretchMode parseStretch(string value)
		{
			if ("fill".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				return StretchMode.Fill;
			}
			return StretchMode.Proportionally;
		}
		public static string writeStretch(StretchMode value)
		{
			if (value == StretchMode.Proportionally)
			{
				return "proportionally";
			}
			if (value == StretchMode.Fill)
			{
				return "fill";
			}
			throw new NotImplementedException("Unrecognized ScaleMode value: " + value.ToString());
		}
		public static KeyValuePair<CropUnits, double> parseCropUnits(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new KeyValuePair<CropUnits, double>(CropUnits.SourcePixels, 0.0);
			}
			double num;
			if (double.TryParse(value, out num) && num > 0.0)
			{
				return new KeyValuePair<CropUnits, double>(CropUnits.Custom, num);
			}
			return new KeyValuePair<CropUnits, double>(CropUnits.SourcePixels, 0.0);
		}
		public static string writeCropUnits(KeyValuePair<CropUnits, double> value)
		{
			if (value.Key == CropUnits.Custom)
			{
				return value.Value.ToString();
			}
			if (value.Key == CropUnits.SourcePixels)
			{
				return "sourcepixels";
			}
			throw new NotImplementedException("Unrecognized CropUnits value: " + value.ToString());
		}
		public static ScaleMode parseScale(string value)
		{
			if (value != null)
			{
				if (value.Equals("both", StringComparison.OrdinalIgnoreCase))
				{
					return ScaleMode.Both;
				}
				if (value.Equals("upscaleonly", StringComparison.OrdinalIgnoreCase))
				{
					return ScaleMode.UpscaleOnly;
				}
				if (value.Equals("downscaleonly", StringComparison.OrdinalIgnoreCase))
				{
					return ScaleMode.DownscaleOnly;
				}
				if (value.Equals("upscalecanvas", StringComparison.OrdinalIgnoreCase))
				{
					return ScaleMode.UpscaleCanvas;
				}
			}
			return ScaleMode.DownscaleOnly;
		}
		public static string writeScale(ScaleMode value)
		{
			if (value == ScaleMode.Both)
			{
				return "both";
			}
			if (value == ScaleMode.DownscaleOnly)
			{
				return "downscaleonly";
			}
			if (value == ScaleMode.UpscaleCanvas)
			{
				return "upscalecanvas";
			}
			if (value == ScaleMode.UpscaleOnly)
			{
				return "upscaleonly";
			}
			throw new NotImplementedException("Unrecognized ScaleMode value: " + value.ToString());
		}
		public static KeyValuePair<CropMode, double[]> parseCrop(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new KeyValuePair<CropMode, double[]>(CropMode.None, null);
			}
			if ("auto".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				return new KeyValuePair<CropMode, double[]>(CropMode.Auto, null);
			}
			double[] array = Utils.parseList(value, double.NaN);
			if (array.Length == 4)
			{
				return new KeyValuePair<CropMode, double[]>(CropMode.Custom, array);
			}
			return new KeyValuePair<CropMode, double[]>(CropMode.None, null);
		}
		public static string writeCrop(CropMode mode, double[] coords)
		{
			if (mode == CropMode.Auto)
			{
				return "auto";
			}
			if (mode == CropMode.None)
			{
				return "none";
			}
			if (mode == CropMode.Custom)
			{
				string text = "(";
				for (int i = 0; i < coords.Length; i++)
				{
					text = text + coords[i].ToString() + ",";
				}
				return text.TrimEnd(new char[]
				{
					','
				}) + ")";
			}
			throw new NotImplementedException("Unrecognized CropMode value: " + mode.ToString());
		}
		public static BoxPadding parsePadding(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return BoxPadding.Empty;
			}
			double[] array = Utils.parseList(value, 0.0);
			if (array.Length == 1)
			{
				return new BoxPadding(array[0]);
			}
			if (array.Length == 4)
			{
				return new BoxPadding(array[0], array[1], array[2], array[3]);
			}
			return BoxPadding.Empty;
		}
		public static PointF parsePointF(string value, PointF defaultValue)
		{
			if (string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			double[] array = Utils.parseList(value, 0.0);
			if (array.Length == 2)
			{
				return new PointF((float)array[0], (float)array[1]);
			}
			return defaultValue;
		}
		public static string writePadding(BoxPadding p)
		{
			if (p.All != -1.0)
			{
				return p.All.ToString();
			}
			return string.Concat(new object[]
			{
				"(",
				p.Left,
				",",
				p.Top,
				",",
				p.Right,
				",",
				p.Bottom,
				")"
			});
		}
		public static void DrawOuterGradient(Graphics g, PointF[] poly, Color inner, Color outer, float width)
		{
			PointF[,] array = PolygonMath.RoundPoints(PolygonMath.GetCorners(poly, width));
			PointF[,] array2 = PolygonMath.RoundPoints(PolygonMath.GetSides(poly, width));
			for (int i = 0; i <= array.GetUpperBound(0); i++)
			{
				PointF[] subArray = PolygonMath.GetSubArray(array, i);
				Brush brush = PolygonMath.GenerateRadialBrush(inner, outer, subArray[0], width + 1f);
				g.FillPolygon(brush, subArray);
			}
			for (int j = 0; j <= array2.GetUpperBound(0); j++)
			{
				PointF[] subArray2 = PolygonMath.GetSubArray(array2, j);
				LinearGradientBrush linearGradientBrush = new LinearGradientBrush(subArray2[3], subArray2[0], inner, outer);
				linearGradientBrush.SetSigmaBellShape(1f);
				linearGradientBrush.WrapMode = WrapMode.TileFlipXY;
				g.FillPolygon(linearGradientBrush, subArray2);
			}
		}
	}
}
