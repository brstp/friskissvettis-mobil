using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public enum IssueSeverity
	{
		Critical,
		Error,
		ConfigurationError,
		Warning
	}
}
