using System;
using System.Drawing;
namespace MoMA.Mobile.Barcode
{
	public interface IBarcode
	{
		string Code
		{
			get;
			set;
		}
		void Initialize(string code);
		void DrawToOutput();
		void DrawToOutput(bool showCodeAsText);
		Image GetImage();
		Image GetImage(bool showCodeAsText);
	}
}
