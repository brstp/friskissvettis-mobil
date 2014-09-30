using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImageResizer.Plugins.DiskCache
{
	[ComVisible(true)]
	public class CachedFolder
	{
		protected readonly object _sync = new object();
		private volatile bool isValid;
		protected Dictionary<string, CachedFolder> folders = new Dictionary<string, CachedFolder>(StringComparer.OrdinalIgnoreCase);
		protected Dictionary<string, CachedFileInfo> files = new Dictionary<string, CachedFileInfo>(StringComparer.OrdinalIgnoreCase);
		public event FileDisappearedHandler FileDisappeared;
		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
			set
			{
				this.isValid = value;
			}
		}
		protected StringComparer KeyComparer
		{
			get
			{
				return StringComparer.OrdinalIgnoreCase;
			}
		}
		public virtual void clear()
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				this.IsValid = false;
				this.folders.Clear();
				this.files.Clear();
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public virtual CachedFileInfo getCachedFileInfo(string relativePath)
		{
			relativePath = this.checkRelativePath(relativePath);
			object sync;
			Monitor.Enter(sync = this._sync);
			CachedFileInfo result;
			try
			{
				int num = relativePath.IndexOf('/');
				if (num < 0)
				{
					CachedFileInfo cachedFileInfo;
					if (this.files.TryGetValue(relativePath, out cachedFileInfo))
					{
						result = cachedFileInfo;
						return result;
					}
				}
				else
				{
					string key = relativePath.Substring(0, num);
					CachedFolder cachedFolder;
					if (!this.folders.TryGetValue(key, out cachedFolder))
					{
						cachedFolder = null;
					}
					if (cachedFolder != null)
					{
						result = cachedFolder.getCachedFileInfo(relativePath.Substring(num + 1));
						return result;
					}
				}
				result = null;
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public virtual void setCachedFileInfo(string relativePath, CachedFileInfo info)
		{
			relativePath = this.checkRelativePath(relativePath);
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				int num = relativePath.IndexOf('/');
				if (num < 0)
				{
					if (info == null)
					{
						this.files.Remove(relativePath);
					}
					else
					{
						this.files[relativePath] = info;
					}
				}
				else
				{
					string key = relativePath.Substring(0, num);
					CachedFolder cachedFolder;
					if (!this.folders.TryGetValue(key, out cachedFolder))
					{
						cachedFolder = null;
					}
					if (info != null || cachedFolder != null)
					{
						if (cachedFolder == null)
						{
							cachedFolder = (this.folders[key] = new CachedFolder());
						}
						cachedFolder.setCachedFileInfo(relativePath.Substring(num + 1), info);
					}
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		public bool bumpDateIfExists(string relativePath)
		{
			relativePath = this.checkRelativePath(relativePath);
			object sync;
			Monitor.Enter(sync = this._sync);
			bool result;
			try
			{
				int num = relativePath.IndexOf('/');
				if (num < 0)
				{
					CachedFileInfo f;
					if (this.files.TryGetValue(relativePath, out f))
					{
						this.files[relativePath] = new CachedFileInfo(f, DateTime.UtcNow);
					}
					result = true;
				}
				else
				{
					string key = relativePath.Substring(0, num);
					CachedFolder cachedFolder = null;
					if (!this.folders.TryGetValue(key, out cachedFolder))
					{
						result = false;
					}
					else
					{
						if (cachedFolder == null)
						{
							result = false;
						}
						else
						{
							result = cachedFolder.bumpDateIfExists(relativePath.Substring(num + 1));
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public virtual CachedFileInfo getFileInfo(string relativePath, string physicalPath)
		{
			relativePath = this.checkRelativePath(relativePath);
			object sync;
			Monitor.Enter(sync = this._sync);
			CachedFileInfo result;
			try
			{
				CachedFileInfo cachedFileInfo = this.getCachedFileInfo(relativePath);
				if (cachedFileInfo == null && File.Exists(physicalPath))
				{
					cachedFileInfo = new CachedFileInfo(new FileInfo(physicalPath));
					this.setCachedFileInfo(relativePath, cachedFileInfo);
				}
				result = cachedFileInfo;
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public virtual CachedFileInfo getFileInfoCertainExists(string relativePath, string physicalPath)
		{
			relativePath = this.checkRelativePath(relativePath);
			bool flag = false;
			CachedFileInfo cachedFileInfo = null;
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				bool flag2 = File.Exists(physicalPath);
				cachedFileInfo = this.getCachedFileInfo(relativePath);
				if (cachedFileInfo == null && flag2)
				{
					cachedFileInfo = new CachedFileInfo(new FileInfo(physicalPath));
					this.setCachedFileInfo(relativePath, cachedFileInfo);
				}
				if (cachedFileInfo != null && !flag2)
				{
					cachedFileInfo = null;
					this.clear();
					flag = true;
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
			if (flag && this.FileDisappeared != null)
			{
				this.FileDisappeared(relativePath, physicalPath);
			}
			return cachedFileInfo;
		}
		public bool GetIsValid(string relativePath)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			bool result;
			try
			{
				CachedFolder folder = this.getFolder(relativePath);
				if (folder != null)
				{
					result = folder.IsValid;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		protected CachedFolder getFolder(string relativePath)
		{
			return this.getOrCreateFolder(relativePath, false);
		}
		protected CachedFolder getOrCreateFolder(string relativePath, bool createIfMissing)
		{
			relativePath = this.checkRelativePath(relativePath);
			if (string.IsNullOrEmpty(relativePath))
			{
				return this;
			}
			int num = relativePath.IndexOf('/');
			string key = relativePath;
			if (num > -1)
			{
				key = relativePath.Substring(0, num);
				relativePath = relativePath.Substring(num + 1);
			}
			else
			{
				relativePath = "";
			}
			CachedFolder cachedFolder;
			if (!this.folders.TryGetValue(key, out cachedFolder))
			{
				if (!createIfMissing)
				{
					return null;
				}
				cachedFolder = (this.folders[key] = new CachedFolder());
			}
			if (cachedFolder != null)
			{
				return cachedFolder.getFolder(relativePath);
			}
			return null;
		}
		public IList<string> getSubfolders(string relativePath)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			IList<string> result;
			try
			{
				CachedFolder folder = this.getFolder(relativePath);
				result = new List<string>(folder.folders.Keys);
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public Dictionary<string, CachedFileInfo> getSubfilesCopy(string relativePath)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			Dictionary<string, CachedFileInfo> result;
			try
			{
				CachedFolder folder = this.getFolder(relativePath);
				result = new Dictionary<string, CachedFileInfo>(folder.files, folder.files.Comparer);
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public ICollection<KeyValuePair<string, CachedFileInfo>> getSortedSubfiles(string relativePath)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			ICollection<KeyValuePair<string, CachedFileInfo>> result;
			try
			{
				CachedFolder folder = this.getFolder(relativePath);
				if (folder == null || folder.files.Count < 1)
				{
					result = null;
				}
				else
				{
					KeyValuePair<string, CachedFileInfo>[] array = new KeyValuePair<string, CachedFileInfo>[folder.files.Count];
					int num = 0;
					foreach (KeyValuePair<string, CachedFileInfo> current in folder.files)
					{
						array[num] = current;
						num++;
					}
					Array.Sort<KeyValuePair<string, CachedFileInfo>>(array, (KeyValuePair<string, CachedFileInfo> a, KeyValuePair<string, CachedFileInfo> b) => DateTime.Compare(a.Value.AccessedUtc, b.Value.AccessedUtc));
					result = array;
				}
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return result;
		}
		public int getFileCount(string relativePath)
		{
			object sync;
			Monitor.Enter(sync = this._sync);
			int count;
			try
			{
				CachedFolder folder = this.getFolder(relativePath);
				count = folder.files.Count;
			}
			finally
			{
				Monitor.Exit(sync);
			}
			return count;
		}
		public void populate(string relativePath, string physicalPath)
		{
			this.populateSubfolders(relativePath, physicalPath);
			this.populateFiles(relativePath, physicalPath);
			this.getOrCreateFolder(relativePath, true).IsValid = true;
		}
		protected void populateSubfolders(string relativePath, string physicalPath)
		{
			relativePath = this.checkRelativePath(relativePath);
			string[] array = null;
			try
			{
				array = Directory.GetDirectories(physicalPath);
			}
			catch (DirectoryNotFoundException)
			{
				array = new string[0];
			}
			object sync;
			Monitor.Enter(sync = this._sync);
			try
			{
				CachedFolder orCreateFolder = this.getOrCreateFolder(relativePath, true);
				Dictionary<string, CachedFolder> dictionary = new Dictionary<string, CachedFolder>(array.Length, this.KeyComparer);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					string text2 = text.Substring(text.LastIndexOf(Path.DirectorySeparatorChar) + 1);
					if (!text2.StartsWith("."))
					{
						if (orCreateFolder.folders.ContainsKey(text2))
						{
							dictionary[text2] = orCreateFolder.folders[text2];
						}
						else
						{
							dictionary[text2] = new CachedFolder();
						}
					}
				}
				orCreateFolder.folders = dictionary;
			}
			finally
			{
				Monitor.Exit(sync);
			}
		}
		protected void populateFiles(string relativePath, string physicalPath)
		{
			relativePath = this.checkRelativePath(relativePath);
			string[] array = null;
			try
			{
				array = Directory.GetFiles(physicalPath);
			}
			catch (DirectoryNotFoundException)
			{
				array = new string[0];
			}
			Dictionary<string, CachedFileInfo> dictionary = new Dictionary<string, CachedFileInfo>(array.Length, this.KeyComparer);
			CachedFolder orCreateFolder = this.getOrCreateFolder(relativePath, true);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string text2 = text.Substring(text.LastIndexOf(Path.DirectorySeparatorChar) + 1);
				if (!text2.EndsWith(".config", StringComparison.OrdinalIgnoreCase) && !text2.StartsWith("."))
				{
					CachedFileInfo old = null;
					object sync;
					Monitor.Enter(sync = this._sync);
					try
					{
						if (!orCreateFolder.files.TryGetValue(relativePath, out old))
						{
							old = null;
						}
					}
					finally
					{
						Monitor.Exit(sync);
					}
					dictionary[text2] = new CachedFileInfo(new FileInfo(text), old);
				}
			}
			object sync2;
			Monitor.Enter(sync2 = this._sync);
			try
			{
				orCreateFolder.files = dictionary;
			}
			finally
			{
				Monitor.Exit(sync2);
			}
		}
		public bool existsCertain(string relativePath, string physicalPath)
		{
			return this.getFileInfoCertainExists(relativePath, physicalPath) != null;
		}
		public bool exists(string relativePath, string physicalPath)
		{
			return this.getFileInfo(relativePath, physicalPath) != null;
		}
		public bool modifiedDateMatches(DateTime utc, string relativePath, string physicalPath)
		{
			CachedFileInfo fileInfo = this.getFileInfo(relativePath, physicalPath);
			return fileInfo != null && this.roughCompare(fileInfo.ModifiedUtc, utc);
		}
		public bool modifiedDateMatchesCertainExists(DateTime utc, string relativePath, string physicalPath)
		{
			CachedFileInfo fileInfoCertainExists = this.getFileInfoCertainExists(relativePath, physicalPath);
			return fileInfoCertainExists != null && this.roughCompare(fileInfoCertainExists.ModifiedUtc, utc);
		}
		protected bool roughCompare(DateTime d1, DateTime d2)
		{
			return Math.Abs(d1.Ticks - d2.Ticks) < 50000L;
		}
		protected string checkRelativePath(string relativePath)
		{
			if (relativePath == null)
			{
				return relativePath;
			}
			if (!relativePath.StartsWith("/"))
			{
				relativePath.EndsWith("/");
			}
			return relativePath;
		}
	}
}
