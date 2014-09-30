using ImageResizer.Configuration;
using System;
using System.Runtime.InteropServices;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class Diagnostic : IPlugin
	{
		private Config c;
		public IPlugin Install(Config c)
		{
			c.Pipeline.PostAuthorizeRequestStart += new RequestEventHandler(this.Pipeline_PostAuthorizeRequestStart);
			c.Plugins.add_plugin(this);
			this.c = c;
			return this;
		}
		private void Pipeline_PostAuthorizeRequestStart(IHttpModule sender, HttpContext context)
		{
			if (context.Request.FilePath.EndsWith("/resizer.debug", StringComparison.OrdinalIgnoreCase) || context.Request.FilePath.EndsWith("/resizer.debug.ashx", StringComparison.OrdinalIgnoreCase))
			{
				IHttpHandler arg_60_1;
				if (!this.AllowResponse(context))
				{
					IHttpHandler httpHandler = new DiagnosticDisabledHandler(this.c);
					arg_60_1 = httpHandler;
				}
				else
				{
					arg_60_1 = new DiagnosticPageHandler(this.c);
				}
				context.RemapHandler(arg_60_1);
			}
		}
		public bool AllowResponse(HttpContext context)
		{
			DiagnosticMode defaultValue = (!context.IsCustomErrorEnabled) ? DiagnosticMode.AllHosts : DiagnosticMode.None;
			DiagnosticMode diagnosticMode = this.c.get<DiagnosticMode>("diagnostics.enableFor", defaultValue);
			return diagnosticMode != DiagnosticMode.None && (diagnosticMode == DiagnosticMode.AllHosts || context.Request.IsLocal);
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			c.Pipeline.PostAuthorizeRequestStart -= new RequestEventHandler(this.Pipeline_PostAuthorizeRequestStart);
			return true;
		}
	}
}
