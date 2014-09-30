using MoMA.Mobile.CSS;
using MoMA.Mobile.Device;
using MoMA.Mobile.UI.Controls.GoogleMaps.Helpers;
using System;
using System.ComponentModel;
using System.Web.UI;
namespace MoMA.Mobile.UI.Controls.GoogleMaps
{
	[ParseChildren(ChildrenAsProperties = true)]
	public class GoogleMaps : MobileControl
	{
		private DeviceInfo device = new DeviceInfo();
		private GoogleMaps _map;
		[Category("Appearance"), DefaultValue(0), Description("")]
		public string Location
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(0), Description("")]
		public double Latitude
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(0), Description("")]
		public double Longitude
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(0), Description("")]
		public int Zoom
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(false), Description("")]
		public bool ShowPosition
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(""), Description("")]
		public string PositionIcon
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(""), Description("")]
		public string PositionTitle
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(GoogleMapsMapType.Roadmap), Description("")]
		public GoogleMapsMapType MapType
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(GoogleMapsStatic.Auto), Description("")]
		public GoogleMapsStatic StaticMap
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(true), Description("")]
		public bool ShowZoomControls
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(true), Description("")]
		public string ZoomInImageUrl
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(true), Description("")]
		public string ZoomOutImageUrl
		{
			get;
			set;
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public GoogleMapsMarkerCollection Markers
		{
			get;
			set;
		}
		internal bool UseStaticMap
		{
			get
			{
				return this.StaticMap == GoogleMapsStatic.Yes || (this.StaticMap == GoogleMapsStatic.Auto && !this.device.ScriptSupport);
			}
		}
		internal string CenterOfMap
		{
			get
			{
				if (!string.IsNullOrEmpty(this.Location))
				{
					return this.Location;
				}
				return this.Longitude.ToString().Replace(",", ".") + "," + this.Latitude.ToString().Replace(",", ".");
			}
		}
		private GoogleMaps Map
		{
			get
			{
				if (this._map == null)
				{
					if (this.UseStaticMap)
					{
						this._map = new StaticGoogleMaps(this);
					}
					else
					{
						this._map = new JavascriptGoogleMaps(this);
					}
					this._map.Page = this.Page;
				}
				return this._map;
			}
		}
		public GoogleMaps()
		{
			this.Markers = new GoogleMapsMarkerCollection();
			this.MapType = GoogleMapsMapType.Roadmap;
			this.StaticMap = GoogleMapsStatic.Auto;
		}
		public GoogleMaps(GoogleMaps map) : this()
		{
			base.ID = map.ID;
			this.Location = map.Location;
			this.Latitude = map.Latitude;
			this.Longitude = map.Longitude;
			this.Zoom = map.Zoom;
			this.ShowPosition = map.ShowPosition;
			this.PositionIcon = map.PositionIcon;
			this.PositionTitle = map.PositionTitle;
			this.MapType = map.MapType;
			this.StaticMap = map.StaticMap;
			this.Markers = map.Markers;
			this.ShowZoomControls = map.ShowZoomControls;
			this.ZoomInImageUrl = map.ZoomInImageUrl;
			this.ZoomOutImageUrl = map.ZoomOutImageUrl;
		}
		internal override string BuildCSS(CSSStylesheet stylesheet)
		{
			return this.Map.BuildCSS(stylesheet);
		}
		internal override void RegisterJsIncludes()
		{
			this.Map.RegisterJsIncludes();
		}
		internal override void RegisterStartupJsScripts()
		{
			this.Map.RegisterStartupJsScripts();
		}
		internal override void RegisterJsScripts()
		{
			this.Map.RegisterJsScripts();
		}
		internal override string BuildHtml()
		{
			return this.Map.BuildHtml();
		}
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(this.BuildHtml());
		}
	}
}
