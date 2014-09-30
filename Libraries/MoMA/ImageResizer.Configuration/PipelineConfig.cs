using ImageResizer.Caching;
using ImageResizer.Collections;
using ImageResizer.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Web.Hosting;
namespace ImageResizer.Configuration
{
	[ComVisible(true)]
	public class PipelineConfig : IPipelineConfig, ICacheProvider
	{
		protected Config c;
		protected object _cachedUrlDataSync = new object();
		protected volatile Dictionary<string, bool> _cachedDirectives;
		protected volatile Dictionary<string, bool> _cachedExtensions;
		protected volatile IList<string> _fakeExtensions;
		private bool skipFileTypeCheck;
		protected volatile bool firedFirstRequest;
		protected object firedFirstRequestSync = new object();
		protected long processedCount;
		public event RequestEventHandler OnFirstRequest;
		public event RequestEventHandler PostAuthorizeRequestStart;
		public event UrlRewritingEventHandler Rewrite;
		public event UrlRewritingEventHandler RewriteDefaults;
		public event UrlRewritingEventHandler PostRewrite;
		public event UrlAuthorizationEventHandler AuthorizeImage;
		public event UrlEventHandler ImageMissing;
		public event PreHandleImageEventHandler PreHandleImage;
		public event CacheSelectionHandler SelectCachingSystem;
		public ICollection<string> AcceptedImageExtensions
		{
			get
			{
				return new List<string>(this.getCachedExtensions().Keys);
			}
		}
		public ICollection<string> SupportedQuerystringKeys
		{
			get
			{
				return new List<string>(this.getCachedDirectives().Keys);
			}
		}
		public IList<string> FakeExtensions
		{
			get
			{
				IList<string> list = this._fakeExtensions;
				if (list != null)
				{
					return list;
				}
				list = new List<string>(this.c.get("pipeline.fakeExtensions", ".ashx").Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries));
				for (int i = 0; i < list.Count; i++)
				{
					if (!list[i].StartsWith(".", StringComparison.OrdinalIgnoreCase))
					{
						list[i] = "." + list[i];
					}
				}
				this._fakeExtensions = list;
				return list;
			}
		}
		public string ModifiedPathKey
		{
			get
			{
				return "resizer.newPath";
			}
		}
		public string PreRewritePath
		{
			get
			{
				if (HttpContext.Current == null)
				{
					return null;
				}
				if (HttpContext.Current.Items[this.ModifiedPathKey] == null)
				{
					return HttpContext.Current.Request.FilePath + HttpContext.Current.Request.PathInfo;
				}
				return HttpContext.Current.Items[this.ModifiedPathKey] as string;
			}
			set
			{
				HttpContext.Current.Items[this.ModifiedPathKey] = value;
			}
		}
		public string ModifiedQueryStringKey
		{
			get
			{
				return "resizer.modifiedQueryString";
			}
		}
		public NameValueCollection ModifiedQueryString
		{
			get
			{
				if (HttpContext.Current == null)
				{
					return null;
				}
				if (HttpContext.Current.Items[this.ModifiedQueryStringKey] == null)
				{
					HttpContext.Current.Items[this.ModifiedQueryStringKey] = new NameValueCollection(HttpContext.Current.Request.QueryString);
				}
				return (NameValueCollection)HttpContext.Current.Items[this.ModifiedQueryStringKey];
			}
			set
			{
				HttpContext.Current.Items[this.ModifiedQueryStringKey] = value;
			}
		}
		public string SkipFileTypeCheckKey
		{
			get
			{
				return "resizer.skipFileTypeCheck";
			}
		}
		public bool SkipFileTypeCheck
		{
			get
			{
				return HttpContext.Current.Items[this.SkipFileTypeCheckKey] != null && (bool)HttpContext.Current.Items[this.SkipFileTypeCheckKey];
			}
			set
			{
				HttpContext.Current.Items[this.SkipFileTypeCheckKey] = value;
			}
		}
		public string ResponseArgsKey
		{
			get
			{
				return "resizer.cacheArgs";
			}
		}
		public bool IsHandlingRequest
		{
			get
			{
				return HttpContext.Current != null && HttpContext.Current.Items[this.ResponseArgsKey] != null;
			}
		}
		public VppUsageOption VppUsage
		{
			get
			{
				return this.c.get<VppUsageOption>("pipeline.vppUsage", VppUsageOption.Fallback);
			}
		}
		public long ProcessedCount
		{
			get
			{
				return this.processedCount;
			}
		}
		public PipelineConfig(Config c)
		{
			this.c = c;
			c.Plugins.QuerystringPlugins.Changed += new SafeList<IQuerystringPlugin>.ChangedHandler(this.urlModifyingPlugins_Changed);
		}
		protected void urlModifyingPlugins_Changed(SafeList<IQuerystringPlugin> sender)
		{
			object cachedUrlDataSync;
			Monitor.Enter(cachedUrlDataSync = this._cachedUrlDataSync);
			try
			{
				this._cachedDirectives = null;
				this._cachedExtensions = null;
			}
			finally
			{
				Monitor.Exit(cachedUrlDataSync);
			}
		}
		protected void _cacheUrlData()
		{
			if (this._cachedDirectives != null && this._cachedExtensions != null)
			{
				return;
			}
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>(48, StringComparer.OrdinalIgnoreCase);
			Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>(24, StringComparer.OrdinalIgnoreCase);
			foreach (IQuerystringPlugin current in this.c.Plugins.QuerystringPlugins)
			{
				IEnumerable<string> enumerable = current.GetSupportedQuerystringKeys();
				if (enumerable != null)
				{
					foreach (string current2 in enumerable)
					{
						dictionary[current2] = true;
					}
				}
			}
			foreach (IFileExtensionPlugin current3 in this.c.Plugins.FileExtensionPlugins)
			{
				IEnumerable<string> enumerable = current3.GetSupportedFileExtensions();
				if (enumerable != null)
				{
					foreach (string current4 in enumerable)
					{
						dictionary2[current4.TrimStart(new char[]
						{
							'.'
						})] = true;
					}
				}
			}
			ImageBuilder currentImageBuilder = this.c.CurrentImageBuilder;
			if (currentImageBuilder != null)
			{
				IEnumerable<string> enumerable = currentImageBuilder.GetSupportedFileExtensions();
				if (enumerable != null)
				{
					foreach (string current5 in enumerable)
					{
						dictionary2[current5.TrimStart(new char[]
						{
							'.'
						})] = true;
					}
				}
				enumerable = currentImageBuilder.GetSupportedQuerystringKeys();
				if (enumerable != null)
				{
					foreach (string current6 in enumerable)
					{
						dictionary[current6] = true;
					}
				}
			}
			this._cachedDirectives = dictionary;
			this._cachedExtensions = dictionary2;
		}
		protected Dictionary<string, bool> getCachedDirectives()
		{
			object cachedUrlDataSync;
			Monitor.Enter(cachedUrlDataSync = this._cachedUrlDataSync);
			Dictionary<string, bool> cachedDirectives;
			try
			{
				this._cacheUrlData();
				cachedDirectives = this._cachedDirectives;
			}
			finally
			{
				Monitor.Exit(cachedUrlDataSync);
			}
			return cachedDirectives;
		}
		protected Dictionary<string, bool> getCachedExtensions()
		{
			object cachedUrlDataSync;
			Monitor.Enter(cachedUrlDataSync = this._cachedUrlDataSync);
			Dictionary<string, bool> cachedExtensions;
			try
			{
				this._cacheUrlData();
				cachedExtensions = this._cachedExtensions;
			}
			finally
			{
				Monitor.Exit(cachedUrlDataSync);
			}
			return cachedExtensions;
		}
		protected string getExtension(string path)
		{
			int num = path.LastIndexOfAny(new char[]
			{
				'.',
				'/',
				' ',
				'\\',
				'?',
				'&',
				':'
			});
			if (num > -1 && path[num] == '.')
			{
				return path.Substring(num + 1);
			}
			return null;
		}
		public bool IsAcceptedImageType(string path)
		{
			string extension = this.getExtension(path);
			return !string.IsNullOrEmpty(extension) && this.getCachedExtensions().ContainsKey(extension);
		}
		public bool HasPipelineDirective(NameValueCollection q)
		{
			Dictionary<string, bool> cachedDirectives = this.getCachedDirectives();
			foreach (string text in q.Keys)
			{
				if (text != null && cachedDirectives.ContainsKey(text))
				{
					return true;
				}
			}
			return false;
		}
		public string TrimFakeExtensions(string path)
		{
			foreach (string current in this.FakeExtensions)
			{
				if (path.EndsWith(current, StringComparison.OrdinalIgnoreCase))
				{
					path = path.Substring(0, path.Length - current.Length).TrimEnd(new char[]
					{
						'.'
					});
					break;
				}
			}
			return path;
		}
		public object GetFile(string virtualPath, NameValueCollection queryString)
		{
			foreach (IVirtualImageProvider current in this.c.Plugins.VirtualProviderPlugins)
			{
				if (current.FileExists(virtualPath, queryString))
				{
					object file = current.GetFile(virtualPath, queryString);
					return file;
				}
			}
			if (HostingEnvironment.VirtualPathProvider == null)
			{
				return null;
			}
			return HostingEnvironment.VirtualPathProvider.GetFile(virtualPath);
		}
		public bool FileExists(string virtualPath, NameValueCollection queryString)
		{
			foreach (IVirtualImageProvider current in this.c.Plugins.VirtualProviderPlugins)
			{
				if (current.FileExists(virtualPath, queryString))
				{
					return true;
				}
			}
			return HostingEnvironment.VirtualPathProvider != null && HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
		}
		public ImageBuilder GetImageBuilder()
		{
			return this.c.CurrentImageBuilder;
		}
		public ICacheProvider GetCacheProvider()
		{
			return this;
		}
		public void FirePostAuthorizeRequest(IHttpModule sender, HttpContext httpContext)
		{
			if (!this.firedFirstRequest)
			{
				object obj;
				Monitor.Enter(obj = this.firedFirstRequestSync);
				try
				{
					if (!this.firedFirstRequest)
					{
						this.firedFirstRequest = true;
						if (this.OnFirstRequest != null)
						{
							this.OnFirstRequest(sender, httpContext);
						}
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			if (this.PostAuthorizeRequestStart != null)
			{
				this.PostAuthorizeRequestStart(sender, httpContext);
			}
		}
		public void FireRewritingEvents(IHttpModule sender, HttpContext context, IUrlEventArgs e)
		{
			if (this.Rewrite != null)
			{
				this.Rewrite(sender, context, e);
			}
			NameValueCollection nameValueCollection = new NameValueCollection(e.QueryString);
			if (this.RewriteDefaults != null)
			{
				this.RewriteDefaults(sender, context, e);
			}
			foreach (string name in nameValueCollection)
			{
				e.QueryString[name] = nameValueCollection[name];
			}
			if (this.PostRewrite != null)
			{
				this.PostRewrite(sender, context, e);
			}
		}
		public void FireAuthorizeImage(IHttpModule sender, HttpContext context, IUrlAuthorizationEventArgs e)
		{
			if (this.AuthorizeImage != null)
			{
				this.AuthorizeImage(sender, context, e);
			}
		}
		public void FireImageMissing(IHttpModule sender, HttpContext context, IUrlEventArgs e)
		{
			if (this.ImageMissing != null)
			{
				this.ImageMissing(sender, context, e);
			}
		}
		public void FirePreHandleImage(IHttpModule sender, HttpContext context, IResponseArgs e)
		{
			Interlocked.Increment(ref this.processedCount);
			if (this.PreHandleImage != null)
			{
				this.PreHandleImage(sender, context, e);
			}
		}
		public ICache GetCachingSystem(HttpContext context, IResponseArgs args)
		{
			ICache defaultCache = null;
			foreach (ICache current in this.c.Plugins.CachingSystems)
			{
				if (current.CanProcess(context, args))
				{
					defaultCache = current;
					break;
				}
			}
			CacheSelectionEventArgs cacheSelectionEventArgs = new CacheSelectionEventArgs(context, args, defaultCache);
			if (this.SelectCachingSystem != null)
			{
				this.SelectCachingSystem(this, cacheSelectionEventArgs);
			}
			return cacheSelectionEventArgs.SelectedCache;
		}
	}
}
