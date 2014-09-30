using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class WebConfigWriter
	{
		protected string physicalDirPath;
		protected string webConfigContents = "<?xml version=\"1.0\"?><configuration xmlns=\"http://schemas.microsoft.com/.NetConfiguration/v2.0\"><system.web><authorization><deny users=\"*\" /></authorization></system.web></configuration>";
		private readonly object _webConfigSyncObj = new object();
		private DateTime _lastCheckedWebConfig = DateTime.MinValue;
		private volatile bool _checkedWebConfigOnce;
		public WebConfigWriter(string physicalDirPath) : this(physicalDirPath, null)
		{
		}
		public WebConfigWriter(string physicalDirPath, string alternateWebConfigContents)
		{
			this.physicalDirPath = physicalDirPath;
			if (alternateWebConfigContents != null)
			{
				this.webConfigContents = alternateWebConfigContents;
			}
		}
		protected virtual string getNewWebConfigContents()
		{
			return this.webConfigContents;
		}
		public void CheckWebConfigEvery5()
		{
			if (this._lastCheckedWebConfig < DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)))
			{
				object webConfigSyncObj;
				Monitor.Enter(webConfigSyncObj = this._webConfigSyncObj);
				try
				{
					if (this._lastCheckedWebConfig < DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)))
					{
						this._checkWebConfig();
					}
				}
				finally
				{
					Monitor.Exit(webConfigSyncObj);
				}
			}
		}
		public void CheckWebConfigOnce()
		{
			if (this._checkedWebConfigOnce)
			{
				return;
			}
			object webConfigSyncObj;
			Monitor.Enter(webConfigSyncObj = this._webConfigSyncObj);
			try
			{
				if (!this._checkedWebConfigOnce)
				{
					this._checkWebConfig();
				}
			}
			finally
			{
				Monitor.Exit(webConfigSyncObj);
			}
		}
		public void CheckWebConfig()
		{
			object webConfigSyncObj;
			Monitor.Enter(webConfigSyncObj = this._webConfigSyncObj);
			try
			{
				this._checkWebConfig();
			}
			finally
			{
				Monitor.Exit(webConfigSyncObj);
			}
		}
		protected void _checkWebConfig()
		{
			try
			{
				string path = this.physicalDirPath.TrimEnd(new char[]
				{
					'/',
					'\\'
				}) + Path.DirectorySeparatorChar + "Web.config";
				if (!File.Exists(path))
				{
					if (!Directory.Exists(this.physicalDirPath))
					{
						Directory.CreateDirectory(this.physicalDirPath);
					}
					File.WriteAllText(path, this.getNewWebConfigContents(), Encoding.UTF8);
				}
			}
			finally
			{
				this._lastCheckedWebConfig = DateTime.UtcNow;
				this._checkedWebConfigOnce = true;
			}
		}
	}
}
