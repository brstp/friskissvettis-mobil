using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Web;
using ThoughtWorks.QRCode.Codec;
namespace MoMA.Mobile.Barcode
{
	public class QR : IBarcode
	{
		public string Code
		{
			get;
			set;
		}
		public void Initialize(string code)
		{
			this.Code = code;
		}
		public void DrawToOutput()
		{
			this.DrawToOutput(true);
		}
		public void DrawToOutput(bool showCodeAsText)
		{
			this.GetImage(showCodeAsText).Save(HttpContext.Current.Response.OutputStream, ImageFormat.Png);
		}
		public Image GetImage()
		{
			return this.GetImage(true);
		}
		public Image GetImage(bool showCodeAsText)
		{
			Bitmap bitmap = new QRCodeEncoder
			{
				QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
				QRCodeScale = 8,
				QRCodeVersion = 3,
				QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M
			}.Encode(this.Code);
			if (showCodeAsText)
			{
				Bitmap bitmap2 = this.ExpandCanvas(bitmap, 0, 25);
				Graphics graphics = Graphics.FromImage(bitmap2);
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				SolidBrush brush = new SolidBrush(Color.Black);
				Font font = new Font("Verdana", 12f, FontStyle.Bold);
				SizeF sizeF = graphics.MeasureString(this.Code, font);
				int num = Convert.ToInt32((graphics.VisibleClipBounds.Width - sizeF.Width) / 2f);
				graphics.DrawString(this.Code, font, brush, new PointF((float)num, (float)(bitmap.Height + 5)));
				graphics.Dispose();
				bitmap.Dispose();
				return bitmap2;
			}
			return bitmap;
		}
		private Bitmap ExpandCanvas(Bitmap bitmap, int AddWidth, int AddHeight)
		{
			Bitmap bitmap2 = new Bitmap(bitmap.Width + AddWidth, bitmap.Height + AddHeight);
			using (Graphics graphics = Graphics.FromImage(bitmap2))
			{
				graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bitmap2.Width, bitmap2.Height));
				graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
			}
			return bitmap2;
		}
	}
}
