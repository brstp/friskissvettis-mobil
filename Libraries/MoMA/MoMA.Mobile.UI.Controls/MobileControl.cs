using MoMA.Mobile.CSS;
using System;
using System.ComponentModel;
using System.Web.UI;
namespace MoMA.Mobile.UI.Controls
{
	public class MobileControl : Control
	{
		private Random _rnd = new Random();
		private string _id = "";
		[Category("Appearance"), DefaultValue(false), Description("")]
		public string RenderImage
		{
			get;
			set;
		}
		[Category("Appearance"), DefaultValue(false), Description("")]
		public string CssClass
		{
			get;
			set;
		}
		public new string ID
		{
			get
			{
				if (string.IsNullOrEmpty(this._id))
				{
					this._id = "id" + this._rnd.Next().ToString();
				}
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}
		internal string CSSIdentifier
		{
			get
			{
				return "#" + this.ID;
			}
		}
		internal virtual string BuildCSS(CSSStylesheet stylesheet)
		{
			return "";
		}
		internal virtual string BuildHtml()
		{
			return "";
		}
		internal virtual void RegisterJsIncludes()
		{
		}
		internal virtual void RegisterJsScripts()
		{
		}
		internal virtual void RegisterStartupJsScripts()
		{
		}
	}
}
