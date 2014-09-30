using ImageResizer.Configuration;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Configuration;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class DiagnosticDisabledHandler : IHttpHandler
	{
		private Config c;
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		public DiagnosticDisabledHandler(Config c)
		{
			this.c = c;
		}
		public void ProcessRequest(HttpContext context)
		{
			context.Response.StatusCode = 200;
			context.Response.ContentType = "text/plain";
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Write(this.GenerateOutput(context, this.c));
		}
		public string GenerateOutput(HttpContext context, Config c)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration(null);
			CustomErrorsSection customErrorsSection = (configuration != null) ? ((CustomErrorsSection)configuration.GetSection("system.web/customErrors")) : null;
			CustomErrorsMode customErrorsMode = (customErrorsSection != null) ? customErrorsSection.Mode : CustomErrorsMode.RemoteOnly;
			DiagnosticMode diagnosticMode = c.get<DiagnosticMode>("diagnostics.enableFor", DiagnosticMode.None);
			bool flag = c.get("diagnostics.enableFor", null) != null;
			bool flag2 = (!flag && customErrorsMode == CustomErrorsMode.RemoteOnly) || diagnosticMode == DiagnosticMode.Localhost;
			stringBuilder.AppendLine("The Resizer diagnostics page is " + (flag2 ? "only available from localhost." : "disabled."));
			stringBuilder.AppendLine();
			if (flag)
			{
				stringBuilder.AppendLine("This is because <diagnostics enableFor=\"" + diagnosticMode.ToString() + "\" />.");
			}
			else
			{
				stringBuilder.AppendLine("This is because <customErrors mode=\"" + customErrorsMode.ToString() + "\" />.");
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("To override for localhost access, add <diagnostics enableFor=\"localhost\" /> in the <resizer> section of Web.config.");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("To ovveride for remote access, add <diagnostics enableFor=\"allhosts\" /> in the <resizer> section of Web.config.");
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}
	}
}
