using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public interface IIssueProvider
	{
		IEnumerable<IIssue> GetIssues();
	}
}
