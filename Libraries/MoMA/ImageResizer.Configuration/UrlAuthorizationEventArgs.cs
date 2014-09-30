using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public class UrlAuthorizationEventArgs : EventArgs, IUrlAuthorizationEventArgs
	{
		private bool allowAccess = true;
		protected string _virtualPath;
		protected NameValueCollection _queryString;
		public bool AllowAccess
		{
			get
			{
				return this.allowAccess;
			}
			set
			{
				this.allowAccess = value;
			}
		}
		public NameValueCollection QueryString
		{
			get
			{
				return this._queryString;
			}
		}
		public string VirtualPath
		{
			get
			{
				return this._virtualPath;
			}
		}
		public UrlAuthorizationEventArgs(string virtualPath, NameValueCollection queryString, bool allowAccess)
		{
			this._virtualPath = virtualPath;
			this._queryString = queryString;
			this.allowAccess = allowAccess;
		}
	}
}
