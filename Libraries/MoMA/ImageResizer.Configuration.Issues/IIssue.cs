using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public interface IIssue
	{
		string Source
		{
			get;
		}
		string Summary
		{
			get;
		}
		string Details
		{
			get;
		}
		IssueSeverity Severity
		{
			get;
		}
	}
}
