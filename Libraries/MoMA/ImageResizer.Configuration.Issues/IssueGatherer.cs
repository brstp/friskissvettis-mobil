using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public class IssueGatherer : IIssueProvider
	{
		private Config c;
		public IssueGatherer(Config c)
		{
			this.c = c;
		}
		public IEnumerable<IIssue> GetIssues()
		{
			List<IIssue> list = new List<IIssue>();
			list.AddRange(this.c.configurationSectionIssues.GetIssues());
			list.AddRange(new ConfigChecker(this.c).GetIssues());
			list.AddRange(this.c.Plugins.GetIssues());
			IIssueProvider issueProvider = this.c.CurrentImageBuilder as IIssueProvider;
			if (issueProvider != null)
			{
				list.AddRange(issueProvider.GetIssues());
			}
			foreach (IIssueProvider current in this.c.Plugins.GetAll<IIssueProvider>())
			{
				list.AddRange(current.GetIssues());
			}
			return list;
		}
	}
}
