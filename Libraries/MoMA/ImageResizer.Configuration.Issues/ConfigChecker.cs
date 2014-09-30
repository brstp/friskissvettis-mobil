using ImageResizer.Plugins.Basic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public class ConfigChecker : IIssueProvider
	{
		private Config c;
		public ConfigChecker(Config c)
		{
			this.c = c;
		}
		public IEnumerable<IIssue> GetIssues()
		{
			List<IIssue> list = new List<IIssue>();
			if (this.c.getNode("sizelimiting") != null)
			{
				list.Add(new Issue("Use <sizelimits>, not <sizelimiting> to configure the SizeLimiting plugin", IssueSeverity.ConfigurationError));
			}
			if (this.c.getNode("sizelimits") != null && !this.c.Plugins.Has<SizeLimiting>())
			{
				list.Add(new Issue("You have specified configuration settings for the SizeLimiting plugin, but it is not installed. ", "Use <add name=\"SizeLimiting\" /> inside <plugins> to install.", IssueSeverity.ConfigurationError));
			}
			if (this.c.Pipeline.ProcessedCount < 1L)
			{
				list.Add(new Issue("To potentially see additional errors here, perform an image resize request.", IssueSeverity.Warning));
			}
			if (!SecurityManager.IsGranted(new SecurityPermission(PermissionState.Unrestricted)))
			{
				list.Add(new Issue("Grant the website SecurityPermission to call UrlAuthorizationModule.CheckUrlAccessForPrincipal", "Without this permission, it may be possible for users to bypass UrlAuthorization rules you have defined for your website, and access images that would otherwise be protected. If you do not use UrlAuthorization rules, this should not be a concern. You may also re-implement your security rules by handling the Config.Current.Pipeline.AuthorizeImage event.", IssueSeverity.Critical));
			}
			string text = "";
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				if (assembly.GetName().Name.IndexOf("ImageResizer", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
					if (customAttributes != null && customAttributes.Length > 0 && ((AssemblyInformationalVersionAttribute)customAttributes[0]).InformationalVersion.IndexOf("hotfix", StringComparison.OrdinalIgnoreCase) > -1)
					{
						text = text + assembly.GetName().Name + ", ";
					}
				}
			}
			text = text.TrimEnd(new char[]
			{
				',',
				' '
			});
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(new Issue("You are running a hotfix version of the ImageResizer.", "You should upgrade to a released version with an equal or higher version number as soon as possible. Hotfix and release DLLs with the same version number are not the same - the release DLL should be used instead.\nAssemblies marked as hotfix versions: " + text, IssueSeverity.Warning));
			}
			return list;
		}
	}
}
