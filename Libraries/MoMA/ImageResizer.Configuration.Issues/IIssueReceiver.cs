using System;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration.Issues
{
	[ComVisible(true)]
	public interface IIssueReceiver
	{
		void AcceptIssue(IIssue i);
	}
}
