using MoMA.Mobile.CSS;
using MoMA.Mobile.UI.Controls.Helpers;
using System;
using System.ComponentModel;
using System.Web.UI;
namespace MoMA.Mobile.UI.Controls
{
	public class FancyButton : MobileControl
	{
		[Category("Appearance"), DefaultValue(false), Description("")]
		public BorderMode BorderMode
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(false), Description("")]
		public string IconUrl
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(false), Description("")]
		public bool Shadow
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(""), Description]
		public string GradientStart
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(""), Description]
		public string GradientEnd
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(""), Description]
		public string TextColor
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(""), Description("Text content")]
		public string Text
		{
			get;
			set;
		}
		public FancyButton()
		{
			this.BorderMode = BorderMode.All;
			this.GradientStart = "#FFFFFF";
			this.GradientEnd = "#E8EFF5";
		}
		internal override void RegisterJsIncludes()
		{
		}
		internal override void RegisterJsScripts()
		{
		}
		internal override void RegisterStartupJsScripts()
		{
		}
		internal override string BuildCSS(CSSStylesheet stylesheet)
		{
			CSSSectionCollection cSSSectionCollection = new CSSSectionCollection();
			CSSSection cSSSection = new CSSSection(stylesheet);
			CSSSection cSSSection2 = new CSSSection(stylesheet);
			cSSSectionCollection.Add(cSSSection);
			cSSSectionCollection.Add(cSSSection2);
			string str = this.GradientStart.StartsWith("#") ? this.GradientStart : ("#" + this.GradientStart);
			string str2 = this.GradientEnd.StartsWith("#") ? this.GradientEnd : ("#" + this.GradientEnd);
			cSSSection.Selector = base.CSSIdentifier;
			cSSSection.AddProperty("font-family", "arial,helvetica,sans-serif");
			cSSSection.AddProperty("font-size", "5CW%");
			cSSSection.AddProperty("padding", "2.5CW% 2.5CW%");
			cSSSection.AddProperty("background-linear-gradient", str + " " + str2);
			cSSSection.AddProperty("vertical-align", "middle");
			cSSSection.AddProperty("color", this.TextColor);
			cSSSection2.Selector = base.CSSIdentifier + " td";
			if (this.Shadow)
			{
				cSSSection.AddProperty("box-shadow", "0px 0px 2CW% #000");
			}
			string text = "3.6CW%";
			switch (this.BorderMode)
			{
			case BorderMode.All:
				cSSSection.AddProperty("border-radius", text);
				break;
			case BorderMode.Top:
				cSSSection.AddProperty("border-radius", text + " " + text + " 0 0");
				break;
			case BorderMode.Bottom:
				cSSSection.AddProperty("border-radius", "0 0 " + text + " " + text);
				break;
			}
			if (!string.IsNullOrEmpty(this.IconUrl))
			{
				CSSSection cSSSection3 = new CSSSection(stylesheet);
				cSSSectionCollection.Add(cSSSection3);
				cSSSection3.Selector = base.CSSIdentifier + " img";
				cSSSection3.AddProperty("margin", "1DW% 3DW% 1DW% 1DW%");
				cSSSection3.AddProperty("width", "16px");
			}
			return cSSSectionCollection.ToString();
		}
		internal override string BuildHtml()
		{
			string text = string.IsNullOrEmpty(base.CssClass) ? "" : (" class=\"" + base.CssClass + "\"");
			this.Text = (string.IsNullOrEmpty(this.Text) ? "&nbsp;" : this.Text);
			string str = string.Concat(new string[]
			{
				"<div id=\"",
				base.ID,
				"\" render-image=\"",
				base.RenderImage,
				"\"",
				text,
				">"
			});
			if (!string.IsNullOrEmpty(this.IconUrl))
			{
				str = str + "<table><tr><td style=\"width:16px;\"><img src=\"" + this.IconUrl + "\" /></td><td>";
			}
			str += this.Text;
			if (!string.IsNullOrEmpty(this.IconUrl))
			{
				str += "</td></tr><table>";
			}
			return str + "</div>";
		}
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(this.BuildHtml());
		}
	}
}
