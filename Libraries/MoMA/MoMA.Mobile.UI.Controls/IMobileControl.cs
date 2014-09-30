using MoMA.Mobile.CSS;
using System;
namespace MoMA.Mobile.UI.Controls
{
	internal interface IMobileControl
	{
		string BuildCSS(CSSStylesheet stylesheet);
		string BuildHtml();
		void RegisterJsIncludes();
		void RegisterJsScripts();
		void RegisterStartupJsScripts();
	}
}
