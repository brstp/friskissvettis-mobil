using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public class UrlEventArgs : EventArgs, IUrlEventArgs
	{
		protected string _virtualPath;
		protected NameValueCollection _queryString;
		public NameValueCollection QueryString
		{
			get
			{
				return this._queryString;
			}
			set
			{
				this._queryString = value;
			}
		}
		public string VirtualPath
		{
			get
			{
				return this._virtualPath;
			}
			set
			{
				this._virtualPath = value;
			}
		}
		public UrlEventArgs(string virtualPath, NameValueCollection queryString)
		{
			this._virtualPath = virtualPath;
			this._queryString = queryString;
		}
	}
}
