using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace MoMA.Mobile.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;
		private static CultureInfo resourceCulture;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("MoMA.Mobile.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}
		internal static string googlemaps_javascript_initialize_latlng
		{
			get
			{
				return Resources.ResourceManager.GetString("googlemaps_javascript_initialize_latlng", Resources.resourceCulture);
			}
		}
		internal static string googlemaps_javascript_initialize_location
		{
			get
			{
				return Resources.ResourceManager.GetString("googlemaps_javascript_initialize_location", Resources.resourceCulture);
			}
		}
		internal static string googlemaps_javascript_marker
		{
			get
			{
				return Resources.ResourceManager.GetString("googlemaps_javascript_marker", Resources.resourceCulture);
			}
		}
		internal static string googlemaps_javascript_marker_deviceposition
		{
			get
			{
				return Resources.ResourceManager.GetString("googlemaps_javascript_marker_deviceposition", Resources.resourceCulture);
			}
		}
		internal static string googlemaps_javascript_zoom
		{
			get
			{
				return Resources.ResourceManager.GetString("googlemaps_javascript_zoom", Resources.resourceCulture);
			}
		}
		internal static string googlemaps_static_deviceposition
		{
			get
			{
				return Resources.ResourceManager.GetString("googlemaps_static_deviceposition", Resources.resourceCulture);
			}
		}
		internal static string html5_positioning
		{
			get
			{
				return Resources.ResourceManager.GetString("html5_positioning", Resources.resourceCulture);
			}
		}
		internal static string onload
		{
			get
			{
				return Resources.ResourceManager.GetString("onload", Resources.resourceCulture);
			}
		}
		internal static string positionlist_deviceposition
		{
			get
			{
				return Resources.ResourceManager.GetString("positionlist_deviceposition", Resources.resourceCulture);
			}
		}
		internal static string positionlist_functions
		{
			get
			{
				return Resources.ResourceManager.GetString("positionlist_functions", Resources.resourceCulture);
			}
		}
		internal Resources()
		{
		}
	}
}
