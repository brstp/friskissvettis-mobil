using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Caching;
namespace ImageResizer.Caching
{
	[ComVisible(true)]
	public class ResponseHeaders : IResponseHeaders
	{
		protected bool applyDuringPreSendRequestHeaders = true;
		protected string contentType;
		protected ApplyResponseHeadersDelegate applyToResponse;
		protected HttpCacheability cacheControl = HttpCacheability.Private;
		protected DateTime expires = DateTime.MinValue;
		protected DateTime lastModified = DateTime.MinValue;
		protected bool validUntilExpires;
		protected bool suppressVaryHeader = true;
		protected NameValueCollection defaultHeaders = new NameValueCollection();
		protected NameValueCollection headers = new NameValueCollection();
		private List<CacheDependency> dependencies = new List<CacheDependency>();
		public bool ApplyDuringPreSendRequestHeaders
		{
			get
			{
				return this.applyDuringPreSendRequestHeaders;
			}
			set
			{
				this.applyDuringPreSendRequestHeaders = value;
			}
		}
		public string ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}
		public ApplyResponseHeadersDelegate ApplyToResponse
		{
			get
			{
				return this.applyToResponse;
			}
			set
			{
				this.applyToResponse = value;
			}
		}
		public HttpCacheability CacheControl
		{
			get
			{
				return this.cacheControl;
			}
			set
			{
				this.cacheControl = value;
			}
		}
		public DateTime Expires
		{
			get
			{
				return this.expires;
			}
			set
			{
				this.expires = value;
			}
		}
		public DateTime LastModified
		{
			get
			{
				return this.lastModified;
			}
			set
			{
				this.lastModified = value;
			}
		}
		public bool ValidUntilExpires
		{
			get
			{
				return this.validUntilExpires;
			}
			set
			{
				this.validUntilExpires = value;
			}
		}
		public bool SuppressVaryHeader
		{
			get
			{
				return this.suppressVaryHeader;
			}
			set
			{
				this.suppressVaryHeader = value;
			}
		}
		public NameValueCollection DefaultHeaders
		{
			get
			{
				return this.defaultHeaders;
			}
			set
			{
				this.defaultHeaders = value;
			}
		}
		public NameValueCollection Headers
		{
			get
			{
				return this.headers;
			}
			set
			{
				this.headers = value;
			}
		}
		public List<CacheDependency> ServerCacheDependencies
		{
			get
			{
				return this.dependencies;
			}
			set
			{
				this.dependencies = value;
			}
		}
		public ResponseHeaders()
		{
			this.ApplyToResponse = new ApplyResponseHeadersDelegate(ResponseHeaders.DefaultApplyToResponseMethod);
		}
		public static void DefaultApplyToResponseMethod(IResponseHeaders headers, HttpContext context)
		{
			foreach (string name in headers.DefaultHeaders)
			{
				context.Response.Headers[name] = headers.DefaultHeaders[name];
			}
			if (headers.ContentType != null)
			{
				context.Response.ContentType = headers.ContentType;
			}
			if (headers.Expires != DateTime.MinValue)
			{
				context.Response.Cache.SetExpires(headers.Expires);
			}
			if (headers.LastModified != DateTime.MinValue)
			{
				context.Response.Cache.SetLastModified(headers.LastModified);
			}
			context.Response.Cache.SetValidUntilExpires(headers.ValidUntilExpires);
			context.Response.Cache.SetOmitVaryStar(headers.SuppressVaryHeader);
			foreach (CacheDependency current in headers.ServerCacheDependencies)
			{
				context.Response.AddCacheDependency(new CacheDependency[]
				{
					current
				});
			}
			context.Response.Cache.SetCacheability(headers.CacheControl);
			foreach (string name2 in headers.Headers)
			{
				context.Response.Headers[name2] = headers.Headers[name2];
			}
		}
	}
}
