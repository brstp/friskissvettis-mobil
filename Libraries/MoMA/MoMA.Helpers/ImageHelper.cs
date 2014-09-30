using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace MoMA.Helpers
{
	public class ImageHelper
	{
		public static Bitmap CropBitmap(Bitmap bitmap, Rectangle rect)
		{
			return bitmap.Clone(rect, bitmap.PixelFormat);
		}
		public static Image ResizeImage(Image FullsizeImage, int NewWidth)
		{
			return ImageHelper.ResizeImage(FullsizeImage, NewWidth, 10000, false);
		}
		public static Image ResizeImage(Image FullsizeImage, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
		{
			FullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			FullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			if (OnlyResizeIfWider && FullsizeImage.Width <= NewWidth)
			{
				NewWidth = FullsizeImage.Width;
			}
			int num = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
			if (num > MaxHeight)
			{
				NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
				num = MaxHeight;
			}
			Image thumbnailImage = FullsizeImage.GetThumbnailImage(NewWidth, num, null, IntPtr.Zero);
			FullsizeImage.Dispose();
			return thumbnailImage;
		}
		public unsafe static Rectangle GetImageNonWhiteSpaceBounds(Bitmap bitmap, Color color)
		{
			Rectangle result = default(Rectangle);
			BitmapData bitmapData = null;
			try
			{
				int width = bitmap.Width;
				int height = bitmap.Height;
				bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
				int num = 3;
				int num2 = width;
				int num3 = 0;
				int num4 = height;
				int num5 = 0;
				for (int i = 0; i < height; i++)
				{
					byte* ptr = (byte*)((void*)bitmapData.Scan0 + (IntPtr)i * (IntPtr)bitmapData.Stride / sizeof(void));
					for (int j = 0; j < width; j++)
					{
						int num6 = j * num;
						byte b = ptr[(IntPtr)num6 / 1];
						byte b2 = ptr[(IntPtr)(num6 + 1) / 1];
						byte b3 = ptr[(IntPtr)(num6 + 2) / 1];
						if (!ImageHelper.Approx(b3, color.R) || !ImageHelper.Approx(b2, color.G) || !ImageHelper.Approx(b, color.B))
						{
							num2 = Math.Min(j, num2);
							num3 = Math.Max(j, num3);
							num4 = Math.Min(i, num4);
							num5 = Math.Max(i, num5);
						}
					}
				}
				result.X = num2;
				result.Y = num4;
				result.Width = num3 - num2;
				result.Height = num5 - num4;
			}
			finally
			{
				if (bitmapData != null)
				{
					bitmap.UnlockBits(bitmapData);
				}
			}
			result.Width += 10;
			return result;
		}
		private static bool Approx(byte b, byte compareTo)
		{
			return b > compareTo - 5 && b < compareTo + 5;
		}
	}
}
