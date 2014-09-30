using System;
namespace Yahoo.Yui.Compressor
{
	public static class CssCompressor
	{
		public static string Compress(string css)
		{
			return CssCompressor.Compress(css, 0, CssCompressionType.StockYuiCompressor, true);
		}
		public static string Compress(string css, int columnWidth, CssCompressionType cssCompressionType, bool removeComments)
		{
			string result;
			switch (cssCompressionType)
			{
			case CssCompressionType.StockYuiCompressor:
				result = YUICompressor.Compress(css, columnWidth, removeComments);
				break;
			case CssCompressionType.MichaelAshRegexEnhancements:
				result = MichaelAshRegexCompressor.Compress(css, columnWidth, removeComments);
				break;
			case CssCompressionType.Hybrid:
			{
				string text = YUICompressor.Compress(css, columnWidth, removeComments);
				string text2 = MichaelAshRegexCompressor.Compress(css, columnWidth, removeComments);
				result = ((text.Length < text2.Length) ? text : text2);
				break;
			}
			default:
				throw new InvalidOperationException("Unhandled CssCompressionType found when trying to determine which compression method to use.");
			}
			return result;
		}
	}
}
