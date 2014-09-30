using ImageResizer.Configuration;
using ImageResizer.Encoding;
using ImageResizer.Resizing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class DefaultEncoder : IEncoder, IQuerystringPlugin, IPlugin
	{
		private ImageFormat _outputFormat = ImageFormat.Jpeg;
		private int quality = 90;
		private static object _syncExts = new object();
		private static IDictionary<string, ImageFormat> _imageExtensions = null;
		public ImageFormat OutputFormat
		{
			get
			{
				return this._outputFormat;
			}
			set
			{
				if (!this.IsValidOutputFormat(value))
				{
					throw new ArgumentException(value.ToString() + " is not a valid OutputFormat for DefaultEncoder.");
				}
				this._outputFormat = value;
			}
		}
		public int Quality
		{
			get
			{
				return this.quality;
			}
			set
			{
				this.quality = value;
			}
		}
		public bool SupportsTransparency
		{
			get
			{
				return this.OutputFormat == ImageFormat.Gif || this.OutputFormat == ImageFormat.Png;
			}
		}
		public string MimeType
		{
			get
			{
				return DefaultEncoder.GetContentTypeFromImageFormat(this.OutputFormat);
			}
		}
		public string Extension
		{
			get
			{
				return DefaultEncoder.GetExtensionFromImageFormat(this.OutputFormat);
			}
		}
		private static IDictionary<string, ImageFormat> imageExtensions
		{
			get
			{
				object syncExts;
				Monitor.Enter(syncExts = DefaultEncoder._syncExts);
				IDictionary<string, ImageFormat> imageExtensions;
				try
				{
					if (DefaultEncoder._imageExtensions == null)
					{
						DefaultEncoder._imageExtensions = new Dictionary<string, ImageFormat>();
						DefaultEncoder.addImageExtension("jpg", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("jpeg", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("jpe", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("jif", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("jfif", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("jfi", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("exif", ImageFormat.Jpeg);
						DefaultEncoder.addImageExtension("bmp", ImageFormat.Bmp);
						DefaultEncoder.addImageExtension("gif", ImageFormat.Gif);
						DefaultEncoder.addImageExtension("png", ImageFormat.Png);
						DefaultEncoder.addImageExtension("tif", ImageFormat.Tiff);
						DefaultEncoder.addImageExtension("tiff", ImageFormat.Tiff);
						DefaultEncoder.addImageExtension("tff", ImageFormat.Tiff);
					}
					imageExtensions = DefaultEncoder._imageExtensions;
				}
				finally
				{
					Monitor.Exit(syncExts);
				}
				return imageExtensions;
			}
		}
		public DefaultEncoder()
		{
		}
		public DefaultEncoder(ImageFormat outputFormat)
		{
			this.OutputFormat = outputFormat;
		}
		public DefaultEncoder(ImageFormat outputFormat, int jpegQuality)
		{
			this.OutputFormat = outputFormat;
			this.Quality = jpegQuality;
		}
		public DefaultEncoder(ResizeSettings settings, object original)
		{
			ImageFormat imageFormat = DefaultEncoder.GetOriginalFormat(original);
			if (!this.IsValidOutputFormat(imageFormat))
			{
				imageFormat = ImageFormat.Jpeg;
			}
			ImageFormat requestedFormat = DefaultEncoder.GetRequestedFormat(settings.Format, imageFormat);
			if (!this.IsValidOutputFormat(requestedFormat))
			{
				throw new ArgumentException("An unrecognized or unsupported output format (" + ((settings.Format != null) ? settings.Format : "(null)") + ") was specified in 'settings'.");
			}
			this.OutputFormat = requestedFormat;
			int num = 90;
			if (!string.IsNullOrEmpty(settings["quality"]) && int.TryParse(settings["quality"], out num))
			{
				this.Quality = num;
			}
		}
		public virtual IEncoder CreateIfSuitable(ResizeSettings settings, object original)
		{
			ImageFormat requestedFormat = DefaultEncoder.GetRequestedFormat(settings.Format, ImageFormat.Jpeg);
			if (requestedFormat == null || !this.IsValidOutputFormat(requestedFormat))
			{
				return null;
			}
			return new DefaultEncoder(settings, original);
		}
		public bool IsValidOutputFormat(ImageFormat f)
		{
			return f == ImageFormat.Gif || f == ImageFormat.Png || f == ImageFormat.Jpeg;
		}
		public void Write(Image image, Stream s)
		{
			if (this.OutputFormat == ImageFormat.Jpeg)
			{
				DefaultEncoder.SaveJpeg(image, s, this.Quality);
				return;
			}
			if (this.OutputFormat == ImageFormat.Png)
			{
				DefaultEncoder.SavePng(image, s);
				return;
			}
			if (this.OutputFormat == ImageFormat.Gif)
			{
				DefaultEncoder.SaveGif(image, s);
			}
		}
		public static ImageFormat GetRequestedFormat(string format, ImageFormat defaultValue)
		{
			if (!string.IsNullOrEmpty(format))
			{
				return DefaultEncoder.GetImageFormatFromExtension(format);
			}
			return defaultValue;
		}
		public static ImageFormat GetOriginalFormat(object original)
		{
			if (original == null)
			{
				return null;
			}
			string text = original as string;
			if (text == null && original is Image)
			{
				text = (((Image)original).Tag as string);
			}
			if (text == null && original is Image && ((Image)original).Tag is BitmapTag)
			{
				text = ((BitmapTag)((Image)original).Tag).Path;
			}
			if (text != null)
			{
				ImageFormat imageFormatFromPhysicalPath = DefaultEncoder.GetImageFormatFromPhysicalPath(text);
				if (imageFormatFromPhysicalPath != null)
				{
					return imageFormatFromPhysicalPath;
				}
			}
			if (original is Image)
			{
				return ((Image)original).RawFormat;
			}
			return null;
		}
		public static ImageFormat GetImageFormatFromPhysicalPath(string path)
		{
			return DefaultEncoder.GetImageFormatFromExtension(Path.GetExtension(path));
		}
		public static string GetExtensionFromImageFormat(ImageFormat format)
		{
			object syncExts;
			Monitor.Enter(syncExts = DefaultEncoder._syncExts);
			try
			{
				foreach (KeyValuePair<string, ImageFormat> current in DefaultEncoder.imageExtensions)
				{
					if (current.Value.Guid.Equals(format.Guid))
					{
						return current.Key;
					}
				}
			}
			finally
			{
				Monitor.Exit(syncExts);
			}
			return null;
		}
		public static ImageFormat GetImageFormatFromExtension(string ext)
		{
			if (string.IsNullOrEmpty(ext))
			{
				return null;
			}
			object syncExts;
			Monitor.Enter(syncExts = DefaultEncoder._syncExts);
			ImageFormat result;
			try
			{
				ext = ext.Trim(new char[]
				{
					' ',
					'.'
				}).ToLowerInvariant();
				if (!DefaultEncoder.imageExtensions.ContainsKey(ext))
				{
					result = null;
				}
				else
				{
					result = DefaultEncoder.imageExtensions[ext];
				}
			}
			finally
			{
				Monitor.Exit(syncExts);
			}
			return result;
		}
		private static void addImageExtension(string extension, ImageFormat matchingFormat)
		{
			DefaultEncoder.imageExtensions.Add(extension.TrimStart(new char[]
			{
				'.',
				' '
			}).ToLowerInvariant(), matchingFormat);
		}
		public static void AddImageExtension(string extension, ImageFormat matchingFormat)
		{
			object syncExts;
			Monitor.Enter(syncExts = DefaultEncoder._syncExts);
			try
			{
				DefaultEncoder.imageExtensions.Add(extension.TrimStart(new char[]
				{
					'.',
					' '
				}).ToLowerInvariant(), matchingFormat);
			}
			finally
			{
				Monitor.Exit(syncExts);
			}
		}
		public static string GetContentTypeFromImageFormat(ImageFormat format)
		{
			if (format == ImageFormat.Png)
			{
				return "image/png";
			}
			if (format == ImageFormat.Jpeg)
			{
				return "image/jpeg";
			}
			if (format == ImageFormat.Gif)
			{
				return "image/gif";
			}
			if (format == ImageFormat.Bmp)
			{
				return "image/x-ms-bmp";
			}
			if (format == ImageFormat.Tiff)
			{
				return "image/tiff";
			}
			throw new ArgumentOutOfRangeException("Unsupported format " + format.ToString());
		}
		public static ImageCodecInfo GetImageCodeInfo(string mimeType)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			ImageCodecInfo[] array = imageEncoders;
			for (int i = 0; i < array.Length; i++)
			{
				ImageCodecInfo imageCodecInfo = array[i];
				if (imageCodecInfo.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
				{
					return imageCodecInfo;
				}
			}
			return null;
		}
		public static void SaveJpeg(Image b, Stream target, int quality)
		{
			if (quality < 0)
			{
				quality = 90;
			}
			if (quality > 100)
			{
				quality = 100;
			}
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
			b.Save(target, DefaultEncoder.GetImageCodeInfo("image/jpeg"), encoderParameters);
		}
		public static void SavePng(Image img, Stream target)
		{
			if (!target.CanSeek)
			{
				using (MemoryStream memoryStream = new MemoryStream(4096))
				{
					img.Save(memoryStream, ImageFormat.Png);
					memoryStream.WriteTo(target);
					return;
				}
			}
			img.Save(target, ImageFormat.Png);
		}
		public static void SaveBmp(Image img, Stream target)
		{
			img.Save(target, ImageFormat.Bmp);
		}
		public static void SaveGif(Image img, Stream target)
		{
			img.Save(target, ImageFormat.Gif);
		}
		public virtual IEnumerable<string> GetSupportedQuerystringKeys()
		{
			return new string[]
			{
				"quality",
				"format",
				"thumbnail"
			};
		}
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
	}
}
