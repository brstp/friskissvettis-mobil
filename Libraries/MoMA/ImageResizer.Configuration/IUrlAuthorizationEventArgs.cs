using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public interface IUrlAuthorizationEventArgs
	{
		bool AllowAccess
		{
			get;
			set;
		}
		NameValueCollection QueryString
		{
			get;
		}
		string VirtualPath
		{
			get;
		}
	}
}
