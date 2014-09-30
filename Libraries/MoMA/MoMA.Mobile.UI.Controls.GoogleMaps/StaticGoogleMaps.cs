using MoMA.Helpers;
using MoMA.Helpers.Url;
using MoMA.Mobile.CSS;
using MoMA.Mobile.Pages;
using MoMA.Mobile.Properties;
using MoMA.Mobile.UI.Controls.GoogleMaps.Helpers;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
namespace MoMA.Mobile.UI.Controls.GoogleMaps
{
	[ParseChildren(ChildrenAsProperties = true)]
	public class StaticGoogleMaps : GoogleMaps
	{
		private string StaticMapUrl
		{
			get
			{
				string centerOfMap = base.CenterOfMap;
				string value = base.Zoom.ToString();
				ExtendedUrl extendedUrl = new ExtendedUrl("http://maps.googleapis.com/maps/api/staticmap");
				extendedUrl.Querystring.Add("center", centerOfMap);
				extendedUrl.Querystring.Add("zoom", value);
				extendedUrl.Querystring.Add("size", "{width}x{height}");
				extendedUrl.Querystring.Add("maptype", base.MapType.ToString().ToLower());
				extendedUrl.Querystring.Add("sensor", "false");
				extendedUrl.Querystring.Add("scale", "2");
				foreach (GoogleMapsMarker current in base.Markers)
				{
					string str = string.IsNullOrEmpty(current.Icon) ? "" : ("icon:" + HttpContext.Current.Server.UrlEncode(current.Icon) + "|");
					string value2 = str + JavascriptHelper.GetValue(current.Longitude) + "," + JavascriptHelper.GetValue(current.Latitude);
					extendedUrl.Querystring.Add("markers", value2);
				}
				return extendedUrl.ToString();
			}
		}
		private string LinkUrl
		{
			get
			{
				return string.Concat(new object[]
				{
					"http://maps.google.com/maps?hl=sv&q=",
					base.Location,
					"&z=",
					base.Zoom
				});
			}
		}
		public StaticGoogleMaps()
		{
		}
		public StaticGoogleMaps(GoogleMaps map) : base(map)
		{
		}
		internal override string BuildCSS(CSSStylesheet stylesheet)
		{
			return "";
		}
		internal override void RegisterJsIncludes()
		{
			if (base.ShowPosition)
			{
				this.Page.ClientScript.RegisterClientScriptInclude("geo", "js/geo.js");
				this.Page.ClientScript.RegisterClientScriptInclude("gears_init", "js/gears_init.js");
			}
		}
		internal override void RegisterStartupJsScripts()
		{
		}
		internal override void RegisterJsScripts()
		{
			if (base.ShowPosition)
			{
				string value = string.IsNullOrEmpty(base.PositionIcon) ? "" : ("icon:" + HttpContext.Current.Server.UrlEncode(base.PositionIcon) + "|");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("id", base.ID);
				dictionary.Add("icon", value);
				TextHelper.ReplaceTemplateValues("googlemaps_{id}", dictionary);
				string str = JavascriptHelper.CreateScript(Resources.googlemaps_static_deviceposition, dictionary, false);
				MobilePage expr_91 = this.Page as MobilePage;
				expr_91.Html5PositionScript += str;
			}
		}
		internal override string BuildHtml()
		{
			string text = string.IsNullOrEmpty(base.CssClass) ? "" : ("\" class=\"" + base.CssClass + "\">");
			string text2 = "";
			string text3 = text2;
			return string.Concat(new string[]
			{
				text3,
				"  <img id=\"",
				base.ID,
				"\" src=\"",
				this.StaticMapUrl,
				"\" ",
				text,
				" >"
			});
		}
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(this.BuildHtml());
		}
	}
}
