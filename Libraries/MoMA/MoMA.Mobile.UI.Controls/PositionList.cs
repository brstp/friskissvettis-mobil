using MoMA.Helpers;
using MoMA.Mobile.CSS;
using MoMA.Mobile.Pages;
using MoMA.Mobile.Properties;
using MoMA.Mobile.UI.Controls.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace MoMA.Mobile.UI.Controls
{
	[ParseChildren(ChildrenAsProperties = true)]
	public class PositionList : MobileControl, INamingContainer
	{
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public PositionListItemCollection Positions
		{
			get;
			set;
		}
		[Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(PositionList))]
		public ITemplate ItemTemplate
		{
			get;
			set;
		}
		internal override void RegisterJsIncludes()
		{
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("gmaps"))
			{
				this.Page.ClientScript.RegisterClientScriptInclude("gmaps", "http://maps.googleapis.com/maps/api/js?sensor=false");
			}
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("geo"))
			{
				this.Page.ClientScript.RegisterClientScriptInclude("geo", "geo.js");
			}
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("gears_init"))
			{
				this.Page.ClientScript.RegisterClientScriptInclude("gears_init", "gears_init.js");
			}
		}
		internal override void RegisterJsScripts()
		{
			string text = "";
			string text2 = "";
			if (this.ItemTemplate != null)
			{
				Literal literal = new Literal();
				this.ItemTemplate.InstantiateIn(literal);
				text2 = ConversionHelper.ControlToHtml(literal);
				text2 = text2.Replace(Environment.NewLine, "").Trim();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("id", base.ID);
			dictionary.Add("template", text2.Replace("\"", "\\\""));
			text += JavascriptHelper.CreateScript(Resources.positionlist_functions, dictionary, false);
			text += JavascriptHelper.CreateScript(Resources.positionlist_deviceposition, dictionary, false);
			MobilePage expr_A9 = this.Page as MobilePage;
			expr_A9.Html5PositionScript += text;
		}
		internal override void RegisterStartupJsScripts()
		{
		}
		internal override string BuildCSS(CSSStylesheet stylesheet)
		{
			return "";
		}
		internal override string BuildHtml()
		{
			string text = string.IsNullOrEmpty(base.CssClass) ? "" : ("\" class=\"" + base.CssClass + "\">");
			string text2 = "";
			string text3 = text2;
			text2 = string.Concat(new string[]
			{
				text3,
				"<select id=\"",
				base.ID,
				"\"",
				text,
				">"
			});
			foreach (PositionListItem current in this.Positions)
			{
				string text4 = text2;
				text2 = string.Concat(new string[]
				{
					text4,
					"<option value=\"",
					current.Latitude.ToString().Replace(",", "."),
					"x",
					current.Longitude.ToString().Replace(",", "."),
					"\">",
					current.Title,
					"</option>"
				});
			}
			text2 += "</select>";
			return text2;
		}
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(this.BuildHtml());
		}
	}
}
