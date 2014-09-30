using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Plugins
{
	[ComVisible(true)]
	public interface IVirtualImageProvider
	{
		bool FileExists(string virtualPath, NameValueCollection queryString);
		IVirtualFile GetFile(string virtualPath, NameValueCollection queryString);
	}
}
