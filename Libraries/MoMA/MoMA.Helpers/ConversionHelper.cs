using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
namespace MoMA.Helpers
{
	public class ConversionHelper
	{
		public static int DoubleToInt(double d)
		{
			return int.Parse(Math.Floor(d).ToString());
		}
		public static string ControlToHtml(Control control)
		{
			StringBuilder stringBuilder = new StringBuilder();
			HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
			control.RenderControl(writer);
			return stringBuilder.ToString();
		}
		public static T Convert<T>(object value, T defaultValue)
		{
			T result = defaultValue;
			try
			{
				result = (T)((object)System.Convert.ChangeType(value, typeof(T)));
			}
			catch
			{
			}
			return result;
		}
		public static List<Control> ControlCollectionToList(ControlCollection collection)
		{
			List<Control> list = new List<Control>();
			foreach (Control item in collection)
			{
				list.Add(item);
			}
			return list;
		}
	}
}
