using ImageResizer.Caching;
using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Web.Hosting;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class DiskCache : ICache, IPlugin, IIssueProvider
	{
		private int subfolders = 32;
		private bool enabled = true;
		private bool autoClean;
		private CleanupStrategy cleanupStrategy = new CleanupStrategy();
		protected int cacheAccessTimeout = 15000;
		private bool hashModifiedDate = true;
		protected string virtualDir = HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
		{
			'/'
		}) + "/imagecache";
		protected CustomDiskCache cache;
		protected CleanupManager cleaner;
		protected WebConfigWriter writer;
		protected readonly object _startSync = new object();
		protected volatile bool _started;
		public int Subfolders
		{
			get
			{
				return this.subfolders;
			}
			set
			{
				this.BeforeSettingChanged();
				this.subfolders = value;
			}
		}
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.BeforeSettingChanged();
				this.enabled = value;
			}
		}
		public bool AutoClean
		{
			get
			{
				return this.autoClean;
			}
			set
			{
				this.BeforeSettingChanged();
				this.autoClean = value;
			}
		}
		public CleanupStrategy CleanupStrategy
		{
			get
			{
				return this.cleanupStrategy;
			}
			set
			{
				this.BeforeSettingChanged();
				this.cleanupStrategy = value;
			}
		}
		public int CacheAccessTimeout
		{
			get
			{
				return this.cacheAccessTimeout;
			}
			set
			{
				this.BeforeSettingChanged();
				this.cacheAccessTimeout = Math.Max(value, 0);
			}
		}
		public bool HashModifiedDate
		{
			get
			{
				return this.hashModifiedDate;
			}
			set
			{
				this.BeforeSettingChanged();
				this.hashModifiedDate = value;
			}
		}
		public string VirtualCacheDir
		{
			get
			{
				return this.virtualDir;
			}
			set
			{
				this.BeforeSettingChanged();
				this.virtualDir = (string.IsNullOrEmpty(value) ? null : value);
				if (this.virtualDir != null)
				{
					if (this.virtualDir.StartsWith("~"))
					{
						this.virtualDir = HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
						{
							'/'
						}) + this.virtualDir.Substring(1);
						return;
					}
					if (!this.virtualDir.StartsWith("/"))
					{
						this.virtualDir = HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
						{
							'/'
						}) + "/" + this.virtualDir;
					}
				}
			}
		}
		public string PhysicalCacheDir
		{
			get
			{
				if (!string.IsNullOrEmpty(this.VirtualCacheDir))
				{
					return HostingEnvironment.MapPath(this.VirtualCacheDir);
				}
				return null;
			}
		}
		public bool Started
		{
			get
			{
				return this._started;
			}
		}
		protected void BeforeSettingChanged()
		{
			if (this._started)
			{
				throw new InvalidOperationException("DiskCache settings may not be adjusted after it is started.");
			}
		}
		public DiskCache()
		{
		}
		public DiskCache(string virtualDir)
		{
			this.VirtualCacheDir = virtualDir;
		}
		public void LoadSettings(Config c)
		{
			this.Subfolders = c.get("diskcache.subfolders", this.Subfolders);
			this.Enabled = c.get("diskcache.enabled", this.Enabled);
			this.AutoClean = c.get("diskcache.autoClean", this.AutoClean);
			this.VirtualCacheDir = c.get("diskcache.dir", this.VirtualCacheDir);
			this.HashModifiedDate = c.get("diskcache.hashModifiedDate", this.HashModifiedDate);
			this.CacheAccessTimeout = c.get("diskcache.cacheAccessTimeout", this.CacheAccessTimeout);
			this.CleanupStrategy.LoadFrom(c.getNode("cleanupStrategy"));
		}
		public IPlugin Install(Config c)
		{
			this.LoadSettings(c);
			this.Start();
			c.Pipeline.AuthorizeImage += new UrlAuthorizationEventHandler(this.Pipeline_AuthorizeImage);
			c.Plugins.add_plugin(this);
			return this;
		}
		private void Pipeline_AuthorizeImage(IHttpModule sender, HttpContext context, IUrlAuthorizationEventArgs e)
		{
			if (e.VirtualPath.IndexOf(this.VirtualCacheDir, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				e.AllowAccess = false;
			}
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			c.Pipeline.AuthorizeImage -= new UrlAuthorizationEventHandler(this.Pipeline_AuthorizeImage);
			return this.Stop();
		}
		public bool IsConfigurationValid()
		{
			return !string.IsNullOrEmpty(this.VirtualCacheDir) && this.Enabled;
		}
		public bool Start()
		{
			if (!this.IsConfigurationValid())
			{
				return false;
			}
			object startSync;
			Monitor.Enter(startSync = this._startSync);
			bool result;
			try
			{
				if (this._started)
				{
					result = true;
				}
				else
				{
					if (!this.IsConfigurationValid())
					{
						result = false;
					}
					else
					{
						this.writer = new WebConfigWriter(this.PhysicalCacheDir);
						this.cache = new CustomDiskCache(this.PhysicalCacheDir, this.Subfolders, this.HashModifiedDate);
						if (this.AutoClean && this.cleanupStrategy == null)
						{
							this.cleanupStrategy = new CleanupStrategy();
						}
						if (this.AutoClean)
						{
							this.cleaner = new CleanupManager(this.cache, this.cleanupStrategy);
						}
						if (this.cleaner != null)
						{
							this.cleaner.CleanAll();
						}
						this._started = true;
						result = true;
					}
				}
			}
			finally
			{
				Monitor.Exit(startSync);
			}
			return result;
		}
		public bool Stop()
		{
			if (this.cleaner != null)
			{
				this.cleaner.Dispose();
			}
			this.cleaner = null;
			return true;
		}
		public bool CanProcess(HttpContext current, IResponseArgs e)
		{
			return ((ResizeSettings)e.RewrittenQuerystring).Cache != ServerCacheMode.No && this.Started;
		}
		public void Process(HttpContext context, IResponseArgs e)
		{
			DateTime sourceModifiedUtc = e.HasModifiedDate ? e.GetModifiedDateUTC() : DateTime.MinValue;
			this.writer.CheckWebConfigEvery5();
			CacheResult cachedFile = this.cache.GetCachedFile(e.RequestKey, e.SuggestedExtension, e.ResizeImageToStream, sourceModifiedUtc, this.CacheAccessTimeout);
			if (cachedFile.Result == CacheQueryResult.Failed)
			{
				throw new ImageProcessingException(string.Concat(new object[]
				{
					"Failed to acquire a lock on file \"",
					cachedFile.PhysicalPath,
					"\" within ",
					this.CacheAccessTimeout,
					"ms. Caching failed."
				}));
			}
			if (cachedFile.Result == CacheQueryResult.Hit && this.cleaner != null)
			{
				this.cleaner.UsedFile(cachedFile.RelativePath, cachedFile.PhysicalPath);
			}
			context.Items["FinalCachedFile"] = cachedFile.PhysicalPath;
			string path = this.VirtualCacheDir.TrimEnd(new char[]
			{
				'/'
			}) + '/' + cachedFile.RelativePath.Replace('\\', '/').TrimStart(new char[]
			{
				'/'
			});
			context.RewritePath(path, false);
		}
		public IEnumerable<IIssue> GetIssues()
		{
			List<IIssue> list = new List<IIssue>();
			if (this.cleaner != null)
			{
				list.AddRange(this.cleaner.GetIssues());
			}
			if (string.IsNullOrEmpty(this.VirtualCacheDir))
			{
				list.Add(new Issue("DiskCache", "cacheDir is empty. Cannot operate", null, IssueSeverity.ConfigurationError));
			}
			if (!this.Started)
			{
				list.Add(new Issue("DiskCache", "DiskCache is not running. Verify cacheDir is a valid path and enabled=true.", null, IssueSeverity.ConfigurationError));
			}
			return list;
		}
	}
}
