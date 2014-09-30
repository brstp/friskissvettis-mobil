using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public interface IUrlEventArgs
	{
		NameValueCollection QueryString
		{
			get;
			set;
		}
		string VirtualPath
		{
			get;
			set;
		}
	}
}
