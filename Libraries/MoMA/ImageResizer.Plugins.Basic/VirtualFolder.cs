using ImageResizer.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
namespace ImageResizer.Plugins.Basic
{
	[ComVisible(true)]
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Medium), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.High)]
	public class VirtualFolder : VirtualPathProvider, IPlugin, IMultiInstancePlugin
	{
		[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
		public class VirtualFolderProviderVirtualFile : VirtualFile, IVirtualFileWithModifiedDate, IVirtualFile
		{
			private VirtualFolder provider;
			private bool? _exists = null;
			private DateTime? _fileModifiedDate = null;
			public bool Exists
			{
				get
				{
					if (!this._exists.HasValue)
					{
						this._exists = new bool?(this.provider.FileExists(base.VirtualPath));
					}
					return this._exists.Value;
				}
			}
			public DateTime ModifiedDateUTC
			{
				get
				{
					if (!this._fileModifiedDate.HasValue)
					{
						this._fileModifiedDate = new DateTime?(this.provider.getDateModifiedUtc(base.VirtualPath));
					}
					return this._fileModifiedDate.Value;
				}
			}
			public VirtualFolderProviderVirtualFile(string virtualPath, VirtualFolder provider) : base(virtualPath)
			{
				this.provider = provider;
			}
			public override Stream Open()
			{
				return this.provider.getStream(base.VirtualPath);
			}
			string IVirtualFile.get_VirtualPath()
			{
				return base.VirtualPath;
			}
		}
		private string virtualPath;
		private string physicalPath;
		public string VirtualPath
		{
			get
			{
				return this.virtualPath;
			}
			set
			{
				this.virtualPath = this.normalizeVirtualPath(value);
			}
		}
		public string PhysicalPath
		{
			get
			{
				return this.physicalPath;
			}
			set
			{
				this.physicalPath = this.resolvePhysicalPath(value);
			}
		}
		public VirtualFolder(string virtualPath, string physicalPath)
		{
			this.VirtualPath = virtualPath;
			this.PhysicalPath = physicalPath;
		}
		public VirtualFolder(NameValueCollection args)
		{
			this.VirtualPath = args["virtualPath"];
			this.PhysicalPath = args["physicalPath"];
		}
		public IPlugin Install(Config c)
		{
			HostingEnvironment.RegisterVirtualPathProvider(this);
			c.Plugins.add_plugin(this);
			return this;
		}
		public bool Uninstall(Config c)
		{
			c.Plugins.remove_plugin(this);
			return false;
		}
		protected string normalizeVirtualPath(string path)
		{
			if (path.StartsWith("~"))
			{
				path = HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
				{
					'/'
				}) + '/' + path.Substring(1).TrimStart(new char[]
				{
					'/'
				});
			}
			return path;
		}
		protected string resolvePhysicalPath(string path)
		{
			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, path);
			}
			return Path.GetFullPath(path);
		}
		public string virtualToPhysical(string virtualPath)
		{
			virtualPath = this.normalizeVirtualPath(virtualPath);
			if (virtualPath.StartsWith(this.VirtualPath, StringComparison.OrdinalIgnoreCase))
			{
				return Path.Combine(this.PhysicalPath, virtualPath.Substring(this.VirtualPath.Length).TrimStart(new char[]
				{
					'/'
				}).Replace('/', Path.DirectorySeparatorChar));
			}
			return null;
		}
		public bool isVirtualPath(string virtualPath)
		{
			virtualPath = this.normalizeVirtualPath(virtualPath);
			return virtualPath.StartsWith(this.VirtualPath, StringComparison.OrdinalIgnoreCase);
		}
		public bool isOnlyVirtualPath(string virtualPath)
		{
			return !base.Previous.FileExists(virtualPath) && this.isVirtualPath(virtualPath);
		}
		public Stream getStream(string virtualPath)
		{
			if (this.isOnlyVirtualPath(virtualPath))
			{
				return File.Open(this.virtualToPhysical(virtualPath), FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			return null;
		}
		public DateTime getDateModifiedUtc(string virtualPath)
		{
			string path = this.virtualToPhysical(virtualPath);
			if (File.Exists(path))
			{
				return File.GetLastWriteTimeUtc(path);
			}
			return DateTime.MinValue;
		}
		public override bool FileExists(string virtualPath)
		{
			if (this.isOnlyVirtualPath(virtualPath))
			{
				return File.Exists(this.virtualToPhysical(virtualPath));
			}
			return base.Previous.FileExists(virtualPath);
		}
		public override VirtualFile GetFile(string virtualPath)
		{
			if (this.isOnlyVirtualPath(virtualPath))
			{
				return new VirtualFolder.VirtualFolderProviderVirtualFile(virtualPath, this);
			}
			return base.Previous.GetFile(virtualPath);
		}
		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			if (!this.isOnlyVirtualPath(virtualPath))
			{
				return base.Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
			}
			List<string> list = new List<string>();
			foreach (string item in virtualPathDependencies)
			{
				list.Add(item);
			}
			return new CacheDependency(new string[]
			{
				this.virtualToPhysical(virtualPath)
			}, list.ToArray(), utcStart);
		}
	}
}
