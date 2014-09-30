using ImageResizer.Resizing;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.InteropServices;
namespace ImageResizer
{
	[ComVisible(true)]
	[Serializable]
	public class ResizeSettings : NameValueCollection
	{
		public int Width
		{
			get
			{
				return this.get("width", -1);
			}
			set
			{
				this.set("width", value);
			}
		}
		public int Height
		{
			get
			{
				return this.get("height", -1);
			}
			set
			{
				this.set("height", value);
			}
		}
		public int MaxWidth
		{
			get
			{
				return this.get("maxwidth", -1);
			}
			set
			{
				this.set("maxwidth", value);
			}
		}
		public int MaxHeight
		{
			get
			{
				return this.get("maxheight", -1);
			}
			set
			{
				this.set("maxheight", value);
			}
		}
		public double Rotate
		{
			get
			{
				return this.get("rotate", 0.0);
			}
			set
			{
				this.set("rotate", value);
			}
		}
		public RotateFlipType Flip
		{
			get
			{
				return Utils.parseFlip(base["flip"]);
			}
			set
			{
				base["flip"] = Utils.writeFlip(value);
			}
		}
		public RotateFlipType SourceFlip
		{
			get
			{
				return Utils.parseFlip(base["sourceFlip"]);
			}
			set
			{
				base["sourceFlip"] = Utils.writeFlip(value);
			}
		}
		public ScaleMode Scale
		{
			get
			{
				return Utils.parseScale(base["scale"]);
			}
			set
			{
				base["scale"] = Utils.writeScale(value);
			}
		}
		public StretchMode Stretch
		{
			get
			{
				return Utils.parseStretch(base["stretch"]);
			}
			set
			{
				base["stretch"] = Utils.writeStretch(value);
			}
		}
		public ServerCacheMode Cache
		{
			get
			{
				return Utils.parseEnum<ServerCacheMode>(base["cache"], ServerCacheMode.Default);
			}
			set
			{
				base["cache"] = value.ToString();
			}
		}
		public ProcessWhen Process
		{
			get
			{
				return Utils.parseEnum<ProcessWhen>(base["process"], (base["useresizingpipeline"] != null) ? ProcessWhen.Always : ProcessWhen.Default);
			}
			set
			{
				base["process"] = value.ToString();
				this.Remove("useresizingpipeline");
			}
		}
		public CropMode CropMode
		{
			get
			{
				return Utils.parseCrop(base["crop"]).Key;
			}
			set
			{
				base["crop"] = Utils.writeCrop(value, this.CropValues);
			}
		}
		protected double[] CropValues
		{
			get
			{
				double[] value = Utils.parseCrop(base["crop"]).Value;
				if (value == null)
				{
					return new double[4];
				}
				return value;
			}
			set
			{
				if (value != null && value.Length == 4)
				{
					base["crop"] = Utils.writeCrop(CropMode.Custom, value);
					return;
				}
				throw new ArgumentException("CropValues must be an array of 4 double values.");
			}
		}
		public PointF CropTopLeft
		{
			get
			{
				return new PointF((float)this.CropValues[0], (float)this.CropValues[1]);
			}
			set
			{
				this.CropValues = new double[]
				{
					(double)value.X,
					(double)value.Y,
					this.CropValues[2],
					this.CropValues[3]
				};
			}
		}
		public PointF CropBottomRight
		{
			get
			{
				return new PointF((float)this.CropValues[2], (float)this.CropValues[3]);
			}
			set
			{
				this.CropValues = new double[]
				{
					this.CropValues[0],
					this.CropValues[1],
					(double)value.X,
					(double)value.Y
				};
			}
		}
		public Color BackgroundColor
		{
			get
			{
				return Utils.parseColor(base["bgcolor"], Color.Transparent);
			}
			set
			{
				base["bgcolor"] = Utils.writeColor(value);
			}
		}
		public Color PaddingColor
		{
			get
			{
				return Utils.parseColor(base["paddingColor"], Color.Transparent);
			}
			set
			{
				base["paddingColor"] = Utils.writeColor(value);
			}
		}
		public BoxPadding Padding
		{
			get
			{
				return Utils.parsePadding(base["paddingWidth"]);
			}
			set
			{
				base["paddingWidth"] = Utils.writePadding(value);
			}
		}
		public BoxPadding Margin
		{
			get
			{
				return Utils.parsePadding(base["margin"]);
			}
			set
			{
				base["margin"] = Utils.writePadding(value);
			}
		}
		public Color BorderColor
		{
			get
			{
				return Utils.parseColor(base["borderColor"], Color.Transparent);
			}
			set
			{
				base["borderColor"] = Utils.writeColor(value);
			}
		}
		public BoxPadding Border
		{
			get
			{
				return Utils.parsePadding(base["borderWidth"]);
			}
			set
			{
				base["borderWidth"] = Utils.writePadding(value);
			}
		}
		public string Format
		{
			get
			{
				if (!string.IsNullOrEmpty(base["format"]))
				{
					return base["format"];
				}
				return base["thumbnail"];
			}
			set
			{
				base["format"] = value;
				base["thumbnail"] = null;
			}
		}
		public ResizeSettings()
		{
		}
		public ResizeSettings(NameValueCollection col) : base(col)
		{
		}
		public ResizeSettings(string queryString) : base(PathUtils.ParseQueryStringFriendly(queryString))
		{
		}
		protected int get(string name, int defaultValue)
		{
			return Utils.getInt(this, name, defaultValue);
		}
		protected void set(string name, int value)
		{
			base[name] = value.ToString();
		}
		protected double get(string name, double defaultValue)
		{
			return Utils.getDouble(this, name, defaultValue);
		}
		protected void set(string name, double value)
		{
			base[name] = value.ToString();
		}
		public bool WasOneSpecified(params string[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				string name = keys[i];
				if (!string.IsNullOrEmpty(base[name]))
				{
					return true;
				}
			}
			return false;
		}
		public RectangleF getCustomCropSourceRect(SizeF imageSize)
		{
			RectangleF result = new RectangleF(new PointF(0f, 0f), imageSize);
			double[] array = this.CropValues;
			KeyValuePair<CropUnits, double> keyValuePair = Utils.parseCropUnits(base["cropxunits"]);
			KeyValuePair<CropUnits, double> keyValuePair2 = Utils.parseCropUnits(base["cropyunits"]);
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = i % 2 == 0;
				if (flag && keyValuePair.Key == CropUnits.Custom)
				{
					array[i] *= (double)imageSize.Width / keyValuePair.Value;
				}
				if (!flag && keyValuePair2.Key == CropUnits.Custom)
				{
					array[i] *= (double)imageSize.Height / keyValuePair2.Value;
				}
				if (flag && array[i] > (double)imageSize.Width)
				{
					array[i] = (double)imageSize.Width;
				}
				if (!flag && array[i] > (double)imageSize.Height)
				{
					array[i] = (double)imageSize.Height;
				}
			}
			if (array.Length == 2)
			{
				if (array[0] < 1.0 || array[1] < 1.0)
				{
					return result;
				}
				double num = ((double)imageSize.Width - array[0]) / 2.0;
				double num2 = ((double)imageSize.Height - array[1]) / 2.0;
				array = new double[]
				{
					num,
					num2,
					num + array[0],
					num2 + array[1]
				};
			}
			double num3 = array[0];
			double num4 = array[1];
			double num5 = array[2];
			double num6 = array[3];
			if (num3 < 0.0)
			{
				num3 += (double)imageSize.Width;
			}
			if (num4 < 0.0)
			{
				num4 += (double)imageSize.Height;
			}
			if (num5 <= 0.0)
			{
				num5 += (double)imageSize.Width;
			}
			if (num6 <= 0.0)
			{
				num6 += (double)imageSize.Height;
			}
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			if (num5 < 0.0)
			{
				num5 = 0.0;
			}
			if (num4 < 0.0)
			{
				num4 = 0.0;
			}
			if (num6 < 0.0)
			{
				num6 = 0.0;
			}
			if (num3 > (double)imageSize.Width)
			{
				num3 = (double)imageSize.Width;
			}
			if (num5 > (double)imageSize.Width)
			{
				num5 = (double)imageSize.Width;
			}
			if (num4 > (double)imageSize.Height)
			{
				num4 = (double)imageSize.Height;
			}
			if (num6 > (double)imageSize.Height)
			{
				num6 = (double)imageSize.Height;
			}
			if (num5 <= num3 || num6 <= num4)
			{
				return new RectangleF(new PointF(0f, 0f), imageSize);
			}
			return new RectangleF((float)num3, (float)num4, (float)(num5 - num3), (float)(num6 - num4));
		}
		public void SetDefaultImageFormat(string format)
		{
			if (string.IsNullOrEmpty(base["thumbnail"]) && string.IsNullOrEmpty(base["format"]))
			{
				base["format"] = format;
			}
		}
		public override string ToString()
		{
			return PathUtils.BuildQueryString(this, false);
		}
		public string ToStringEncoded()
		{
			return PathUtils.BuildQueryString(this);
		}
	}
}
