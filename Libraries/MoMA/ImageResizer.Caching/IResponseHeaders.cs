using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Caching;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public interface IResponseHeaders
	{
		string ContentType
		{
			get;
			set;
		}
		HttpCacheability CacheControl
		{
			get;
			set;
		}
		DateTime Expires
		{
			get;
			set;
		}
		DateTime LastModified
		{
			get;
			set;
		}
		bool ValidUntilExpires
		{
			get;
			set;
		}
		bool SuppressVaryHeader
		{
			get;
			set;
		}
		NameValueCollection DefaultHeaders
		{
			get;
			set;
		}
		NameValueCollection Headers
		{
			get;
			set;
		}
		List<CacheDependency> ServerCacheDependencies
		{
			get;
			set;
		}
		ApplyResponseHeadersDelegate ApplyToResponse
		{
			get;
			set;
		}
		bool ApplyDuringPreSendRequestHeaders
		{
			get;
			set;
		}
	}
}
