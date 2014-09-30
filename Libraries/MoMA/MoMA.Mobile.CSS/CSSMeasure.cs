using MoMA.Mobile.Device;
using NCalc;
using System;
using System.Text.RegularExpressions;
namespace MoMA.Mobile.CSS
{
	internal class CSSMeasure
	{
		public int containerWidth;
		public int containerHeight;
		public int Top
		{
			get;
			set;
		}
		public int Right
		{
			get;
			set;
		}
		public int Bottom
		{
			get;
			set;
		}
		public int Left
		{
			get;
			set;
		}
		public CSSMeasure()
		{
			this.Top = 0;
			this.Right = 0;
			this.Bottom = 0;
			this.Left = 0;
		}
		public CSSMeasure(int top, int right, int bottom, int left)
		{
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
			this.Left = left;
		}
		public CSSMeasure(string measureString, int containerWidth, int containerHeight)
		{
			this.Top = 0;
			this.Right = 0;
			this.Bottom = 0;
			this.Left = 0;
			if (string.IsNullOrWhiteSpace(measureString))
			{
				return;
			}
			measureString = CSSMeasure.GetValue(measureString, containerWidth, containerHeight);
			measureString = measureString.Replace("px", "");
			string[] array = measureString.Trim().Split(new char[]
			{
				' '
			});
			int num = 0;
			switch (array.Length)
			{
			case 1:
				int.TryParse(array[0], out num);
				this.Right = (this.Left = (this.Top = (this.Bottom = num)));
				return;
			case 2:
				int.TryParse(array[0], out num);
				this.Top = (this.Bottom = num);
				int.TryParse(array[1], out num);
				this.Right = (this.Left = num);
				return;
			case 3:
				int.TryParse(array[0], out num);
				this.Top = num;
				int.TryParse(array[1], out num);
				this.Right = num;
				this.Left = this.Right;
				int.TryParse(array[2], out num);
				this.Bottom = num;
				return;
			case 4:
				int.TryParse(array[0], out num);
				this.Top = num;
				int.TryParse(array[1], out num);
				this.Right = num;
				int.TryParse(array[2], out num);
				this.Bottom = num;
				int.TryParse(array[3], out num);
				this.Left = num;
				return;
			default:
				return;
			}
		}
		public static int GetIntValue(string completeValue, int containerWidth, int containerHeight)
		{
			return CSSMeasure.GetIntValue(completeValue, containerWidth, containerHeight, 0, 0);
		}
		public static int GetIntValue(string completeValue, int containerWidth, int containerHeight, int offsetWidth, int offsetHeight)
		{
			completeValue = CSSMeasure.GetValue(completeValue, containerWidth, containerHeight, offsetWidth, offsetHeight);
			Match match = Regex.Match(completeValue, "\\d+px");
			int result = 0;
			if (match != null)
			{
				string value = match.Value;
				int.TryParse(value.Replace("px", ""), out result);
			}
			return result;
		}
		public static string GetValue(string completeValue, int containerWidth, int containerHeight)
		{
			return CSSMeasure.GetValue(completeValue, containerWidth, containerHeight, 0, 0, "");
		}
		public static string GetValue(string completeValue, int containerWidth, int containerHeight, string defaultValue)
		{
			return CSSMeasure.GetValue(completeValue, containerWidth, containerHeight, 0, 0, defaultValue);
		}
		public static string GetValue(string completeValue, int containerWidth, int containerHeight, int offsetWidth, int offsetHeight)
		{
			return CSSMeasure.GetValue(completeValue, containerWidth, containerHeight, offsetWidth, offsetHeight, "");
		}
		public static string GetValue(string completeValue, int containerWidth, int containerHeight, int offsetWidth, int offsetHeight, string defaultValue)
		{
			if (string.IsNullOrEmpty(completeValue))
			{
				return defaultValue;
			}
			string text = CSSMeasure.ReplacePercent(completeValue, containerWidth, containerHeight, offsetWidth, offsetHeight);
			return CSSMeasure.Calculate(text);
		}
		private static string ReplacePercent(string completeValue, int containerWidth, int containerHeight)
		{
			return CSSMeasure.ReplacePercent(completeValue, containerWidth, containerHeight, 0, 0);
		}
		private static string ReplacePercent(string completeValue, int containerWidth, int containerHeight, int offsetWidth, int offsetHeight)
		{
			completeValue.Contains("CW%");
			Regex regex = new Regex("\\d+(\\.(\\d)+){0,1}(D%|DW%|DH%|CW%|CH%|%|px)", RegexOptions.Singleline);
			return regex.Replace(completeValue, delegate(Match match)
			{
				string text = match.ToString();
				new DeviceInfo();
				int displayWidth = new DeviceInfo().DisplayWidth;
				int displayHeight = new DeviceInfo().DisplayHeight;
				double d = 0.0;
				string size = text.Replace(".", ",");
				StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
				if (text.IndexOf("DW%", comparisonType) >= 0)
				{
					double rawSize = CSSMeasure.GetRawSize(size);
					d = (double)((float)rawSize / 100f * (float)displayWidth - (float)offsetWidth);
				}
				else
				{
					if (text.IndexOf("DH%", comparisonType) >= 0)
					{
						double rawSize2 = CSSMeasure.GetRawSize(size);
						d = (double)((float)rawSize2 / 100f * (float)displayHeight - (float)offsetHeight);
					}
					else
					{
						if (text.IndexOf("CH%", comparisonType) >= 0)
						{
							double rawSize3 = CSSMeasure.GetRawSize(size);
							d = (double)((float)rawSize3 / 100f * (float)containerHeight - (float)offsetHeight);
						}
						else
						{
							if (text.Contains("%") || text.IndexOf("CW%", comparisonType) >= 0)
							{
								double rawSize4 = CSSMeasure.GetRawSize(size);
								d = (double)((float)rawSize4 / 100f * (float)containerWidth - (float)offsetWidth);
							}
							else
							{
								int num = 0;
								if (int.TryParse(text.Replace("px", ""), out num))
								{
									d = CSSMeasure.GetRawSize(text);
								}
							}
						}
					}
				}
				int num2 = 0;
				if (int.TryParse(Math.Floor(d).ToString(), out num2))
				{
					return num2 + "px";
				}
				return match.ToString();
			});
		}
		public static string Calculate(string text)
		{
			Regex regex = new Regex("(?x) # for sanity!\r\n\r\n                calc\r\n                (?'Value'\r\n                  (\r\n                     (\r\n                       [^()]*\r\n                       (?'Open'\\()\r\n                     )+\r\n                     (\r\n                       [^()]*\r\n                       (?'Close-Open'\\))\r\n                     )+\r\n                  )+?\r\n                )\r\n                (?(Open)(?!))\r\n\r\n            ");
			MatchCollection matchCollection = regex.Matches(text);
			foreach (Match match in matchCollection)
			{
				string text2 = match.Groups["Value"].ToString();
				text2 = text2.Remove(0, 1);
				text2 = text2.Remove(text2.Length - 1);
				text2 = text2.Replace("px", "");
				Expression expression = new Expression("Floor(" + text2 + ")");
				text = text.Replace(match.ToString(), expression.Evaluate().ToString());
			}
			return text;
		}
		private static double GetRawSize(string size)
		{
			string text = size.Replace("%", "");
			text = Regex.Replace(text, "([Cc])", "");
			text = Regex.Replace(text, "([Ww])", "");
			text = Regex.Replace(text, "([Hh])", "");
			text = Regex.Replace(text, "([Dd])", "");
			text = Regex.Replace(text, "([Pp][Xx])", "");
			double result = 0.0;
			double.TryParse(text, out result);
			return result;
		}
		public override string ToString()
		{
			if (this.Left == this.Right && this.Top == this.Bottom && this.Left == this.Top)
			{
				return this.Top + "px";
			}
			if (this.Left == this.Right && this.Top == this.Bottom)
			{
				return string.Concat(new object[]
				{
					this.Top,
					"px ",
					this.Right,
					"px "
				});
			}
			return string.Concat(new object[]
			{
				this.Top,
				"px ",
				this.Right,
				"px ",
				this.Bottom,
				"px ",
				this.Left,
				"px"
			});
		}
	}
}
