using MoMA.Helpers;
using MoMA.Helpers.Url;
using MoMA.Mobile.CSS;
using MoMA.Mobile.Device;
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
	public class JavascriptGoogleMaps : GoogleMaps
	{
		private DeviceInfo device = new DeviceInfo();
		public JavascriptGoogleMaps()
		{
		}
		public JavascriptGoogleMaps(GoogleMaps map) : base(map)
		{
		}
		private string CreatePositionMarkerJavascript(string title, string icon)
		{
			title = JavascriptHelper.GetProperty("title", title);
			icon = JavascriptHelper.GetProperty("icon", icon);
			new Dictionary<string, string>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("id", base.ID);
			dictionary.Add("title", title);
			dictionary.Add("icon", icon);
			return JavascriptHelper.CreateScript(Resources.googlemaps_javascript_marker_deviceposition, dictionary, false);
		}
		private string CreateMarkerJavascript(GoogleMapsMarker marker, int index)
		{
			string property = JavascriptHelper.GetProperty("title", marker.Title);
			string property2 = JavascriptHelper.GetProperty("index", index);
			string value = JavascriptHelper.GetValue(marker.Longitude);
			string value2 = JavascriptHelper.GetValue(marker.Latitude);
			string property3 = JavascriptHelper.GetProperty("icon", marker.Icon);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("id", base.ID);
			dictionary.Add("index", property2);
			dictionary.Add("title", property);
			dictionary.Add("icon", property3);
			dictionary.Add("longitude", value);
			dictionary.Add("latitude", value2);
			return JavascriptHelper.CreateScript(Resources.googlemaps_javascript_marker, dictionary, false);
		}
		internal override string BuildCSS(CSSStylesheet stylesheet)
		{
			CSSSection cSSSection = new CSSSection(stylesheet);
			cSSSection.Selector = base.CSSIdentifier;
			cSSSection.AddProperty("width", "100CW%");
			cSSSection.AddProperty("height", "100CW%");
			return cSSSection.ToString();
		}
		internal override void RegisterJsIncludes()
		{
			ExtendedUrl extendedUrl = new ExtendedUrl(HttpContext.Current.Request.Url.ToString());
			extendedUrl.Querystring.Update("mode", "resource");
			extendedUrl.Querystring.Update("type", HttpContext.Current.Server.UrlEncode("text/js"));
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("gmaps"))
			{
				this.Page.ClientScript.RegisterClientScriptInclude("gmaps", "http://maps.googleapis.com/maps/api/js?sensor=false");
			}
			if (base.ShowPosition)
			{
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("geo"))
				{
					extendedUrl.Querystring.Update("file", HttpContext.Current.Server.UrlEncode("js/libraries/geo.js"));
					this.Page.ClientScript.RegisterClientScriptInclude("geo", extendedUrl.ToString());
				}
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("gears_init"))
				{
					extendedUrl.Querystring.Update("file", HttpContext.Current.Server.UrlEncode("js/libraries/gears_init.js"));
					this.Page.ClientScript.RegisterClientScriptInclude("gears_init", extendedUrl.ToString());
				}
			}
		}
		internal override void RegisterStartupJsScripts()
		{
		}
		internal override void RegisterJsScripts()
		{
			string text = "initialize_" + base.ID;
			string text2 = "    function " + text + "() {";
			string value = base.ID.ToString();
			string location = base.Location;
			string value2 = JavascriptHelper.GetValue(base.Longitude);
			string value3 = JavascriptHelper.GetValue(base.Latitude);
			string value4 = (base.Zoom == 0) ? "14" : base.Zoom.ToString();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("id", value);
			dictionary.Add("zoom", value4);
			dictionary.Add("longitude", value2);
			dictionary.Add("latitude", value3);
			dictionary.Add("location", location);
			if (string.IsNullOrEmpty(base.Location))
			{
				text2 += Resources.googlemaps_javascript_initialize_latlng;
			}
			else
			{
				text2 += Resources.googlemaps_javascript_initialize_location;
			}
			if (base.ShowZoomControls)
			{
				int intValue = CSSMeasure.GetIntValue("10CW%", DeviceInfo.CurrentDevice.DisplayWidth, DeviceInfo.CurrentDevice.DisplayWidth);
				ExtendedUrl extendedUrl = new ExtendedUrl(this.Page.Request.Url.ToString());
				extendedUrl.Querystring.Update("mode", "resource");
				extendedUrl.Querystring.Update("type", "image/png");
				extendedUrl.Querystring.Update("width", intValue.ToString());
				if (string.IsNullOrEmpty(base.ZoomInImageUrl))
				{
					extendedUrl.Querystring.Update("file", "images/maps/gm-plus.png");
					dictionary.Add("zoomInImageUrl", extendedUrl.ToString());
				}
				if (string.IsNullOrEmpty(base.ZoomOutImageUrl))
				{
					extendedUrl.Querystring.Update("file", "images/maps/gm-minus.png");
					dictionary.Add("zoomOutImageUrl", extendedUrl.ToString());
				}
				text2 += Resources.googlemaps_javascript_zoom;
			}
			text2 = JavascriptHelper.CreateScript(text2, dictionary, false);
			if (base.ShowPosition)
			{
				MobilePage expr_220 = this.Page as MobilePage;
				expr_220.Html5PositionScript += this.CreatePositionMarkerJavascript(base.PositionTitle, base.PositionIcon);
			}
			for (int i = 0; i < base.Markers.Count; i++)
			{
				GoogleMapsMarker marker = base.Markers[i];
				text2 += this.CreateMarkerJavascript(marker, i);
			}
			text2 += "     }";
			MobilePage expr_296 = this.Page as MobilePage;
			string javascript = expr_296.Javascript;
			expr_296.Javascript = string.Concat(new string[]
			{
				javascript,
				text2,
				Environment.NewLine,
				text,
				"();"
			});
		}
		internal override string BuildHtml()
		{
			string text = string.IsNullOrEmpty(base.CssClass) ? "" : ("\" class=\"" + base.CssClass + "\">");
			return string.Concat(new string[]
			{
				"<div id=\"",
				base.ID,
				"\"",
				text,
				"></div>"
			});
		}
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(this.BuildHtml());
		}
	}
}
