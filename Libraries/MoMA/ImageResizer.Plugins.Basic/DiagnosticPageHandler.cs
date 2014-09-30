using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	public class DiagnosticPageHandler : IHttpHandler
	{
		private Config c;
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		public DiagnosticPageHandler(Config c)
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
			List<IIssue> list = new List<IIssue>(c.AllIssues.GetIssues());
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Image resizier diagnostic sheet\t\t" + DateTime.UtcNow.ToString() + "\n");
			stringBuilder.AppendLine(list.Count + " Issues detected:\n");
			foreach (IIssue current in list)
			{
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					current.Source,
					"(",
					current.Severity.ToString(),
					"):\t",
					current.Summary,
					("\n" + current.Details).Replace("\n", "\n\t\t\t"),
					"\n"
				}));
			}
			stringBuilder.AppendLine("\nRegistered plugins:\n");
			foreach (IPlugin current2 in c.Plugins.AllPlugins)
			{
				stringBuilder.AppendLine(current2.ToString());
			}
			stringBuilder.AppendLine("\nConfiguration:\n");
			stringBuilder.AppendLine(c.getConfigXml().ToString());
			stringBuilder.AppendLine("\nAccepted querystring keys:\n");
			foreach (string current3 in c.Pipeline.SupportedQuerystringKeys)
			{
				stringBuilder.Append(current3 + ", ");
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("\nAccepted file extensions:\n");
			foreach (string current4 in c.Pipeline.AcceptedImageExtensions)
			{
				stringBuilder.Append(current4 + ", ");
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("\nEnvironment information:\n");
			string text = (context != null) ? context.Request.ServerVariables["SERVER_SOFTWARE"] : "NOT ASP.NET";
			if (!string.IsNullOrEmpty(text))
			{
				text += " on ";
			}
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"Running ",
				text,
				Environment.OSVersion.ToString(),
				" and CLR ",
				Environment.Version.ToString()
			}));
			stringBuilder.AppendLine("Executing assembly: " + Process.GetCurrentProcess().MainModule.FileName);
			stringBuilder.AppendLine("IntegratedPipeline=" + HttpRuntime.UsingIntegratedPipeline.ToString());
			stringBuilder.AppendLine("\nLoaded assemblies:\n");
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				stringBuilder.Append(assembly.GetName().Name.PadRight(40, ' '));
				stringBuilder.Append(" Assembly: " + assembly.GetName().Version.ToString().PadRight(15));
				object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					stringBuilder.Append(" File: " + ((AssemblyFileVersionAttribute)customAttributes[0]).Version.PadRight(15));
				}
				customAttributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					stringBuilder.Append(" Info: " + ((AssemblyInformationalVersionAttribute)customAttributes[0]).InformationalVersion);
				}
				customAttributes = assembly.GetCustomAttributes(typeof(CommitAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					stringBuilder.Append("  Commit: " + ((CommitAttribute)customAttributes[0]).Value);
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}
	}
}
